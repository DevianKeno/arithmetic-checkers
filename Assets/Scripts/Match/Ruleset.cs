using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aritche.Rules
{
    public struct Rule<T>
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public T Value { get; private set; }

        public Rule(int id, string name, T value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        public void SetValue(T value)
        {
            Value = value;
        }
    }

    public enum PieceValueType {
        Naturals,
        Integers,
        Fractions,
        Radicals,
        Polynomials,
        Binary,
        Octal,
        Hexadecimal
    }

    public enum PieceMapType {
        Standard,
        RandomSymmetrical,
        Random,
        Custom,
    }

    public enum SymbolMapType {
        Standard,
        RandomSymmetrical,
        Random,
        Custom
    }

    /// <summary>
    /// Pertains which predefined ruleset a ruleset is based on.
    /// </summary>
    public enum RulesetPreset { Classic, ClassicRandom, Speed, HundredPercent, Custom }

    /// <summary>
    /// Determines which/who are players are participating in the match. 
    /// </summary>
    public enum RulesetPlayersType { Solo, TwoPlayer, VersusAI }
        
    /// <summary>
    /// Serves as a holder for the rules that define a match. 
    /// Rule values can be accessed by indexing the rule name.
    /// </summary>
    [Serializable]
    public class Ruleset
    {
        public RulesetPreset Preset { get; set; }
        public PieceValueType PieceValueType { get; set; }
        public PieceMapType PieceMapType { get; set; }
        public SymbolMapType SymbolMapType { get; set; }
        /// <summary>
        /// The individual rules this ruleset holds.
        /// </summary>
        public Dictionary<string, object> Rules = new();
        public Dictionary<(int, int), Operation> SymbolMap = new();
        public Dictionary<(int, int), PieceData> PieceMap = new();


        /// <summary>
        /// These data is initialized from files at every start of the program (game).
        /// </summary>
        #region Predefined data

        static Dictionary<(int, int), Operation> SymbolMapStandard { get; set; }
        static Dictionary<(int, int), PieceData> PieceMapStandard { get; set; }
        
        public static Ruleset Standard { get; private set; }
        public static Ruleset Speed { get; private set; }

        #endregion
                
        /// <summary>
        /// Initializing function. This runs when the game is first being loaded.
        /// </summary>
        public static void Init()
        {
            Game.Console.LogDebug("Initializing rulesets");
            
            /// These should read from a file that's easier to read and handle            
            // foreach (var entry in data)
            // {
            //     dict.Add(entry.cell, entry.data);
            // }
            //

            SymbolMapStandard = new()
            {
                { (1, 0), Operation.Addition},
                { (3, 0), Operation.Subtraction},
                { (5, 0), Operation.Division},
                { (7, 0), Operation.Multiplication},
                { (0, 1), Operation.Subtraction},
                { (2, 1), Operation.Addition},
                { (4, 1), Operation.Multiplication},
                { (6, 1), Operation.Division},
                { (1, 2), Operation.Division},
                { (3, 2), Operation.Multiplication},
                { (5, 2), Operation.Addition},
                { (7, 2), Operation.Subtraction},
                { (0, 3), Operation.Multiplication},
                { (2, 3), Operation.Division},
                { (4, 3), Operation.Subtraction},
                { (6, 3), Operation.Addition},
                { (1, 4), Operation.Addition},
                { (3, 4), Operation.Subtraction},
                { (5, 4), Operation.Division},
                { (7, 4), Operation.Multiplication},
                { (0, 5), Operation.Subtraction},
                { (2, 5), Operation.Addition},
                { (4, 5), Operation.Multiplication},
                { (6, 5), Operation.Division},
                { (1, 6), Operation.Division},
                { (3, 6), Operation.Multiplication},
                { (5, 6), Operation.Addition},
                { (7, 6), Operation.Subtraction},
                { (0, 7), Operation.Multiplication},
                { (2, 7), Operation.Division},
                { (4, 7), Operation.Subtraction},
                { (6, 7), Operation.Addition}
            };

            PieceMapStandard = new()
            {
                /// Bottom pieces
                { (1, 0), new ("-11", Side.Blue, false)},
                { (3, 0), new ("8", Side.Blue, false)},
                { (5, 0), new ("-5", Side.Blue,false)},
                { (7, 0), new ("2", Side.Blue, false)},
                { (0, 1), new ("0", Side.Blue, false)},
                { (2, 1), new ("-3", Side.Blue, false)},
                { (4, 1), new ("10", Side.Blue, false)},
                { (6, 1), new ("-7", Side.Blue, false)},
                { (1, 2), new ("-9", Side.Blue, false)},
                { (3, 2), new ("6", Side.Blue, false)},
                { (5, 2), new ("-1", Side.Blue, false)},
                { (7, 2), new ("4", Side.Blue, false)},

                /// Top pieces
                { (0, 5), new ("4", Side.Orange, false)},
                { (2, 5), new ("-1", Side.Orange, false)},
                { (4, 5), new ("6", Side.Orange, false)},
                { (6, 5), new ("-9", Side.Orange, false)},
                { (1, 6), new ("-7", Side.Orange, false)},
                { (3, 6), new ("10", Side.Orange, false)},
                { (5, 6), new ("-3", Side.Orange, false)},
                { (7, 6), new ("0", Side.Orange, false)},
                { (0, 7), new ("2", Side.Orange, false)},
                { (2, 7), new ("-5", Side.Orange, false)},
                { (4, 7), new ("8", Side.Orange, false)},
                { (6, 7), new ("-11", Side.Orange, false)}
            };

            /// These are pre-defined rulesets and
            /// must be first initialized at the start of the program
            Standard = CreateClassic();
            Speed = CreateSpeed();
            
            Game.Console.LogDebug("Initialized rulesets");
        }

        static Dictionary<(int, int), Operation> RandomizeOperationsSymmetrical(Dictionary<(int, int), Operation> basis)
        {
            var random = new System.Random();
            var symbolMap = new Dictionary<(int, int), Operation>();
            
            var operations = new List<Operation>(basis.Values);
            var coordinates = new List<(int, int)>(basis.Keys);
            
            /// Shuffle
            for (int i = operations.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (operations[j], operations[i]) = (operations[i], operations[j]);
            }
            
            for (int i = 0; i < coordinates.Count / 2; i++)
            {
                symbolMap[coordinates[i]] = operations[i];
                var rotatedCoordinate = (7 - coordinates[i].Item1, 7 - coordinates[i].Item2);
                symbolMap[rotatedCoordinate] = operations[i];
            }

            return symbolMap;
        }

        static Dictionary<(int, int), PieceData> RandomizePiecesSymmetrical(Dictionary<(int, int), PieceData> basis)
        {
            var random = new System.Random();
            var pieceMap = new Dictionary<(int, int), PieceData>();

            var bluePieces = new List<PieceData>();
            var orangePieces = new List<PieceData>();

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                    bluePieces.Add(kvp.Value);
                else if (kvp.Value.Side == Side.Orange)
                    orangePieces.Add(kvp.Value);
            }

            ShuffleList(random, bluePieces);
            ShuffleList(random, orangePieces);

            int blueIndex = 0;
            int orangeIndex = 0;

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                {
                    pieceMap[kvp.Key] = bluePieces[blueIndex];
                    // pieceMap[(7 - kvp.Key.Item1, 7 - kvp.Key.Item2)] = bluePieces[blueIndex];
                    blueIndex++;
                }
                else if (kvp.Value.Side == Side.Orange)
                {
                    pieceMap[kvp.Key] = orangePieces[orangeIndex];
                    // pieceMap[(7 - kvp.Key.Item1, 7 - kvp.Key.Item2)] = orangePieces[orangeIndex];
                    orangeIndex++;
                }
            }

            return pieceMap;
        }

        static void ShuffleList<T>(System.Random random, List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        #region Public methods

        /// <summary>
        /// Create a ruleset given a ruleset type.
        /// </summary>
        /// <returns> A ruleset object. </returns>
        public static Ruleset CreateFromPreset(RulesetPreset preset)
        {
            return preset switch
            {
                RulesetPreset.Classic => CreateClassic(),
                RulesetPreset.ClassicRandom => CreateClassicRandom(),
                RulesetPreset.Speed => CreateSpeed(),
                RulesetPreset.Custom => CreateCustom(),
                _ => null,
            };
        }

        public static Ruleset CreateClassic()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.Classic,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.Standard,
                SymbolMapType = SymbolMapType.Standard,
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
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }
        
        public static Ruleset CreateClassicRandom()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.ClassicRandom,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.RandomSymmetrical,
                SymbolMapType = SymbolMapType.RandomSymmetrical,
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
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = RandomizeOperationsSymmetrical(SymbolMapStandard);
            ruleset.PieceMap = RandomizePiecesSymmetrical(PieceMapStandard);

            return ruleset;
        }

        public static Ruleset CreateSpeed()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.Speed,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.Standard,
                SymbolMapType = SymbolMapType.Standard,
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
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }

        public static Ruleset CreateCustom()
        {
            Ruleset ruleset = CreateClassic();
            ruleset.Preset = RulesetPreset.Custom;

            return ruleset;
        }

        public void AddRule<T>(int id, string name, T value)
        {
            Rules.Add(name, new Rule<T>(id, name, value));
        }

        public void AddRule<T>(Rule<T> rule)
        {
            Rules.Add(rule.Name, rule);
        }

        public bool TryGetRule<T>(string rulename, out Rule<T> rule)
        {
            if (Rules.ContainsKey(rulename))
            {
                rule = (Rule<T>) Rules[rulename];
                return true;
            } else
            {                
                rule = default;
                return false;
            }
        }

        public object GetRule(string rulename)
        {
            if (Rules.ContainsKey(rulename))
            {
                return Rules[rulename];
            }
            return default;
        }

        public T GetValue<T>(string rulename)
        {
            var rule = (Rule<T>) GetRule(rulename);
            return rule.Value;
        }

        public void SetRule<T>(string rulename, T value)
        {
            if (TryGetRule(rulename, out Rule<T> rule))
            {
                rule.SetValue(value);
                Rules[rulename] = rule;
            }
        }

        public int GetRulesetType()
        {
            return (int) Preset;
        }

        // public new string ToString()
        // {
        //     string rulesetData = "";

        //     foreach (var r in Rules)
        //     {
        //         object rule = r.Value;
        //         rulesetData += $";{rule.Id},{rule.Value}";
        //     }

        //     return rulesetData;
        // }

        #endregion
    }
}