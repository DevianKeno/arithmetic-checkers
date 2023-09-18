using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath
{
    public class Button : UnityEngine.UI.Button, IHoverable
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
    }
}