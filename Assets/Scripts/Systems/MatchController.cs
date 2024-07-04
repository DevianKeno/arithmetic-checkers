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
        public int TurnNumber = -1;
        [SerializeField] Side turnOf;
        public Side TurnOf => turnOf;
        bool _changeTurnOnFinish = false;
        public bool _turnRequiresCapture = false;

        [SerializeField] Board board;
        public Board Board => board;
        [SerializeField] Scoreboard scoreboard;
        public Scoreboard Scoreboard => scoreboard;
        [SerializeField] TimerManager timerManager;
        public TimerManager TimerManager => timerManager;
        [SerializeField] Selector selector;
        [SerializeField] IndicatorHandler indicators;
        [SerializeField] Cheats cheats;
        public Cheats Cheats => cheats;

        PauseMenu pauseMenu;
        PrematchWindow prematchWindow;
        PostmatchWindow postmatchWindow;

        #region Match events

        public event EventHandler<Player> OnPlayerCreate;
        public event EventHandler<Side> OnTurnChanged;

        #endregion

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
            CaptureBoardEvents();

            OnTurnChanged += OnTurnChangedCallback;
        }

        void CaptureBoardEvents()
        {
            board.OnMovePerform += OnMovePerform;
            board.OnUpdateValidMoves += OnUpdateValidMoves;
            board.OnUpdateCaptureMoves += OnUpdateCaptureables;
        }

        void SetupLocalMatch()
        {
            prematchWindow = Game.UI.Create<PrematchWindow>("prematch_window");
            prematchWindow.OnStart += InitializePlayers;
        }

        void InitializePlayers()
        {
            var blueName = prematchWindow.blueNameField.text;
            BluePlayer = InstantiatePlayerPrefab(new()
            {
                Name = blueName,
                Side = Side.Blue
            });
            BluePlayer.AssignPieceOwnership(board.BluePieces);

            bluePlayerDisplay.DisplayName = blueName;
            bluePlayerDisplay.SetSomebody();
            BluePlayer.ToggleControls(false);
            
            var orangeName = prematchWindow.orangeNameField.text;
            OrangePlayer = InstantiatePlayerPrefab(new()
            {
                Name = orangeName,
                Side = Side.Orange
            });
            OrangePlayer.AssignPieceOwnership(board.OrangePieces);

            orangePlayerDisplay.DisplayName = orangeName;
            orangePlayerDisplay.SetSomebody();
            OrangePlayer.ToggleControls(false);
        }

        void SetupOnlineMatch()
        {
            throw new NotImplementedException();
        }


        #region Callback listeners

        void CapturePlayerEvents(Player player)
        {
            player.OnCellSelect += OnPlayerCellSelect;
            player.OnCellDeselect += OnPlayerCellDeselect;
            player.OnPieceSelect += OnPlayerPieceSelect;
        }

        void OnMovePerform(object sender, MovePerformInfo moveInfo)
        {
            board.ClearValidMoves();
            selector.Hide();
            indicators.ClearMoveIndicators();
            indicators.ClearCaptureIndicators();

            _changeTurnOnFinish = true;

            if (moveInfo.MoveType == MoveType.Move)
            {
                HandleMove(moveInfo);
            }
            else if (moveInfo.MoveType == MoveType.Capture)
            {
                HandleCapture(moveInfo);
            }

            if (Ruleset.GetValue<bool>("AllowMandatoryCapture"))
            {
                if (board.CheckIfCaptureableMax(moveInfo.CapturingPiece, out var moves, setAsValid: true))
                {
                    _turnRequiresCapture = true;
                }
                else
                {
                    _turnRequiresCapture = false;
                }
            }

            moveInfo.Player.FinishTurn();
            if (_changeTurnOnFinish)
            {
                ChangeTurns();
            }
        }

        void HandleMove(MovePerformInfo moveInfo)
        {
            
        }

        void HandleCapture(MovePerformInfo moveInfo)
        {
            scoreboard.OnMovePerformed(moveInfo); /// Handle score
            CheckForLossCondition(moveInfo);
            
            if (Ruleset.GetValue<bool>("AllowChainCapture")) /// Check for chain captures
            {
                board.GetPieceMoves(moveInfo.CapturingPiece, MoveType.Capture);
                if (moveInfo.CapturingPiece.CanCapture)
                {
                    _changeTurnOnFinish = false;
                }
            }
        }

        void OnPlayerCellSelect(object sender, Cell cell)
        {
            var player = sender as Player;
            if (!IsTurnOf(player)) return;
            
            if (cell.HasPiece)
            {
                selector.SelectCell(cell);
            }
            else
            {
                selector.Hide();
            }
            
            board.SelectCell(cell, player);
        }

        void OnPlayerCellDeselect(object sender, EventArgs e)
        {
            var player = sender as Player;
            if (!IsTurnOf(player)) return;
            
            board.ClearValidMoves();
            selector.Hide();
            indicators.ClearMoveIndicators();
        }

        void OnPlayerPieceSelect(object sender, Piece piece)
        {
            if (_turnRequiresCapture && !piece.CanCapture)
            {
                indicators.HighlightCaptures();
                return;
            }

            board.SelectPiece(piece, _turnRequiresCapture);
        }

        void OnUpdateValidMoves(object sender, List<MovePerformInfo> moves)
        {
            indicators.ClearMoveIndicators();
            indicators.CreateMoveIndicators(
                moves: moves,
                shape: IndicatorShape.Circle,
                color: new(1, 0.92f, 0.016f, 1f)); /// yellow
        }

        void OnUpdateCaptureables(object sender, List<MovePerformInfo> moves)
        {
            indicators.CreateCaptureIndicators(
                moves: moves,
                shape: IndicatorShape.Box,
                color: Colors.Lime);
        }

        void OnTurnChangedCallback(object sender, Side e)
        {
            timerManager.OnBoardTurnChangedCallback(sender, e);
        }

        #endregion


        public void ResetTurns()
        {
            var first = Ruleset.GetValue<Side>("FirstTurn");
            ChangeTurnTo(first);
            GetPlayerFromSide(first).ToggleControls(true);
            TurnNumber = 0;
        }
        
        public void ChangeTurns()
        {
            if (turnOf == Side.Blue)
            {
                turnOf = Side.Orange;
                OrangePlayer.ToggleControls(true);
                BluePlayer.ToggleControls(false);
            }
            else if (turnOf == Side.Orange)
            {
                turnOf = Side.Blue;
                OrangePlayer.ToggleControls(false);
                BluePlayer.ToggleControls(true);
            }

            TurnNumber++;
            OnTurnChanged?.Invoke(this, turnOf);
        }

        public void ChangeTurnTo(Side side)
        {
            turnOf = side;
            if (turnOf == Side.Blue)
            {
                OrangePlayer.ToggleControls(false);
                BluePlayer.ToggleControls(true);
            }
            else if (turnOf == Side.Orange)
            {
                OrangePlayer.ToggleControls(true);
                BluePlayer.ToggleControls(false);
            }

            TurnNumber++;
            OnTurnChanged?.Invoke(this, turnOf);
        }


        #region Public methods

        public bool IsTurnOf(Player player)
        {
            return turnOf == player.Side;
        }

        public void CheckForLossCondition(MovePerformInfo move)
        {
            if (move.CapturedPiece.Owner.Pieces.Count <= 0)
            {
                /// No more pieces
                BluePlayer.ToggleControls(false);
                OrangePlayer.ToggleControls(false);
                /// Handled not pretty
                Invoke(nameof(ConcludeMatch), 1f);
            }
        }

        public MatchConclusion ConcludeMatch()
        {
            BluePlayer.AllowControls = false;
            OrangePlayer.AllowControls = false;
            timerManager.PauseAll();

            var postmatchWindow = Game.UI.Create<PostmatchWindow>("postmatch_window");
            postmatchWindow.Match = this;
            MatchConclusion matchConclusion;
            var scoreConclusion = scoreboard.GetConclusion();

            if (scoreConclusion.Winner != null)
            {
                matchConclusion = MatchConclusion.Victory;
                postmatchWindow.SetWinningSide(scoreConclusion.Winner.Side);
            }
            else
            {
                matchConclusion = MatchConclusion.Draw;
                postmatchWindow.SetDraw();
            }
            
            postmatchWindow.Show();
            return matchConclusion;
        }

        public void EndMatch()
        {
            // Destroy(board.gameObject);
            // Destroy(BluePlayer.gameObject);
            // Destroy(OrangePlayer.gameObject);
            // Ruleset = null;
            // IsPlaying = false;
            // IsPaused = false;
            
            Game.Main.LoadScene("Title", playTransition: true, 0f, () =>
            {
                
            });
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
                pauseMenu.Player = requestor;

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

        Player InstantiatePlayerPrefab(PlayerData data)
        {
            var go = Instantiate(playerPrefab);
            go.name = $"{data.Side} Player";
            
            Player newPlayer = go.GetComponent<Player>();
            newPlayer.SetData(data);
            newPlayer.SetPlaying(true);
            CapturePlayerEvents(newPlayer);
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
            ResetTurns();
            board.BeginMatch();
            timerManager.BeginMatch();
            IsPlaying = true;
        }

        void StartSolo()
        {
            
        }
    }
}