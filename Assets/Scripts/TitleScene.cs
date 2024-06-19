using System.Collections.Generic;
using Damath.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Damath
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] FrameController mainFrame;

        void Start()
        {
            mainFrame.SwitchFrame("Title");
        }
    }
}
