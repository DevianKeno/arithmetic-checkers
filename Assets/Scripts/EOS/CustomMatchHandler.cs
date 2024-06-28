using System;
using UnityEngine;
using Aritche.Rules;
using Aritche.UI;

namespace Aritche
{
    public class CustomMatchHandler : MonoBehaviour
    {
        Ruleset ruleset;

        void Start()
        {
            ruleset = new();
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
    }
}