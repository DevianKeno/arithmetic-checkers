using UnityEngine;

namespace Damath
{
    public class Settings : MonoBehaviour
    {
        public bool EnableDebugMode = true;

        [Space]
        public bool EnableConsole = true;
        [Range(0f, 1f)] public float masterVolume = 1.0f;
        [Range(0f, 1f)] public float soundVolume = 1.0f;        
        [Range(0f, 1f)] public float musicVolume = 1.0f;
        public bool EnableAnimations = true;
        public float AnimationFactor = 0.5f;
        public bool AllowPieceDragging = false;
        public float PieceGrabDelay = 0.1f; // milliseconds

        public class KeyBinds
        {
            public static KeyCode OpenDeveloperConsole = KeyCode.BackQuote;
        }
    }
}
