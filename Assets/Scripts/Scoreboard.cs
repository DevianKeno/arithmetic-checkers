using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Damath
{
    public class Scoreboard : MonoBehaviour
    {
        Ruleset Rules { get; set; }
        [SerializeField] private TextMeshProUGUI BlueScore;
        [SerializeField] private TextMeshProUGUI OrangeScore;

        void OnEnable()
        {
            Game.Events.OnRulesetDistribute -= ReceiveRuleset;
            Game.Events.OnPieceCapture += Compute;
        }

        void OnDisable()
        {
            Game.Events.OnRulesetDistribute -= ReceiveRuleset;
            Game.Events.OnPieceCapture -= Compute;
        }

        public void ReceiveRuleset(Ruleset rules)
        {
            Rules = rules;
        }        

        public void Refresh(Player player)
        {
            if (player.Side == Side.Bot)
            {
                BlueScore.text = player.Score.ToString();
            } else if (player.Side == Side.Top)
            {
                OrangeScore.text = player.Score.ToString();
            }
        }

        public void Compute(Move move)
        {
            float score;
            char operation;
            float x = float.Parse(move.capturingPiece.Value);
            float y = float.Parse(move.capturedPiece.Value);

            (score, operation) = move.destinationCell.Operation switch
            {
                Operation.Add => (x + y, '+'),
                Operation.Subtract => (x - y, '-'),
                Operation.Multiply => (x * y, '×'),
                Operation.Divide => (x / y, '÷'),
                _ => throw new NotImplementedException(),
            };

            Game.Console.Log($"[ACTION]: {move.capturingPiece.Value} {operation} {move.capturedPiece.Value} = {score}");

            move.SetScoreValue(score);
            AddScore(move.Player, score);
            Refresh(move.Player);
        }

        public void AddScore(Player player, float value)
        {
            player.Score += value;
        }

        public void RemoveScore(Player player, float value)
        {
            player.Score -= value;
        }

        public float GetScore(Player player)
        {
            return player.Score;
        }
    }
}