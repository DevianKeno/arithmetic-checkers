using UnityEngine;
using Damath.UI;
using Damath.Players;

namespace Damath
{
    public class Selector : MonoBehaviour
    {
        public int Col, Row;
        public Color Color = Colors.SkyBlue;
        RectTransform rect;
        SpriteRenderer spriteRenderer;
        
        void Awake()
        {
            rect = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Start()
        {
            Game.Events.OnPlayerSelectPiece += Move;
            Game.Events.OnPieceCapture += Hide;
            Game.Events.OnPieceDone += Hide;
            Game.Events.OnDeselect += Hide;
        }
        
        void OnDisable()
        {
            Game.Events.OnPlayerSelectPiece -= Move;
            Game.Events.OnDeselect -= Hide;
            Game.Events.OnPieceCapture += Hide;
            Game.Events.OnPieceDone -= Hide;
            Game.Events.OnDeselect -= Hide;
        }

        public void Move(Player player, Piece piece)
        {
            Show();
            transform.position = piece.Cell.transform.position;
        }
        
        public void Show()
        {
            spriteRenderer.enabled = true;
        }

        public void Hide()
        {
            spriteRenderer.enabled = false;
        }

        public void Hide(Piece piece)
        {
            Hide();
        }
        public void Hide(Move move)
        {
            Hide();
        }
    }
}
