using System;

using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;

using PlayEveryWare.EpicOnlineServices;

using Aritche.UI;
using Epic.OnlineServices.Connect;
using Unity.VisualScripting;

namespace Aritche.EOS
{
    public class EOSAuthHandler : MonoBehaviour
    {
        DevAuthWindow devAuthWindow;

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

        void Start()
        {
            SetUIForLogin();
        }

        #region Login flow
        public void OnSignInClick()
        {
            devAuthWindow = Game.UI.Create<DevAuthWindow>("dev_auth_window");
            devAuthWindow.OnSignInBtnClick += OnStartLogin;
        }

        void OnStartLogin()
        {
            devAuthWindow.OnSignInBtnClick -= OnStartLogin;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("Internet not reachable.");
                return;
            }

            SetAccountDisplayForLoading();
            usernameTMP.text = "Signing in...";
            greetingTMP.gameObject.SetActive(false);

            var authType = devAuthWindow.GetLoginType();
            if (authType == LoginCredentialType.AccountPortal)
            {
                Game.EOS.StartLoginWithLoginTypeAndToken(LoginCredentialType.AccountPortal,
                                                        null,
                                                        null,
                                                        OnAuthLoginCallback);
            }
            else if (authType == LoginCredentialType.Developer)
            {
                var usernameAsString = devAuthWindow.GetUsername();
                var passwordAsString = devAuthWindow.GetPassword();

                Game.EOS.StartLoginWithLoginTypeAndToken(LoginCredentialType.Developer,
                                                        usernameAsString,
                                                        passwordAsString,
                                                        OnAuthLoginCallback);
            }
            else if (authType == LoginCredentialType.ExternalAuth)
            {
                ConnectWithDiscord();
            }
            else
            {
                Game.Console.Log("Unhandled login type." + authType.ToString());
                Debug.LogError("Unhandled login type." + authType.ToString());
                devAuthWindow.SetUIForLogin();
                SetUIForLogin();
            }
        }

        void OnAuthLoginCallback(Epic.OnlineServices.Auth.LoginCallbackInfo info)
        {
            if (info.ResultCode == Result.Success)
            {
                Game.EOS.StartConnectLoginWithEpicAccount(info.LocalUserId, OnConnectLoginCallback);
            }
            else
            {
                Game.Console.Log($"Encountered an error logging in. [{info.ResultCode}]");
                Debug.LogError($"Encountered an error logging in. [{info.ResultCode}]");
                devAuthWindow.SetUIForLogin();
                SetUIForLogin();
            }
        }

        void OnConnectLoginCallback(Epic.OnlineServices.Connect.LoginCallbackInfo info)
        {
            if (info.ResultCode == Result.Success)
            {
                devAuthWindow.Destroy();
                SetAccountDisplayForLogout();

                var options = new QueryProductUserIdMappingsOptions()
                {
                    LocalUserId = info.LocalUserId,
                    ProductUserIds = new ProductUserId[]
                    {
                        info.LocalUserId
                    }
                };
                Game.EOS.GetEOSConnectInterface().QueryProductUserIdMappings(ref options, null, OnQueryUserInfoCallback);
            }
            else
            {
                Game.Console.Log($"Encountered an error logging in. [{info.ResultCode}]");
                Debug.LogError($"Encountered an error logging in. [{info.ResultCode}]");
                devAuthWindow.SetUIForLogin();
                SetUIForLogin();
            }
        }

        void OnQueryUserInfoCallback(ref QueryProductUserIdMappingsCallbackInfo info)
        {
            if (info.ResultCode == Result.Success)
            {
                var options = new CopyProductUserInfoOptions()
                {
                    TargetUserId = info.LocalUserId
                };
                var result = Game.EOS.GetEOSConnectInterface().CopyProductUserInfo(ref options, out ExternalAccountInfo? outUserInfo);

                if (result == Result.Success && outUserInfo.HasValue)
                {
                    var userInfo = outUserInfo.Value;
                    SetDisplayedAccount(userInfo);
                    return;
                }
                info.ResultCode = result;
            }

            Game.Console.Log($"Failed to retrieve user information. [{info.ResultCode}]");
            Debug.LogError($"Failed to retrieve user information. [{info.ResultCode}]");
        }
        #endregion


