using System;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet;
using FishNet.Transporting;

namespace Damath
{
    /// <summary>
    /// Manages lobbies and its connections.
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        /// <summary>
        /// Current selected lobby.
        /// </summary>
        public Lobby Lobby = null;
        /// <summary>
        /// List of available lobbies.
        /// </summary>
        private int[] LobbyList;
        public bool HasLobby { get; private set; }
        public bool EnableDebug = true;

        [Space]
        [SerializeField] private GameObject lobbyPrefab;
        [SerializeField] private GameObject playerPrefab;

        public event Action OnLobbyCreate;
        public event Action OnLobbyUpdate;

        void Start()
        {
            HasLobby = false;

            InstanceFinder.ServerManager.OnRemoteConnectionState += ServerRemoteConnectionStateCallback;
            // InstanceFinder.SceneManager.OnClientLoadedStartScenes += ClientLoadedStartScenesCallback;
        }

        void OnDisable()
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState -= ServerRemoteConnectionStateCallback;
            // InstanceFinder.SceneManager.OnClientLoadedStartScenes -= ClientLoadedStartScenesCallback;
        }

        void Update()
        {
            // if (Input.GetKeyDown(KeyCode.L))
            // {
            //     CheckOwnerServerRpc();
            // }
        }

        // [ServerRpc]
        // void CheckOwnerServerRpc(NetworkConnection connection = default)
        // {
        //     Game.Console.Log("[Debug]: Lobby manager owner check");
        // }

        void ServerRemoteConnectionStateCallback(NetworkConnection connection, RemoteConnectionStateArgs args)
        {
            if (args.ConnectionState == RemoteConnectionState.Started)
            {
                ConnectClientToLobby(connection);

            } else if (args.ConnectionState == RemoteConnectionState.Started)
            {
                if (EnableDebug) Game.Console.Log($"[Debug]: Client {connection.ClientId} disconnected");

                //
            }
        }



        // [ServerRpc]
        // void DoneLoadStartSceneRpc(NetworkConnection connection = default)
        // {
        //     if (EnableDebug) Game.Console.Log($"[Debug]: Client {connection.ClientId} has finished loading start scene");

        //     SpawnPlayer(connection);
        // }
        


        /// <summary>
        /// Connect client to lobby.
        /// </summary>
        public void ConnectClientToLobby(NetworkConnection connection)
        {
            if (Lobby == null) return;
            
            if (!Lobby.HasPlayer(connection.ClientId))
            {    
                // Join 2nd player as opponent        
                if (!Lobby.HasOpponent)
                {
                    Lobby.ConnectPlayerAsOpponent(connection);
                } else // Join succeeding player as spectator
                {
                    Lobby.ConnectPlayer(connection);
                }
            }
        }

        public void DisconnectClientToLobby(NetworkConnection connection)
        {
            if (Lobby == null) return;
            
            if (Lobby.HasPlayer(connection.ClientId))
            {
                Lobby.DisconnectPlayer(connection);
            }
        }
        
        public void CreateLobby(NetworkConnection hostConnection)
        {
            if (Lobby != null) return;
            if (!Game.Main.HasRuleset) return;

            GameObject go = Instantiate(lobbyPrefab);            
            Lobby = go.GetComponent<Lobby>();
            Lobby.ConnectPlayerAsHost(hostConnection);
            Lobby.SetRuleset(Game.Main.Ruleset);
            go.name = $"Lobby ({Lobby.Ruleset.Type}) ";
            InstanceFinder.ServerManager.Spawn(go, hostConnection);
            HasLobby = true;
            if (EnableDebug) Game.Console.Log($"Hosted lobby");
            // OnLobbyCreate?.Invoke();
        }

        // [TargetRpc]
        // public void ReceiveLobbyDataRpc(NetworkConnection target)
        // {
        //     Game.Console.Log("Fetching lobby info...");
        
        //     Lobby = FindObjectOfType<Lobby>();
            
        //     if (Lobby != null)
        //     {
        //         Game.Console.Log($"Joined lobby with match {Lobby.Ruleset.Mode}");
                
        //         Game.Main.SetRuleset(Lobby.Ruleset);
        //     }
        // }
    }
}
