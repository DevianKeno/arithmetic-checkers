using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public enum MoveType {All, Normal, Capture}

    /// <summary>
    /// Represents a move made by a player.
    /// </summary>
    public class Move
    {
        /// <summary>
        /// The player that performed this move.
        /// </summary>
        public Player Player;
        public Cell Origin, From;
        public Cell Destination, To;
        /// <summary>
        /// The piece that this move belongs to. 
        /// </summary>
        public Piece Piece { get; private set; }
        public Piece CapturingPiece = null;
        public Piece CapturedPiece = null;
        public bool HasCapture = false;
        public float Score = 0;
        public MoveType type;

        public Move(Piece moved, Cell origin, Cell destination)
        {

        }

        public Move(Cell origin, Cell destination)
        {
            // Set the piece that this move belongs to
            Piece = origin.Piece;
            // Set the capturing piece. This member is essentially same as "Piece"
            CapturingPiece = origin.Piece;
            // Set the player who performed this move
            Player = Piece.Owner;

            // Assign contructor arguments to members
            Origin = origin; From = Origin;
            Destination = destination; To = Destination;

            // Mark this cell as a valid move
            Destination.IsValidMove = true;


            type = MoveType.Normal;
        }
        
        public Move(Cell origin, Cell destination, Piece toCapture)
        {
            Piece = origin.Piece;
            Origin = origin; From = Origin;
            Destination = destination; To = Destination;
            Destination.IsValidMove = true;
            CapturingPiece = origin.Piece;
            Player = CapturingPiece.Owner;
            CapturedPiece = toCapture;
            HasCapture = true;
            type = MoveType.Capture;
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