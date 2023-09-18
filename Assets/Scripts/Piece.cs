using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Unity.VisualScripting;

namespace Damath
{
    public struct PieceData
    {
        public string Value;
        public Side Side;
        public bool IsKing;

        public PieceData(string value, Side side, bool isKing)
        {
            Value = value;
            Side = side;
            IsKing = isKing;
        }
    }

    public class Piece : MonoBehaviour
    {
        public Cell Cell;
        public int Col, Row;
        public string Value;
        public Side Side;
        public Player Owner;
        public bool IsKing = false;
        public bool CanCapture = false;
        public bool HasCaptured = false;
        public int Forward = 0;
        public List<Move> Moves = new();
        public List<Sprite> Sprites;
        public Color Color;
        public Color Shadow;
        public bool IsSelected = false;
        public Player SelectedBy = null;
        [SerializeField] RectTransform rect;
        [SerializeField] TextMeshPro tmp;
        [SerializeField] SpriteRenderer overlayTop;
        [SerializeField] SpriteRenderer overlayShadow;
        private static GameObject prefab;

        void Awake()
        {
            prefab = Resources.Load<GameObject>("Prefabs/Chip");
        }
        
        void Start()
        {
            tmp.text = Value;

            if (IsKing)
            {
                overlayTop.sprite = Sprites[1];
            } else
            {
                overlayTop.sprite = Sprites[0];
            }
        }

        public static Piece Create(int col, int row)
        {
            Cell cell = Board.GetCell(col, row);
            return Create(cell);
        }
        
        public static Piece Create(Cell cell)
        {
            Piece newPiece = Instantiate(prefab).GetComponent<Piece>();
            newPiece.SetCell(cell);
            return newPiece;
        }

        public static Piece Create(Piece piece, int sortingLayer = 5)
        {
            Piece newPiece = Instantiate(piece.gameObject, piece.transform.parent).GetComponent<Piece>();
            // newPiece.GetComponent<SpriteRenderer>().sortingLayerID = sortingLayer;
            return newPiece;
        }

        public void SetSortingLayer(int layerID)
        {

        }

        public void Init(Side side, string value, bool isKing)
        {

        }

        public void ResetPosition()
        {
            transform.position = Cell.transform.position;
        }

        public void SetCell(Cell cell, bool move = true)
        {
            Cell = cell;
            Col = cell.Col;
            Row = cell.Row;

            if (move)
            {
                transform.position = cell.transform.position;
            }
        }

        public void SetValue(string value)
        {
            Value = value;
            tmp.SetText(value);
        }

        public void SetSide(Side value)
        {
            Side = value;
            if (value == Side.Bot)
            {
                Forward = 1;
                SetColor(Colors.RichCerulean, Colors.darkJungleBlue);
            } else if (value == Side.Top)
            {
                Forward = -1;
                SetColor(Colors.PersimmonOrange, Colors.burntUmber);
            }
        }
        
        public void SetColor(Color top, Color shadow)
        {
            overlayTop.color = top;
            overlayShadow.color = shadow;
        }

        public void SetOwner(Player player)
        {
            Owner = player;
            player.Pieces.Add(this);
        }

        public void SetKing(bool value)
        {
            if (value)
            {
                Promote();
            } else
            {
                Demote();
            }
        }
        
        public void Promote()
        {
            if (IsKing) return;
            IsKing = true;
            overlayTop.sprite = Sprites[1];
            tmp.color = new Color(1, 1, 1, 1);
        }

        public void Demote()
        {
            if (!IsKing) return;
            IsKing = false;
            overlayTop.sprite = Sprites[0];
            tmp.color = new Color(0, 0, 0, 1);
        }

        public void Capture()
        {

        }

        public void Remove()
        {
            Destroy(this.gameObject);
        }
    }
}