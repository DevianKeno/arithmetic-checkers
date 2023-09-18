using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

namespace Damath
{
    public class Network
    {
        public static Network Main { get; private set; }
        public Lobby Lobby;
        private int[] lobbyList;
        private List<Lobby> lobbies;
        public string localIp; 
        public bool EnableDebug = true; 

        /*
        void Start()
        {
            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
            }

            OnClientConnectedCallback += ClientConnectedCallback;
        }

        private void ClientConnectedCallback(ulong clientId)
        {
            if (IsServer)
            {
                if (ConnectedClients.ContainsKey(clientId)) return;

                if (EnableDebug)
                {
                    Game.Console.Log("Client connected with id " + clientId);
                }
            }
        }
        
        [ServerRpc]
        public void RequestLobbiesServerRpc(ServerRpcParams serverRpcParams = default)
        {
            if (!IsServer) return;

            var senderClientId = serverRpcParams.Receive.SenderClientId;
            
            ClientRpcParams clientRpcParams = new()
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]{senderClientId}
                }
            };

            RetrieveLobbiesClientRpc(lobbyList, clientRpcParams);
        }

        [ClientRpc]
        public void RetrieveLobbiesClientRpc(int[] lobbies, ClientRpcParams clientRpcParams)
        {
            lobbyList = lobbies;
        }

        public Lobby CreateLobby()
        {
            Lobby = new(Game.Main.Ruleset);
            Game.Console.Log($"Created lobby");
            Game.Events.LobbyCreate(Lobby);
            return Lobby;
        }

        public void Host()
        {
            if (Lobby == null) return;     
            StartHost();
            Game.Events.LobbyHost(Lobby);
            Game.Console.Log("Hosted lobby");
        }

        public void JoinLobby(string address, ushort port = default)
        {
            // If hosting, stop host
            if (address == "localhost") address = "127.0.0.1";
            if (port == default) port = 7777;

            Main.GetComponent<UnityTransport>().SetConnectionData(address, port);

            // Handle invalid ip address
            StartClient();
        }

        public void RemoveLobby()
        {
            Lobby = null;
        }

        */
    }
}
