using System;
using UnityEngine;

namespace Damath.Systems
{
    [Serializable]
    [CreateAssetMenu(fileName = "Settings", menuName = "Damath/Settings")]
    public class Settings : ScriptableObject
    {
        public bool EnableConsole = true;
        public bool EnableDebugMode = true;

        [Space(10)]
        [Range(0, 100)] public int MasterVolume = 100;
        [Range(0, 100)] public int SoundVolume = 100;        
        [Range(0, 100)] public int MusicVolume = 100;
        public bool EnableAnimations = true;
        public float AnimationFactor = 0.5f;
        public bool AllowPieceDragging = false;
        public float PieceGrabDelay = 0.1f; // milliseconds
    }
}
