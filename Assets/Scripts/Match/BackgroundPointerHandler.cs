using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath
{
    public class BackgroundPointerHandler : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right)
            {
                Game.Main.Match.Cheats.DisplayBoardActions();
            }
        }
    }
}