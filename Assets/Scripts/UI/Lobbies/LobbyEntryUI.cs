using System;

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using Epic.OnlineServices.Lobby;

using Aritche.EOS.Lobbies;

namespace Aritche.UI.Lobbies
{
    public class LobbyEntryUI : MonoBehaviour, IPointerDownHandler
    {
        public Lobby Lobby;
        public LobbyDetails LobbyDetails;
        public event EventHandler OnClick;
        [SerializeField] TextMeshProUGUI displayNameTMP;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(this, new());
        }

        public void SetLobbyInfo(Lobby lobby, LobbyDetails lobbyDetails)
        {
            Lobby = lobby;
            LobbyDetails = lobbyDetails;
            foreach (var a in lobby.Attributes)
            {
                if (a.Key == "RULESET")
                {
                    displayNameTMP.text = a.AsString;
                }
            }
        }
    }
}