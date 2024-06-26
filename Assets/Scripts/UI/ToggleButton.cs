using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class ToggleButton : UnityEngine.UI.Toggle, IHoverable
    {
        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnStartHover;
        public event Action<PointerEventData> OnEndHover;

        protected override void Awake()
        {
            onValueChanged.AddListener(ToggleColor);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            IsHovered = true;
            OnStartHover?.Invoke(eventData);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            IsHovered = false;
            OnEndHover?.Invoke(eventData);
        }

        public void ToggleColor(bool value)
        {
            if (value)
            {
                image.color = new (0.56f, 0.69f, 0.77f, 1f);
            } else
            {
                image.color = Color.white;
            }
        }
    }
}