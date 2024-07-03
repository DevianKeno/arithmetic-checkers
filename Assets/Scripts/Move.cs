using System;
using Aritche.Players;

namespace Aritche
{
    public enum MoveType { All, Move, Capture }

    /// <summary>
    /// Represents a move made by a player.
    /// </summary>
    public class MovePerformInfo
    {
        /// <summary>
        /// The player that performed this move.
        /// </summary>
        public Player Player { get; set; }
        public MoveType MoveType { get; set; }
        public Cell Origin { get; set; }
        public Cell Destination { get; set; }
        /// <summary>
        /// The piece that moved. 
        /// </summary>
        public Piece CapturingPiece { get; set; }
        /// <summary>
        /// The piece that will be captured.
        /// Will be null if none.
        /// </summary>
        public Piece CapturedPiece { get; set; }
        public bool HasCapture { get; set; }
        /// <summary>
        /// The score that wield yield the player.
        /// </summary>
        public float Score { get; set; }

        public MovePerformInfo(Cell origin, Cell destination)
        {
            /// Set the capturing piece. The piece that this move belongs to
            CapturingPiece = origin.Piece;
            /// Set the player who performed this move
            Player = CapturingPiece.Owner;

            /// Assign cells
            Origin = origin;
            Destination = destination;

            MoveType = MoveType.Move;
        }
        
        public MovePerformInfo(Cell origin, Cell destination, Piece toCapture)
        {
            /// Set the capturing piece. The piece that this move belongs to
            CapturingPiece = origin.Piece;
            /// Set the player who performed this move
            Player = CapturingPiece.Owner;

            /// Assign cells
            Origin = origin;
            Destination = destination;

            CapturedPiece = toCapture;
            HasCapture = true;

            MoveType = MoveType.Capture;
        }

        public void Perform()
        {
            Destination.SetPiece(Origin.Piece);
            Origin.ClearPiece();
        }

        public bool IsPerformedBy(Player player)
        {
            return Player == player;
        }

        public void SetScoreValue(float value)
        {
            Score = value;
        }

        public void SetPlayer(Player player)
        {
            Player = player;
        }
    }
}