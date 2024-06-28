using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

using Aritche.Systems;
using System;

namespace Aritche.UI
{
    public class PostmatchWindow : Window
    {
        public MatchController Match { get; set; }
        public string ConludeMessage
        {
            get
            {
                return subheadingTMP.text;
            }
            set
            {
                subheadingTMP.text = value;
            }
        }

        [Header("UI Elements")]
        [SerializeField] TextMeshProUGUI headingTMP;
        [SerializeField] TextMeshProUGUI subheadingTMP;
        [SerializeField] TextMeshProUGUI winnerTMP;
        [SerializeField] TextMeshProUGUI drawTMP;
        
        [Space(10)]
        [SerializeField] TextMeshProUGUI blueNameTMP;
        [SerializeField] GameObject blueAvatarContainer;

        [Space(10)]
        [SerializeField] TextMeshProUGUI orangeNameTMP;
        [SerializeField] GameObject orangeAvatarContainer;

        [Space(10)]
        [SerializeField] Button rematchBtn;
        [SerializeField] Button titleBtn;

        void Awake()
        {
            rematchBtn.onClick.AddListener(OnRematch);
            titleBtn.onClick.AddListener(OnTitle);
        }

        void OnRematch()
        {
            Destroy();
            Game.Main.LoadScene("Title", playTransition: true);
        }

        void OnTitle()
        {
            Destroy();
            Game.Main.LoadScene("Title", playTransition: true);
        }

        public override void OnShow()
        {
            var bluePlayer = Match.GetPlayerFromSide(Side.Blue);
            blueNameTMP.text = bluePlayer.Name;

            var orangePlayer = Match.GetPlayerFromSide(Side.Orange);
            orangeNameTMP.text = orangePlayer.Name;
        }

        public void SetWinningSide(Side side)
        {
            drawTMP.gameObject.SetActive(false);
            winnerTMP.gameObject.SetActive(true);

            if (side == Side.Blue)
            {
                winnerTMP.transform.SetParent(blueAvatarContainer.transform);
            }
            else if (side == Side.Orange)
            {
                winnerTMP.transform.SetParent(orangeAvatarContainer.transform);
            }
            
            (winnerTMP.transform as RectTransform).anchoredPosition = new (0f, 110f);
        }

        public void SetDraw()
        {
            winnerTMP.gameObject.SetActive(false);
            drawTMP.gameObject.SetActive(true);
        }
    }
}