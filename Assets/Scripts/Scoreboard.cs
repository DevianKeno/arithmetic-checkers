using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Damath
{
    /// <summary>
    /// This class imlpements methods for score evalution.
    /// </summary>
    public class Score
    {
        public float Value { get; private set; }

        public float Get()
        {
            return Value;
        }

        public void Set(float value)
        {
            Value = value;
        }

        public void Add(float value)
        {
            Value += value;
        }

        public void Remove(float value)
        {
            Value -= value;
        }

        public Score CompareHigher(Score score)
        {
            if (score.Get() > Value)
            {
                return score;
            }

            return this;
        }
    }

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
            if (Game.Settings.EnableDebugMode) Game.Console.Log("[Debug: Scoreboard]: Received ruleset");

            Rules = rules;
            Init();
        }

        public void Init()
        {
            if (Rules == null)
            {
                Game.Console.Log("[Debug: Scoreboard]: Failed to initialize Scoreboard, no ruleset");
                return;
            }

            if (Game.Settings.EnableDebugMode) Game.Console.Log("[Debug: Scoreboard]: Initializing Scoreboard");

        }

        public void Refresh(Player player)
        {
            if (player.Side == Side.Bot)
            {
                BlueScore.text = player.fScore.ToString();
            } else if (player.Side == Side.Top)
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
                Operation.Add => (x + y, '+'),
                Operation.Subtract => (x - y, '-'),
                Operation.Multiply => (x * y, '×'),
                Operation.Divide => (x / y, '÷'),
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