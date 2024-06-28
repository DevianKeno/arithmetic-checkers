using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Aritche.UI
{
    public class Themes : MonoBehaviour
    {
        public Color botChipColor;
        public Color botChipShadow;

        public Color topChipColor;
        public Color topChipShadow;

        void Awake()
        {
            SetDefaults();
        }

        public void SetDefaults()
        {
            botChipColor = Colors.darkCerulean;
            botChipShadow = Colors.darkJungleBlue;
            
            topChipColor = Colors.PersimmonOrange;
            topChipShadow = Colors.burntUmber;
        }
    }
}