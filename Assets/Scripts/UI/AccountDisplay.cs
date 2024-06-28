using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Aritche.UI;

using Epic.OnlineServices.Connect;
using Epic.OnlineServices.UserInfo;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Aritche
{
    public class AccountDisplay : MonoBehaviour, IEOSOnConnectLogin, IEOSOnAuthLogin
    {
        public const string NoUser = "Not logged in";
        public string Username
        {
            get
            {
                return usernameTMP.text;
            }
            set
            {
                usernameTMP.text = value;
            }
        }

        EOSUserInfoManager userInfoManager;

        [SerializeField] TextMeshProUGUI greetingTMP;
        [SerializeField] TextMeshProUGUI usernameTMP;
        [SerializeField] Button signInBtn;

        void OnEnable()
        {
            Username = NoUser;

            if (EOSManager.Instance.GetLocalUserId() != null)
            {
                OnLogin();
            }
        }

        void OnDisable()
        {
            EOSManager.Instance.RemoveConnectLoginListener(this);
            EOSManager.Instance.RemoveAuthLoginListener(this);

            userInfoManager?.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
            Username = string.Empty;
        }
        
        void OnLocalUserInfoChanged(UserInfoData UserInfo)
        {
            if (UserInfo.UserId?.IsValid() == true)
            {
                greetingTMP.gameObject.SetActive(true);
                signInBtn.gameObject.SetActive(false);
                Username = UserInfo.DisplayName;
            }
            else
            {
                greetingTMP.gameObject.SetActive(false);
                Username = NoUser;
            }
        }

        void OnLogin()
        {
            userInfoManager ??= EOSManager.Instance.GetOrCreateManager<EOSUserInfoManager>();
            userInfoManager.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
            userInfoManager.AddNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);

            var userInfo = userInfoManager.GetLocalUserInfo();
            OnLocalUserInfoChanged(userInfo);
        }

        public void OnAuthLogin(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
        {
            OnLogin();
        }

        public void OnConnectLogin(LoginCallbackInfo loginCallbackInfo)
        {
            OnLogin();
        }

    }
}