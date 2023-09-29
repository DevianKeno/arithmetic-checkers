using System.Collections.Generic;

namespace Damath
{
    public enum Side {Bot, Top, Spectator}

    public class Constants
    {
        public const int MaximumColumns = 8;
        public const int MaximumRows = 8;
        public const float PieceScale = 2.75f;
        public const float CellSize = 2.5f;
        public const float CellOffset = 1.5f;
        public const float ZLocation = -3f;
        public const float ZLocationWindow = -3f;
        public const float ZLocationTooltip = -4f;
        public const float PieceZLocation = -2f;
        public const float CellZLocation = -3f;

        // Board size
        public float[] BoardDimensions = {0.03023016f, 0.03809f, 0.3809f}; 
        
    }
}