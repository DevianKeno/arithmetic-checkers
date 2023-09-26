using System;
using UnityEngine;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Transporting;

namespace Damath
{
    /// <summary>
    /// Servers as a wrapper class for FishNet's NetworkManager.
    /// </summary>
    public class Network : MonoBehaviour
    {
        public static Network Main { get; private set; }
        public NetworkManager NetworkManager { get; private set; }
        public LobbyManager LobbyManager { get; private set; }

        public bool IsOnline { get; private set; }
        /// <summary>
        /// True if hosting a lobby.
        /// </summary>
        public bool IsHosting { get; private set; }
        // private List<Lobby> lobbies;
        // public string localIp; 
        public bool EnableDebug = true;

        [SerializeField] GameObject playerPrefab;
        [SerializeField] GameObject lobbyManagerPrefab;

        public event Action OnServerStart;
        public event Action OnClientStart;

        void Awake()
        {
            DontDestroyOnLoad(this);

            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
                NetworkManager = InstanceFinder.NetworkManager;
                LobbyManager = GetComponentInChildren<LobbyManager>();
            }
        }
        
        void Start()
        {
            InstanceFinder.ServerManager.OnServerConnectionState += ServerConnectionStateCallback;
            InstanceFinder.ServerManager.OnRemoteConnectionState += RemoteConnectionStateCallback;
            // InstanceFinder.ClientManager.OnClientConnectionState += ClientConnectionStateCallback;
            InstanceFinder.SceneManager.OnClientLoadedStartScenes  += ClientLoadedStartScenesCallback;
        }

        void OnDisable()
        {
            // Fire these when network is stopped
            InstanceFinder.ServerManager.OnServerConnectionState -= ServerConnectionStateCallback;
            InstanceFinder.ServerManager.OnRemoteConnectionState -= RemoteConnectionStateCallback;
            // InstanceFinder.ClientManager.OnClientConnectionState -= ClientConnectionStateCallback;
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= ClientLoadedStartScenesCallback;
        }

        /// <summary>
        /// Server method.
        /// </summary>
        void ServerConnectionStateCallback(ServerConnectionStateArgs args)
        {
            if (args.ConnectionState == LocalConnectionState.Starting)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Starting server ");

                //

            } else if (args.ConnectionState == LocalConnectionState.Started)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Started server ");

