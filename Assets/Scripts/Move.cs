using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public enum MoveType {All, Normal, Capture}

    public class Move
    {
        public Player Player;
        public Cell originCell, from;
        public Cell destinationCell, to;
        public Piece Piece { get; private set; }
        public Piece capturingPiece = null;
        public Piece capturedPiece = null;
        public bool HasCapture = false;
        public float Score = 0;
        public MoveType type;

        public Move(Piece moved, Cell origin, Cell destination)
        {

        }

        public Move(Cell origin, Cell destination)
        {
            Piece = origin.Piece;
            originCell = origin; from = originCell;
            destinationCell = destination; to = destinationCell;
            destinationCell.IsValidMove = true;
            capturingPiece = origin.Piece;
            Player = capturingPiece.Owner;
            type = MoveType.Normal;
        }
        
        public Move(Cell origin, Cell destination, Piece toCapture)
        {
            Piece = origin.Piece;
            originCell = origin; from = originCell;
            destinationCell = destination; to = destinationCell;
            destinationCell.IsValidMove = true;
            capturingPiece = origin.Piece;
            Player = capturingPiece.Owner;
            capturedPiece = toCapture;
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