using UnityEngine;
using Damath.UI;
using System;

namespace Damath.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        public bool EnableDebug = true;
        Cell selectedCell;
        ChoiceWindow choiceWindow;

        internal void Initialize()
        {
            choiceWindow = Game.UI.Create<ChoiceWindow>("choice_window");
            choiceWindow.Hide();

            Game.Events.OnPlayerRightClick += PlayerRightClickCallback;
            Game.Events.OnDeselect += DeselectCallback;
        }

        void DeselectCallback()
        {
            choiceWindow.Hide();
        }

        public void PlayerRightClickCallback(Player player)
        {
            Debug.Log("player right clicked haha");
            // if (!player.IsModerator) return;

            selectedCell = player.SelectedCell;
            if (selectedCell == null) return;
            
            choiceWindow.Hide();
            choiceWindow.Position = Input.mousePosition;
            choiceWindow.ClearChoices();

            if (!selectedCell.HasPiece)
            {
                choiceWindow.AddChoice("Add Piece", Game.UI.icons[0])
                    .AddCallback(AddPiece);
            } else
            {
                choiceWindow.AddChoice("Remove Piece", Game.UI.icons[1])
                    .AddCallback(RemovePiece);
                choiceWindow.AddChoice("Capture Piece", Game.UI.icons[1])
                    .AddCallback(RemovePiece); /// Should be a capture action

                if (!selectedCell.Piece.IsKing)
                {
                    choiceWindow.AddChoice("Promote", Game.UI.icons[2])
                        .AddCallback(Promote);
                } else
                {
                    choiceWindow.AddChoice("Demote", Game.UI.icons[3])
                        .AddCallback(Demote);
                }
            }
            choiceWindow.Show();
        }

        void AddPieceMenu()
        {
            
        }
    }
}