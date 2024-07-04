using System;

using UnityEngine;
using UnityEngine.EventSystems;

using Aritche.UI;
using Aritche.Players;

namespace Aritche.Systems
{
    public partial class Cheats : MonoBehaviour
    {
        bool _isEnabled;
        public bool EnableDebug = true;

        [SerializeField] Player player;
        Cell _selectedCell => player.SelectedCell;

        ChoiceWindow choiceWindow;
        MatchController match;

        internal void Initialize(MatchController match)
        {
            _isEnabled = true;
            this.match = match;
            choiceWindow = Game.UI.Create<ChoiceWindow>("choice_window");
            choiceWindow.Hide();

            Game.Events.OnPlayerCreate += OnPlayerCreate;
        }

        void OnPlayerCreate(Player player)
        {
            /// Capture player events
            player.OnClick += OnPlayerClick;
            player.OnCellDeselect += OnCellDeselect;
        }

        void OnCellDeselect(object sender, EventArgs e)
        {
            HideWindow();
        }

        public void OnPlayerClick(object sender, PointerEventData.InputButton button)
        {
            player = sender as Player;
            
            if (button == PointerEventData.InputButton.Left)
            {
                // HideWindow();
            }
            else if (button == PointerEventData.InputButton.Right)
            {
                if (player.Hit.collider == null) return;

                if (player.Hit.collider.CompareTag("Cell"))
                {
                    player.Deselect();
                    DisplayPieceActions();
                    return;
                }

                if (player.Hit.collider.CompareTag("Background"))
                {
                    DisplayBoardActions();
                    return;
                }
            }
        }

        void HideWindow()
        {
            choiceWindow.Hide();
        }

        void DisplayPieceActions()
        {
            if (!_isEnabled) return;
            
            if (_selectedCell == null) return;
            
            choiceWindow.Position = Input.mousePosition;
            choiceWindow.Clear();

            if (!_selectedCell.HasPiece)
            {
                choiceWindow.AddChoice("Add Piece", Game.UI.GetIcon("add_piece"))
                    .AddCallback(AddPiece);
                choiceWindow.AddChoice("Set Operation", Game.UI.GetIcon("symbols"))
                    .AddCallback(SetOperation);
            } else
            {
                choiceWindow.AddChoice("Grab Piece", Game.UI.GetIcon("hand"))
                    .AddCallback(GrabPiece);
                choiceWindow.AddChoice("Remove Piece", Game.UI.GetIcon("remove"))
                    .AddCallback(RemovePiece);
                choiceWindow.AddChoice("Capture Piece", Game.UI.GetIcon("restart"))
                    .AddCallback(CapturePiece)
                    .SetEnabled(false);

                if (!_selectedCell.Piece.IsKing)
                {
                    choiceWindow.AddChoice("Promote", Game.UI.GetIcon("promote"))
                        .AddCallback(Promote);
                } else
                {
                    choiceWindow.AddChoice("Demote", Game.UI.GetIcon("demote"))
                        .AddCallback(Demote);
                }
            }
            choiceWindow.Show();
        }

        public void DisplayBoardActions()
        {
            if (!_isEnabled) return;
            
            choiceWindow.Position = Input.mousePosition;
            choiceWindow.Clear();

            choiceWindow.AddChoice("Change Turns", Game.UI.GetIcon("toggle_false"))
                .AddCallback(ChangeTurns);

            choiceWindow.AddDivider();

            choiceWindow.AddChoice("Remove All Pieces", Game.UI.GetIcon("toggle_false"))
                .AddCallback(RemoveAll);
                
            choiceWindow.Show();
        }

        public void DisplayGlobalTimerActions()
        {
            if (!_isEnabled) return;

            choiceWindow.Position = Input.mousePosition;
            choiceWindow.Clear();
            
            if (match.TimerManager.GlobalTimer.IsRunning)
            {
                choiceWindow.AddChoice("Pause Time", Game.UI.GetIcon("timer_pause"))
                    .AddCallback(() =>
                    {
                        match.TimerManager.GlobalTimer.Pause();
                    });
            }
            else
            {
                choiceWindow.AddChoice("Resume Time", Game.UI.GetIcon("timer_play"))
                    .AddCallback(() =>
                    {
                        match.TimerManager.GlobalTimer.Resume();
                    });
            }

            choiceWindow.AddChoice("Set Time", Game.UI.GetIcon("timer"))
                .AddCallback(() =>
                {
                    throw new NotImplementedException();
                });

            choiceWindow.AddChoice("Conclude Match", Game.UI.GetIcon("toggle_false"))
                .AddCallback(() =>
                {
                    /// Add confirmation(?)
                    match.ConcludeMatch();
                });

            choiceWindow.Show();
        }
    }
}