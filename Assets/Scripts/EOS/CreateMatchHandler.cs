using System;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
        Window creatingLobbyWindow;
        [SerializeField] CustomMatchHandler customMatchHandler;

        [Header("UI Elements")]
        [SerializeField] FrameController firstFrameController;
        [SerializeField] TextMeshProUGUI rulesetInfoTMP;
        [SerializeField] UI.Button createLobbyBtn;
        [SerializeField] UI.Button createCustomBtn;
        
        [Header("Button Groups")]
        [SerializeField] ToggleGroup rulePresetBtnGrp;
        
        void Awake()
        {
            createLobbyBtn.onClick.AddListener(CreateLobby);
            createCustomBtn.onClick.AddListener(CreateLobby);
        }

        void Start()
        {
            createLobbyBtn.interactable = true;
            createCustomBtn.interactable = true;
            CreateRulesetAndListen(Ruleset.CreateFromPreset(RulesetPreset.Classic));

            /// UnityToggles execute events twice, when setting it to true and to false
            /// I want to execute an event only when true, and still use it in the inspector
            /// This is my third option on implementing this
            /// I can definitely make my own UI elements but I have no time right now
            InitializeRulesetPresetButtons();
        }

        /// <summary>
        /// Create ruleset and listen for updates.
        /// </summary>
        public void CreateRulesetAndListen(Ruleset value)
        {
            ruleset = value;
            customMatchHandler.Ruleset = value;
            rulesetInfoTMP.text = ruleset.ToRulesetInfoDisplay();
            ruleset.OnUpdate += OnRulesetUpdate;
        }

        void OnRulesetUpdate(Ruleset ruleset)
        {
            rulesetInfoTMP.text = ruleset.ToRulesetInfoDisplay();
        }

        void InitializeRulesetPresetButtons()
        {
            for (int i = 0; i < rulePresetBtnGrp.transform.childCount - 1; i++)
            {
                int btnIndex = i;
                rulePresetBtnGrp.transform.GetChild(i).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
                {
                    if (value)
                    {
                        var preset = (RulesetPreset) btnIndex;
                        CreateRulesetAndListen(Ruleset.CreateFromPreset(preset));
                        customMatchHandler.OverrideFromPreset(preset); /// Override selected button on the Custom screen
                        Debug.Log($"Set current match ruleset to {preset}");
                    }
                });
            }

            /// Custom
            rulePresetBtnGrp.transform.GetChild(4).GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener((value) =>
            {
                if (value)
                {
                    firstFrameController.SwitchToFrame("Custom");
                    Debug.Log($"Set current match ruleset to {RulesetPreset.Custom}");
                }
            });
        }

        public Lobby CreateLobbyProperties()
        {
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
            createLobbyBtn.interactable = false;
            createCustomBtn.interactable = false;

            if (Game.Main.IsOnline) /// Network
            {
                var loginStatus = Game.EOS.GetEOSAuthInterface().GetLoginStatus(Game.EOS.GetLocalUserId());

                if (loginStatus == LoginStatus.NotLoggedIn)
                {
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
