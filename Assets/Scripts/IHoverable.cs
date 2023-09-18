using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

namespace Damath
{
    public interface IHoverable : IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnMouseEnter;
        public event Action<PointerEventData> OnMouseExit;
    }
}
