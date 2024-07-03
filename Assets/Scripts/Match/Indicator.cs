using UnityEngine;

namespace Aritche
{
    public class Indicator : MonoBehaviour
    {
        public int Col;
        public int Row;
        public IndicatorType Type { get; set; }
        public Color Color
        {
            get
            {
                return spriteRenderer.color;
            }
            set
            {
                spriteRenderer.color = value;
            }
        }
        public Color FlashColor = Color.white;
        public Sprite Sprite
        {
            get
            {
                return spriteRenderer.sprite;
            }
            set
            {
                spriteRenderer.sprite = value;
            }
        }
        public bool ForceDisplay = false;

        [SerializeField] RectTransform rect;
        [SerializeField] SpriteRenderer spriteRenderer;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void MoveToCell(Cell cell)
        {
            transform.position = cell.transform.position;
            Col = cell.Col;
            Row = cell.Row;
            gameObject.SetActive(true);
        }

        public void Flash()
        {
            Color oldColor = Color;
            Color = FlashColor;
            LeanTween.color(gameObject, oldColor, 1f)
            .setEase(LeanTweenType.easeOutCubic);
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}