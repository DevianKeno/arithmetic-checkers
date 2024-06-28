using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TMPro;

using Epic.OnlineServices;

using Aritche.Rules;
using Aritche.Players;
using Aritche.UI;

namespace Aritche.Systems
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
        public bool IsPaused { get; private set; }
        public bool IsMultiplayer { get; set; }
        [field: SerializeField] public Ruleset Ruleset { get; private set; }
        public Player BluePlayer;
        public Player OrangePlayer;
        public List<Player> Spectators = new();

        [SerializeField] Board board;
        public Board Board => board;
        [SerializeField] Scoreboard scoreboard;
        public Scoreboard Scoreboard => scoreboard;
        [SerializeField] TimerManager timerManager;
        public TimerManager TimerManager => timerManager;
        [SerializeField] Cheats cheats;
        public Cheats Cheats => cheats;

        PauseMenu pauseMenu;

        [Header("UI Elements")]
        [SerializeField] TextMeshProUGUI rulesetText;
        [SerializeField] MatchPlayerDisplay bluePlayerDisplay;
        [SerializeField] MatchPlayerDisplay orangePlayerDisplay;
        
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
        
            if (Game.Main.IsOnline)
            {
                var loginStatus = Game.EOS.GetEOSAuthInterface().GetLoginStatus(Game.EOS.GetLocalUserId());

                if (loginStatus == LoginStatus.NotLoggedIn)
                {
                    SetupLocalMatch();
                }
                else if (loginStatus == LoginStatus.LoggedIn)
                {
                    SetupOnlineMatch();
                }
                else if (loginStatus == LoginStatus.UsingLocalProfile)
                {
                    Game.Console.Log($"Unhandled login status." + loginStatus);
                    Debug.LogError($"Unhandled login status." + loginStatus);
                }
            }
            else
            {
                SetupLocalMatch();
            }

            Game.Main.SetMatch(this);
        }

        void InitializeRuleset()
        {
            /// Retrieve cached ruleset
            if (Game.Main.HasRuleset)
            {
                Ruleset = Game.Main.Ruleset;
                rulesetText.text = Ruleset.Preset.ToString();

                Game.Events.RulesetDistribute(Ruleset);
                Game.Events.MatchCreate(this);
                Game.Console.Log($"Created {Ruleset.Preset} match");
                
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
            scoreboard.Match = this;
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
            board.OnPieceMove += OnPieceMoveCallback;
            board.OnPieceCapture += OnPieceCaptureCallback;
            board.OnTurnChange += OnTurnChangedCallback;
        }

        void SetupLocalMatch()
        {
            BluePlayer = CreatePlayer(Side.Blue);
            BluePlayer.SetPieceOwnership(board.BluePieces);
            BluePlayer.ToggleControls(true);

            OrangePlayer = CreatePlayer(Side.Orange);
            OrangePlayer.SetPieceOwnership(board.OrangePieces);
            OrangePlayer.ToggleControls(true);

            var prematchWindow = Game.UI.Create<PrematchWindow>("prematch_window");
            prematchWindow.OnStart += () =>
            {
                var blueName = prematchWindow.blueNameField.text;
                BluePlayer.Name = blueName;
                bluePlayerDisplay.DisplayName = blueName;
                bluePlayerDisplay.SetSomebody();

                var orangeName = prematchWindow.orangeNameField.text;
                BluePlayer.Name = orangeName;
                orangePlayerDisplay.DisplayName = orangeName;
                orangePlayerDisplay.SetSomebody();
            };
        }

        void SetupOnlineMatch()
        {
            throw new NotImplementedException();
        }


        #region Callback listeners

        void OnPieceMoveCallback(object sender, Move e)
        {
            
        }

        void OnPieceCaptureCallback(object sender, Move e)
        {
            var capturedPieceOwner = GetPlayerFromSide(e.CapturedPiece.Owner.Side);

            var result = capturedPieceOwner.Pieces.Find((Piece piece) =>
            {
                return piece == e.CapturedPiece;
            });
            if (result != null)
            {
                capturedPieceOwner.Pieces.Remove(result);
            }

            if (capturedPieceOwner.Pieces.Count <= 0)
            {
                BluePlayer.ToggleControls(false);
                OrangePlayer.ToggleControls(false);

                /// Handled not pretty
                Invoke(nameof(ConcludeMatch), 1f);
            }

            scoreboard.OnPieceCaptureCallback(sender, e);
        }

        void OnTurnChangedCallback(object sender, Side e)
        {
            timerManager.OnBoardTurnChangedCallback(sender, e);
        }

        #endregion


        #region Public methods

        public void ConcludeMatch()
        {
            var postmatchWindow = Game.UI.Create<PostmatchWindow>("postmatch_window");
            postmatchWindow.Match = this;
            var conclusion = scoreboard.GetConclusion();

            if (conclusion.MatchConclusion == MatchConclusion.Victory)
            {
                postmatchWindow.SetWinningSide(conclusion.Winner.Side);
            }
            else if (conclusion.MatchConclusion == MatchConclusion.Draw)
            {
                postmatchWindow.SetDraw();
            }
            postmatchWindow.Show();
        }

        public Player GetPlayerFromSide(Side side)
        {
            if (side == Side.Blue)
            {
                return BluePlayer;
            } else if (side == Side.Orange)
            {
                return OrangePlayer;
            }
            throw new ArgumentOutOfRangeException();
        }

        public void Pause(Player requestor)
        {
            if (IsPaused) /// unpause
            {
                IsPaused = false;
                pauseMenu.Destroy();
            }
            else /// pause
            {
                IsPaused = true;
                pauseMenu = Game.UI.Create<PauseMenu>("pause_menu");

                if (requestor.Side == Side.Blue)
                {
                    pauseMenu.BorderColor = Colors.RichCerulean;
                }
                else if (requestor.Side == Side.Orange)
                {
                    pauseMenu.BorderColor = Colors.PersimmonOrange;
                }

                pauseMenu.Show();
            }
        }

        public void Unpause()
        {
            if (IsPaused) /// unpause
            {
                IsPaused = false;
                pauseMenu.Destroy();
            }
        }

        #endregion


        Player CreatePlayer(Side side)
        {
            var go = Instantiate(playerPrefab);
            go.name = $"{side} Player";
            
            Player newPlayer = go.GetComponent<Player>();
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
            
        }
    }
}