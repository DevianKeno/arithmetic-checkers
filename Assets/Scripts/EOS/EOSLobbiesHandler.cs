using System;
using System.Collections.Generic;
using UnityEngine;
using Damath.UI;

using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

using Damath.UI.Lobbies;
using Damath.EOS.Lobbies;

namespace Damath.EOS
{
    public class EOSLobbiesHandler : MonoBehaviour
    {
        EOSLobbyManager lobbyManager => Game.EOSManagers.LobbyManager;
        Lobby selectedLobby = null;
        LobbyDetails selectedLobbyDetails = null;
        List<LobbyEntryUI> lobbyEntries = new();
        
        [Header("Scripts")]
        [SerializeField] LobbyInfoContainer lobbyInfo;

        [Header("UI Elements")]
        [SerializeField] GameObject lobbyContainer;
        [SerializeField] Button refreshBtn;
        [SerializeField] Button joinBtn;
        [SerializeField] LoadingIconAnimation loadingIcon;

        [Header("Prefabs")]
        [SerializeField] GameObject lobbyEntryPrefab;

        void Awake()
        {
            refreshBtn.onClick.AddListener(SearchLobbies);
            joinBtn.onClick.AddListener(JoinSelectedLobby);
        }

        public void SearchLobbies()
        {
            selectedLobby = null;
            selectedLobbyDetails = null;
            ClearLobbyEntries();
            lobbyInfo.Clear();
            joinBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(true);
            /// HMMMMMMMMMMMMMMMMMMMMMMMM
            lobbyManager.SearchByAttribute("RULESET", "Standard", OnSearchCompleted);
        }
        
        public void JoinSelectedLobby()
        {
            if (selectedLobby == null)
            {
                return;
            }
            Game.EOSManagers.LobbyManager.JoinLobby(selectedLobby.Id, selectedLobbyDetails, false, OnJoinLobbyCompleted);
        }

        void OnJoinLobbyCompleted(Result result)
        {
            if (result == Result.Success)
            {
                if (selectedLobby.TryGetAttribute("RULESET", out var attr))
                {
                    Ruleset ruleset = attr.AsString switch
                    {
                        "Standard" => Ruleset.Create(RulesetType.Standard),
                        "Speed" => Ruleset.Create(RulesetType.Speed),
                        "Custom" => Ruleset.Create(RulesetType.Custom),
                    };
                    Game.Main.CreateMatch(ruleset);
                    Game.Main.LoadScene("Match", playTransition: true);
                }
            } else
            {
                Game.Console.Log($"Error joining lobby. [" + result + "]");
                Debug.LogError("Error joining lobby. [" + result + "]");
            }
        }

        void ClearLobbyEntries()
        {
            foreach (LobbyEntryUI entry in lobbyEntries)
            {
                Destroy(entry.gameObject);
            }
            lobbyEntries.Clear();
        }

        void OnSearchCompleted(Result result)
        {
            loadingIcon.gameObject.SetActive(false);

            if (result == Result.Success)
            {
                Dictionary<Lobby, LobbyDetails> searchResults = lobbyManager.GetSearchResults();
                ClearLobbyEntries();
                
                foreach (var kv in searchResults)
                {
                    var lobby = kv.Key;
                    var lobbyDetails = kv.Value;
                    var go = Instantiate(lobbyEntryPrefab, lobbyContainer.transform);

                    LobbyEntryUI entry = go.GetComponent<LobbyEntryUI>();
                    entry.SetLobbyInfo(lobby, lobbyDetails);
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
            selectedLobby = lobbyEntry.Lobby;
            selectedLobbyDetails = lobbyEntry.LobbyDetails;
            lobbyInfo.SetLobbyInfo(lobbyEntry.Lobby);
            joinBtn.gameObject.SetActive(true);
        }
    }
}