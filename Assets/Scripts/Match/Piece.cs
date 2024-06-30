using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Aritche.UI;
using Aritche.Players;
using Aritche.Rules;

namespace Aritche
{
    /// <summary>
    /// This class implements conversion methods to convert a "string value" to a computable score value.
    /// </summary>
    public class PieceValue
    {
        public PieceValueType ValueType { get; set; }
        [SerializeField] string value;
        public string AsString => value;

        public PieceValue(string value)
        {
            this.value = value;
        }
    }

    public class PieceData
    {
        public PieceValueType ValueType { get; set; }
        public int ValueIndex { get; set; }
        public Side Side { get; set; }
        public bool IsKing { get; set; }

        public PieceData(PieceValueType valueType, int valueIndex, Side side, bool isKing)
        {
            ValueType = valueType;
            ValueIndex = valueIndex;
            Side = side;
            IsKing = isKing;
        }
    }

    public class Piece : MonoBehaviour
    {
        [SerializeField] Cell _cell;
        public Cell Cell => _cell;
        public int Col => _cell.Col;
        public int Row => _cell.Row;
        public Vector2Int Coordinates => new(_cell.Col, _cell.Row);
        public PieceData Data { get; private set; }
        public PieceValue Value { get; private set; }
        public string ValueAsString => Value.AsString;
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
        public Color ShadowColor;
        /// <summary>
        /// Sprite placeholder for custom sprites. (Not implemented)
        /// </summary>
        public List<Sprite> Sprites;

        [SerializeField] TextMeshPro text;
        [SerializeField] SpriteRenderer overlayTop;
        [SerializeField] SpriteRenderer overlayShadow;
                
        void Start()
        {
            text.text = ValueAsString;

            if (IsKing)
            {
                overlayTop.sprite = Sprites[1];
            } else
            {
                overlayTop.sprite = Sprites[0];
            }
        }

        void OnValidate()
        {
            if (Application.isPlaying) return;

            overlayTop.color = Color;
            overlayShadow.color = ShadowColor;
            text.color = ShadowColor;
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

        public void SetData(PieceData data)
        {
            Data = data;
            Value = new(Ruleset.GetValueFromTypeAndIndex(data.ValueType, data.ValueIndex));
            SetSide(data.Side);
            SetKing(data.IsKing);
            Refresh();
        }

        void Refresh()
        {
            text.text = ValueAsString;
        }

        /// <summary>
        /// Sets the position of this piece back to its cell.
        /// </summary>
        public void ResetPositionToCell()
        {
            transform.position = Cell.transform.position;
        }

        /// <summary>
        /// Set this piece's "home cell".
        /// </summary>
        /// <param name="setPosition"> Move the piece to its cell. </param>
        public void SetCell(Cell cell, bool setPosition = true)
        {
            this._cell = cell;            
            if (setPosition)
            {
                transform.position = cell.transform.position;
            }
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