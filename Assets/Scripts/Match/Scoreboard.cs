using System;

using UnityEngine;
using TMPro;

using Aritche.Systems;
using Aritche.Rules;
using Aritche.Players;

namespace Aritche
{
    public enum MatchConclusion
    {
        Victory, Draw
    }

    public class ScoreboardConclusion
    {
        public string Message { get; set; }
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

        internal void OnMovePerformed(MovePerformInfo moveInfo)
        {
            ComputeScore(moveInfo);
        }

        public void RefreshUI()
        {
            blueScoreTMP.text = Match.BluePlayer.fScore.ToString();
            orangeScoreTMP.text = Match.OrangePlayer.fScore.ToString();
        }

        public void ComputeScore(MovePerformInfo moveInfo)
        {
            float score;
            char operation;
            float x = 0f;
            if (float.TryParse(moveInfo.CapturingPiece.ValueAsString, out x));
            float y = 0f;            
            if (float.TryParse(moveInfo.CapturedPiece.ValueAsString, out y));

            (score, operation) = moveInfo.Destination.Operation switch
            {
                Operation.Addition => (x + y, '+'),
                Operation.Subtraction => (x - y, '-'),
                Operation.Multiplication => (x * y, '×'),
                Operation.Division => (x / y, '÷'),
                _ => throw new ArgumentOutOfRangeException(),
            };

            Game.Console.Log($"[ACTION]: {moveInfo.CapturingPiece.ValueAsString} {operation} {moveInfo.CapturedPiece.ValueAsString} = {score}");

            moveInfo.SetScoreValue(score);
            AddScore(moveInfo.Player, score);
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
            
            if (Match.BluePlayer.fScore > Match.OrangePlayer.fScore)
            {
                conclusion.Message = $"Winning score";
                conclusion.Winner = Match.BluePlayer;
            }
            else if (Match.BluePlayer.fScore < Match.OrangePlayer.fScore)
            {
                conclusion.Message = $"Winning score";
                conclusion.Winner = Match.OrangePlayer;
            }
            else if (Match.BluePlayer.fScore == Match.OrangePlayer.fScore)
            {
                conclusion.Message = $"Scores are equal";
                conclusion.Winner = null;
            }

            return conclusion;
        }
    }
}