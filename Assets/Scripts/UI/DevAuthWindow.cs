using System;
using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;

using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;
using PlayEveryWare.EpicOnlineServices.Samples.Discord;

namespace Damath.UI
{
    public class DevAuthWindow : Window
    {
        [SerializeField] EpicAccountId loggedInAccountId;
        EOSUserInfoManager userInfoManager;
        
        // const ExternalCredentialType invalidConnectType = (ExternalCredentialType) (-1);
        // [SerializeField] ExternalCredentialType connectType = invalidConnectType;
        
        [SerializeField] Dropdown loginTypeDropdown;
        [SerializeField] GameObject login;
        [SerializeField] GameObject loading;

        [SerializeField] TMP_InputField usernameField;
        [SerializeField] TMP_InputField passwordField;
        [SerializeField] Button loginBtn;

        void Start()
        {
            Game.UI.Dim(0.25f);
            loginBtn.onClick.AddListener(StartLogin);
            OnClose += () =>
            {
                Game.UI.Dim(0.25f);
            };
        }

        public void StartLogin()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Debug.LogError("Internet not reachable.");
                return;
            }

            login.SetActive(false);
            loading.SetActive(true);

            string usernameAsString = usernameField.text.Trim();
            string passwordAsString = passwordField.text.Trim();

            // if (loginType == connect)
            // {
            //     AcquireTokenForConnectLogin(connectType);
            // }
            // else

            LoginCredentialType loginType = loginTypeDropdown.value switch
            {
                0 => LoginCredentialType.AccountPortal,
                1 => LoginCredentialType.PersistentAuth,
                2 => LoginCredentialType.Developer
            };

