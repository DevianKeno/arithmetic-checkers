using System;

using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;

using PlayEveryWare.EpicOnlineServices;

using Damath.UI;
using Epic.OnlineServices.Connect;

namespace Damath.EOS
{
    public class EOSAuthHandler : MonoBehaviour
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

        void Start()
        {
            SetUIForLogin();
        }

        public void OnSignInClick()
        {
            SetUIForLoading();
            usernameTMP.text = "Signing in...";
            greetingTMP.gameObject.SetActive(false);
            
            ConnectWithDiscord();
        }

        public void OnSignOutClick()
        {
            SetUIForLoading();
            usernameTMP.text = "Logging out...";
            greetingTMP.gameObject.SetActive(false);

            var options = new Epic.OnlineServices.Connect.LogoutOptions()
            {
                LocalUserId = Game.EOS.GetProductUserId()
            };
            Game.EOS.GetEOSConnectInterface().Logout(ref options, null, OnLogoutCompleted);
        }

        void OnLogoutCompleted(ref Epic.OnlineServices.Connect.LogoutCallbackInfo data)
        {
            if (data.ResultCode == Result.Success)
            {
                var options = new UnlinkAccountOptions()
                {
                    LocalUserId = Game.EOS.GetProductUserId()
                };
                Game.EOS.GetEOSConnectInterface().UnlinkAccount(ref options, null, OnUnlinkAccountCompleted);
                return;
            }

            Game.Console.Log($"Encountered an error upon logging out. [{data.ResultCode}]");
            Debug.LogWarning($"Encountered an error upon logging out. [{data.ResultCode}]");
            SetUIForLogout();
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
                SetUIForLogout();
            }
        }

        public void SetDisplayedAccount(ExternalAccountInfo info)
        {
            usernameTMP.text = info.DisplayName;
            greetingTMP.gameObject.SetActive(true);
        }
    }
}