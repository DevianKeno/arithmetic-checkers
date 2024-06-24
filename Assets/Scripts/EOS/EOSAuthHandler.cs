using System;

using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;

using PlayEveryWare.EpicOnlineServices;

using Damath.UI;

namespace Damath.EOS
{
    public class EOSAuthHandler : MonoBehaviour, IEOSOnAuthLogin, IEOSOnConnectLogin
    {
        [Header("UI Elements")]
        [SerializeField] UI.Button signInBtn;
        [SerializeField] UI.Button signOutBtn;
        [SerializeField] TextMeshProUGUI usernameTMP;
        [SerializeField] TextMeshProUGUI greetingTMP;
        [SerializeField] LoadingIconAnimation loadingIcon;
        
        void Awake()
        {
            signInBtn.onClick.AddListener(OnSignInClick);
            signOutBtn.onClick.AddListener(OnSignOutClick);
        }

        void OnEnable()
        {
            Game.EOS.AddAuthLoginListener(this);
            Game.EOS.AddConnectLoginListener(this);
        }

        void OnDisable()
        {
            EOSManager.Instance.RemoveAuthLoginListener(this);
            EOSManager.Instance.RemoveConnectLoginListener(this);

            Game.EOSManagers.UserInfoManager.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
        }

        void Start()
        {
            SetUIForLogin();
        }

        public void OnSignInClick()
        {
            SetUIForLoading();
            usernameTMP.text = "Signing in...";
            greetingTMP.gameObject.SetActive(false);
            /// Try persistent login first
            Game.EOS.StartPersistentLogin(PersistentLoginCallback);
        }

        public void OnSignOutClick()
        {
            SetUIForLoading();
            usernameTMP.text = "Logging out...";
            greetingTMP.gameObject.SetActive(false);

            var localEpicId = Game.EOS.GetLocalUserId();
            Game.EOS.StartLogout(localEpicId, OnLogoutComplete);
            Game.EOS.RemovePersistentToken();
        }
        
        void OnLocalUserInfoChanged(UserInfoData UserInfo)
        {
            if (UserInfo.UserId?.IsValid() == true)
            {
                greetingTMP.gameObject.SetActive(true);
                signInBtn.gameObject.SetActive(false);
                usernameTMP.text = UserInfo.DisplayName;
            }
            else
            {
                greetingTMP.gameObject.SetActive(false);
                usernameTMP.text = "Not logged in";
            }
        }

        void OnLogoutComplete(ref LogoutCallbackInfo data)
        {
            SetUIForLogin();
        }

        void SetUIForLogin()
        {
            signInBtn.gameObject.SetActive(true);

            signOutBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false); 
        }
        
        void SetUIForLoading()
        {
            loadingIcon.gameObject.SetActive(true);

            signInBtn.gameObject.SetActive(false);
            signOutBtn.gameObject.SetActive(false);
        }

        void SetUIForLogout()
        {
            signOutBtn.gameObject.SetActive(true);

            signInBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false); 
        }

        void OnSuccessfulLogin()
        {
            Game.EOSManagers.UserInfoManager.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
            Game.EOSManagers.UserInfoManager.AddNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);

            OnLocalUserInfoChanged(Game.EOSManagers.UserInfoManager.GetLocalUserInfo());
            
            SetUIForLogout();
            usernameTMP.text = "...";
            greetingTMP.gameObject.SetActive(true);
        }

        void PersistentLoginCallback(LoginCallbackInfo loginCallbackInfo)
        {
            /// Other result codes are unhandled
            if (loginCallbackInfo.ResultCode == Result.Success)
            {
                LoginWithEpicAccount(loginCallbackInfo.LocalUserId);
                return;
            } else
            {
                /// Persistent login failure
                /// Login with account portal
                Game.EOS.StartLoginWithLoginTypeAndToken(LoginCredentialType.AccountPortal,
                                                         null,
                                                         null,
                                                         LoginWithLoginTypeAndTokenCallback);
            }
        }

        void LoginWithEpicAccount(EpicAccountId localUserId)
        {
            Game.EOS.StartConnectLoginWithEpicAccount(localUserId, LoginWithEpicAccountCallback);
        }

        void LoginWithEpicAccountCallback(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                OnSuccessfulLogin();
            } else
            {
                print("Error logging in. [" + connectLoginCallbackInfo.ResultCode + "]");
                SetUIForLogin();
            }
        }

        void LoginWithLoginTypeAndTokenCallback(LoginCallbackInfo loginCallbackInfo)
        {
            if (loginCallbackInfo.ResultCode == Result.Success)
            {
                LoginWithEpicAccount(loginCallbackInfo.LocalUserId);
            }
            else
            {
                print("Error logging in. [" + loginCallbackInfo.ResultCode + "]");
                SetUIForLogin();
            }
        }
        
        public void OnAuthLogin(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
        {
            OnSuccessfulLogin();
        }

        public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
        {
            OnSuccessfulLogin();
        }
    }
}