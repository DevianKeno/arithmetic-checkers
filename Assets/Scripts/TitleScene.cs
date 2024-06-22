using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Damath.UI;

namespace Damath
{
    public class TitleScene : MonoBehaviour
    {
        DevAuthWindow devAuthWindow;
        [SerializeField] FrameController mainFrame;

        void Start()
        {
            mainFrame.SwitchFrame("Title");
        }

        public void OnSignInClick()
        {
            if (devAuthWindow == null)
            {
                devAuthWindow = Game.UI.Create<DevAuthWindow>("dev_auth_window");
                devAuthWindow.Show();
            }
        }
    }
}
