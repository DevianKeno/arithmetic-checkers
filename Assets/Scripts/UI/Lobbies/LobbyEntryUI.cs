using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using PlayEveryWare.EpicOnlineServices.Samples;
using System;

namespace Damath.UI.Lobbies
{
    public class LobbyEntryUI : MonoBehaviour, IPointerDownHandler
    {
        public Lobby Lobby;
        public event EventHandler OnClick;
        [SerializeField] TextMeshProUGUI displayNameTMP;

        public void OnPointerDown(PointerEventData eventData)
        {
            OnClick?.Invoke(this, new());
        }

        public void SetLobbyInfo(Lobby lobby)
        {
            Lobby = lobby;
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