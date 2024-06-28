using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aritche.UI;
using System;
using UnityEngine.EventSystems;

namespace Aritche
{
    public enum Operation { None, Addition, Subtraction, Multiplication, Division }

    [Serializable]
    public struct CellData
    {
        public Vector2 Coordinates;
        public Operation Operation;
    }

    public class Cell : MonoBehaviour, IPointerDownHandler
    {
        public int Col { get; private set; }
        public int Row { get; private set; }
        public Vector2Int Coordinates
        {
            get
            {
                return new(Col, Row);
            }
        }
        public Operation Operation { get; private set; }
        /// <summary>
        /// The piece in this cell.
        /// </summary>
        public Piece Piece { get; private set; }
        public bool HasPiece { get; private set; }
        public bool IsValidMove { get; set; }

        public List<Sprite> Sprites;
        static GameObject prefab;

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
            HasPiece = false;
        }

        public void OnPointerDown(PointerEventData e)
        {
            Game.Events.CellClick(this, e);
        }
    }
}