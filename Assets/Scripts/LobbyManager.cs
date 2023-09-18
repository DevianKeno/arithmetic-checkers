using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using Unity.Netcode;
using System.Text.RegularExpressions;

namespace Damath
{
    /*
    public class LobbyManager : NetworkBehaviour
    {
        public Lobby Lobby;
        [SerializeField] private Image readyBotImage;
        [SerializeField] private Image readyTopImage;
        [SerializeField] private ToggleButton button;

        void Awake()
        {
            // Don't forget to unsub!
            Game.Events.OnLobbyHost += RetrieveLobby;
            Game.Events.OnLobbyJoin += GiveLobby;
            Game.Events.OnPlayerCreate += SetSides;
        }

        public void SetSides(Player player)
        {
            if (!Network.Main.IsListening) return;

            // This should is sides are swapped via rules
            if (Lobby.IsHost)
            {
                player.SetSide(Side.Bot);
                player.IsControllable = true; // It's inside so you can't control the other player!
            } else if (Lobby.IsOpponent)
            {
                player.SetSide(Side.Top);
                player.IsControllable = true;
            }
        }
        
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Network.Main.OnClientConnectedCallback += OnClientConnectedCallback;
        }

        public void RetrieveLobby(Lobby lobby)
        {
            Lobby = lobby;
            Lobby.SetHost(Game.Network.LocalClientId);
        }

        public void GiveLobby(ulong clientId, Lobby lobby)
        {
            if (IsHost) return;
            if (lobby == null) return;

            ReceiveLobbyClientRpc(1);
        }

        [ClientRpc]
        public void ReceiveLobbyClientRpc(int mode, ClientRpcParams clientRpcParams = default)
        {
            if (IsServer) return;
            Game.Console.Log($"Received lobby with mode");
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if (Lobby == null) return;
            if (Lobby.Host == clientId) return;

            if (IsServer)
            {
                if (Lobby.HasPlayer(clientId)) return;

                if (!Lobby.HasOpponent)
                {
                    Lobby.SetOpponent(clientId);
                    Game.Console.Log($"Client {clientId} joined lobby as opponent");
                } else
                {
                    Lobby.ConnectPlayer(clientId);
                }
                
                
                ClientRpcParams clientRpcParams = new()
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[]{clientId}
                    }
                };

                Game.Console.Log("Passing ruleset to opponent");
                // if (Lobby.Ruleset.Mode != Ruleset.Type.Custom)
                // {
                //     ReceiveLobbyInfoClientRpc((int)Lobby.Ruleset.Mode, clientRpcParams);
                // } else
                // {
                //     ReceiveLobbyInfoClientRpc(Lobby.Ruleset, clientRpcParams);
                // }
            }
        }
        
        [ClientRpc]
        public void ReceiveLobbyInfoClientRpc(int mode, ClientRpcParams clientRpcParams)
        {
            Game.Console.Log($"Fetching ruleset from lobby");
            if (IsOwner) return;
            Game.Console.Log($"Fetched ruleset {mode} from lobby");

            // This should handle other lobby information as well
            // Lobby = new(new ((Ruleset.Type)mode));
            // Game.Console.Log($"Joined lobby with match {Lobby.Ruleset.Mode}");
        }

        /// <summary>
        /// For custom matches.
        /// </summary>
        [ClientRpc]
        public void ReceiveLobbyInfoClientRpc(Ruleset ruleset, ClientRpcParams clientRpcParams)
        {
            //
        }

        public void ToggleReady(bool value)
        {
            if (IsServer)
            {
                CheckOpponentReady();
            } else
            {
                readyBotImage.gameObject.SetActive(value);
                SendOpponentReadyStatusServerRpc(value);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void SendOpponentReadyStatusServerRpc(bool value, ServerRpcParams serverRpcParams = default)
        {
            var clientId = serverRpcParams.Receive.SenderClientId;

            if (Lobby.HasPlayer(clientId))
            {
                Lobby.SetOpponentReady(value);

                if (IsServer)
                {
                    readyTopImage.gameObject.SetActive(value);
                    Game.Console.Log($"Client id {clientId} ready: {value}");
                }
            }
        }

        public void CheckOpponentReady()
        {
            if (Lobby == null) return;

            if (Lobby.OpponentIsReady)
            {
                // Game.Network.OnClientConnectedCallback -= OnClientConnectedCallback;
                BeginMatchClientRpc();
            }
        }

        public void BeginMatchClientRpc(ClientRpcParams clientRpcParams = default)
        {
            button.gameObject.SetActive(false);
            readyBotImage.gameObject.SetActive(false);
            readyTopImage.gameObject.SetActive(false);
            Game.Console.Log("Starting match");
            Game.Main.StartMatch();
        }

        [ServerRpc]
        public void HadJoinedLobbyServerRpc(ServerRpcParams serverRpcParams = default)
        {

        }
    }
    */
}
