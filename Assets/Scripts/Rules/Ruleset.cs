using System;
using System.Collections.Generic;

using UnityEngine;

namespace Aritche.Rules
{
    /// <summary>
    /// Serves as a holder for the rules that define a match.
    /// </summary>
    [Serializable]
    public partial class Ruleset
    {
        [SerializeField] RulesetPreset _preset;
        public RulesetPreset Preset
        {
            get => _preset;
            set
            {
                _preset = value;
                OnUpdate?.Invoke(this);
            }
        }
        [SerializeField] PieceValueType _pieceValueType;
        public PieceValueType PieceValueType
        {
            get => _pieceValueType;
            set
            {
                SetPieceValueType(value);
                OnUpdate?.Invoke(this);
            }
        }
        [SerializeField] PieceMapType _pieceMapType;
        public PieceMapType PieceMapType
        {
            get => _pieceMapType;
            set
            {
                SetPieceMapType(value);
                OnUpdate?.Invoke(this);
            }
        }
        [SerializeField] SymbolMapType _symbolMapType;
        public SymbolMapType SymbolMapType
        {
            get => _symbolMapType;
            set
            {
                SetSymbolMapType(value);
                OnUpdate?.Invoke(this);
            }
        }
        /// <summary>
        /// The individual rules this ruleset holds.
        /// </summary>
        public Dictionary<string, object> Rules = new();
        public Dictionary<(int, int), Operation> SymbolMap = new();
        public Dictionary<(int, int), PieceData> PieceMap = new();
        public event Action<Ruleset> OnUpdate;
                
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

        string BoolToYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        string GetBoolAsString(string name)
        {
            return BoolToYesNo(GetValue<bool>(name));
        }

        public string ToRulesetInfoDisplay()
        {
            //<color=white>Preset: <color=#24415E>{Preset.ToSpacedString()}
            return @$"<color=white>Symbol map: <color=#24415E>{SymbolMapType.ToSpacedString()}
<color=white>Chip values: <color=#24415E>{PieceValueType.ToSpacedString()}
<color=white>Chip map: <color=#24415E>{PieceMapType.ToSpacedString()}
<color=white>Global timer: <color=#24415E>{GetBoolAsString("EnableGlobalTimer")}
  <color=white>Global time: <color=#24415E>{GetValue<float>("GlobalTimeSeconds")} s
<color=white>Turn timer: <color=#24415E>{GetBoolAsString("EnableTurnTimer")}
  <color=white>Turn time: <color=#24415E>{GetValue<float>("TurnTimeSeconds")} s
<color=white>Mandatory capture: <color=#24415E>{GetBoolAsString("AllowMandatoryCapture")}
<color=white>Chain capture: <color=#24415E>{GetBoolAsString("AllowChainCapture")}
<color=white>Promotion: <color=#24415E>{GetBoolAsString("AllowPromotion")}
";
        }

        public void SetSymbolMapType(SymbolMapType value)
        {
            _symbolMapType = value;

            if (value == SymbolMapType.Standard)
            {
                SymbolMap = SymbolMapStandard;
            }
            else if (value == SymbolMapType.Random)
            {
                SymbolMap = RandomizeOperationsSymmetrical(SymbolMapStandard);
            }
            else if (value == SymbolMapType.TrueRandom)
            {
                /// Unimplemented
            }
            else if (value == SymbolMapType.Custom)
            {
                /// Unimplemented
            }
        }
        
        public void SetPieceValueType(PieceValueType value)
        {
            _pieceValueType = value;

            if (value == PieceValueType.Naturals)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Integers)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Fractions)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Radicals)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Polynomials)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Binary)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Octal)
            {
                /// Unimplemented
            }
            else if (value == PieceValueType.Hexadecimal)
            {
                /// Unimplemented
            }
        }

        public void SetPieceMapType(PieceMapType value)
        {
            _pieceMapType = value;

            if (value == PieceMapType.Standard)
            {
                PieceMap = PieceMapStandard;
            }
            else if (value == PieceMapType.Random)
            {
                PieceMap = RandomizePiecesSymmetrical(PieceMapStandard);
            }
            else if (value == PieceMapType.TrueRandom)
            {
                /// Unimplemented
            }
            else if (value == PieceMapType.Custom)
            {
                /// Unimplemented
            }
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

        public void SetValue<T>(string rulename, T value)
        {
            if (TryGetRule(rulename, out Rule<T> rule))
            {
                rule.SetValue(value);
                Rules[rulename] = rule;
                OnUpdate?.Invoke(this);
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