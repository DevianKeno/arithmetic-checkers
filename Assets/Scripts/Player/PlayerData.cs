using System;

namespace Aritche
{
    /// <summary>
    /// This is used for network transport.
    /// </summary>
    [Serializable]
    public struct PlayerData
    {
        // public Image ProfileImage;
        public string Name { get; set; }
        public Side Side { get; set; }
    }
}