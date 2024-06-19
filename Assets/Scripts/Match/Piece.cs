using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Damath.UI;
using Damath.Players;

namespace Damath
{
    /// <summary>
    /// This class implements conversion methods to convert a "string value" to a computable score value.
    /// </summary>
    public class PieceValue
    {

    }

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
        Cell cell;
        public Cell Cell => cell;
        public int Col => Cell.Col;
        public int Row => Cell.Row;
        public Vector2Int Coordinates
        {
            get
            {
                return new(Col, Row);
            }
        }
        public PieceValue ValueConvertible;
        public string Value { get; private set; }
        public Side Side { get; private set; }
        public bool IsKing { get; private set; }
        /// <summary>
        /// The player owner of this piece.
        /// </summary>
        public Player Owner;
        /// <summary>
        /// Whether if the piece has a possible capture or not.
        /// </summary>
        public bool CanCapture = false;
        /// <summary>
        /// Whether the piece had captured a piece on its previous turn. 
        /// </summary>
        public bool HasCaptured = false;
        /// <summary>
        /// Represents the forward value of the piece. 1 (up) for bottom pieces, 0 (down) for top pieces.
        /// </summary>
        public int Forward = 0;
        /// <summary>
        /// Piece top color (ring). (Not implemented)
        /// </summary>
        public Color Color;
        /// <summary>
        /// Piece shadow color. (Not implemented)
        /// </summary>
        public Color Shadow;
        /// <summary>
        /// Sprite placeholder for custom sprites. (Not implemented)
        /// </summary>
        public List<Sprite> Sprites;

        [SerializeField] TextMeshPro text;
        [SerializeField] SpriteRenderer overlayTop;
        [SerializeField] SpriteRenderer overlayShadow;
        private static GameObject prefab;

        public Piece(string value, Side side, bool isKing)
        {
            Value = value;
            Side = side;
            IsKing = isKing;
        }

        void Awake()
        {
            prefab = Resources.Load<GameObject>("Prefabs/Chip");
        }
        
        void Start()
        {
            text.text = Value;

            if (IsKing)
            {
                overlayTop.sprite = Sprites[1];
            } else
            {
                overlayTop.sprite = Sprites[0];
            }
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

        /// <summary>
        /// Drags the piece along the cursor.
        /// </summary>
        public void ToCursor()
        {
            var target = new Vector3(
                Input.mousePosition.x,
                Input.mousePosition.y,
                1
            );
            transform.position = Camera.main.ScreenToWorldPoint(target);
        }

        /// <summary>
        /// Sets the position of this piece back to its cell.
        /// </summary>
        public void ResetPosition()
        {
            transform.position = Cell.transform.position;
        }

        /// <summary>
        /// Set this piece's "home cell".
        /// </summary>
        /// <param name="setPosition"> Move the piece to its cell. </param>
        public void SetCell(Cell cell, bool setPosition = true)
        {
            this.cell = cell;            
            if (setPosition) transform.position = cell.transform.position;
        }

        public void SetValue(string value)
        {
            Value = value;
            text.SetText(value);
        }

        public void SetSide(Side value)
        {
            Side = value;
            if (value == Side.Blue)
            {
                Forward = 1;
                SetColor(Colors.RichCerulean, Colors.darkJungleBlue);
            } else if (value == Side.Orange)
            {
                Forward = -1;
                SetColor(Colors.PersimmonOrange, Colors.burntUmber);
            }
        }
        
        public void SetColor(Color top, Color shadow)
        {
            text.color = shadow;
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

            overlayTop.sprite = Sprites[1];
            text.color = Color.white;
            IsKing = true;
        }

        public void Demote()
        {
            if (!IsKing) return;

            overlayTop.sprite = Sprites[0];
            text.color = overlayShadow.color;
            IsKing = false;
        }

        public void Capture()
        {

        }

        public void Remove()
        {
            /// This is bad handling
            Destroy(gameObject);
        }
    }
}