        #region Logout flow
        public void OnSignOutClick()
        {
            SetAccountDisplayForLoading();
            usernameTMP.text = "Logging out...";
            greetingTMP.gameObject.SetActive(false);

            var options = new Epic.OnlineServices.Connect.LogoutOptions()
            {
                LocalUserId = Game.EOS.GetProductUserId()
            };
            Game.EOS.GetEOSConnectInterface().Logout(ref options, null, OnLogoutCompleted);
        }

        void OnLogoutCompleted(ref Epic.OnlineServices.Connect.LogoutCallbackInfo info)
        {
            if (info.ResultCode == Result.Success)
            {
                var options = new UnlinkAccountOptions()
                {
                    LocalUserId = Game.EOS.GetProductUserId()
                };
                Game.EOS.GetEOSConnectInterface().UnlinkAccount(ref options, null, OnUnlinkAccountCompleted);
                return;
            }

            Game.Console.Log($"Encountered an error upon logging out. [{info.ResultCode}]");
            Debug.LogWarning($"Encountered an error upon logging out. [{info.ResultCode}]");
            SetAccountDisplayForLogout();
        }

        void OnUnlinkAccountCompleted(ref UnlinkAccountCallbackInfo data)
        {
            if (data.ResultCode == Result.Success)
            {
                SetUIForLogin();
                return;
            }
            
            Game.Console.Log($"Unlink error. [{data.ResultCode}]");
            Debug.LogWarning($"Unlink error. [{data.ResultCode}]");
            SetUIForLogin();
        }

        #endregion


        void SetUIForLogin()
        {
            usernameTMP.text = "Not logged in";
            signInBtn.gameObject.SetActive(true);

            signOutBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false); 
        }
        
        void SetAccountDisplayForLoading()
        {
            loadingIcon.gameObject.SetActive(true);

            signInBtn.gameObject.SetActive(false);
            signOutBtn.gameObject.SetActive(false);
        }

        void SetAccountDisplayForLogout()
        {
            signOutBtn.gameObject.SetActive(true);

            signInBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false); 
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
            Game.EOS.StartConnectLoginWithOptions(ExternalCredentialType.DiscordAccessToken, token, onloginCallback: ConnectLoginTokenCallback);
        }

        void ConnectLoginTokenCallback(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                OnSuccessfulLogin(connectLoginCallbackInfo.LocalUserId);
                return;
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                /// Create new Connect user
                Game.EOS.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    if (createUserCallbackInfo.ResultCode == Result.Success)
                    {
                        OnSuccessfulLogin(createUserCallbackInfo.LocalUserId);
                        return;
                    }
                });
            }
            
            Game.Console.Log($"Encountered an error upon logging in. [{connectLoginCallbackInfo.ResultCode}]");
            Debug.LogWarning($"Encountered an error upon logging in. [{connectLoginCallbackInfo.ResultCode}]");
            /// Reset to login UI
            SetUIForLogin();
        }
        
        void OnSuccessfulLogin(ProductUserId productUserId)
        {
            var options = new QueryProductUserIdMappingsOptions()
            {
                LocalUserId = productUserId,
                ProductUserIds = new[] { productUserId }
            };

            Game.EOS.GetEOSConnectInterface().QueryProductUserIdMappings(ref options, null, OnQueryProductUserIdMappingsComplete);
        }

        void OnQueryProductUserIdMappingsComplete(ref QueryProductUserIdMappingsCallbackInfo data)
        {
            if (data.ResultCode == Result.Success)
            {
                var options = new CopyProductUserInfoOptions()
                {
                    TargetUserId = data.LocalUserId,
                };

                var result = Game.EOS.GetEOSConnectInterface().CopyProductUserInfo(ref options, out ExternalAccountInfo? outExternalAccountInfo);
                
                if (result == Result.Success && outExternalAccountInfo.HasValue)
                {
                    SetDisplayedAccount(outExternalAccountInfo.Value);
                }
                SetAccountDisplayForLogout();
            }
        }

        public void SetDisplayedAccount(ExternalAccountInfo info)
        {
            usernameTMP.text = info.DisplayName;
            greetingTMP.gameObject.SetActive(true);
        }

        public void SetDisplayedAccount(UserInfoData info)
        {
            usernameTMP.text = info.DisplayName;
            greetingTMP.gameObject.SetActive(true);
        }
    }
}