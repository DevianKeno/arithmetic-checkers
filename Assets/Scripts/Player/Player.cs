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
        [SerializeField] Side _side;
        public Side Side => _side;
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
        /// The piece this player had previously moved in this same turn.
        /// </summary>
        public Piece MovedPiece { get; private set; }
        /// <summary>
        /// Whether if the player is currently holding down the mouse button.
        /// </summary>
        public bool IsHoldingMouse { get; private set; }
        /// <summary>
        /// The player's turn count.
        /// </summary>
        public int TurnCount = 0;
        /// <summary>
        /// Whether if it's this player's turn or not.
        /// </summary>
        // public bool IsMyTurn { get; private set; }
        public bool IsPaused { get; private set; }

        [SerializeField] float mouseHoldTime = 0f;


        #region Events
        public event EventHandler<PointerEventData.InputButton> OnClick;
        public event EventHandler<Cell> OnCellSelect;
        public event EventHandler<Piece> OnPieceSelect;
        public event EventHandler OnCellDeselect;

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

        internal void Initialize()
        {
            name = $"{Game.Main.Nickname} (Player)";
            Score = new();
            InitializeInputs();
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
            
            CastRay();
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
            if (!Game.Main.HasMatch) return;
            if (!AllowControls) return;

            /// When on online matches, these should be handled differently
            /// As they will now be "requesting" pauses or just pause on their own
            Deselect();
            IsPaused = !IsPaused;
            Game.Main.Match.Pause(this);
        }

        #region Public

        public void RemoveFromPieces(Piece piece)
        {
            var capturedPiece = Pieces.Find((Piece p) =>
            {
                return p == piece;
            });
            if (capturedPiece != null)
            {
                capturedPiece.Remove();
            }
        }

        public void SetData(PlayerData data)
        {
            SetName(data.Name);
            SetSide(data.Side);
        }

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
            _side = value;
        }

        public void SetScore(float value)
        {
            fScore = value;
        }

        public void AssignPieceOwnership(List<Piece> pieces)
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

                mouseHoldTime += 1 * Time.deltaTime;
                if (mouseHoldTime >= Game.Main.Settings.PieceGrabDelay)
                {
                    mouseHoldTime = Game.Main.Settings.PieceGrabDelay; /// Inhibit value to max hold time
                    IsHoldingMouse = true;

                    /// If there's a piece already held, move it along the cursor
                    if (HeldPiece != null)
                    {
                        HeldPiece.ToCursor();
                    }
                    else /// "hold" the selected piece first 
                    {
                        HeldPiece = SelectedPiece;
                    }
                }
            }

            /// Left click release
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                mouseHoldTime = 0f;
                CastRay();

                if (IsHoldingMouse)
                {
                    IsHoldingMouse = false;
                    ReleaseHeldPiece();
                }
            }
        }
        
        /// <summary>
        /// Perform a raycast, then cache any hit objects (single).
        /// </summary>
        void CastRay()
        {
            if (!AllowControls) return;

            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  
            if (hit.collider == null) return;

            if (hit.collider.CompareTag("Cell"))
            {
                if (hit.collider.gameObject.TryGetComponent(out Cell cell))
                {
                    SelectCell(cell);
                }
            }
            else if (hit.collider.CompareTag("Background"))
            {
                Deselect();
            }
        }

        /// <summary>
        /// Select a cell objcet.
        /// </summary>
        public void SelectCell(Cell cell)
        {
            SelectedCell = cell;

            if (SelectedCell.HasPiece)
            {
                SelectPiece(SelectedCell.Piece);
            }
            else /// empty cell
            {
                if (cell.IsValidMove && SelectedPiece != null)
                {
                    MovedPiece = SelectedPiece;
                }
                else
                {
                    Deselect();
                }
            }

            OnCellSelect?.Invoke(this, SelectedCell);
        }

        /// <summary>
        /// Select a piece object.
        /// </summary>
        public void SelectPiece(Piece piece)
        {
            if (!IsMyPiece(piece))
            {
                Deselect();
                return;
            }

            /// 
            if (SelectedPiece != null)
            {
                if (SelectedPiece == piece)
                {
                    ReleaseHeldPiece();
                }
                else
                {
                    Deselect();
                }
                return;
            }

            if (MovedPiece != null)
            {
                if (piece != MovedPiece)
                {
                    Deselect();
                }
                return;
            }
            else
            {
                SelectedPiece = piece;
                Game.Audio.PlaySound("select");
                OnPieceSelect?.Invoke(this, SelectedPiece);
                return;
            }
        }
        
        public void FinishTurn()
        {
            SelectedCell = null;
            SelectedPiece = null;
            MovedPiece = null;
            Deselect();
        }

        /// <summary>
        /// Check if this player owns the piece it has currently selected.
        /// </summary>
        public bool OwnsSelectedPiece()
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
            if (piece == null) return false;
            return piece.Side == Side;
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
            if (SelectedCell != null)
            {
                OnCellDeselect?.Invoke(this, null);
            }
            SelectedPiece = null;
            ReleaseHeldPiece();
        }

        public void ToggleControls(bool value)
        {
            AllowControls = value;
        }
    }
}