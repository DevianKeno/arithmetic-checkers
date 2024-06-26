using Damath.Network;

namespace Damath.Systems
{
    public class NetworkManager : Mirror.NetworkManager
    {
        LobbyManager lobbyManager;
        public LobbyManager Lobbies => lobbyManager;
        
        public bool IsOnline { get; private set; }
        public bool IsHosting { get; private set; }

        public override void Awake()
        {
            lobbyManager = GetComponentInChildren<LobbyManager>();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            IsHosting = true;
        }

        public override void OnStopHost()
        {
            base.OnStopHost();
            IsHosting = false;
        }
    }
}