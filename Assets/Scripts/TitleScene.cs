using UnityEngine;
using Aritche.UI;

namespace Aritche
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
