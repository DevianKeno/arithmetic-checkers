using System;
using UnityEngine;
using Damath.UI;

using PlayEveryWare.EpicOnlineServices.Samples;
using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

namespace Damath.EOS
{
    public class EOSLobbyCreateUI : MonoBehaviour
    {
        public uint MaxPlayers = 2;
        public LobbyPermissionLevel lobbyPermissionLevel;

        Ruleset cachedRuleset;
        RulesetType selectedRulesetType;
        Window creatingLobbyWindow;

        [SerializeField] Button createLobbyBtn;
        
        void Awake()
        {
            createLobbyBtn.onClick.AddListener(CreateLobby);
        }

        public Ruleset CreateRuleset()
        {
            var ruleset = selectedRulesetType switch
            {
                RulesetType.Standard => Ruleset.CreateStandard(),
                RulesetType.Speed => Ruleset.CreateSpeed(),
                RulesetType.Custom => throw new NotImplementedException()
            };
            return ruleset;
        }

        public void SetSelectedRulesetType(string type)
        {
            /// Lmao all standard hshuhuw
            selectedRulesetType = type switch
            {
                "classic" => RulesetType.Standard,
                "classic_random" => RulesetType.Standard,
                "speed" => RulesetType.Standard,
                "custom" => RulesetType.Standard,
            };
        }

        public Lobby CreateLobbyProperties()
        {
            cachedRuleset = CreateRuleset();
            if (cachedRuleset == null)
            {
                Game.Console.Log("Error creating lobby, no ruleset specified.");
                Debug.LogError("Error creating lobby, no ruleset specified.");
            }

            /// The top-level, game-specific filtering information for session searches.
            /// This criteria should be set with mostly static, coarse settings,
            /// often formatted like "GameMode:Region:MapName".
            string rulesetType = cachedRuleset.Type.ToString();
            string region = "PH";
            string bucketId = $"{rulesetType}:{region}";

            var lobbyProperties = new Lobby
            {
                /// Bucket Id
                BucketId = bucketId,
                /// Max Players
                MaxNumLobbyMembers = MaxPlayers,
                /// Allow Invites
                AllowInvites = true,
                /// Presence Enabled
                PresenceEnabled = true,
                /// Voice chat
                RTCRoomEnabled = false,
                /// Permission
                LobbyPermissionLevel = lobbyPermissionLevel
            };

            /// -- Lobby attributes --
            /// Ruleset attribute
            var rulesetAttribute = new LobbyAttribute
            {
                Key = "RULESET",
                AsString = rulesetType,
                ValueType = AttributeType.String,
                Visibility = LobbyAttributeVisibility.Public  /// Needs to be public for Search
            };
            lobbyProperties.Attributes.Add(rulesetAttribute);

            /// Anti-Cheat attribute
            var antiCheatAttribute = new LobbyAttribute
            {
                Key = "ANTICHEAT",
                AsBool = false, /// False for now :(
                ValueType = AttributeType.Boolean,
                Visibility = LobbyAttributeVisibility.Public
            };
            lobbyProperties.Attributes.Add(antiCheatAttribute);
            
            return lobbyProperties;
        }

        public void CreateLobby()
        {
            ProductUserId currentUserProductId = Game.EOS.GetProductUserId();
            if (!currentUserProductId.IsValid())
            {
                Debug.LogError("Lobbies (CreateLobby): Current player is invalid!");
                return;
            }

            Game.UI.Dim(0.25f);
            creatingLobbyWindow = Game.UI.Create("creating_lobby_window") as Window;
            Game.EOSManagers.LobbyManager.CreateLobby(CreateLobbyProperties(), OnCreateLobbyCompleted);
        }

        void OnCreateLobbyCompleted(Result result)
        {
            creatingLobbyWindow.Destroy();

            if (result == Result.Success)
            {
                var localUserId = Game.EOS.GetProductUserId();
                Game.EOSManagers.LobbyManager.PromoteMember(localUserId, OnPromoteMemberCompleted);
            } else
            {
                Game.Console.Log($"Error creating lobby. [" + result + "]");
                Debug.LogError("Error creating lobby. [" + result + "]");
            }
        }

        void OnPromoteMemberCompleted(Result result)
        {
            if (result == Result.Success)
            {
                Game.UI.Undim(0.25f);
                Game.Main.CreateMatch(cachedRuleset);
                Game.Main.LoadScene("Match", playTransition: true);
            } else
            {
                Game.Console.Log($"Error promoting owner. [" + result + "]");
                Debug.LogError("Error promoting owner. [" + result + "]");
            }
        }
    }
}