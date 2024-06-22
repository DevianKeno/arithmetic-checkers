using System;
using UnityEngine;
using TMPro;

using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.UserInfo;

using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Damath.UI
{
    public class DevAuthWindow : Window
    {
        const LoginCredentialType connect = (LoginCredentialType) (-1);

        EpicAccountId loggedInAccountId;
        EOSUserInfoManager userInfoManager;
        
        [SerializeField] LoginCredentialType LoginType = LoginCredentialType.Developer;

        [SerializeField] GameObject login;
        [SerializeField] GameObject loading;
        [SerializeField] AccountDisplay accountDisplay;

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

            if (LoginType == LoginCredentialType.Developer)
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

                EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginType,
                                                                    usernameAsString,
                                                                    passwordAsString,
                                                                    StartLoginWithLoginTypeAndTokenCallback);
            } 
            else if (LoginType == LoginCredentialType.AccountPortal)
            {
                EOSManager.Instance.StartLoginWithLoginTypeAndToken(LoginType, usernameAsString, passwordAsString, StartLoginWithLoginTypeAndTokenCallback);
            }
            else if (LoginType == LoginCredentialType.PersistentAuth)
            {
                EOSManager.Instance.StartPersistentLogin(StartLoginWithLoginTypeAndTokenCallback);
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
                    // ask user if they want to connect; sample assumes they do
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
    }
}