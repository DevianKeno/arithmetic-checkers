using System;

using UnityEngine;
using UnityEngine.UI;

using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

using Aritche.Rules;
using Aritche.UI;
using Aritche.EOS.Lobbies;

namespace Aritche.EOS
{
    public class CreateMatchHandler : MonoBehaviour
    {
        public uint MaxPlayers = 2;
        public LobbyPermissionLevel lobbyPermissionLevel;

        Ruleset ruleset;
        RulesetPreset selectedRulesetPreset;
        Window creatingLobbyWindow;
        CustomMatchHandler customMatchHandler;

        [Header("UI Elements")]
        [SerializeField] FrameController firstFrameController;
        [SerializeField] UI.Button createLobbyBtn;
        
        [Header("Button Groups")]
        [SerializeField] ToggleGroup rulePresetBtnGrp;
        
        void Awake()
        {
            customMatchHandler = GetComponent<CustomMatchHandler>();
            createLobbyBtn.onClick.AddListener(CreateLobby);
        }

        void Start()
        {
            createLobbyBtn.interactable = true;
            ruleset = new();

            /// UnityToggles execute events twice, when setting it to true and to false
            /// I want to execute an event only when true, and still use it in the inspector
            /// This is my third option on implementing this
            /// I can definitely make my own UI elements but I have no time right now
            InitializeRulesetPresetButtons();
        }

        void InitializeRulesetPresetButtons()
        {
            /// Classic
            rulePresetBtnGrp.transform.GetChild(0).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ruleset = Ruleset.CreateFromPreset(RulesetPreset.Classic);
                    Debug.Log($"Set current match ruleset to {RulesetPreset.Classic}");
                }
            });
            /// Classic Random
            rulePresetBtnGrp.transform.GetChild(1).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ruleset = Ruleset.CreateFromPreset(RulesetPreset.ClassicRandom);
                    Debug.Log($"Set current match ruleset to {RulesetPreset.ClassicRandom}");
                }
            });
            /// Speed
            rulePresetBtnGrp.transform.GetChild(2).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ruleset = Ruleset.CreateFromPreset(RulesetPreset.Speed);
                    Debug.Log($"Set current match ruleset to {RulesetPreset.Speed}");
                }
            });
            /// 100%
            rulePresetBtnGrp.transform.GetChild(3).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ruleset = Ruleset.CreateFromPreset(RulesetPreset.HundredPercent);
                    Debug.Log($"Set current match ruleset to {RulesetPreset.HundredPercent}");
                }
            });
            /// Custom
            rulePresetBtnGrp.transform.GetChild(4).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    ruleset = Ruleset.CreateFromPreset(RulesetPreset.Custom);
                    firstFrameController.SwitchToFrame("Custom");
                    Debug.Log($"Set current match ruleset to {RulesetPreset.Custom}");
                }
            });
        }

        // public void SetSelectedRulesetPreset(string value)
        // {
        //     selectedRulesetPreset = value switch
        //     {
        //         "classic" => RulesetPreset.Classic,
        //         "random" => RulesetPreset.ClassicRandom,
        //         "speed" => RulesetPreset.Speed,
        //         "100" => RulesetPreset.HundredPercent,
        //         "custom" => RulesetPreset.Custom,
        //     };

        //     if (value != "custom")
        //     {
        //         var preset = value switch
        //         {
        //             "classic" => RulesetPreset.Classic,
        //             "random" => RulesetPreset.ClassicRandom,
        //             "speed" => RulesetPreset.Speed,
        //             "100" => RulesetPreset.HundredPercent
        //         };

        //         ruleset = Ruleset.CreateFromPreset(preset);
        //     }
        //     else
        //     {
        //         ruleset = new();
        //     }
        //     Debug.Log($"Set current match ruleset to {RulesetPreset.Classic}");
        // }

        public Lobby CreateLobbyProperties()
        {
            if (selectedRulesetPreset == RulesetPreset.Custom)
            {
                ruleset = customMatchHandler.GetCachedCustomRuleset();
            }

            if (ruleset == null)
            {
                Game.Console.Log("Error creating lobby, no ruleset specified.");
                Debug.LogError("Error creating lobby, no ruleset specified.");
                return null;
            }

            /// The top-level, game-specific filtering information for session searches.
            /// This criteria should be set with mostly static, coarse settings,
            /// often formatted like "GameMode:Region:MapName".
            string rulesetType = ruleset.Preset.ToString();
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
            if (Game.Main.IsOnline) /// Network
            {
                var loginStatus = Game.EOS.GetEOSAuthInterface().GetLoginStatus(Game.EOS.GetLocalUserId());

                if (loginStatus == LoginStatus.NotLoggedIn)
                {
                    ruleset = Ruleset.CreateFromPreset(selectedRulesetPreset);
                    Game.Main.CreateMatch(ruleset);
                    Game.Main.LoadScene("Match", playTransition: true, 3f);
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
            else
            {
                createLobbyBtn.interactable = false;
                Game.Main.CreateMatch(ruleset);
                Game.Main.LoadScene("Match", playTransition: true, 3f);
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
                Game.Main.CreateMatch(ruleset);
                Game.Main.LoadScene("Match", playTransition: true);
            } else
            {
                Game.Console.Log($"Error promoting owner. [" + result + "]");
                Debug.LogError("Error promoting owner. [" + result + "]");
            }
        }
    }
}