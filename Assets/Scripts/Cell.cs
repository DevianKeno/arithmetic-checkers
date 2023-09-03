using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public enum Operation {None, Add, Sub, Mul, Div}

    public class Cell : MonoBehaviour
    {
        public int number;
        public int col, row;
        public Piece piece = null;
        public bool HasPiece = false;
        public bool IsValidMove = false;
        public Operation operation;
        public List<Sprite> sprite;

        SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Refresh()
        {
            IsValidMove = false;
        }

        public void SetOperation(Operation value)
        {
            operation = value;

            switch (value)
            {
                case Operation.Add:
                    spriteRenderer.sprite = sprite[0]; 
                    break;
                case Operation.Sub:
                    spriteRenderer.sprite = sprite[1]; 
                    break;
                case Operation.Mul:
                    spriteRenderer.sprite = sprite[2]; 
                    break;
                case Operation.Div:
                    spriteRenderer.sprite = sprite[3]; 
                    break;
                default:
                    spriteRenderer.sprite = null; 
                    break;
            }
            spriteRenderer.color = Colors.darkCerulean;
        }

        public void SetColRow(int colValue, int rowValue)
        {
            col = colValue;
            row = rowValue;
        }

        public Piece GetPiece()
        {
            if( piece != null)
            {
                return piece;
            }
            return null;
        }

        public void SetPiece(Piece piece)
        {
            if (piece == null) return;
            
            this.piece = piece;
            HasPiece = true;
        }
        
        public void RemovePiece()
        {
            piece = null;
            HasPiece = false;
        }
    }
}