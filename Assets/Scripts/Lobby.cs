using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.UI;

namespace Damath
{
    public struct LobbyData
    {
        public bool IsPrivate;
        public string Password;
        public int Host;
        public int Opponent;
        public bool HasOpponent;
    }

    /// <summary>
    /// 
    /// </summary>
    public class Lobby : NetworkBehaviour
    {
        public List<int> ConnectedClients;

        [field: Header("Lobby Information")]
        public bool IsPrivate { get; private set; }
        public string Password { get; private set; }
        public int Host = -1;
        public int Opponent = -1;
        public bool HasHost = false;
        public bool HasOpponent = false;
        
        [SerializeField] private Image readyBotImage;
        [SerializeField] private Image readyTopImage;
        [SerializeField] private ToggleButton button;

        [Space]
        [SerializeField] private bool OpponentIsReady;
        [SerializeField] private GameObject playerPrefab;

        public Ruleset Ruleset { get; private set; }

        void Start()
        {
            Ruleset = Game.Main.Ruleset;
            button.onValueChanged.AddListener(ToggleReady);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                Game.Console.Log($"{Ruleset.Mode}");
            }
        }

        [ServerRpc]
        void CheckOwnerServerRpc()
        {
            Game.Console.Log("[Debug]: Lobby owner check");
        }

        public void SetPrivacy(bool value)
        {
            IsPrivate = value;
        }

        public void SetRuleset(Ruleset ruleset)
        {
            Ruleset = new Ruleset();
        }

        public Player SpawnPlayer(NetworkConnection owner)
        {
            GameObject go = Instantiate(playerPrefab);
            Player newPlayer = go.GetComponent<Player>();
            Spawn(go, owner);
            return newPlayer;
        }

        public void ConnectPlayerAsHost(NetworkConnection connection)
        {
            if (HasPlayer(connection.ClientId)) return;

            ConnectPlayer(connection);
            Host = connection.ClientId;
            HasHost = true;

            Game.Console.Log($"Player {connection.ClientId} joined as host");
        }

        public void ConnectPlayerAsOpponent(NetworkConnection connection)
        {
            if (HasPlayer(connection.ClientId)) return;

            ConnectPlayer(connection);
            Opponent = connection.ClientId;
            HasOpponent = true;

            Game.Console.Log($"Player {connection.ClientId} joined as opponent");
    }

        /// <summary>
        /// Connect player to lobby.
        /// </summary>
        /// <param name="connection"></param>
        public void ConnectPlayer(NetworkConnection connection)
        {
            if (HasPlayer(connection.ClientId)) return;

            ConnectedClients.Add(connection.ClientId);
            
            Game.Console.Log($"Player {connection.ClientId} joined lobby");
            // Game.Events.LobbyJoin(connection.ClientId, this);
        }

        public void DisconnectPlayer(NetworkConnection connection)
        {
            if (!HasPlayer(connection.ClientId)) return;

            ConnectedClients.Remove(connection.ClientId);
            
            Game.Console.Log($"Player {connection.ClientId} left lobby");
            // Game.Events.LobbyLeave(connection.ClientId, this);
        }
        
        public bool TryConnectPlayer(int clientId)
        {
            if (HasPlayer(clientId)) return false;

            ConnectedClients.Add(clientId);
            
            Game.Console.Log($"Player {clientId} joined lobby");
            Game.Events.LobbyJoin(clientId, this);
            return true;
        }
        
        public bool HasPlayer(int clientId)
        {
            if (ConnectedClients.Contains(clientId)) return true;
            return false;
        }

        public void SetOpponentReady(bool isReady)
        {
            OpponentIsReady = isReady;
        }

        public void Begin()
        {
            Game.Events.LobbyStart(this);
        }

        public LobbyData GetLobbyData()
        {
            return new()
            {
                IsPrivate = IsPrivate,
                Password = Password,
                Host = Host,
                Opponent = Opponent,
                HasOpponent = HasOpponent
            };
        }

        public void SetLobbyData(LobbyData data)
        {
            IsPrivate = data.IsPrivate;
            Password = data.Password;
            Host = data.Host;
            Opponent = data.Opponent;
            HasOpponent = data.HasOpponent;
        }

        public void ToggleReady(bool value)
        {
            if (IsServer)
            {
                CheckOpponentReady();
            } else
            {
                readyBotImage.gameObject.SetActive(value);
                SendOpponentReadyStatusRpc(value);
            }
        }

        [ServerRpc]
        public void SendOpponentReadyStatusRpc(bool value, NetworkConnection connection = default)
        {
            if (HasPlayer(connection.ClientId))
            {
                SetOpponentReady(value);

                if (IsServer)
                {
                    readyTopImage.gameObject.SetActive(value);
                    Game.Console.Log($"Client id {connection.ClientId} ready: {value}");
                }
            }
        }

        public void CheckOpponentReady()
        {
            if (OpponentIsReady)
            {
                // Game.Network.OnClientConnectedCallback -= OnClientConnectedCallback;
            }
        }
    }
}
