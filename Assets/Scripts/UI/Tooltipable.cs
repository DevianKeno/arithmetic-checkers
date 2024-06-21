using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class Tooltipable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsEnabled = true;
        [TextArea] public string Text; 
        public Color Color = Colors.PersimmonOrange;

        Tooltip tooltip;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsEnabled) return;

            tooltip = Game.UI.Create<Tooltip>("tooltip");
            tooltip.Text = Text;
            tooltip.Color = Color;
            tooltip.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltip.Hide();
        }
    }
}