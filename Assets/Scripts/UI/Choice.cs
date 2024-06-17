using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Damath.UI
{
    public class Choice : MonoBehaviour
    {
        public List<UnityAction> callbacks = new();
        public TextMeshProUGUI c_tmp;
        public Image c_image;
        public Button c_button;

        void Awake()
        {
            c_tmp = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            c_image = transform.Find("Icon").GetComponent<Image>();
            c_button = GetComponent<Button>();
        }

        public void Init(UnityAction function, string text, Sprite icon=null)
        {

            c_tmp.text = text;
            if (icon != null) c_image.sprite = icon;
            AddListener(function);
        }
        
        public void AddListener(UnityAction function)
        {
            c_button.onClick.AddListener(function);
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