using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Damath
{
    public class GlobalTimerPointerHandler : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("I right clicked the global timer.");
                Game.Main.Match.Cheats.DisplayGlobalTimerActions();
            } 
        }
    }
}