using UnityEngine;

using Damath.UI;

namespace Damath
{
    public class LobbyProperties
    {
        public int MaxPlayers;
        public RulesetType RulesetType;
    }

    public class LobbyCreateUI : MonoBehaviour
    {
        LobbyProperties cachedProperties = new();

        [Header("UI Elements")]
        [SerializeField] Button createLobbyBtn;

        void Awake()
        {
            createLobbyBtn.onClick.AddListener(CreateLobby);
        }

        public void SetRuleset(string type)
        {
            cachedProperties.RulesetType = type switch
            {
                "classic" => RulesetType.Standard,
                "speed" => RulesetType.Speed,
                "custom" => RulesetType.Custom,
                _ => RulesetType.Standard,
            };
        }

        public LobbyProperties GetCachedProperties()
        {
            return cachedProperties;
        }

        public void CreateLobby()
        {
            Game.Network.Lobbies.CreateLobby(GetCachedProperties());
        }
    }
}