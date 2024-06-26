using System.Collections.Generic;
using Mirror;

namespace Damath.Network
{
    public class Lobby : NetworkBehaviour
    {
        public LobbyProperties Properties { get; set; }
        public UserInfo Owner { get; set; }
        public UserInfo BluePlayer { get; set; }
        public UserInfo OrangePlayer { get; set; }
        public List<UserInfo> Members = new();

        public void SetProperties(LobbyProperties properties)
        {
            Properties = properties;
        }

        public void SetOwner(UserInfo owner)
        {
            Owner = owner;
        }

        public bool IsOwner(UserInfo user)
        {
            return Owner.id == user.id;
        }

        public bool FindUser(out UserInfo user)
        {
            user = null;
            var result = Members.Find((user) =>
            {
                return Owner.id == user.id;
            });
            return result != null;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
        }
    }
}