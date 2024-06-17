using System;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public interface IHoverable : IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsHovered { get; set; }
        public event Action<PointerEventData> OnStartHover;
        public event Action<PointerEventData> OnEndHover;
    }
}
