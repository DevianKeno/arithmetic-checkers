using Unity.VisualScripting;
using UnityEngine;

namespace Damath.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        #region Actions

        public void AddPiece()
        {
            
        }

        public void RemovePiece()
        {
            if (selectedCell.Piece != null)
            {
                Debug.Log($"[CHEATS]: Removed {selectedCell.Piece}");
                selectedCell.Piece.Remove();
            }
        }

        public void Promote()
        {
            if (selectedCell.Piece != null)
            {
                Game.Console.Log($"[CHEATS]: Promoted {selectedCell.Piece}");
                Debug.Log($"[CHEATS]: Promoted {selectedCell.Piece}");
                selectedCell.Piece.Promote();
            }
        }

        public void Demote()
        {
            if (selectedCell.HasPiece)
            {
                Game.Console.Log($"[CHEATS]: Demoted {selectedCell.Piece}");
                Debug.Log($"[CHEATS]: Demoted {selectedCell.Piece}");
                selectedCell.Piece.Demote();
            }
        }

        public void RemoveAll()
        {
            // foreach (var c in Game.Main.Match.Board.cellMap)
            // {
            //     if (c.Value.piece != null) c.Value.piece.Remove();
            // }
        }

        public void PromoteAll()
        {
            // foreach (var c in Game.Main.Match.Board.cellMap)
            // {
            //     if (c.Value.piece != null) c.Value.piece.Promote();
            // }
        }

        public void DemoteAll()
        {
            // foreach (var c in Game.Main.Match.Board.cellMap)
            // {
            //     if (c.Value.piece != null) c.Value.piece.Demote();
            // }
        }
        
        #endregion
    }
}