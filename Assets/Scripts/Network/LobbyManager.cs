using System;

using UnityEngine;
using TMPro;

using Mirror;
using Mirror.SimpleWeb;

namespace Damath.Network
{
    /// <summary>
    /// Manages lobbies and its connections.
    /// </summary>
    public class LobbyManager : MonoBehaviour
    {
        Lobby currentLobby = null;
        /// <summary>
        /// List of available lobbies.
        /// </summary>
        int[] LobbyList;
        
        [Header("Prefabs")]
        [SerializeField] GameObject lobbyPrefab;

        public void RefreshLobbies()
        {
            
        }

        public Lobby GetCurrentLobby()
        {
            return currentLobby;
        }

        public void CreateLobby(LobbyProperties properties)
        {
            if (currentLobby != null)
            {
                Game.Console.Log($"Already hosting a lobby");
                return;
            }

            var go = Instantiate(lobbyPrefab);
            var newLobby = go.GetComponent<Lobby>();
            newLobby.SetProperties(properties);
            newLobby.SetOwner(DiscordManager.Instance.LocalUser);
            currentLobby = newLobby;
            
            Game.Network.StartHost();
            var aa = NetworkManager.singleton.transport as SimpleWebTransport;
            Game.Console.Log($"Hosted lobby on port {aa.port}");
        }

        public void ConnectToLobby()
        {

        }
    }
}
