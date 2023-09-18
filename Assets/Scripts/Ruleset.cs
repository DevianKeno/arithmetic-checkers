using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Damath
{
    public static class XmlHelper
    {

    }
    
    public class Rule
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public object Value { get; private set; }

        public static Rule Create(string name, object value)
        {
            return new ()
            {
                Name = name,
                Value = value
            };
        }

        public object GetValue()
        {
            return Value;
        }

        public void SetValue(object value)
        {
            Value = value;
        }
    }
    
    /// <summary>
    /// A ruleset defines the rules for a match.
    /// </summary>
    public class Ruleset
    {
        public Dictionary<string, Rule> Rules = new ();
        public bool EnableCheats;
        public bool EnableCapture;
        public bool EnableMandatoryCapture;
        public bool EnableChainCapture;
        public bool EnablePromotion;
        public bool EnableTouchMove;
        public bool EnableScoring;
        public bool EnableTimer;
        public bool EnableTurnTimer;
        public bool EnableGlobalTimer;
        public float GlobalTimerSeconds;
        public float TurnTimerSeconds;

        public Side FirstTurn;
        
        public Cellmap<Operation> Symbols = new();
        public Cellmap<(Side, string, bool)> Pieces = new();

        public Dictionary<(int, int), Operation> SymbolMapStandard = new()
        {
            {(1, 0), Operation.Add},
            {(3, 0), Operation.Sub},
            {(5, 0), Operation.Div},
            {(7, 0), Operation.Mul},
            {(0, 1), Operation.Sub},
            {(2, 1), Operation.Add},
            {(4, 1), Operation.Mul},
            {(6, 1), Operation.Div},
            {(1, 2), Operation.Div},
            {(3, 2), Operation.Mul},
            {(5, 2), Operation.Add},
            {(7, 2), Operation.Sub},
            {(0, 3), Operation.Mul},
            {(2, 3), Operation.Div},
            {(4, 3), Operation.Sub},
            {(6, 3), Operation.Add},
            {(1, 4), Operation.Add},
            {(3, 4), Operation.Sub},
            {(5, 4), Operation.Div},
            {(7, 4), Operation.Mul},
            {(0, 5), Operation.Sub},
            {(2, 5), Operation.Add},
            {(4, 5), Operation.Mul},
            {(6, 5), Operation.Div},
            {(1, 6), Operation.Div},
            {(3, 6), Operation.Mul},
            {(5, 6), Operation.Add},
            {(7, 6), Operation.Sub},
            {(0, 7), Operation.Mul},
            {(2, 7), Operation.Div},
            {(4, 7), Operation.Sub},
            {(6, 7), Operation.Add},
        };

        public Dictionary<(int, int), (Side, string, bool)> PieceMapStandard = new()
        {
            {(1, 0), (Side.Bot, "-11", false)},
            {(3, 0), (Side.Bot, "8", false)},
            {(5, 0), (Side.Bot, "-5", false)},
            {(7, 0), (Side.Bot, "2", false)},
            {(0, 1), (Side.Bot, "0", false)},
            {(2, 1), (Side.Bot, "-3", false)},
            {(4, 1), (Side.Bot, "10", false)},
            {(6, 1), (Side.Bot, "-7", false)},
            {(1, 2), (Side.Bot, "-9", false)},
            {(3, 2), (Side.Bot, "6", false)},
            {(5, 2), (Side.Bot, "-1", false)},
            {(7, 2), (Side.Bot, "4", false)},

            {(0, 5), (Side.Top, "4", false)},
            {(2, 5), (Side.Top, "-1", false)},
            {(4, 5), (Side.Top, "6", false)},
            {(6, 5), (Side.Top, "-9", false)},
            {(1, 6), (Side.Top, "-7", false)},
            {(3, 6), (Side.Top, "10", false)},
            {(5, 6), (Side.Top, "-3", false)},
            {(7, 6), (Side.Top, "0", false)},
            {(0, 7), (Side.Top, "2", false)},
            {(2, 7), (Side.Top, "-5", false)},
            {(4, 7), (Side.Top, "8", false)},
            {(6, 7), (Side.Top, "-11", false)}
        };

        public Ruleset()
        {
            SetStandard();
        }

        public static Ruleset CreateStandard()
        {
            Ruleset ruleset = new();

            ruleset.AddRule(0, "enableCapture", true);
            Rule.Create("enableMandatoryCapture", true);
            Rule.Create("enableChainCapture", true);
            Rule.Create("enablePromotion", true);
            Rule.Create("enableTouchMove", true);

            return new Ruleset();
        }

        public void AddRule(int id, string name, object value)
        {
            //Rules.Add(name, new(id, name, value));
        }

        public object GetRule(string rule)
        {
            return Rules[rule].GetValue();
        }

        public new string ToString()
        {
            return $"";
        }

        public void SetRule(string rule, object value)
        {
            Rules[rule].SetValue(value);
        }

        public void RetreiveRules()
        {
            
        }

        public void SetStandard()
        {
            EnableMandatoryCapture = true;
            EnablePromotion = true;
            EnableChainCapture = true;
            EnableCapture = true;
            EnableTouchMove = false;
            EnableTimer = true;
            EnableTurnTimer = true;
            EnableGlobalTimer = true;
            GlobalTimerSeconds = 1200f;
            TurnTimerSeconds = 60f;
            FirstTurn = Side.Bot;
            EnableCheats = false;

            Pieces.SetMap(PieceMapStandard);
            Symbols.SetMap(SymbolMapStandard);
        }

        public void SetSpeed()
        {
            EnableMandatoryCapture = true;
            EnablePromotion = true;
            EnableChainCapture = true;
            EnableCapture = true;
            EnableTouchMove = false;
            EnableTimer = true;
            EnableTurnTimer = true;
            EnableGlobalTimer = true;
            GlobalTimerSeconds = 300f;
            TurnTimerSeconds = 15f;
            FirstTurn = Side.Bot;
            EnableCheats = true;

            Pieces.SetMap(PieceMapStandard);
            Symbols.SetMap(SymbolMapStandard);
        }
    }
}