using System;

using UnityEngine;
using UnityEngine.UI;

using Aritche.Players;

namespace Aritche.UI
{
    public class PauseMenu : Window
    {
        public Player Player;
        public Color BorderColor
        {
            get
            {
                return border.color;
            }
            set
            {
                border.color = value;
            }
        }

        [Header("UI Elements")]
        [SerializeField] Image border;
        [SerializeField] Button backBtn;
        [SerializeField] Button optionsBtn;
        [SerializeField] Button drawBtn;
        [SerializeField] Button forfeitBtn;

        void Awake()
        {
            backBtn.onClick.AddListener(OnBackClick);
            optionsBtn.onClick.AddListener(OnOptionsClick);
            drawBtn.onClick.AddListener(OnDrawClick);
            forfeitBtn.onClick.AddListener(OnForfeitClick);
        }

        void OnBackClick()
        {
            Game.Main.Match.Unpause();
            Destroy();
        }

        void OnOptionsClick()
        {
            throw new NotImplementedException();
        }

        void OnDrawClick()
        {
            throw new NotImplementedException();
        }

        void OnForfeitClick()
        {
            Destroy();
            Game.Console.Log($"{Player.Side} Player forfeited.");
            Game.Main.Match.ConcludeMatch();
        }
    }
}