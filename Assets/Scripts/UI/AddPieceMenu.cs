using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

namespace Damath.UI
{
    public class AddPieceMenu : Window
    {
        public Cell SelectedCell;

        [SerializeField] ColorButton colorBtn;
        [SerializeField] TMP_InputField valueField;
        [SerializeField] Toggle toggle;
        [SerializeField] Button addBtn;

        void Start()
        {
            addBtn.onClick.AddListener(Hide);
            addBtn.onClick.AddListener(AddPiece);
        }

        void AddPiece()
        {
            int col = SelectedCell.Col;
            int row = SelectedCell.Row;
            var pieceColor = colorBtn.SelectedIndex switch
            {
                0 => "blue",
                1 => "orange",
                _ => throw new NotImplementedException(),
            };
            string value = valueField.text;
            bool isKing = toggle.Value;

            string command = $"piece add {col} {row} {value} {pieceColor} {isKing}";
            Game.Console.ExecuteCommand(command);
            SelectedCell = null;
        }
    }
}