using System;

using UnityEngine;
using UnityEngine.UI;

using Aritche.Rules;
using Aritche.UI;

namespace Aritche
{
    public class CustomMatchHandler : MonoBehaviour
    {
        Ruleset ruleset;

        [Header("Rule Entries")]
        [SerializeField] CustomMatchRuleEntry symbolMapEntry;
        [SerializeField] CustomMatchRuleEntry chipValuesEntry;
        [SerializeField] CustomMatchRuleEntry chipMapEntry;

        void Start()
        {
            ruleset = new();
            InitializeEvents();
        }

        void InitializeEvents()
        {
            symbolMapEntry.OnInteractElement += (Selectable s, int index) =>
            {
                ruleset.SymbolMapType = (SymbolMapType) index;
                Debug.Log($"Ruleset updated. Symbol Map Type: {(SymbolMapType) index}");
            };

            chipValuesEntry.OnInteractElement += (Selectable s, int index) =>
            {
                ruleset.PieceValueType = (PieceValueType) index;
                Debug.Log($"Ruleset updated. Piece Value Type: {(PieceValueType) index}");
            };

            chipMapEntry.OnInteractElement += (Selectable s, int index) =>
            {
                ruleset.PieceMapType = (PieceMapType) index;
                Debug.Log($"Ruleset updated. Piece Map Type: {(PieceMapType) index}");
            };
        }

        public void SetFromPreset(RulesetPreset preset)
        {
            if (preset == RulesetPreset.Classic)
            {
                SetButtonsClassicPreset();
            }
            else if (preset == RulesetPreset.ClassicRandom)
            {
                SetButtonsClassicRandomPreset();
            }
            else if (preset == RulesetPreset.Speed)
            {
                SetButtonsSpeedPreset();
            }
            else if (preset == RulesetPreset.HundredPercent)
            {
                SetButtonsHundredPreset();
            }
            else if (preset == RulesetPreset.Custom)
            {
                ///
            }
        }

        public void CreateCustomRuleset(bool overwrite)
        {
            if (overwrite)
            {
                ruleset = new();
            }
            else
            {
                ruleset ??= new();
            }
        }

        public void SetPieceValuesType(int value)
        {
            ruleset.PieceValueType = (PieceValueType) value;
            Debug.Log($"[Ruleset]: Set Piece values to {(PieceValueType) value}");
        }

        public void SetPieceMapType(int value)
        {
            ruleset.PieceMapType = (PieceMapType) value;
            Debug.Log($"[Ruleset]: Set Piece map to {(PieceMapType) value}");
        }

        public void SetSymbolMapType(int value)
        {
            ruleset.SymbolMapType = (SymbolMapType) value;
            Debug.Log($"[Ruleset]: Set Symbol map to {(SymbolMapType) value}");
        }

        public Ruleset GetCachedCustomRuleset()
        {
            return ruleset;
        }

        void SetButtonsClassicPreset()
        {
            symbolMapEntry.SelectButton(0);
            chipValuesEntry.SelectButton(1);
            chipMapEntry.SelectButton(0);
        }

        void SetButtonsClassicRandomPreset()
        {
            symbolMapEntry.SelectButton(1);
            chipValuesEntry.SelectButton(1);
            chipMapEntry.SelectButton(1);
        }

        void SetButtonsSpeedPreset()
        {
            symbolMapEntry.SelectButton(0);
            chipValuesEntry.SelectButton(1);
            chipMapEntry.SelectButton(0);
        }

        void SetButtonsHundredPreset()
        {
            // symbolMapEntry.SelectButton(3);
        }
    }
}