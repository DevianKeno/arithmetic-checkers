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
        bool _isEnabled;
        public bool EnableDebug = true;
        Player _player;
        Cell _selectedCell;
        ChoiceWindow choiceWindow;
        MatchController match;

        internal void Initialize(MatchController match)
        {
            _isEnabled = true;
            this.match = match;
            choiceWindow = Game.UI.Create<ChoiceWindow>("choice_window");
            choiceWindow.Hide();

            // Game.Events.OnPlayerCreate += (Player p) =>
            // {
            //     p.OnClick += PlayerClickCallback;
            // };
            // Game.Events.OnDeselect += DeselectCallback;
            Game.Events.OnCellClick += CellClickCallback;
        }

        void CellClickCallback(Cell cell, PointerEventData e)
        {
            if (e.button == PointerEventData.InputButton.Left)
            {
                choiceWindow.Hide();
            }
            else if (e.button == PointerEventData.InputButton.Right)
            {
                _selectedCell = cell;
                DisplayPieceActions();
            }
        }

        public void PlayerClickCallback(object sender, PointerEventData.InputButton button)
        {
            _player = sender as Player;
            if (_player == null) return;
            // if (!player.IsOwner) return;
            if (!_player.IsModerator) return;


            if (button == PointerEventData.InputButton.Right)
            {
                if (_player.Hit.collider.CompareTag("Cell"))
                {
                    _selectedCell = match.Board.GetCell(_player.SelectedCell.Coordinates);
                    _player.Deselect();
                    DisplayPieceActions();
                    return;
                }

                if (_player.Hit.collider.CompareTag("Background"))
                {
                    DisplayBoardActions();
                    return;
                }
            }
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

            _player.Deselect();
            
            choiceWindow.Position = Input.mousePosition;
            choiceWindow.Clear();

            // choiceWindow.AddDivider();

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
            } else
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