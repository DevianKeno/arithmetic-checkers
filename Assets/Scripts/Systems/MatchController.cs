using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Damath.Players;

namespace Damath.Systems
{
    /// <summary>
    /// Controls the behavior of a match.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [field: Header("Information")]
        /// <summary>
        /// Whether if the match is currently active.
        /// </summary>
        public bool IsPlaying { get; set; }
        public bool IsMultiplayer { get; set; }
        [field: SerializeField] public Ruleset Ruleset { get; private set; }

        public Board Board => board;
        [SerializeField] Board board;
        [SerializeField] Scoreboard scoreboard;
        [SerializeField] TimerManager timerManager;
        [SerializeField] Cheats cheats;
        public Cheats Cheats => cheats;

        [SerializeField] TextMeshProUGUI rulesetText;
        

        [Header("Prefabs")]
        [SerializeField] GameObject boardPrefab;
        [SerializeField] GameObject playerPrefab;
        
        void Start()
        {
            Initialize();
        }

        internal void Initialize()
        {
            InitializeRuleset();
            InitializeBoard();
            InitializeScoreboard();
            InitializeTimers();
            InitializeEvents();

            Game.Main.SetMatch(this);
        }

        void InitializeRuleset()
        {
            /// Retrieve cached ruleset
            if (Game.Main.HasRuleset)
            {
                Ruleset = Game.Main.Ruleset;
                rulesetText.text = Ruleset.Type.ToString();

                Game.Events.RulesetDistribute(Ruleset);
                Game.Events.MatchCreate(this);
                Game.Console.Log($"Created {Ruleset.Type} match");
                
                if (Ruleset.GetValue<bool>("EnableCheats"))
                {
                    cheats.Initialize(this);
                    Game.Console.Log("Cheats enabled.");
                }
            } else
            {
                Game.Console.Log($"No ruleset created. Set one first before starting.");
            }            
        }

        void InitializeBoard()
        {
            board = Instantiate(boardPrefab).GetComponent<Board>();
            board.SetRuleset(Ruleset);
            board.Initialize();
        }

        void InitializeScoreboard()
        {
            scoreboard.SetRuleset(Ruleset);
            scoreboard.Initialize();
        }

        void InitializeTimers()
        {
            timerManager.SetRuleset(Ruleset);
            timerManager.Initialize();
        }

        void InitializeEvents()
        {
            board.OnPieceCapture += scoreboard.OnPieceCaptureCallback;
            
            board.OnTurnChange += timerManager.OnBoardTurnChangedCallback;
        }

        public Player CreatePlayer(Side side)
        {
            Player newPlayer = Instantiate(playerPrefab).GetComponent<Player>();
            newPlayer.SetSide(side);
            newPlayer.SetPlaying(true);
            Game.Events.PlayerCreate(newPlayer);
            return newPlayer;
        }
        
        internal void StartMatch()
        {
            if (!IsMultiplayer)
            {
                switch (Ruleset.GetValue<RulesetPlayersType>("PlayersType"))
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
            } else
            {

            }
            board.BeginMatch();
            timerManager.BeginMatch();
            IsPlaying = true;
        }

        void StartSolo()
        {
            Player bluePlayer = CreatePlayer(Side.Blue);
            Player orangePlayer = CreatePlayer(Side.Orange);
            
            bluePlayer.ToggleControls(true);
            orangePlayer.ToggleControls(true);
        }
    }
}