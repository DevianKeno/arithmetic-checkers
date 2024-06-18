using UnityEngine;
using System;
using Damath.UI;
using Damath.Players;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Text.RegularExpressions;

namespace Damath.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        public bool EnableDebug = true;
        Cell _selectedCell;
        ChoiceWindow choiceWindow;
        MatchController match;

        internal void Initialize(MatchController match)
        {
            this.match = match;
            choiceWindow = Game.UI.Create<ChoiceWindow>("choice_window");
            choiceWindow.Hide();

            Game.Events.OnPlayerCreate += (Player p) =>
            {
                p.OnClick += PlayerClickCallback;
            };
            Game.Events.OnDeselect += DeselectCallback;
        }

        void DeselectCallback()
        {
            choiceWindow.Hide();
        }

        public void PlayerClickCallback(object sender, PointerEventData.InputButton button)
        {
            var player = sender as Player;
            if (player == null) return;
            // if (!player.IsOwner) return;
            if (!player.IsModerator) return;

            if (button == PointerEventData.InputButton.Right)
            {
                _selectedCell = match.Board.GetCell(player.SelectedCell.Coordinates);
                player.Deselect();
                if (_selectedCell == null) return;
                
                choiceWindow.Position = Input.mousePosition;
                choiceWindow.ClearChoices();

                if (!_selectedCell.HasPiece)
                {
                    choiceWindow.AddChoice("Add Piece", Game.UI.icons[0])
                        .AddCallback(AddPiece);
                } else
                {
                    choiceWindow.AddChoice("Remove Piece", Game.UI.icons[1])
                        .AddCallback(RemovePiece);
                    choiceWindow.AddChoice("Capture Piece", Game.UI.icons[1])
                        .AddCallback(RemovePiece); /// Should be a capture action

                    if (!_selectedCell.Piece.IsKing)
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
        }

        void AddPieceMenu()
        {
            
        }
    }
}