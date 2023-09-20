using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace Damath
{
    /// <summary>
    /// Manages rpcs and data transport.
    /// </summary>
    public class Network : NetworkBehaviour
    {
        public static Network Main { get; private set; }
        public Lobby Lobby;
        private int[] lobbyList;
        // private List<Lobby> lobbies;
        // public string localIp; 
        public bool EnableDebug = true;

        [SerializeField] GameObject playerPrefab;
        
        void Start()
        {
            Game.Events.OnServerSend += SendServerData;
            Game.Events.OnObserverSend += SendObserverData;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            GameObject player = Instantiate(playerPrefab);
            // Spawn(player, LocalConnection);
            Spawn(player);
        }

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);

            if (EnableDebug)
            {
                Game.Console.Log($"Client joined with id {connection.ClientId}");
            }
                        
            // Game.Console.Log($"{player.GetComponent<NetworkObject>().IsOwner}");
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            if (IsOwner)
            {
                Game.Console.Log("I am owner");
            } else
            {
                Game.Console.Log("Not owner");
            }
        }

        void SendServerData(string serverData)
        {
            SendServerRpc(serverData);
        }

        // Every client should be able to send message to the server.
        // However, only the owner of the object can send the rpc to the server.
        [ServerRpc(RequireOwnership = true)]
        void SendServerRpc(string serverData, NetworkConnection conn = null)
        {            
            Pack type = Parser.Parse(serverData, out string[] args);

            // Only the owner should receive the observerRpc
            // if (IsOwner)
            // {
            switch (type)
            {
                case Pack.Ruleset:
                {
                    //
                    break;
                }

                case Pack.Chat:
                {
                    ReceiveChatRpc(conn, serverData);
                    ReceiveChatRpc(null, serverData);
                    break;
                }

                case Pack.Command:
                {
                    //
                    break;
                }
                // }
            }
        }

        [ObserversRpc(ExcludeOwner = true)][TargetRpc]
        void ReceiveChatRpc(NetworkConnection target, string data)
        {
            Pack type = Parser.Parse(data, out string[] args);

            Game.Console.Log($"<{args[1]}> {args[2]}");
        }

        void SendObserverData(string serverData)
        {
            SendObserverRpc(serverData);
        }

        [ObserversRpc]
        void SendObserverRpc(string serverData, NetworkConnection conn = null)
        {
            Pack type = Parser.Parse(serverData, out string[] args);

            // Only the owner should receive the observerRpc
            if (IsOwner)
            {
                switch (type)
                {
                    case Pack.Ruleset:
                        break;

                    case Pack.Chat:

                        Game.Console.Log($"<{args[2]}> {args[3]}");

                        break;

                    case Pack.Command:
                        break;
                }
            }
       
        }

        void OnDestroy()
        {
            Game.Events.OnServerSend -= SendServerData;
            Game.Events.OnObserverSend -= SendObserverData;
        }
    }
}