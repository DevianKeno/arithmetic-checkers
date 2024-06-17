using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public class GraveyardControl : MonoBehaviour
    {
        [SerializeField] private RectTransform GraveyardB;
        [SerializeField] private RectTransform GraveyardT;
        void Start()
        {
            Game.Events.OnBoardFlip += FlipGraveyard;
        }
        void FlipGraveyard()
        {
            Vector3 TempHolder = GraveyardB.position;
            GraveyardB.position = GraveyardT.position;
            GraveyardT.position = TempHolder;
        }
    }
}