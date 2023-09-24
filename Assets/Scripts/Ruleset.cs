using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public static class XmlHelper
    {

    }
    
    public struct Rule
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
    }

    public enum RulesetType { Standard, Speed, Custom }
    
    /// <summary>
    /// Defines the rules for a match. 
    /// Rule values can be accessed by indexing the rule name.
    /// </summary>
    public class Ruleset
    {
        public Dictionary<string, Rule> Rules = new ();
        public RulesetType Mode;
        public Dictionary<Vector2, Operation> SymbolMap = new();
        public Dictionary<Vector2, PieceData> PieceMap = new();

        static Dictionary<Vector2, Operation> SymbolMapStandard { get; set; }
        static Dictionary<Vector2, PieceData> PieceMapStandard { get; set; }
        
        public static Ruleset Standard { get; private set; }
        public static Ruleset Speed { get; private set; }
        
        public object this[string name, bool value = true]
        {            
            get
            {
                if (value)
                {
                    return Rules[name].Value;
                } else
                {
                    return Rules[name];
                }
            }
        }
        
        public static void Init()
        {
            if (Settings.EnableDebugMode)
            {
                Game.Console.Log("Initializing rulesets");
            }

            // 
            // These should read from a file that's easier to read and handle
            //
            // foreach (var entry in data)
            // {
            //     dict.Add(entry.cell, entry.data);
            // }
            //

            SymbolMapStandard = new()
            {
                { new (1, 0), Operation.Add},
                { new (3, 0), Operation.Subtract},
                { new (5, 0), Operation.Divide},
                { new (7, 0), Operation.Multiply},
                { new (0, 1), Operation.Subtract},
                { new (2, 1), Operation.Add},
                { new (4, 1), Operation.Multiply},
                { new (6, 1), Operation.Divide},
                { new (1, 2), Operation.Divide},
                { new (3, 2), Operation.Multiply},
                { new (5, 2), Operation.Add},
                { new (7, 2), Operation.Subtract},
                { new (0, 3), Operation.Multiply},
                { new (2, 3), Operation.Divide},
                { new (4, 3), Operation.Subtract},
                { new (6, 3), Operation.Add},
                { new (1, 4), Operation.Add},
                { new (3, 4), Operation.Subtract},
                { new (5, 4), Operation.Divide},
                { new (7, 4), Operation.Multiply},
                { new (0, 5), Operation.Subtract},
                { new (2, 5), Operation.Add},
                { new (4, 5), Operation.Multiply},
                { new (6, 5), Operation.Divide},
                { new (1, 6), Operation.Divide},
                { new (3, 6), Operation.Multiply},
                { new (5, 6), Operation.Add},
                { new (7, 6), Operation.Subtract},
                { new (0, 7), Operation.Multiply},
                { new (2, 7), Operation.Divide},
                { new (4, 7), Operation.Subtract},
                { new (6, 7), Operation.Add}
            };

            PieceMapStandard = new()
            {
                // Bottom pieces
                { new (1, 0), new ("-11", Side.Bot, false)},
                { new (3, 0), new ("8", Side.Bot, false)},
                { new (5, 0), new ("-5", Side.Bot,false)},
                { new (7, 0), new ("2", Side.Bot, false)},
                { new (0, 1), new ("0", Side.Bot, false)},
                { new (2, 1), new ("-3", Side.Bot, false)},
                { new (4, 1), new ("10", Side.Bot, false)},
                { new (6, 1), new ("-7", Side.Bot, false)},
                { new (1, 2), new ("-9", Side.Bot, false)},
                { new (3, 2), new ("6", Side.Bot, false)},
                { new (5, 2), new ("-1", Side.Bot, false)},
                { new (7, 2), new ("4", Side.Bot, false)},

                // Top pieces
                { new (0, 5), new ("4", Side.Top, false)},
                { new (2, 5), new ("-1", Side.Top, false)},
                { new (4, 5), new ("6", Side.Top, false)},
                { new (6, 5), new ("-9", Side.Top, false)},
                { new (1, 6), new ("-7", Side.Top, false)},
                { new (3, 6), new ("10", Side.Top, false)},
                { new (5, 6), new ("-3", Side.Top, false)},
                { new (7, 6), new ("0", Side.Top, false)},
                { new (0, 7), new ("2", Side.Top, false)},
                { new (2, 7), new ("-5", Side.Top, false)},
                { new (4, 7), new ("8", Side.Top, false)},
                { new (6, 7), new ("-11", Side.Top, false)}
            };

            Standard = CreateStandard();
            Speed = CreateSpeed();
            
            if (Settings.EnableDebugMode)
            {
                Game.Console.Log("Initialized rulesets");
            }
        }

        public static Ruleset Create(RulesetType mode)
        {
            if (mode == RulesetType.Standard)
            {
                return CreateStandard();

            } else if (mode == RulesetType.Speed)
            {
                return CreateSpeed();
            
            } else if (mode == RulesetType.Custom)
            {
                // return CreateCustom();
            }
            return null;
        }

        public static Ruleset CreateStandard()
        {
            Ruleset ruleset = new()
            {
                Mode = RulesetType.Standard
            };

            ruleset.AddRule(0, "AllowCapture", true);
            ruleset.AddRule(1, "AllowMandatoryCapture", true);
            ruleset.AddRule(2, "AllowChainCapture", true);
            ruleset.AddRule(3, "AllowPromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", false);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableGlobalTimer", true);
            ruleset.AddRule(8, "EnableTurnTimer", true);
            ruleset.AddRule(9, "GlobalTimerSeconds", 1200f);
            ruleset.AddRule(10, "TurnTimerSeconds", 60f);
            ruleset.AddRule(11, "FirstTurn", Side.Bot);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }
        
        public static Ruleset CreateSpeed()
        {
            Ruleset ruleset = new()
            {
                Mode = RulesetType.Speed
            };

            ruleset.AddRule(0, "AllowCapture", true);
            ruleset.AddRule(1, "AllowMandatoryCapture", true);
            ruleset.AddRule(2, "AllowChainCapture", true);
            ruleset.AddRule(3, "AllowPromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", false);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableGlobalTimer", true);
            ruleset.AddRule(8, "EnableTurnTimer", true);
            ruleset.AddRule(9, "GlobalTimerSeconds", 300f);
            ruleset.AddRule(10, "TurnTimerSeconds", 15f);
            ruleset.AddRule(11, "FirstTurn", Side.Bot);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }

        public void AddRule(int id, string name, object value)
        {
            Rules.Add(name, new(id, name, value));
        }

        public void AddRule(Rule rule)
        {
            Rules.Add(rule.Name, rule);
        }

        public object GetRule(string rule)
        {
            return Rules[rule].Value;
        }

        public new string ToString()
        {
            string rulesetData = "";

            foreach (var r in Rules)
            {
                Rule rule = r.Value;
                rulesetData += $";{rule.Id},{rule.Value}";
            }

            return rulesetData;
        }
    }
}