using UnityEngine;
using Aritche.UI;
using Aritche.Players;
using System;
using Aritche.Systems;

namespace Aritche
{
    public class Selector : MonoBehaviour
    {
        public MatchController Match;
        public Color Color
        {
            get
            {
                return spriteRenderer.color;
            }
            set
            {
                spriteRenderer.color = value;
            }
        }

        RectTransform rect;
        SpriteRenderer spriteRenderer;
        
        void Awake()
        {
            rect = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Start()
        {
            Game.Events.OnPlayerCreate += OnPlayerCreate;
            Hide();
        }

        void OnDisable()
        {
            Game.Events.OnPlayerCreate -= OnPlayerCreate;
        }

        void OnPlayerCreate(Player player)
        {
            if (player == null) return;

            player.OnCellSelect += OnPlayerCellSelect;
            player.OnCellDeselect += OnPlayerCellDeselect;
        }

        void OnPlayerCellSelect(object sender, Cell cell)
        {
            SelectCell(cell);
        }

        void OnPlayerCellDeselect(object sender, EventArgs e)
        {
            Hide();
        }


        #region Public methods

        public void SelectCell(Cell cell)
        {
            Show();
            transform.position = cell.transform.position;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        #endregion
    }
}
