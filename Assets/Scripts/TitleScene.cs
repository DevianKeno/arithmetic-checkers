using UnityEngine;
using Damath.UI;

namespace Damath
{
    public class TitleScene : MonoBehaviour
    {        
        [SerializeField] DevAuthWindow devAuthWindow;
        [SerializeField] FrameController mainFrame;

        void Start()
        {
            mainFrame.SwitchFrame("Title");
        }

        public void OnSignInClick()
        {
            devAuthWindow.Show();
        }
    }
}