                if (IsHosting) InstanceFinder.ClientManager.StartConnection();

            } else if (args.ConnectionState == LocalConnectionState.Stopping)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Stopping server");

                //

            } else if (args.ConnectionState == LocalConnectionState.Stopped)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Stopped server");

                //
            }
        }

        /// <summary>
        /// Server method.
        /// </summary>
        void RemoteConnectionStateCallback(NetworkConnection connection, RemoteConnectionStateArgs args)
        {
            if (args.ConnectionState == RemoteConnectionState.Started)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Client {connection.ClientId} connected");

                connection.OnLoadedStartScenes += LoadedStartScenesCallback;

                if (!LobbyManager.HasLobby && Game.Main.HasRuleset)
                {
                    if (IsHosting) LobbyManager.CreateLobby(connection);
                } else
                {
                    LobbyManager.ConnectClientToLobby(connection);
                }

            } else if (args.ConnectionState == RemoteConnectionState.Stopped)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Client {connection.ClientId} disconnected");

                if (LobbyManager.Lobby != null)
                {
                    LobbyManager.DisconnectClientToLobby(connection);
                }
            }
        }

        void LoadedStartScenesCallback(NetworkConnection connection, bool isServer)
        {
            if (!isServer) return;
            
            if (EnableDebug) Game.Console.Log($"[Debug]: Client {connection.ClientId} had loaded");

            if (LobbyManager.Lobby != null)
            {
                Player player = LobbyManager.Lobby.SpawnPlayer(connection);
                Game.Events.NetworkSend(Parser.Pack(LobbyManager.Lobby.Ruleset.GetRulesetType(), Pack.RuleType));
            }
            
            // connection.OnLoadedStartScenes -= LoadedStartScenesCallback;
        }

        // void ClientConnectionStateCallback(ClientConnectionStateArgs args)
        // {
        //     if (args.ConnectionState == LocalConnectionState.Started)
        //     {
        //         if (EnableDebug) Game.Console.Log($"[Debug]: Started local client");

        //         //

        //     } else if (args.ConnectionState == LocalConnectionState.Stopped)
        //     {
        //         if (EnableDebug) Game.Console.Log($"[Debug]: Stopped local client");

        //         // 
        //     }
        // }

        /// <summary>
        /// Runs on the client (lmao).
        /// </summary>
        void ClientLoadedStartScenesCallback(NetworkConnection connection, bool isServer)
        {
            if (isServer) return;
            if (EnableDebug) Game.Console.Log($"[Debug]: Done loading start scene");
        }

        public void StartHost()
        {
            // Check if hosted started
            IsHosting = true;
            InstanceFinder.ServerManager.StartConnection();
        }

        public void StartServer()
        {
            // Check if server successfully started
            InstanceFinder.ServerManager.StartConnection();
        }

        public void StartClient()
        {
            // Check if client successfully started
            InstanceFinder.ClientManager.StartConnection();
        }

        public void Connect(string address)
        {
            // Check if connected successfully
            InstanceFinder.ClientManager.StartConnection(address);
        }

        public void LoadConnectionScene(string sceneName)
        {
            if (EnableDebug) Game.Console.Log($"[Debug]: Loading scene {sceneName}");

            SceneLoadData sld = new(sceneName) { ReplaceScenes = ReplaceOption.All };
            NetworkManager.SceneManager.LoadConnectionScenes(sld);
        }
        

        // Unused for now
        // public override void OnStartNetwork()
        // {
        //     base.OnStartNetwork();
        // }

        // public override void OnStartServer()
        // {
        //     base.OnStartServer();

        //     Game.Console.Log($"Hosted server");
            
        //     Game.Main.LoadGlobalScene("Match");
        //     Game.Main.UnloadGlobalScene("Title");
        // }

        // public override void OnSpawnServer(NetworkConnection connection)
        // {
        //     base.OnSpawnServer(connection);

        //     if (EnableDebugMode)
        //     {
        //         Game.Console.Log($"Client joined with id {connection.ClientId}");
        //     }
                        
        //     GameObject player = Instantiate(playerPrefab);
        //     Spawn(player, connection);
        // }

        // public void Host()
        // {
        //     NetworkManager.ServerManager.StartConnection();
        //     NetworkManager.ClientManager.StartConnection();
        // }

        // public void Connect(string address)
        // {
        //     if (address == "localhost") address = "127.0.0.1";
        //     NetworkManager.ClientManager.StartConnection(address);
        // }

        // public void SendServerData(string serverData)
        // {
        //     Debug.Log($"2 Is owner? {IsOwner}");
        //     SendServerRpc(serverData);
        // }

        // // Every client should be able to send message to the server.
        // // However, only the owner of the object can send the rpc to the server.
        // [ServerRpc(RequireOwnership = true)]
        // public void SendServerRpc(string serverData, NetworkConnection conn = null)
        // {            
        //     Pack type = Parser.Parse(serverData, out string[] args);

        //     // Only the owner should receive the observerRpc
        //     // if (IsOwner)
        //     // {
        //     switch (type)
        //     {
        //         case Pack.Ruleset:
        //         {
        //             //
        //             break;
        //         }

        //         case Pack.Chat:
        //         {
        //             ReceiveChatRpc(conn, serverData);
        //             ReceiveChatRpc(null, serverData);
        //             break;
        //         }

        //         case Pack.Command:
        //         {
        //             //
        //             break;
        //         }
        //         // }
        //     }
        // }

        // [ObserversRpc(ExcludeOwner = true)][TargetRpc]
        // void ReceiveChatRpc(NetworkConnection target, string data)
        // {
        //     Pack type = Parser.Parse(data, out string[] args);

        //     Game.Console.Log($"<{args[1]}> {args[2]}");
        // }

        // void SendObserverData(string serverData)
        // {
        //     SendObserverRpc(serverData);
        // }

        // [ObserversRpc]
        // void SendObserverRpc(string serverData, NetworkConnection conn = null)
        // {
        //     Pack type = Parser.Parse(serverData, out string[] args);

        //     // Only the owner should receive the observerRpc
        //     if (IsOwner)
        //     {
        //         switch (type)
        //         {
        //             case Pack.Ruleset:
        //                 break;

        //             case Pack.Chat:

        //                 Game.Console.Log($"<{args[2]}> {args[3]}");

        //                 break;

        //             case Pack.Command:
        //                 break;
        //         }
        //     }
       
        // }

        // void OnDestroy()
        // {
        //     Game.Events.OnServerSend -= SendServerData;
        //     Game.Events.OnObserverSend -= SendObserverData;
        // }
    }
}