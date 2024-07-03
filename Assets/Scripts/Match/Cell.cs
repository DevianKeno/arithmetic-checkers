using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using Aritche.UI;

namespace Aritche
{
    public enum Operation { None, Addition, Subtraction, Multiplication, Division }

    [Serializable]
    public struct CellData
    {
        public Vector2 Coordinates { get; set; }
        public Operation Operation { get; set; }
    }

    public class Cell : MonoBehaviour
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public Vector2Int Coordinates
        {
            get
            {
                return new (Col, Row);
            }
        }
        public Operation Operation { get; private set; }
        /// <summary>
        /// The piece in this cell.
        /// </summary>
        public Piece Piece { get; private set; }
        public bool HasPiece
        {
            get
            {
                return Piece != null;
            }
        }
        public bool IsValidMove { get; internal set;}

        public event EventHandler<PointerEventData> OnClick;

        public List<Sprite> Sprites;
        static GameObject prefab;

        SpriteRenderer spriteRenderer;

        void Awake()
        {
            prefab = Resources.Load<GameObject>("Prefabs/Cell");

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public static Cell Create(int col, int row)
        {
            Cell cell = Instantiate(prefab).GetComponent<Cell>();
            cell.SetColRow(col, row);

            return cell;
        }

        public void SetOperation(Operation value)
        {
            Operation = value;

            spriteRenderer.sprite = value switch
            {
                Operation.Addition => Sprites[0],
                Operation.Subtraction => Sprites[1],
                Operation.Multiplication => Sprites[2],
                Operation.Division => Sprites[3],
                _ => null,
            };
            spriteRenderer.color = Colors.darkCerulean;
        }

        public void SetColRow(int colValue, int rowValue)
        {
            Col = colValue;
            Row = rowValue;
        }

        public void SetPiece(Piece piece)
        {
            if (piece == null) return;
            
            Piece = piece;
            Piece.SetCell(this, setPosition: true);
        }

        public void RemovePiece()
        {
            if (Piece != null)
            {
                Piece.Remove();
            }
            ClearPiece();
        }
        
        public void ClearPiece()
        {
            Piece = null;
        }

        public void OnPointerDown(PointerEventData e)
        {
            OnClick?.Invoke(this, e);
        }
    }
}