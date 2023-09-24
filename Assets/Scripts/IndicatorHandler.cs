using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public enum IndicatorType {Circle, Box}

    public class IndicatorHandler : MonoBehaviour
    {
        public Ruleset Rules { get; private set; }
        public List<Indicator> MoveIndicators = new();
        public List<Indicator> CaptureIndicators = new();

        [Header("Prefabs")]
        [SerializeField] GameObject _circleIndicatorPrefab;
        [SerializeField] GameObject _boxIndicatorPrefab;

        void OnEnable()
        {
            Game.Events.OnRulesetDistribute += ReceiveRuleset;
            Game.Events.OnDeselect += ClearMoveIndicators;
            Game.Events.OnPieceCapture += ClearCaptureIndicators;
            Game.Events.OnPieceDone += ClearMoveIndicators;
            Game.Events.OnBoardUpdateValidMoves += IndicateValidMoves;
            Game.Events.OnBoardUpdateCaptureables += IndicateCapturingPieces;
        }
        void OnDisable()
        {
            Game.Events.OnRulesetDistribute -= ReceiveRuleset;
            Game.Events.OnDeselect -= ClearMoveIndicators;
            Game.Events.OnPieceCapture -= ClearCaptureIndicators;
            Game.Events.OnPieceDone -= ClearMoveIndicators;
            Game.Events.OnBoardUpdateValidMoves -= IndicateValidMoves;
            Game.Events.OnBoardUpdateCaptureables -= IndicateCapturingPieces;
        }

        public void ReceiveRuleset(Ruleset rules)
        {
            Rules = rules;
        }

        public void IndicateValidMoves(List<Move> moves)
        {
            foreach (Move move in moves)
            {
                MoveIndicators.Add(CreateIndicator(IndicatorType.Circle, move.destinationCell, new Color(1, 0.92f, 0.016f, 1f)));
            }
        }

        public void IndicateCapturingPieces(List<Move> moves)
        {
            if ((bool) Rules["AllowMandatoryCapture"])
            {
                foreach (var move in moves)
                {
                    CaptureIndicators.Add(CreateIndicator(IndicatorType.Box, move.originCell, Colors.Lime));
                }
            }
        }
        
        public void ClearMoveIndicators()
        {
            foreach (Indicator i in MoveIndicators)
            {
                Destroy(i.gameObject);
            }
            MoveIndicators.Clear();
        }
        
        public void ClearMoveIndicators(Piece piece)
        {
            ClearMoveIndicators();
        }
        
        public void ClearCaptureIndicators()
        {
            foreach (Indicator i in CaptureIndicators)
            {
                Destroy(i.gameObject);
            }
            CaptureIndicators.Clear();
        }
        
        public void ClearCaptureIndicators(Move move)
        {
            ClearCaptureIndicators();
        }

        public Indicator CreateIndicator(IndicatorType indicatorType, Cell cell, Color color)
        {
            return indicatorType switch
            {
                IndicatorType.Circle => CreateIndicatorCircle(cell, color),
                IndicatorType.Box => CreateIndicatorBox(cell, color),
                _ => null,
            };
        }

        Indicator CreateIndicatorCircle(Cell cell, Color color)
        {
            var newIndicator = Instantiate(_circleIndicatorPrefab, cell.transform.position, Quaternion.identity);
            Indicator c_indicator = newIndicator.GetComponent<Indicator>();
            SpriteRenderer c_spriteRenderer = newIndicator.GetComponent<SpriteRenderer>();

            newIndicator.name = $"Indicator Circle";
            newIndicator.transform.SetParent(transform);
            newIndicator.transform.position = cell.transform.position;
            (c_indicator.Col, c_indicator.Row) = (cell.Col, cell.Row);
            c_spriteRenderer.color = color;
            newIndicator.SetActive(true);
            return c_indicator;
        }

        Indicator CreateIndicatorBox(Cell cell, Color color)
        {
            var newIndicator = Instantiate(_boxIndicatorPrefab, cell.transform.position, Quaternion.identity);
            Indicator c_indicator = newIndicator.GetComponent<Indicator>();
            SpriteRenderer c_spriteRenderer = newIndicator.GetComponent<SpriteRenderer>();

            newIndicator.name = $"Indicator Box";
            newIndicator.transform.SetParent(transform);
            newIndicator.transform.position = cell.transform.position;
            (c_indicator.Col, c_indicator.Row) = (cell.Col, cell.Row);
            c_spriteRenderer.color = color;
            newIndicator.SetActive(true);
            return c_indicator;
        }
    }
}