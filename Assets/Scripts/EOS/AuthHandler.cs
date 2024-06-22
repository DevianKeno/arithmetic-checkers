using UnityEngine;
using Damath.UI;

using Epic.OnlineServices.Auth;
using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;

namespace Damath.EOS
{
    public class AuthHandler : MonoBehaviour
    {
        [SerializeField] Button signInBtn;

        void Awake()
        {
            signInBtn.onClick.AddListener(OnSignInClick);
        }

        public void OnSignInClick()
        {
            /// Try persistent login first
            EOSManager.Instance.StartPersistentLogin(PersistentLoginCallback);
            signInBtn.interactable = false;
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
            }
        }

        void LoginWithLoginTypeAndTokenCallback(LoginCallbackInfo loginCallbackInfo)
        {
            if (loginCallbackInfo.ResultCode == Result.Success)
            {
                StartConnectLoginWithEpicAccount(loginCallbackInfo.LocalUserId);
            }
            else
            {
                print("Error logging in. [" + loginCallbackInfo.ResultCode + "]");
                signInBtn.interactable = true;
            }
        }

        void StartConnectLoginWithEpicAccount(EpicAccountId localUserId)
        {
            EOSManager.Instance.StartConnectLoginWithEpicAccount(localUserId, (Epic.OnlineServices.Connect.LoginCallbackInfo connectLoginCallbackInfo) =>
            {
                if (connectLoginCallbackInfo.ResultCode == Result.Success)
                {
                    print("Connect Login Successful. [" + connectLoginCallbackInfo.ResultCode + "]");
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
    }
}