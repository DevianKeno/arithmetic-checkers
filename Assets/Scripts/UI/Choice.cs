using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

namespace Aritche.UI
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
        bool _hideOnSubmit = true;
        List<Action> _callbacks = new();
        ChoiceWindow parent;

        [SerializeField] TextMeshProUGUI labelTMP;
        [SerializeField] Image iconImage;
        [SerializeField] UnityEngine.UI.Button button;

        void Awake()
        {
            parent = GetComponentInParent<ChoiceWindow>();
        }

        void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        void OnClick()
        {
            button.interactable = false;
            Invoke();

            if (_hideOnSubmit)
            {
                parent.Hide();
            }
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

        public Choice HideOnSubmit(bool value)
        {
            _hideOnSubmit = value;
            return this;
        }
    }
}