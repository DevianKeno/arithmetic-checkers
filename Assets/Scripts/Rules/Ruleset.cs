using System;
using System.Collections.Generic;
using Epic.OnlineServices.Presence;

namespace Aritche.Rules
{        
    /// <summary>
    /// Serves as a holder for the rules that define a match.
    /// </summary>
    [Serializable]
    public partial class Ruleset
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
        /// Initializing function. This runs when the game is first being loaded.
        /// </summary>
        public static void Init()
        {
            CreatePredefined();
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

        public string ToRulesetInfoDisplay()
        {
            string info = "Symbol map: ";
            return info;
        }

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
                PieceMapType = PieceMapType.Random,
                SymbolMapType = SymbolMapType.Random,
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