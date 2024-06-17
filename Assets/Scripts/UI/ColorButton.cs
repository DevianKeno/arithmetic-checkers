using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace Damath.UI
{
    public class ColorButton : MonoBehaviour
    {
        public int currentColorIndex = 0;
        public List<Color> Colors = new();

        Image image;
        UnityEngine.UI.Button button;


        void Awake()
        {
            image = GetComponent<Image>();
            button = GetComponent<UnityEngine.UI.Button>();
        }

        void Start()
        {
            if (Colors.Count == 0) return;

            SetColor(Colors[currentColorIndex]);
            AddListener(NextColor);
        }

        public void SetColor(Color color)
        {
            if (Colors.Contains(color))
            {
                image.color = color;
            }
        }

        public void AddColor(Color color)
        {
            Colors.Add(color);
        }
        
        public void AddListener(UnityAction function)
        {
            button.onClick.AddListener(function);
        }

        public void NextColor()
        {
            if (Colors.Count == 0) return;

            if (currentColorIndex < Colors.Count - 1)
            {
                currentColorIndex += 1;
                SetColor(Colors[currentColorIndex]);
            } else
            {
                currentColorIndex = 0;
                SetColor(Colors[currentColorIndex]);
            }
        }

        public void Cycle()
        {

        }
    }
}

