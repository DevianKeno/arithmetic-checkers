using UnityEngine;
using Damath.UI;

namespace Damath
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] FrameController mainFrame;

        void Start()
        {
            mainFrame.SwitchToFrame("Title");
        }
    }
}
