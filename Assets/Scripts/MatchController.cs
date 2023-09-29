using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using TMPro;

namespace Damath
{
    /// <summary>
    /// Controls the behavior of a match.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [field: Header("Information")]
        public Ruleset Rules { get; private set; }
        /// <summary>
        /// Whether if the match is currently active.
        /// </summary>
        public bool IsPlaying { get; set; }
        public bool IsMultiplayer { get; set; }

        [SerializeField] private TextMeshProUGUI rulesetText;
        [SerializeField] private GameObject playerPrefab;

        void Awake()
        {
            //
        }

        void Start()
        {
            if (Game.Main.HasRuleset)
            {
                Rules = Game.Main.Ruleset;
                rulesetText.text = Rules.Type.ToString();

                Game.Events.RulesetDistribute(Rules);
                Game.Events.MatchCreate(this);
                Game.Console.Log($"Created {Rules.Type} match");
            } else
            {
                Game.Console.Log($"No ruleset created. Set one first before starting.");
            }
        }

        void OnEnable() 
        {
            Game.Events.OnLobbyStart += BeginMatch;
        }

        void OnDisable() 
        {
            Game.Events.OnLobbyStart -= BeginMatch;
        }

        public void Init()
        {
            if (IsPlaying) return;
            IsPlaying = true;

            StartMatch();

            // if (Game.Network.IsOffline)
            // {
            //     StartSolo();
            // } else
            // {
            //     StartOnline();
            // }
        }

        public Player CreatePlayer(Side side)
        {
            Player newPlayer = Instantiate(playerPrefab).GetComponent<Player>();
            newPlayer.SetSide(side);
            newPlayer.SetPlaying(true);
            Game.Events.PlayerCreate(newPlayer);
            return newPlayer;
        }
        
        void StartOnline()
        {
            // Rules = Network.Network.Lobby.Ruleset;

            // This should be called before the match starts (pre-initialization)
            BeginMatch(true);
        }

        void StartMatch()
        {
            if (!IsMultiplayer)
            {
                switch (Rules["Players"])
                {
                    case RulesetPlayersType.Solo:
                        StartSolo();
                        break;
                    
                    case RulesetPlayersType.TwoPlayer:
                        // Start2P();
                        break;
                    
                    case RulesetPlayersType.VersusAI:    
                        // StartVersusAI(AI);       
                        break;
                }
            }

            // Multiplayer matches
            // if (IsClient)
        }

        void StartSolo()
        {
            Player botPlayer = CreatePlayer(Side.Bot);
            Player topPlayer = CreatePlayer(Side.Top);
            
            botPlayer.IsControllable = true;
            topPlayer.IsControllable = true;
            
            BeginMatch(true);
        }

        public void BeginMatch(bool force = false)
        {
            Game.Events.MatchBegin();
            IsPlaying = true;
        }

        public void BeginMatch(Lobby lobby)
        {
            BeginMatch(true);
        }
    }
}