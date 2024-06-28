using UnityEngine;

namespace Aritche.Players
{
    /// <summary>
    /// An actor can represent either a player or just a spectator.
    /// </summary>
    public class Actor : MonoBehaviour
    {
        public string @string = "ds";
        public string Name = "Actor";
        protected RaycastHit2D hit;
        public RaycastHit2D Hit => hit;
    }
}