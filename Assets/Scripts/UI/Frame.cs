using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath.UI
{
    public class Frame : MonoBehaviour
    {
        public string Name = "Frame";
        public Vector2 size = new (1670f, 1080f); // Default size
        [SerializeField] public RectTransform rect;

        public Frame(string name)
        {
            Name = name;
        }

        void Start()
        {
            rect.sizeDelta = size;
        }
    }
}

