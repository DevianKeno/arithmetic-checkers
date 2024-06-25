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
    public class EOSAuthHandler : MonoBehaviour, IEOSOnConnectLogin//, IEOSOnAuthLogin
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
            Game.EOS.AddConnectLoginListener(this);
            // Game.EOS.AddAuthLoginListener(this);
        }

        void OnDisable()
        {
            EOSManager.Instance.RemoveConnectLoginListener(this);
            // EOSManager.Instance.RemoveAuthLoginListener(this);

            Game.EOSManagers.UserInfo.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
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
            usernameTMP.text = "Not logged in";
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
            Game.EOSManagers.UserInfo.RemoveNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);
            Game.EOSManagers.UserInfo.AddNotifyLocalUserInfoChanged(OnLocalUserInfoChanged);

            OnLocalUserInfoChanged(Game.EOSManagers.UserInfo.GetLocalUserInfo());
            
            SetUIForLogout();
            usernameTMP.text = "...";
            greetingTMP.gameObject.SetActive(true);
        }

        void PersistentLoginCallback(LoginCallbackInfo loginCallbackInfo)
        {
            /// Other result codes are unhandled
            // if (loginCallbackInfo.ResultCode == Result.Success)
            // {
            //     LoginWithEpicAccount(loginCallbackInfo.LocalUserId);
            //     return;
            // } else /// Persistent login failure
            {
                // /// Login with account portal
                // Game.EOS.StartLoginWithLoginTypeAndToken(LoginCredentialType.AccountPortal,
                //                                          null,
                //                                          null,
                //                                          LoginWithLoginTypeAndTokenCallback);
                
                /// Login with external account (Discord)
                ConnectWithDiscord();
            }
        }

        void ConnectWithDiscord()
        {
            if (Game.EOSManagers.Discord == null)
            {
                Debug.LogError("Connect Login failed: DiscordManager unavailable");
                SetUIForLogin();
                return;
            }
            Game.EOSManagers.Discord.RequestOAuth2Token(OnDiscordAuthReceived);
        }

        void OnDiscordAuthReceived(string token)
        {
            if (token == null)
            {
                Debug.LogError("Connect Login failed: Unable to get Discord OAuth2 token");
                SetUIForLogin();
                return;
            }
            
            Game.EOS.StartConnectLoginWithOptions(ExternalCredentialType.DiscordAccessToken,
                token,
                onloginCallback: ConnectLoginTokenCallback);
        }

        void ConnectLoginTokenCallback(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                SetUIForLogout();
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                /// Ask user if they want to connect; sample assumes they do
                Game.EOS.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    print("Creating new connect user");
                    if (createUserCallbackInfo.ResultCode == Result.Success)
                    {
                        SetUIForLogout();
                    }
                    else
                    {
                        SetUIForLogin();
                    }
                });
            }
            else
            {
                SetUIForLogin();
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
        
        public void OnConnectLogin(Epic.OnlineServices.Connect.LoginCallbackInfo loginCallbackInfo)
        {
            OnSuccessfulLogin();
        }

        // public void OnAuthLogin(Epic.OnlineServices.Auth.LoginCallbackInfo loginCallbackInfo)
        // {
        //     OnSuccessfulLogin();
        // }
    }
}