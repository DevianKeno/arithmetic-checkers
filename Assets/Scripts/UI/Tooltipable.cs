using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    [RequireComponent(typeof(IHoverable))]
    public class Tooltipable : MonoBehaviour
    {
        public bool Enable = true;
        [TextArea] public string Text; 
        public Color Color = Colors.PersimmonOrange;

        private IHoverable Element;

        void Awake()
        {
            Element = GetComponent<IHoverable>();
            
            Element.OnStartHover += Show;
            Element.OnEndHover += Hide;
        }

        void OnDisable()
        {
            Element.OnStartHover -= Show;
            Element.OnEndHover -= Hide;
        }
        
        void Show(PointerEventData eventData)
        {
            if (!Enable) return;

            Debug.Log("hover");
            Game.UI.CreateTooltip(Text, Color);
        }
        
        void Hide(PointerEventData eventData)
        {
            Debug.Log("unhover");
            Game.UI.HideTooltip();
        }
    }
}