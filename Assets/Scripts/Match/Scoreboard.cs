using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

using Damath.Systems;
using Damath.Players;

namespace Damath
{
    public enum MatchConclusion
    {
        Victory, Draw
    }

    public class ScoreboardConclusion
    {
        public string Reason { get; set; }
        public MatchConclusion MatchConclusion  { get; set; }
        public Player Winner { get; set; }
    }

    public class Scoreboard : MonoBehaviour
    {
        [field: SerializeField] public MatchController Match { get; set; }
        [field: SerializeField] public Ruleset Ruleset { get; set; }
        
        [Header("UI Elements")]
        [SerializeField] TextMeshProUGUI blueScoreTMP;
        [SerializeField] TextMeshProUGUI orangeScoreTMP;

        internal void SetRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        internal void Initialize()
        {
            
        }

        internal void OnPieceCaptureCallback(object sender, Move e)
        {
            ComputeScoreByMove(e);
        }

        public void RefreshUI()
        {
            blueScoreTMP.text = Match.BluePlayer.fScore.ToString();
            orangeScoreTMP.text = Match.OrangePlayer.fScore.ToString();
        }

        public void ComputeScoreByMove(Move move)
        {
            float score;
            char operation;
            float x = float.Parse(move.CapturingPiece.Value);
            float y = float.Parse(move.CapturedPiece.Value);

            (score, operation) = move.Destination.Operation switch
            {
                Operation.Addition => (x + y, '+'),
                Operation.Subtraction => (x - y, '-'),
                Operation.Multiplication => (x * y, '×'),
                Operation.Division => (x / y, '÷'),
                _ => throw new NotImplementedException(),
            };

            Game.Console.Log($"[ACTION]: {move.CapturingPiece.Value} {operation} {move.CapturedPiece.Value} = {score}");

            move.SetScoreValue(score);
            AddScore(move.Player, score);
            RefreshUI();
        }

        public void AddScore(Player player, float value)
        {
            player.fScore += value;
        }

        public void RemoveScore(Player player, float value)
        {
            player.fScore -= value;
        }

        public float GetScore(Player player)
        {
            return player.fScore;
        }

        public ScoreboardConclusion GetConclusion()
        {
            var conclusion = new ScoreboardConclusion();

            if (Match == null)
            {
                Game.Console.Log($"[SCOREBOARD]: Invalid MatchController");
                Debug.LogError($"[SCOREBOARD]: Invalid MatchController");
                return default;
            }
            
            conclusion.MatchConclusion = MatchConclusion.Victory;
            if (Match.BluePlayer.fScore > Match.OrangePlayer.fScore)
            {
                conclusion.Winner = Match.BluePlayer;
            }
            else if (Match.BluePlayer.fScore < Match.OrangePlayer.fScore)
            {
                conclusion.Winner =  Match.OrangePlayer;
            }
            else if (Match.BluePlayer.fScore == Match.OrangePlayer.fScore)
            {
                conclusion.Winner =  Match.OrangePlayer;
                conclusion.MatchConclusion = MatchConclusion.Draw;
            }

            return conclusion;
        }
    }
}