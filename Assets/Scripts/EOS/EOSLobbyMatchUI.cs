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

        [SerializeField] TextMeshProUGUI lobbyIdTMP;
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;

        void Start()
        {
            var lobby = Game.EOSManagers.LobbyManager.GetCurrentLobby();

            if (lobby.IsValid())
            {
                Game.Console.Log($"Hosted lobby with id: {lobby.Id}");
                bluePlayerDisplay.gameObject.SetActive(true);
                orangePlayerDisplay.gameObject.SetActive(true);
                lobbyIdTMP.gameObject.SetActive(true);

                currentLobby = lobby;
                lobbyIdTMP.text = $"Lobby Id: {lobby.Id}";
                SetLobbyOwnerAsBlue();
                Game.EOSManagers.LobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdate);
                
            } else
            {
                Game.Console.Log("Match has no lobby. Playing on sandbox mode.");
                bluePlayerDisplay.gameObject.SetActive(false);
                orangePlayerDisplay.gameObject.SetActive(false);
                lobbyIdTMP.gameObject.SetActive(false);
            }
        }

        void SetLobbyOwnerAsBlue()
        {
            var localEpicUserId = Game.EOS.GetLocalUserId();
            var localProductUserId = Game.EOS.GetProductUserId();

            if (currentLobby.IsOwner(localProductUserId))
            {
                Game.EOSManagers.UserInfoManager.QueryUserInfoById(localEpicUserId, OnQueryUserInfoCompleted);
            }
        }

        void OnQueryUserInfoCompleted(EpicAccountId id, Result result)
        {
            var userInfo = Game.EOSManagers.UserInfoManager.GetUserInfoById(id);
            bluePlayerDisplay.SetPlayerDisplayed(userInfo);
        }

        void OnMemberUpdate(string lobbyId, ProductUserId memberId)
        {
            if (currentLobby.Id != lobbyId)
            {
                return;
            }
            
            /// Check if memberId exists in the lobby
            if (!currentLobby.FindLobbyMember(memberId, out LobbyMember member))
            {
                var newMember = new LobbyMember
                {
                    ProductId = memberId
                };
                currentLobby.Members.Add(newMember);

                if (currentLobby.BluePlayer == null)
                {
                    currentLobby.BluePlayer = newMember;
                    bluePlayerDisplay.DisplayName = member.DisplayName;
                } else /// set as orange player
                {
                    currentLobby.OrangePlayer = newMember;
                    orangePlayerDisplay.DisplayName = member.DisplayName;
                }
            }
        }
    }
}