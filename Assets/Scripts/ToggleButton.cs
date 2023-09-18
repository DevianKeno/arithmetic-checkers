using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath
{
    public class ToggleButton : UnityEngine.UI.Toggle, IHoverable
    {
        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            IsHovered = true;
            OnMouseEnter?.Invoke(eventData);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            IsHovered = false;
            OnMouseExit?.Invoke(eventData);
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