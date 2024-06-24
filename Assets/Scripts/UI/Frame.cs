using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Damath.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class Frame : MonoBehaviour
    {
        public string Name = "Frame";
        public Vector2 size = new (1670f, 1080f); // Default size
        [field: SerializeField] public RectTransform rect { get; set; }

        public Frame(string name)
        {
            Name = name;
        }

        void Awake()
        {
            rect = GetComponent<RectTransform>();
        }

        void Start()
        {
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
    }
}

