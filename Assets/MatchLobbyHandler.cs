using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;
using System;
using Damath.UI;

namespace Damath.Network
{
    public class MatchLobbyHandler : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;
        
        void Start()
        {
            if (Game.Network.isNetworkActive)
            {
                SetupOnlineMatch();
            } else
            {
                SetupOfflineMatch();
            }
        }

        void SetupOnlineMatch()
        {
            var lobby = Game.Network.Lobbies.GetCurrentLobby();

            var ruleset = Ruleset.Create(lobby.Properties.RulesetType);
            Game.Main.SetRuleset(ruleset);

            /// Set lobby owner as blue player
            bluePlayerDisplay.SetUserInfoDisplayed(lobby.Owner);
        }
        
        void SetupOfflineMatch()
        {
            
        }
    }
}