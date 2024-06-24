using UnityEngine;
using UnityEngine.UI;
using Damath.UI;

using Epic.OnlineServices.Auth;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;
using System;
using TMPro;

namespace Damath.EOS
{
    public class AuthHandler : MonoBehaviour
    {
        AccountDisplay accountDisplay;

        [SerializeField] UI.Button signInBtn;
        [SerializeField] UI.Button signOutBtn;
        [SerializeField] TextMeshProUGUI greetingTMP;
        [SerializeField] LoadingIconAnimation loadingIcon;

        void Awake()
        {
            accountDisplay = GetComponent<AccountDisplay>();

            signInBtn.onClick.AddListener(OnSignInClick);
            signOutBtn.onClick.AddListener(OnSignOutClick);
        }

        void Start()
        {
            SetUIForLogin();
        }

        public void OnSignInClick()
        {
            accountDisplay.Username = "Signing in...";
            /// Try persistent login first
            Game.EOS.StartPersistentLogin(PersistentLoginCallback);
            SetUIForLoading();
        }

        public void OnSignOutClick()
        {
            greetingTMP.gameObject.SetActive(false);
            accountDisplay.Username = "Logging out...";
            var localEpicId = Game.EOS.GetLocalUserId();
            Game.EOS.StartLogout(localEpicId, OnLogoutComplete);
            Game.EOS.RemovePersistentToken();
            SetUIForLoading();
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
            signInBtn.gameObject.SetActive(false);
            signOutBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(true); 
        }

        void SetUIForLogout()
        {
            signInBtn.gameObject.SetActive(false);
            signOutBtn.gameObject.SetActive(true);
            loadingIcon.gameObject.SetActive(false); 
        }

        void OnSuccessfulLogin()
        {
            greetingTMP.gameObject.SetActive(true);
            SetUIForLogout();
        }

        void PersistentLoginCallback(LoginCallbackInfo loginCallbackInfo)
        {
            if (loginCallbackInfo.ResultCode == Result.Success)
            {
                LoginWithEpicAccount(loginCallbackInfo.LocalUserId);
                return;
            } else
            {
                /// Login with account portal on persistent login failure
                EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginCredentialType.AccountPortal,
                                                                    null,
                                                                    null,
                                                                    LoginWithLoginTypeAndTokenCallback);
            }
        }

        void LoginWithEpicAccount(EpicAccountId localUserId)
        {
            EOSManager.Instance.StartConnectLoginWithEpicAccount(localUserId, LoginWithEpicAccountCallback);
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
    }
}