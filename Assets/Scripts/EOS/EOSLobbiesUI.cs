using System;
using System.Collections.Generic;
using UnityEngine;
using Damath.UI;

using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;
using PlayEveryWare.EpicOnlineServices.Samples;
using Damath.UI.Lobbies;

namespace Damath.EOS
{
    public class EOSLobbiesUI : MonoBehaviour
    {
        LobbyEntryUI currentlySelectedEntry = null;
        EOSLobbyManager lobbyManager => Game.EOSManagers.LobbyManager;
        List<LobbyEntryUI> lobbyEntries = new();

        [SerializeField] GameObject lobbyContainer;
        [SerializeField] Button refreshBtn;

        [Header("Prefabs")]
        [SerializeField] GameObject lobbyEntryPrefab;

        void Awake()
        {
            refreshBtn.onClick.AddListener(SearchLobbies);
        }

        void SearchLobbies()
        {
            lobbyManager.SearchByLobbyId("ASIA", OnSearchCompleted);
        }

        void OnSearchCompleted(Result result)
        {
            if (result == Result.Success)
            {
                Dictionary<Lobby, LobbyDetails> searchResults = lobbyManager.GetSearchResults();

                foreach (LobbyEntryUI entry in lobbyEntries)
                {
                    Destroy(entry.gameObject);
                }
                lobbyEntries.Clear();
                
                foreach (Lobby lobby in searchResults.Keys)
                {
                    var go = Instantiate(lobbyEntryPrefab, lobbyContainer.transform);

                    LobbyEntryUI entry = go.GetComponent<LobbyEntryUI>();
                    entry.SetLobbyInfo(lobby);
                    entry.OnClick += (object sender, EventArgs e) =>
                    {
                        OnClickLobbyEntry(sender as LobbyEntryUI);
                    };
                    lobbyEntries.Add(entry);
                }
            } else
            {
                Game.Console.Log($"Error searching lobbies. [" + result + "]");
                Debug.LogError("Error searching lobbies. [" + result + "]");
            }
        }

        void OnClickLobbyEntry(LobbyEntryUI lobbyEntry)
        {
            currentlySelectedEntry = lobbyEntry;
            DisplayLobbyInfoUI(lobbyEntry.Lobby);
        }

        void DisplayLobbyInfoUI(Lobby lobby)
        {
            
        }
    }
}