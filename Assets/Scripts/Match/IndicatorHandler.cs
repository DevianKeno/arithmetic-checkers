using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Aritche.Rules;
using Aritche.UI;

namespace Aritche
{
    public enum IndicatorShape { Circle, Box }
    public enum IndicatedCell { Origin, Destination }
    public enum IndicatorType { Move, Capture }

    public class IndicatorHandler : MonoBehaviour
    {
        public List<Indicator> MoveIndicators = new();
        public List<Indicator> CaptureIndicators = new();

        [Header("Prefabs")]
        [SerializeField] GameObject _circleIndicatorPrefab;
        [SerializeField] GameObject _boxIndicatorPrefab;
        
        public List<Indicator> CreateIndicators(List<MovePerformInfo> moves, IndicatedCell indicated, IndicatorType type, IndicatorShape shape, Color color)
        {
            var indicators = new List<Indicator>();

            foreach (MovePerformInfo move in moves)
            {
                var cell = indicated switch
                {
                    IndicatedCell.Origin => move.Origin,
                    IndicatedCell.Destination => move.Destination,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var indicator = CreateIndicator(cell: cell, type: type, shape: shape, color: color);
                indicators.Add(indicator);
            }
            return indicators;
        }

        public void CreateMoveIndicators(List<MovePerformInfo> moves, IndicatorShape shape, Color color)
        {
            MoveIndicators = CreateIndicators(
                moves: moves,
                indicated: IndicatedCell.Destination,
                type: IndicatorType.Move,
                shape: shape,
                color: color
            );
        }

        public void CreateCaptureIndicators(List<MovePerformInfo> moves, IndicatorShape shape, Color color)
        {
            CaptureIndicators = CreateIndicators(
                moves: moves,
                indicated: IndicatedCell.Origin,
                type: IndicatorType.Capture,
                shape: shape,
                color: color
            );
        }

        /// <summary>
        /// Makes the capture indicators flash.
        /// </summary>
        public void HighlightCaptures()
        {
            foreach (Indicator indicator in CaptureIndicators)
            {
                indicator.Flash();
            }
        }
        
        public void ClearMoveIndicators()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Indicator>(out var indicator))
                {
                    if (indicator.Type == IndicatorType.Move)
                    {
                        indicator.Destroy();
                    }
                }
            }
            MoveIndicators.Clear();
        }
        
        public void ClearCaptureIndicators()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Indicator>(out var indicator))
                {
                    if (indicator.Type == IndicatorType.Capture)
                    {
                        indicator.Destroy();
                    }
                }
            }
            CaptureIndicators.Clear();
        }

        public Indicator CreateIndicator(Cell cell, IndicatorType type, IndicatorShape shape, Color color)
        {
            var prefab = shape switch
            {
                IndicatorShape.Circle => _circleIndicatorPrefab,
                IndicatorShape.Box => _boxIndicatorPrefab,
            };

            var go = Instantiate(prefab, transform);
            go.name = $"Indicator ({shape})";

            Indicator indicator = go.GetComponent<Indicator>();
            indicator.MoveToCell(cell);
            indicator.Color = color;
            indicator.Type = type;
            return indicator;
        }
    }
}