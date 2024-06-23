using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Damath.UI;
using Damath.EOS;

using Epic.OnlineServices;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.UserInfo;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Damath
{
    public class EOSLobbyMatchUI : MonoBehaviour
    {
        Lobby currentLobby;
        // List<UIMemberEntry> UIMemberEntries = new();

        [SerializeField] TextMeshProUGUI lobbyIdTMP;
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;

        void Start()
        {
            var lobby = Game.EOSManagers.LobbyManager.GetCurrentLobby();

            if (lobby.IsValid())
            {
                currentLobby = lobby;
                lobbyIdTMP.text = $"Lobby Id: {lobby.Id}";

                Game.EOSManagers.LobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdate);
            }
        }

        void OnMemberUpdate(string lobbyId, ProductUserId memberId)
        {
            if (currentLobby.Id != lobbyId)
            {
                return;
            }
            
            /// Check if memberId exists in the lobby
            var member = currentLobby.Members.Find((LobbyMember member) =>
            {
                return member.ProductId == memberId;
            });
            if (member != null)
            {
                currentLobby.Members.Add(member);

                orangePlayerDisplay.DisplayName = member.DisplayName;
            }
        }
    }
}