            if (loginType == LoginCredentialType.Developer)
            {
                if (usernameAsString.Length <= 0)
                {
                    print("Username is missing.");
                    return;
                }
                if (passwordAsString.Length <= 0)
                {
                    print("Password is missing.");
                    return;
                }

                EOSManager.Instance.StartLoginWithLoginTypeAndToken(loginType,
                                                                    usernameAsString,
                                                                    passwordAsString,
                                                                    StartLoginWithLoginTypeAndTokenCallback);
            }
            else if (loginType == LoginCredentialType.AccountPortal)
            {
                EOSManager.Instance.StartLoginWithLoginTypeAndToken(loginType,
                                                                    usernameAsString,
                                                                    passwordAsString,
                                                                    StartLoginWithLoginTypeAndTokenCallback);
            }
            else if (loginType == LoginCredentialType.PersistentAuth)
            {
                EOSManager.Instance.StartPersistentLogin(StartLoginWithLoginTypeAndTokenCallback);
            }
            else if (loginType == LoginCredentialType.ExternalAuth)
            {
                throw new NotImplementedException();
            }
        }
        
        void StartLoginWithLoginTypeAndTokenCallback(LoginCallbackInfo loginCallbackInfo)
        {
            if (loginCallbackInfo.ResultCode == Result.AuthMFARequired)
            {
                /// Collect MFA
                /// Do something to give the MFA to the SDK
                print("MFA Authentication not supported in sample. [" + loginCallbackInfo.ResultCode + "]");
            }
            else if (loginCallbackInfo.ResultCode == Result.AuthPinGrantCode)
            {
                ///TODO(mendsley): Handle pin-grant in a more reasonable way
                Debug.LogError("------------PIN GRANT------------");
                Debug.LogError("External account is not connected to an Epic Account. Use link below");
                Debug.LogError($"URL: {loginCallbackInfo.PinGrantInfo?.VerificationURI}");
                Debug.LogError($"CODE: {loginCallbackInfo.PinGrantInfo?.UserCode}");
                Debug.LogError("---------------------------------");
            }
            else if (loginCallbackInfo.ResultCode == Result.Success)
            {
                StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId);
            }
            else if (loginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                print("Trying Auth link with external account: " + loginCallbackInfo.ContinuanceToken);
                EOSManager.Instance.AuthLinkExternalAccountWithContinuanceToken(loginCallbackInfo.ContinuanceToken, 
#if UNITY_SWITCH
                                                                                LinkAccountFlags.NintendoNsaId,
#else
                                                                                LinkAccountFlags.NoFlags,
#endif
                                                                                (LinkAccountCallbackInfo linkAccountCallbackInfo) =>
                {
                    if (linkAccountCallbackInfo.ResultCode == Result.Success)
                    {
                        StartConnectLoginWithEpicAccount(linkAccountCallbackInfo.LocalUserId);
                    }
                    else
                    {
                        print("Error Doing AuthLink with continuance token in. [" + linkAccountCallbackInfo.ResultCode + "]");
                    }
                });
            }

            else
            {
                print("Error logging in. [" + loginCallbackInfo.ResultCode + "]");
            }

            /// Re-enable the login button and associated UI on any error
            if (loginCallbackInfo.ResultCode != Result.Success)
            {
                // ConfigureUIForLogin();
            }
        }
        void StartConnectLoginWithEpicAccount(EpicAccountId localUserId)
        {
            EOSManager.Instance.StartConnectLoginWithEpicAccount(localUserId, (Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo) =>
            {
                if (connectLoginCallbackInfo.ResultCode == Result.Success)
                {
                    print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                    Destroy();
                }
                else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
                {
                    /// Ask user if they want to connect; sample assumes they do
                    EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                    {
                        print("Creating new connect user");
                        EOSManager.Instance.StartConnectLoginWithEpicAccount(localUserId, (Epic.OnlineServices.Connect.LoginCallbackInfo retryConnectLoginCallbackInfo) =>
                        {
                            if (retryConnectLoginCallbackInfo.ResultCode == Result.Success)
                            {
                                // ConfigureUIForLogout();
                            }
                            else
                            {
                                // For any other error, re-enable the login procedure
                                // ConfigureUIForLogin();
                            }
                        });
                    });
                }
            });
        }

        void AcquireTokenForConnectLogin(ExternalCredentialType externalType)
        {
            switch (externalType)
            {
                // case ExternalCredentialType.SteamSessionTicket:
                //     ConnectSteamSessionTicket();
                //     break;

                // case ExternalCredentialType.SteamAppTicket:
                //     ConnectSteamAppTicket();
                //     break;

                // case ExternalCredentialType.DeviceidAccessToken:
                //     ConnectDeviceId();
                //     break;

                // case ExternalCredentialType.AppleIdToken:
                //     ConnectAppleId();
                //     break;

                case ExternalCredentialType.DiscordAccessToken:
                    ConnectDiscord();
                    break;

                // case ExternalCredentialType.OpenidAccessToken:
                //     ConnectOpenId();
                //     break;

                // case ExternalCredentialType.OculusUseridNonce:
                //     ConnectOculus();
                //     break;

                // default:
                //     if (externalType == invalidConnectType)
                //     {
                //         Debug.LogError($"Connect type not valid");
                //     }
                //     else
                //     {
                //         Debug.LogError($"Connect Login for {externalType} not implemented");
                //     }
                //     // loginButton.interactable = true;
                //     break;
            }
        }
        
        void ConnectDiscord()
        {
            if (DiscordManager.Instance == null)
            {
                Debug.LogError("Connect Login failed: DiscordManager unavailable");
                // ConfigureUIForLogin();
                return;
            }
            DiscordManager.Instance.RequestOAuth2Token(OnDiscordAuthReceived);
        }

        void OnDiscordAuthReceived(string token)
        {
            if (token == null)
            {
                Debug.LogError("Connect Login failed: Unable to get Discord OAuth2 token");
                // ConfigureUIForLogin();
            }
            else
            {
                EOSManager.Instance.StartConnectLoginWithOptions(ExternalCredentialType.DiscordAccessToken,
                    token,
                    onloginCallback: ConnectLoginTokenCallback);
            }
        }

        void ConnectLoginTokenCallback(Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo)
        {
            if (connectLoginCallbackInfo.ResultCode == Result.Success)
            {
                print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
                // ConfigureUIForLogout();
            }
            else if (connectLoginCallbackInfo.ResultCode == Result.InvalidUser)
            {
                // ask user if they want to connect; sample assumes they do
                EOSManager.Instance.CreateConnectUserWithContinuanceToken(connectLoginCallbackInfo.ContinuanceToken, (Epic.OnlineServices.Connect.CreateUserCallbackInfo createUserCallbackInfo) =>
                {
                    print("Creating new connect user");
                    if (createUserCallbackInfo.ResultCode == Result.Success)
                    {
                        // ConfigureUIForLogout();
                    }
                    else
                    {
                        // ConfigureUIForLogin();
                    }
                });
            }
            else
            {
                // ConfigureUIForLogin();
            }
        }
    }
}