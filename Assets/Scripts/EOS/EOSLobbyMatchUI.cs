using Damath.UI;
using UnityEngine;
using TMPro;

namespace Damath
{
    public class EOSLobbyMatchUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI lobbyIdTMP;
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;

        void Start()
        {
            var lobby = Game.EOSManagers.LobbyManager.GetCurrentLobby();

            if (lobby.IsValid())
            {
                lobbyIdTMP.text = $"Lobby Id: {lobby.Id}";
            }
        }
    }
}