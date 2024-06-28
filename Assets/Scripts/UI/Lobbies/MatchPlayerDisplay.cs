using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.UserInfo;

namespace Aritche.UI
{
    public class MatchPlayerDisplay : MonoBehaviour
    {
        public string DisplayName
        {
            get
            {
                return displayNameTMP.text;
            }
            set
            {
                displayNameTMP.text = value;
            }
        }
        public Side Side;
        public const string NoPlayer = "Waiting...";

        [SerializeField] TextMeshProUGUI displayNameTMP;
        [SerializeField] Image avatarImage;
        [SerializeField] LoadingIconAnimation loadingIcon;

        void Start()
        {
            SetNobody();
        }

        public void SetNobody()
        {
            DisplayName = NoPlayer;
            avatarImage.sprite = null;
            loadingIcon.gameObject.SetActive(true);
        }

        public void SetSomebody()
        {
            loadingIcon.gameObject.SetActive(false);
        }

        public void SetPlayerDisplayed(UserInfoData userInfo, bool isLobbyOwner = false)
        {
            if (isLobbyOwner)
            {
                DisplayName = $"{userInfo.DisplayNameSanitized} (You)";
            }
            else
            {
                DisplayName = userInfo.DisplayNameSanitized;
            }
            avatarImage.sprite = null; /// TODO:
            loadingIcon.gameObject.SetActive(false);
        }
    }
}