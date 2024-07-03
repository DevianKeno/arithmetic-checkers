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
        public Dictionary<(int, int), Operation> SymbolMap { get; set; }
        public Dictionary<(int, int), PieceData> PieceMap { get; set; }
        public event Action<Ruleset> OnUpdate;

        #region Public methods

        string BoolAsYesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        string GetBoolAsString(string name)
        {
            return BoolAsYesNo(GetValue<bool>(name));
        }

        public string ToRulesetInfoDisplay()
        {
            // <color=white>Preset: <color=#24415E>{Preset.ToSpacedString()}
            return @$"<color=white>Symbol map: <color=#24415E>{SymbolMapType.ToSpacedString()}
<color=white>Chip values: <color=#24415E>{PieceValueType.ToSpacedString()}
<color=white>Chip map: <color=#24415E>{PieceMapType.ToSpacedString()}
<color=white>Mandatory capture: <color=#24415E>{GetBoolAsString("AllowMandatoryCapture")}
<color=white>Chain capture: <color=#24415E>{GetBoolAsString("AllowChainCapture")}
<color=white>Promotion: <color=#24415E>{GetBoolAsString("AllowPromotion")}
<color=white>Global timer: <color=#24415E>{GetBoolAsString("EnableGlobalTimer")}
  <color=white>Global time: <color=#24415E>{GetValue<float>("GlobalTimeSeconds")} s
<color=white>Turn timer: <color=#24415E>{GetBoolAsString("EnableTurnTimer")}
  <color=white>Turn time: <color=#24415E>{GetValue<float>("TurnTimeSeconds")} s
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
                // SymbolMap = RandomizeOperations(SymbolMapStandard);
            }
            else if (value == SymbolMapType.Custom)
            {
                /// Unimplemented
            }
        }
        
        public void SetPieceValueType(PieceValueType value)
        {
            _pieceValueType = value;
            var map = PieceMapStandard;

            foreach (PieceData data in map.Values)
            {
                data.ValueType = value;
            }
            PieceMap = map;
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
                PieceMap = RandomizePieces(PieceMapStandard);
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
            rule = default;
            if (Rules.ContainsKey(rulename))
            {
                rule = (Rule<T>) Rules[rulename];
                return true;
            }
            return false;
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

        public bool TryGetValue<T>(string rulename, out T value)
        {
            value = default;
            if (Rules.ContainsKey(rulename))
            {
                value = ((Rule<T>) Rules[rulename]).Value;
                return true;
            }
            return false;
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