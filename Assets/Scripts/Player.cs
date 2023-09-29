using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace Damath
{
    /// <summary>
    /// This is used for network transport.
    /// </summary>
    public struct PlayerData
    {
        // public Image image;
        public string Name;
    }

    /// <summary>
    /// An actor can represent either a player or just a spectator.
    /// </summary>
    public class Actor : NetworkBehaviour
    {
        public string Name = "Actor";
        public RaycastHit2D Hit;
    }

    public class Spectator : Actor
    {
        public bool IsModerator = false;
    }

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
        public bool IsPlaying = false;
        public bool IsModerator = false;
        public bool IsControllable = false;
        /// <summary>
        /// Whether if the player is an AI.
        /// </summary>
        public bool IsAI = false;
        public Cell SelectedCell = null;
        /// <summary>
        /// The player's currently selected piece.
        /// </summary>
        public Piece SelectedPiece = null;
        public Piece HeldPiece = null;
        /// <summary>
        /// The piece this player had moved previously.
        /// </summary>
        public Piece MovedPiece = null;
        /// <summary>
        /// Whether if the player is currently holding down the mouse button.
        /// </summary>
        public bool IsHolding = false;
        /// <summary>
        /// The player's turn count.
        /// </summary>
        public int TurnNumber = 0;
        /// <summary>
        /// Whether if it's this player's turn or not.
        /// </summary>
        public bool IsTurn;

        [SerializeField] private float MouseHoldTime = 0f;

        void Start()
        {                
            Init();
        }

        void Update()
        {
            DetectRaycast();
        }

        void OnEnable()
        {
            Game.Events.OnTurnChanged += OnTurnChanged;
            Game.Events.OnLobbyStart += InitOnline;
            Game.Events.OnPieceDone += Deselect;
        }

        void OnDisable()
        {
            // Game.Events.OnPieceMove -= IMadeAMove;
            Game.Events.OnTurnChanged -= OnTurnChanged;
            Game.Events.OnLobbyStart -= InitOnline;
            Game.Events.OnPieceDone -= Deselect;
        }

        public void InitOnline(Lobby lobby)
        {
            // Game.Events.OnPieceMove += IMadeAMove;
        }

        public void Init()
        {
            // Name = Game.Main.Nickname;
            name = $"{Game.Main.Nickname} (Player)";
            Score = new();
            Game.Events.PlayerCreate(this);
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
                IsControllable = true;
                IsTurn = true;
            } else
            {
                IsControllable = false;
                IsTurn = false;
            }
        }

        #endregion

        void DetectRaycast()
        {
            // Left click
            if (Input.GetMouseButtonDown(0))
            {
                // Check if there's a piece currently selected and if the player is not holding the mouse button
                // This is to avoid selecting the moved piece again after selecting a cell via left click
                if (SelectedPiece != null && !IsHolding) return;

                CastRay();
                Game.Events.PlayerLeftClick(this);    
            }
            
            // Right click
            if (Input.GetMouseButtonDown(1))
            {
                CastRay();
                Game.Events.PlayerRightClick(this);    
            }

            // Left click and hold
            if (Input.GetMouseButton(0))
            {
                if (!Game.Settings.AllowPieceDragging) return;

                MouseHoldTime += 1 * Time.deltaTime;

                if (MouseHoldTime >= Game.Settings.PieceGrabDelay)
                {
                    MouseHoldTime = Game.Settings.PieceGrabDelay; // Inhibit value to max hold time
                    IsHolding = true;

                    // If there's a piece already held, move it along the cursor
                    if (HeldPiece != null)
                    {
                        HeldPiece.ToCursor();
                    } else // "hold" the selected piece first 
                    {
                        HeldPiece = SelectedPiece;
                    }
                }
            }

            // Left click release
            if (Input.GetMouseButtonUp(0))
            {
                MouseHoldTime = 0f;
                CastRay();

                if (IsHolding)
                {
                    IsHolding = false;
                    Game.Events.PlayerRelease(this);
                }
            }

            // Right click release
            if (Input.GetMouseButtonUp(1))
            {
                // CastRay();
            }
        }
        
        /// <summary>
        /// Perform a raycast, then cache any hit objects (single).
        /// </summary>
        void CastRay()
        {
            // If network is online (an instance of a client is active)
            // only allow control of owned player object
            if (IsClient && !IsOwner) return;
            if (!IsControllable) return;

            Hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  
            if (Hit.collider == null) return;

            if (Hit.collider.CompareTag("Cell"))
            {
                SelectedCell = Hit.collider.gameObject.GetComponent<Cell>();
                SelectCell(SelectedCell);

            } else if (Hit.collider.CompareTag("Background"))
            {
                Debug.Log("Left clicked background");
                Game.Events.CellDeselect(SelectedCell);
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
            } else
            {
                SelectMovecell(SelectedCell);
            }
        }

        /// <summary>
        /// Select a piece object.
        /// </summary>
        public void SelectPiece(Piece piece)
        {
            if (!OwnsPiece(piece))
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
                Game.Audio.PlaySound("Select");
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

            if (SelectedCell.IsValidMove)
            {
                if (IsTurn) // it's a normal move
                {
                    Game.Events.PlayerSelectMovecell(this, SelectedCell);
                    Game.Audio.PlaySound("Move");
                    return;

                } else // it's a premove
                {
                    // Premove logic
                    return;
                }
            } else
            {
                Deselect();
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
        public bool OwnsPiece(Piece piece)
        {
            if (piece.Side == Side) return true;
            return false;
        }

        /// <summary>
        /// Release the piece this player is holding.
        /// </summary>
        public void ReleaseHeldPiece()
        {
            HeldPiece?.ResetPosition();       
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
    }
}