using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.UserInfo;

namespace Damath.UI
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
        public const string NoPlayer = "Waiting for opponent...";

        [SerializeField] TextMeshProUGUI displayNameTMP;
        [SerializeField] Image avatarImage;
        [SerializeField] LoadingIconAnimation loadingIcon;

        void Start()
        {
            if (Side == Side.Blue)
            {
                var lobby = Game.EOSManagers.LobbyManager.GetCurrentLobby();
                if (!lobby.IsValid())
                {
                    Debug.LogError("Lobby error??");
                    return;
                }
                
                var localUserId = Game.EOS.GetProductUserId();
                var localEpicId = Game.EOS.GetLocalUserId();

                if (lobby.IsOwner(localUserId))
                {
                    var options = new QueryUserInfoOptions()
                    {
                        LocalUserId = localEpicId,
                        TargetUserId = localEpicId
                    };

                    Game.EOS.GetEOSUserInfoInterface().QueryUserInfo(ref options, null, OnQueryUserInfoComplete);
                }
            } else
            {
                SetNobody();
            }
        }

        void OnQueryUserInfoComplete(ref QueryUserInfoCallbackInfo callbackInfo)
        {
            if (callbackInfo.ResultCode == Result.Success)
            {
                var userInfoOptions = new CopyUserInfoOptions()
                {
                    LocalUserId = callbackInfo.LocalUserId,
                    TargetUserId = callbackInfo.TargetUserId
                };

                var result = Game.EOS.GetEOSUserInfoInterface().CopyUserInfo(ref userInfoOptions, out UserInfoData? userInfo);
                if (result == Result.Success && userInfo.HasValue)
                {
                    SetPlayerDisplayed(userInfo.Value, true);
                }
            }
        }

        public void SetNobody()
        {
            DisplayName = NoPlayer;
            avatarImage.sprite = null;
            loadingIcon.gameObject.SetActive(true);
        }

        public void SetPlayerDisplayed(UserInfoData userInfo, bool isLobbyOwner = false)
        {
            if (isLobbyOwner)
            {
                DisplayName = $"{userInfo.DisplayName} (You)";
            }
            else
            {
                DisplayName = userInfo.DisplayName;
            }
            avatarImage.sprite = null; /// TODO:
            loadingIcon.gameObject.SetActive(false);
        }
    }
}