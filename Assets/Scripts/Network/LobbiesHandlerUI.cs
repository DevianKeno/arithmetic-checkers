using UnityEngine;
using TMPro;

namespace Damath.UI
{
    public class LobbiesHandlerUI : MonoBehaviour
    {
        [SerializeField] LobbyCreateUI lobbyCreator;
        
        [Header("UI Elements")]
        [SerializeField] Button refreshLobbiesBtn;
        [SerializeField] TMP_InputField searchField;
        [SerializeField] GameObject lobbyEntriesContainer;
        
        [Header("Prefabs")]
        [SerializeField] GameObject lobbyEntryPrefab;

        void Awake()
        {
            refreshLobbiesBtn.onClick.AddListener(RefreshLobbies);
        }

        public void RefreshLobbies()
        {

        }
    }
}