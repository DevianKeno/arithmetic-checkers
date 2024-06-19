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
        ChoiceWindow parent;
        List<Action> _callbacks = new();

        [SerializeField] TextMeshProUGUI labelTMP;
        [SerializeField] Image iconImage;
        [SerializeField] UnityEngine.UI.Button button;

        void Start()
        {
            parent = GetComponentInParent<ChoiceWindow>();
            button.onClick.AddListener(() =>
            {
                button.interactable = false;
                Invoke();
            });
        }

        public Choice AddCallback(Action callback)
        {
            _callbacks.Add(callback);
            return this;
        }

        public Choice Invoke()
        {
            if (_callbacks.Count == 0) return this;
            foreach (var c in _callbacks)
            {
                c.Invoke();
            }
            return this;
        }

        public Choice SetEnabled(bool value)
        {
            button.interactable = value;
            return this;
        }

        public Choice CloseOnSubmit(bool value = true)
        {
            if (value)
            {
                button.onClick.AddListener(() =>
                {
                    parent.Hide();
                });
            }
            return this;
        }
    }
}