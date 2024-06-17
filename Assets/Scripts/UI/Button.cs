using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class Button : UnityEngine.UI.Button, IHoverable
    {
        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnStartHover;
        public event Action<PointerEventData> OnEndHover;

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
    }
}