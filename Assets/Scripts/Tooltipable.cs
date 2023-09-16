using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath
{
    public class Tooltipable : MonoBehaviour
    {
        public bool Enable = true;
        [TextArea] public string Text; 
        public Color Color = Colors.PersimmonOrange;

        private IHoverable Element;

        void Awake()
        {
            Element = GetComponent<IHoverable>();
            
            Element.OnMouseEnter += Show;
            Element.OnMouseExit += Hide;
        }

        void OnDisable()
        {
            Element.OnMouseEnter -= Show;
            Element.OnMouseExit -= Hide;
        }
        
        void Show(PointerEventData eventData)
        {
            if (!Enable) return;

            Game.UI.CreateTooltip(Text, Color);
        }
        
        void Hide(PointerEventData eventData)
        {
            Game.UI.HideTooltip();
        }
    }
}