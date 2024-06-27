using System;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Platform;

using PlayEveryWare.EpicOnlineServices;

using Damath.UI;
using Damath.EOS.Lobbies;

namespace Damath
{
    public class EOSLobbyMatchUI : MonoBehaviour
    {
        Lobby currentLobby;
        OpponentJoinedWindow ojw;

        [SerializeField] TextMeshProUGUI lobbyIdTMP;
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;

        void Start()
        {
            var lobby = Game.EOSManagers.Lobbies.GetCurrentLobby();

            if (lobby.IsValid()) /// Match has lobby, probably online
            {
                currentLobby = lobby;

                if (lobby.IsOwner(Game.EOS.GetProductUserId()))
                {
                    SetupMatchLobbyAsOwner();
                } else
                {
                    SetupMatchLobbyAsMember();
                }

                Game.EOSManagers.Lobbies.AddNotifyMemberUpdateReceived(OnMemberUpdate);
            } else /// probably offline
            {
                Game.Console.Log("Match has no lobby. Playing offline.");
                lobbyIdTMP.gameObject.SetActive(false);
            }
        }

        void SetupMatchLobbyAsOwner()
        {
            Game.Console.Log($"Hosted lobby with id: {currentLobby.Id}");
            bluePlayerDisplay.gameObject.SetActive(true);
            orangePlayerDisplay.gameObject.SetActive(true);
            lobbyIdTMP.gameObject.SetActive(true);
            lobbyIdTMP.text = $"Lobby Id: {currentLobby.Id}";
            SetLobbyOwnerAsBlue();
        }

        void SetupMatchLobbyAsMember()
        {
            Game.Console.Log($"Joined lobby with id: {currentLobby.Id}");
            bluePlayerDisplay.gameObject.SetActive(true);
            orangePlayerDisplay.gameObject.SetActive(true);
            lobbyIdTMP.gameObject.SetActive(true);
            lobbyIdTMP.text = $"Lobby Id: {currentLobby.Id}";

            QueryPlayersAndSetDisplay();
        }

        /// <summary>
        /// The lobby owner is automatically set as the blue player.
        /// Can be changed though.
        /// </summary>
        void SetLobbyOwnerAsBlue()
        {
            var localEpicId = Game.EOS.GetLocalUserId();
            var localProductId = Game.EOS.GetProductUserId();

            /// I have no idea why the lobby uses ProductUserId
            /// then the query uses EpicUserId
            if (currentLobby.IsOwner(localProductId))
            {
                Game.EOSManagers.UserInfo.QueryUserInfoById(localEpicId, (EpicAccountId id, Result result) =>
                {
                    var userInfo = Game.EOSManagers.UserInfo.GetUserInfoById(id);
                    bluePlayerDisplay.SetPlayerDisplayed(userInfo);
                });
            }
        }

        /// <summary>
        /// I think this is called everytime someone joins/leaves the lobby?
        /// </summary>
        /// <param name="lobbyId"></param>
        /// <param name="memberId"></param>
        void OnMemberUpdate(string lobbyId, ProductUserId memberId)
        {
            if (currentLobby.Id != lobbyId)
            {
                return;
            }
            
            /// Check if memberId already exists in the lobby
            if (!currentLobby.FindLobbyMember(memberId, out LobbyMember member))
            {
                var newMember = new LobbyMember
                {
                    ProductId = memberId
                };
                currentLobby.Members.Add(newMember);
                SetMemberAsPlayer(newMember);
            }

            if (memberId != currentLobby.LobbyOwner)
            {
                PrepareForMatch();
            }
        }

        void SetMemberAsPlayer(LobbyMember member)
        {
            if (currentLobby.BluePlayer == null)
            {
                currentLobby.BluePlayer = member;
                bluePlayerDisplay.DisplayName = member.DisplayName;
            } else /// set as orange player
            {
                currentLobby.OrangePlayer = member;
                orangePlayerDisplay.DisplayName = member.DisplayName;
            }
        }

        void PrepareForMatch()
        {
            QueryPlayersAndSetDisplay();

            ojw = Game.UI.Create<OpponentJoinedWindow>("opponent_joined");
            ojw.SetPlayer(Side.Blue, currentLobby.BluePlayer);
            ojw.SetPlayer(Side.Orange, currentLobby.OrangePlayer);
            ojw.Show();
        }

        void QueryPlayersAndSetDisplay()
        {
            var blueId = currentLobby.BluePlayer.AccountId;
            var orangeId = currentLobby.OrangePlayer.AccountId;

            Game.EOSManagers.UserInfo.QueryUserInfoById(blueId, (EpicAccountId id, Result result) =>
            {
                var userInfo = Game.EOSManagers.UserInfo.GetUserInfoById(id);
                bluePlayerDisplay.SetPlayerDisplayed(userInfo);
            });

            Game.EOSManagers.UserInfo.QueryUserInfoById(orangeId, (EpicAccountId id, Result result) =>
            {
                var userInfo = Game.EOSManagers.UserInfo.GetUserInfoById(id);
                orangePlayerDisplay.SetPlayerDisplayed(userInfo);
            });
        }
    }
}