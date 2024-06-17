using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Damath
{
    public class Coordinates : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private TextMeshPro x;
        [SerializeField] private TextMeshPro y;
        public bool isFlipped = false;
        void Start(){
            Game.Events.OnBoardFlip += FlipCoordinates;
        }
        void FlipCoordinates(){
            isFlipped = !isFlipped;
            if (isFlipped)
            {
                x.text = "7 6 5 4 3 2 1 0";
                y.text = "01234567";
            } else
            {
                x.text = "0 1 2 3 4 5 6 7";
                y.text = "76543210";
            }
        }
    }
}