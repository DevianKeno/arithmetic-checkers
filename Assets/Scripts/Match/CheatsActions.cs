using Damath.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Damath.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        #region Actions

        public void AddPiece()
        {
            // Debug.Log("add piece clicked");
            if (_selectedCell == null) return;

            if (!_selectedCell.HasPiece)
            {
                var addPieceMenu = Game.UI.Create<AddPieceMenu>("add_piece_menu");
                addPieceMenu.SelectedCell = _selectedCell;
                addPieceMenu.Show();
            }
        }

        public void RemovePiece()
        {
            // Debug.Log("Remove piece clicked");
            if (_selectedCell == null) return;

            if (_selectedCell.HasPiece)
            {
                int col = _selectedCell.Col;
                int row = _selectedCell.Row;
                var command = $"piece remove {col} {row}";
                Game.Console.ExecuteCommand(command);
                
                Debug.Log($"[CHEATS]: Removed {_selectedCell.Piece}");
            }
        }

        public void Promote()
        {
            // Debug.Log("Promote clicked");
            if (_selectedCell == null) return;

            if (_selectedCell.HasPiece)
            {
                Game.Console.Log($"[CHEATS]: Promoted {_selectedCell.Piece}");
                Debug.Log($"[CHEATS]: Promoted {_selectedCell.Piece}");
                _selectedCell.Piece.Promote();
            }
        }

        public void Demote()
        {
            // Debug.Log("Demote clicked");
            if (_selectedCell == null) return;

            if (_selectedCell.HasPiece)
            {
                Game.Console.Log($"[CHEATS]: Demoted {_selectedCell.Piece}");
                Debug.Log($"[CHEATS]: Demoted {_selectedCell.Piece}");
                _selectedCell.Piece.Demote();
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