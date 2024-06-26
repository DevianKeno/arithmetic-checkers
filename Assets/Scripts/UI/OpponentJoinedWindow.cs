using UnityEngine;
using UnityEngine.UI;
using TMPro;

// using Damath.EOS.Lobbies;

namespace Damath.UI
{
    public class OpponentJoinedWindow : Window
    {
        Side loadingLocation;

        [Header("UI Elements")]
        [Header("Blue")]
        [SerializeField] Image blueAvatar;
        [SerializeField] TextMeshProUGUI blueDisplayName;
        [SerializeField] Image blueReadyIndicator;
        [SerializeField] LoadingIconAnimation blueLoadingIcon;

        [Header("Orange")]
        [SerializeField] Image orangeAvatar;
        [SerializeField] TextMeshProUGUI orangeDisplayName;
        [SerializeField] Image orangeReadyIndicator;
        [SerializeField] LoadingIconAnimation orangeLoadingIcon;

        [Space(10)]
        [SerializeField] Button readyBtn;

        void Start()
        {
            blueReadyIndicator.gameObject.SetActive(false);
            orangeReadyIndicator.gameObject.SetActive(false);
        }

        // public void SetPlayer(Side side, LobbyMember member)
        // {
        //     if (side == Side.Blue)
        //     {
        //         if (member != null)
        //         {
        //             blueDisplayName.text = member.DisplayName;
        //             blueLoadingIcon.gameObject.SetActive(false);
        //         } else
        //         {
        //             blueDisplayName.text = "Waiting...";
        //             blueLoadingIcon.gameObject.SetActive(true);
        //         }
        //     } else if (side == Side.Orange)
        //     {
        //         if (member != null)
        //         {
        //             orangeDisplayName.text = member.DisplayName;
        //             orangeLoadingIcon.gameObject.SetActive(false);
        //         } else
        //         {
        //             orangeDisplayName.text = "Waiting...";
        //             orangeLoadingIcon.gameObject.SetActive(true);
        //         }
        //     }
        // }
    }
}