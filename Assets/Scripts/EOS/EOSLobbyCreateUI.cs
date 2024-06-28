using System;
using UnityEngine;
using Aritche.UI;

using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

using Aritche.EOS.Lobbies;
using Aritche.Rules;

namespace Aritche.EOS
{
    public class EOSLobbyCreateUI : MonoBehaviour
    {
        public uint MaxPlayers = 2;
        public LobbyPermissionLevel lobbyPermissionLevel;

        Ruleset cachedRuleset;
        RulesetPreset selectedRulesetPreset;
        Window creatingLobbyWindow;

        [SerializeField] Button createLobbyBtn;
        
        void Awake()
        {
            createLobbyBtn.onClick.AddListener(CreateLobby);
        }

        public void SetSelectedRulesetPreset(string value)
        {
            selectedRulesetPreset = value switch
            {
                "classic" => RulesetPreset.Classic,
                "random" => RulesetPreset.ClassicRandom,
                "speed" => RulesetPreset.Speed,
                "100" => RulesetPreset.HundredPercent,
                "custom" => RulesetPreset.Custom,
            };
        }

        public Lobby CreateLobbyProperties()
        {
            cachedRuleset = Ruleset.CreateFromPreset(selectedRulesetPreset);
            if (cachedRuleset == null)
            {
                Game.Console.Log("Error creating lobby, no ruleset specified.");
                Debug.LogError("Error creating lobby, no ruleset specified.");
                return null;
            }

            /// The top-level, game-specific filtering information for session searches.
            /// This criteria should be set with mostly static, coarse settings,
            /// often formatted like "GameMode:Region:MapName".
            string rulesetType = cachedRuleset.Preset.ToString();
            string region = "ASIA";
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
            var loginStatus = Game.EOS.GetEOSAuthInterface().GetLoginStatus(Game.EOS.GetLocalUserId());

            if (loginStatus == LoginStatus.NotLoggedIn)
            {
                cachedRuleset = Ruleset.CreateFromPreset(selectedRulesetPreset);
                Game.Main.CreateMatch(cachedRuleset);
                Game.Main.LoadScene("Match", playTransition: true);
            }
            else if (loginStatus == LoginStatus.LoggedIn)
            {
                ProductUserId currentUserProductId = Game.EOS.GetProductUserId();
                if (!currentUserProductId.IsValid())
                {
                    Debug.LogError("Lobbies (CreateLobby): Current player is invalid!");
                    return;
                }

                Game.UI.Dim(0.25f);
                creatingLobbyWindow = Game.UI.Create("creating_lobby_window") as Window;
                Game.EOSManagers.Lobbies.CreateLobby(CreateLobbyProperties(), OnCreateLobbyCompleted);
            }
            else if (loginStatus == LoginStatus.UsingLocalProfile)
            {
                Game.Console.Log($"Unhandled login status." + loginStatus);
                Debug.LogError($"Unhandled login status." + loginStatus);
            }
        }

        void OnCreateLobbyCompleted(Result result)
        {
            creatingLobbyWindow.Destroy();

            if (result == Result.Success)
            {
                var localUserId = Game.EOS.GetProductUserId();
                Game.EOSManagers.Lobbies.PromoteMember(localUserId, OnPromoteMemberCompleted);
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