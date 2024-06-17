using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace Damath.UI
{
    public class Choice : MonoBehaviour
    {
        public string Label
        {
            get
            {
                return labelTMP.text;
            }
            set
            {
                labelTMP.text = value;
            }
        }
        public Sprite Icon
        {
            get
            {
                return iconImage.sprite;
            }
            set
            {
                iconImage.sprite = value;
            }
        }
        List<Action> callbacks = new();

        [SerializeField] TextMeshProUGUI labelTMP;
        [SerializeField] Image iconImage;
        [SerializeField] UnityEngine.UI.Button button;

        void Start()
        {
            button.onClick.AddListener(() =>
            {
                Invoke();
            });
        }

        public void AddCallback(Action function)
        {
            callbacks.Add(function);
        }

        public void Invoke()
        {
            if (callbacks.Count == 0) return;
            foreach (var c in callbacks)
            {
                c.Invoke();
            }
        }
    }
}