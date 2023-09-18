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

        public Rule(int id, string name, object value)
        {
            Id = id;
            Name = name;
            Value = value;
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
        public Cellmap<PieceData> Pieces = new();

        public static Dictionary<(int, int), Operation> SymbolMapStandard = new()
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

        public static Dictionary<(int, int), PieceData> PieceMapStandard = new()
        {
            {(1, 0), new("-11", Side.Bot, false)},
            {(3, 0), new("8", Side.Bot, false)},
            {(5, 0), new("-5", Side.Bot,false)},
            {(7, 0), new("2", Side.Bot, false)},
            {(0, 1), new("0", Side.Bot, false)},
            {(2, 1), new("-3", Side.Bot, false)},
            {(4, 1), new("10", Side.Bot, false)},
            {(6, 1), new("-7", Side.Bot, false)},
            {(1, 2), new("-9", Side.Bot, false)},
            {(3, 2), new("6", Side.Bot, false)},
            {(5, 2), new("-1", Side.Bot, false)},
            {(7, 2), new("4", Side.Bot, false)},

            {(0, 5), new("4", Side.Top, false)},
            {(2, 5), new("-1", Side.Top, false)},
            {(4, 5), new("6", Side.Top, false)},
            {(6, 5), new("-9", Side.Top, false)},
            {(1, 6), new("-7", Side.Top, false)},
            {(3, 6), new("10", Side.Top, false)},
            {(5, 6), new("-3", Side.Top, false)},
            {(7, 6), new("0", Side.Top, false)},
            {(0, 7), new("2", Side.Top, false)},
            {(2, 7), new("-5", Side.Top, false)},
            {(4, 7), new("8", Side.Top, false)},
            {(6, 7), new("-11", Side.Top, false)}
        };

        public Ruleset()
        {
            SetStandard();
        }

        public static Ruleset CreateStandard()
        {
            Ruleset ruleset = new();

            ruleset.AddRule(0, "enableCapture", true);
            ruleset.AddRule(1, "enableMandatoryCapture", true);
            ruleset.AddRule(2, "enableChainCapture", true);
            ruleset.AddRule(3, "EnablePromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", true);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableTurnTimer", true);
            ruleset.AddRule(8, "EnableGlobalTimer", true);
            ruleset.AddRule(9, "GlobalTimerSeconds", 1200f);
            ruleset.AddRule(10, "TurnTimerSeconds", 60f);

            // ruleset.SetPieceMap(PieceMapStandard);

            return new Ruleset();
        }

        public void AddRule(int id, string name, object value)
        {
            Rules.Add(name, new(id, name, value));
        }

        public object GetRule(string rule)
        {
            return Rules[rule].GetValue();
        }

        public void SetPieceMap(Cellmap<PieceData> value)
        {
            Pieces = value;
        }

        public void SetSymbolMap(Cellmap<Cell> value)
        {

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

            // Pieces.SetMap(PieceMapStandard);
            // Symbols.SetMap(SymbolMapStandard);
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

            // Pieces.SetMap(PieceMapStandard);
            // Symbols.SetMap(SymbolMapStandard);
        }
    }
}