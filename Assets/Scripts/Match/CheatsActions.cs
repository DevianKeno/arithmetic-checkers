using Damath.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace Damath.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        AddPieceMenu addPieceMenu;

        #region Actions
    
        void AddPiece()
        {
            // Debug.Log("add piece clicked");
            if (_selectedCell == null) return;

            if (!_selectedCell.HasPiece)
            {
                if (addPieceMenu == null)
                {
                    addPieceMenu = Game.UI.Create<AddPieceMenu>("add_piece_menu");
                }
                addPieceMenu.SelectedCell = _selectedCell;
                addPieceMenu.Show();
            }
        }

        void SetOperation()
        {
            if (_selectedCell == null) return;

            Operation op;
            choiceWindow.Clear();
            choiceWindow.AddChoice("Addition", Game.UI.GetIcon("operator_add"))
                .AddCallback(() =>
                {
                    op = Operation.Addition;
                    InjectOperation();
                });
            choiceWindow.AddChoice("Subtraction", Game.UI.GetIcon("operator_subtract"))
                .AddCallback(() =>
                {
                    op = Operation.Subtraction;
                    InjectOperation();
                });
            choiceWindow.AddChoice("Multiplication", Game.UI.GetIcon("operator_multiply"))
                .AddCallback(() =>
                {
                    op = Operation.Multiplication;
                    InjectOperation();
                });
            choiceWindow.AddChoice("Division", Game.UI.GetIcon("operator_division"))
                .AddCallback(() =>
                {
                    op = Operation.Division;
                    InjectOperation();
                });
            
            void InjectOperation()
            {
                _selectedCell.SetOperation(op);
            }
        }

        void GrabPiece()
        {

        }

        void RemovePiece()
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

        void Promote()
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

        void Demote()
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

        void RemoveAll()
        {
            match.Board.RemoveAllPieces();
            Game.Console.Log($"[CHEATS]: Removed all pieces!");
            Debug.Log($"[CHEATS]: Removed all pieces!");
        }
        
        #endregion
    }
}