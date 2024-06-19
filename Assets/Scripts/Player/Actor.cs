using UnityEngine;
using FishNet.Object;

namespace Damath.Players
{
    /// <summary>
    /// An actor can represent either a player or just a spectator.
    /// </summary>
    public class Actor : NetworkBehaviour
    {
        public string @string = "ds";
        public string Name = "Actor";
        protected RaycastHit2D hit;
        public RaycastHit2D Hit => hit;
    }
}