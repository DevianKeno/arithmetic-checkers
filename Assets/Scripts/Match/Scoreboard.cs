using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Damath
{
    public class Scoreboard : MonoBehaviour
    {
        Ruleset Ruleset { get; set; }
        
        [SerializeField] private TextMeshProUGUI BlueScore;
        [SerializeField] private TextMeshProUGUI OrangeScore;

        internal void SetRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        internal void Initialize()
        {

        }

        internal void OnPieceCaptureCallback(object sender, Move e)
        {
            Compute(e);
        }

        public void Refresh(Player player)
        {
            if (player.Side == Side.Blue)
            {
                BlueScore.text = player.fScore.ToString();
            } else if (player.Side == Side.Orange)
            {
                OrangeScore.text = player.fScore.ToString();
            }
        }

        public void Compute(Move move)
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
            Refresh(move.Player);
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
    }
}