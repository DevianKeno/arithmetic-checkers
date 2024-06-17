using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Damath
{
    public class Indicator : MonoBehaviour
    {
        public int Col;
        public int Row;
        public Color Color;
        public Sprite Sprite;
        public bool ForceDisplay = false;
        public RectTransform rectTransform;
        public SpriteRenderer spriteRenderer;

        void Awake()
        {
            Sprite = GetComponent<Sprite>();
            rectTransform = GetComponent<RectTransform>();
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

        public void Move(Cell cell, bool show = false)
        {
            if (show) gameObject.SetActive(true);
            (Col, Row) = (cell.Col, cell.Row);
            transform.position = cell.transform.position;
        }

        public void Delete()
        {
            Destroy(this);
        }

        public void SetColor(Color value)
        {
            spriteRenderer.color = value;
        }
    }
}