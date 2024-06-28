using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace Aritche.UI
{
    public class ColorButton : MonoBehaviour
    {
        public Color Color
        {
            get
            {
                return Colors[_current];
            }
        }
        int _current = 0;
        public int SelectedIndex => _current;
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

            SetColor(Colors[_current]);
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

            if (_current < Colors.Count - 1)
            {
                _current += 1;
                SetColor(Colors[_current]);
            } else
            {
                _current = 0;
                SetColor(Colors[_current]);
            }
        }

        public void Cycle()
        {

        }
    }
}

