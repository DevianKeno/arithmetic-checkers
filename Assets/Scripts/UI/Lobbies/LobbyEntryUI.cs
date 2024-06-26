using System;

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using Damath.Network;

namespace Damath.UI.Lobbies
{
    public class LobbyEntryUI : MonoBehaviour, IPointerDownHandler
    {
        public Lobby Lobby;
        public event Action<PointerEventData> OnClick;
        [SerializeField] TextMeshProUGUI displayNameTMP;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(eventData);
        }

        public void SetLobbyInfo(Lobby lobby)
        {
            Lobby = lobby;
        }
    }
}