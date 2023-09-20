using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public enum Operation {None, Add, Subtract, Multiply, Divide}

    public struct CellData
    {
        public Vector2 Coordinates;
        public Operation Operation;
    }

    public class Cell : MonoBehaviour
    {
        public Vector2 Coordinates;
        public int Col, Row;
        public Operation Operation;
        public Piece Piece = null;
        public bool HasPiece = false;
        public bool IsValidMove = false;
        public List<Sprite> Sprite;
        
        private static GameObject prefab;

        SpriteRenderer spriteRenderer;

        void Awake()
        {
            prefab = Resources.Load<GameObject>("Prefabs/Cell");

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            Game.Events.OnDeselect += Reset;
        }

        public void OnMouseEnter()
        {
            
        }

        public void Reset()
        {
            IsValidMove = false;
        }

        public static Cell Create(int col, int row)
        {
            Cell cell = Instantiate(prefab).GetComponent<Cell>();
            cell.SetColRow(col, row);

            return cell;
        }

        public void Refresh()
        {
            IsValidMove = false;
        }

        public void SetOperation(Operation value)
        {
            Operation = value;

            spriteRenderer.sprite = value switch
            {
                Operation.Add => Sprite[0],
                Operation.Subtract => Sprite[1],
                Operation.Multiply => Sprite[2],
                Operation.Divide => Sprite[3],
                _ => null,
            };
            spriteRenderer.color = Colors.darkCerulean;
        }

        public void SetColRow(int colValue, int rowValue)
        {
            Col = colValue;
            Row = rowValue;
        }

        public Piece GetPiece()
        {
            if(Piece != null)
            {
                return Piece;
            }
            return null;
        }

        public void SetPiece(Piece piece)
        {
            if (piece == null) return;
            
            Piece = piece;
            Piece.SetCell(this);
            HasPiece = true;
            IsValidMove = false;
        }
        
        public Piece RemovePiece()
        {
            var ret = Piece;
            Piece = null;
            HasPiece = false;
            return ret;
        }
    }
}