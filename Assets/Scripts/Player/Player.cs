using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Aritche.Players
{
    public class Player : Actor
    {
        [field: Header("Information")]
        public Side Side { get; private set; }
        /// <summary>
        /// The player's current alive pieces.
        /// </summary>
        public List<Piece> Pieces;
        /// <summary>
        /// The player's current captured pieces. Note that this should contain enemy pieces.
        /// </summary>
        public List<Piece> CapturedPieces;
        /// <summary>
        /// Represents the player's current score.
        /// </summary>
        public Score Score;
        /// <summary>
        /// The player's current score represent in float. (Subject to removal)
        /// </summary>
        public float fScore = 0f;
        /// <summary>
        /// Whether if the player is participating in the match.
        /// </summary>
        public bool IsPlaying { get; private set; }
        [field: SerializeField] public bool IsModerator { get; private set; }
        public bool AllowControls { get; set; }
        /// <summary>
        /// Whether if the player is an AI.
        /// </summary>
        public bool IsAI { get; private set; }
        
        public Cell SelectedCell { get; private set; }
        /// <summary>
        /// The player's currently selected piece.
        /// </summary>
        public Piece SelectedPiece { get; private set; }
        public Piece HeldPiece { get; private set; }
        /// <summary>
        /// The piece this player had moved previously.
        /// </summary>
        public Piece MovedPiece { get; private set; }
        /// <summary>
        /// Whether if the player is currently holding down the mouse button.
        /// </summary>
        public bool IsHoldingMouse { get; private set; }
        /// <summary>
        /// The player's turn count.
        /// </summary>
        public int TurnNumber = 0;
        /// <summary>
        /// Whether if it's this player's turn or not.
        /// </summary>
        public bool IsMyTurn { get; private set; }
        public bool IsPaused { get; private set; }

        [SerializeField] private float MouseHoldTime = 0f;

        #region Events
        public event EventHandler<PointerEventData.InputButton> OnClick;
        public event EventHandler<Cell> OnCellSelect;
        public event EventHandler<Cell> OnCellDeselect;
        public event EventHandler<Piece> OnPieceSelect;

        #endregion

        Dictionary<string, InputAction> inputs = new();

        void Start()
        {                
            Initialize();
        }

        void Update()
        {
            DetectRaycast();
        }

        void OnEnable()
        {
            Game.Events.OnTurnChanged += OnTurnChanged;
            // Game.Events.OnLobbyStart += InitOnline;
            Game.Events.OnPieceDone += Deselect;
        }

        void OnDisable()
        {
            // Game.Events.OnPieceMove -= IMadeAMove;
            Game.Events.OnTurnChanged -= OnTurnChanged;
            // Game.Events.OnLobbyStart -= InitOnline;
            Game.Events.OnPieceDone -= Deselect;
        }

        // public void InitOnline(Lobby lobby)
        // {
        //     // Game.Events.OnPieceMove += IMadeAMove;
        // }

        internal void Initialize()
        {
            // Name = Game.Main.Nickname;
            name = $"{Game.Main.Nickname} (Player)";
            Score = new();
            InitializeInputs();
            Game.Events.PlayerCreate(this);
        }

        void InitializeInputs()
        {
            var map = Game.Main.GetActionMap("Player");
            foreach (var action in map.actions)
            {
                inputs[action.name] = action;
                action.Enable();            
            }

            // inputs["Left Click"].started += HandleLeftClickStarted;
            inputs["Left Click"].performed += HandleLeftClick;
            inputs["Left Click"].canceled += HandleLeftClickRelease;
            // inputs["Middle Click"].started += HandleMiddleClick;
            // inputs["Middle Click"].performed += HandleMiddleClick;
            // inputs["Middle Click"].canceled += HandleMiddleClick;
            // inputs["Right Click"].started += HandleRightClickStarted;
            inputs["Right Click"].performed += HandleRightClick;
            inputs["Right Click"].canceled += HandleRightClickRelease;
            inputs["Pause"].performed += HandlePause;
        }

        void HandleLeftClick(InputAction.CallbackContext c)
        {
            if (IsPaused) return;
            /// Check if there's a piece currently selected and if the player is not holding the mouse button
            /// This is to avoid selecting the moved piece again after selecting a cell via left click
            if (SelectedPiece != null && !IsHoldingMouse) return;

            CastRay();
            Game.Events.PlayerLeftClick(this);
            OnClick?.Invoke(this, PointerEventData.InputButton.Left);
        }

        // void HandleMiddleClick()
        // {
        //     OnClick?.Invoke(this, PointerEventData.InputButton.Middle);
        // }

        void HandleRightClick(InputAction.CallbackContext c)
        {
            if (IsPaused) return;

            CastRay();
            Game.Events.PlayerRightClick(this);   
            OnClick?.Invoke(this, PointerEventData.InputButton.Right);
        }

        void HandleLeftClickRelease(InputAction.CallbackContext c)
        {
            if (IsPaused) return;
            
        }

        // void HandleMiddleClickRelease(InputAction.CallbackContext c)
        // {
            
        // }

        void HandleRightClickRelease(InputAction.CallbackContext c)
        {
            if (IsPaused) return;
            
        }

        void HandlePause(InputAction.CallbackContext c)
        {
            /// When on online matches, these should be handled differently
            /// As they will now be "requesting" pauses or just pause on their own
            if (IsMyTurn)
            {
                Deselect();
                IsPaused = !IsPaused;
                if (Game.Main.HasMatch)
                {
                    Game.Main.Match.Pause(this);
                }
            }
        }

        #region Public

        public string SetName(string value)
        {
            name = $"Player {value}";
            Name = value;
            return value;
        }

        public void SetPlaying(bool value)
        {
            IsPlaying = value;
        }

        public void SetSide(Side value)
        {
            Side = value;
        }

        public void SetScore(float value)
        {
            fScore = value;
        }

        /// <summary>
        /// Whenever the board calls for a turn change, listen if it's this player's turn.
        /// </summary>
        public void OnTurnChanged(Side side)
        {
            if (Side == side)
            {
                AllowControls = true;
                IsMyTurn = true;
            } else
            {
                AllowControls = false;
                IsMyTurn = false;
            }
        }

        public void SetPieceOwnership(List<Piece> pieces)
        {
            foreach (Piece piece in pieces)
            {
                piece.SetOwner(this);
                Pieces.Add(piece);
            }
        }

        #endregion

        void DetectRaycast()
        {
            if (IsPaused) return;
            
            /// Left click and hold
            if (Mouse.current.leftButton.isPressed)
            {
                if (!Game.Main.Settings.AllowPieceDragging) return;

                MouseHoldTime += 1 * Time.deltaTime;
                if (MouseHoldTime >= Game.Main.Settings.PieceGrabDelay)
                {
                    MouseHoldTime = Game.Main.Settings.PieceGrabDelay; /// Inhibit value to max hold time
                    IsHoldingMouse = true;

                    /// If there's a piece already held, move it along the cursor
                    if (HeldPiece != null)
                    {
                        HeldPiece.ToCursor();
                    } else /// "hold" the selected piece first 
                    {
                        HeldPiece = SelectedPiece;
                    }
                }
            }

            /// Left click release
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                MouseHoldTime = 0f;
                CastRay();

                if (IsHoldingMouse)
                {
                    IsHoldingMouse = false;
                    Game.Events.PlayerRelease(this);
                }
            }
        }
        
        /// <summary>
        /// Perform a raycast, then cache any hit objects (single).
        /// </summary>
        void CastRay()
        {
            /// If network is online (an instance of a client is active)
            /// only allow control of owned player object
            // if (IsClient && !IsOwner) return;
            if (!AllowControls) return;

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  
            if (Hit.collider == null) return;

            if (Hit.collider.CompareTag("Cell"))
            {
                if (Hit.collider.gameObject.TryGetComponent(out Cell cell))
                {
                    SelectCell(cell);
                }

            } else if (Hit.collider.CompareTag("Background"))
            {
                Debug.Log("Clicked background");
                Game.Events.CellDeselect(SelectedCell);
            }
        }

        /// <summary>
        /// Select a cell objcet.
        /// </summary>
        public void SelectCell(Cell cell)
        {
            SelectedCell = cell;
            OnCellSelect?.Invoke(this, cell);

            if (SelectedCell.HasPiece)
            {
                SelectPiece(SelectedCell.Piece);
            
            } else /// empty cell
            {
                if (SelectedCell.IsValidMove)
                {
                    SelectMovecell(SelectedCell);
                } else
                {
                    Deselect();
                }
            }
        }

        /// <summary>
        /// Select a piece object.
        /// </summary>
        public void SelectPiece(Piece piece)
        {
            if (piece == null) return;

            if (!IsMyPiece(piece))
            {
                Deselect();
                return;
            }

            if (SelectedPiece != null)
            {
                if (SelectedPiece == piece)
                {
                    ReleaseHeldPiece();
                    return;
                } else
                {
                    Deselect();
                    return;
                }
            }

            if (MovedPiece != null)
            {
                if (piece != MovedPiece)
                {
                    Deselect();
                    return;
                }
            } else
            {
                SelectedPiece = piece;
                Game.Events.PlayerSelectPiece(this, SelectedPiece);
                OnPieceSelect?.Invoke(this, SelectedPiece);

                Game.Audio.PlaySound("select");
                return;
            }
        }

        /// <summary>
        /// A movecell is a basically a cell, but is marked as a valid move.
        /// </summary>
        /// <param name="cell"></param>
        public void SelectMovecell(Cell cell = null)
        {
            if (SelectedPiece == null) return;
            if (!OwnsSelectedPiece(SelectedPiece)) return;

            if (cell != null) SelectedCell = cell;

            if (IsMyTurn) /// it's a normal move
            {
                Game.Events.PlayerSelectMovecell(this, SelectedCell);
                Game.Audio.PlaySound("move");
                return;

            } else /// it's a premove
            {
                /// Premove logic
                return;
            }
        }

        /// <summary>
        /// Check if this player owns the piece it has currently selected.
        /// </summary>
        public bool OwnsSelectedPiece(Piece piece)
        {
            if (SelectedPiece == null) return false;
            if (SelectedPiece.Side == Side) return true;
            return false;
        }

        /// <summary>
        /// Check if this player owns this piece.
        /// </summary>
        public bool IsMyPiece(Piece piece)
        {
            if (piece.Side == Side)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Release the piece this player is holding.
        /// </summary>
        public void ReleaseHeldPiece()
        {
            HeldPiece?.ResetPositionToCell();       
            HeldPiece = null;
        }

        public void Deselect()
        {
            SelectedPiece = null;
            ReleaseHeldPiece();

            Game.Events.Deselect();
            Game.Events.PlayerDeselect(this);
        }

        public void Deselect(Piece piece)
        {
            Deselect();
        }

        public void ToggleControls(bool value)
        {
            AllowControls = value;
        }
    }
}