using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

using Aritche.EOS.Lobbies;

namespace Aritche.UI
{
    public class LobbyInfoContainer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI rulesetTMP;
        [SerializeField] TextMeshProUGUI blueTMP;
        [SerializeField] TextMeshProUGUI orangeTMP;

        public void SetLobbyInfo(Lobby lobby)
        {
            foreach (var attr in lobby.Attributes)
            {
                if (attr.Key == "RULESET")
                {
                    rulesetTMP.text = $"Ruleset: {attr.AsString}";
                }
            }
            blueTMP.text = $"Blue: {lobby.BluePlayer.DisplayName}";
            orangeTMP.text = $"Orange: {lobby.OrangePlayer.DisplayName}";
            
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            rulesetTMP.text = $"Ruleset: ";
            blueTMP.text = $"Blue: ";
            orangeTMP.text = $"Orange: ";
        }
    }
}