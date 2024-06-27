using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Damath.UI;
using System;
using Damath.Network.Discord;
using Damath.Network.Itch;

namespace Damath.Network
{
    public class AuthHandler : MonoBehaviour
    {
        public bool EnableDiscordAuth;
        public bool EnableItchAuth;

        [Header("UI Elements")]
        [SerializeField] UI.Button signInDiscordBtn;
        [SerializeField] UI.Button signInItchBtn;
        [SerializeField] UI.Button signOutBtn;
        [SerializeField] TextMeshProUGUI usernameTMP;
        [SerializeField] Image avatarImage;
        [SerializeField] TextMeshProUGUI greetingTMP;
        [SerializeField] LoadingIconAnimation loadingIcon;
        
        [SerializeField] Sprite noAvatar;

        void Awake()
        {
            signInDiscordBtn.onClick.AddListener(StartLoginDiscord);
            signInItchBtn.onClick.AddListener(StartLoginItch);
            signOutBtn.onClick.AddListener(StartLogout);
        }

        void Start()
        {
            SetUIForLogin();
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public void StartLoginDiscord()
        {
            SetUIForLoading();
            usernameTMP.text = "Signing in...";
            greetingTMP.gameObject.SetActive(false);
            SignInWithDiscord();
        }
        
        public void StartLoginItch()
        {
            SetUIForLoading();
            usernameTMP.text = "Signing in...";
            greetingTMP.gameObject.SetActive(false);
            SignInWithItch();
        }

        public void StartLogout()
        {
            SetUIForLoading();
            usernameTMP.text = "Logging out...";
            greetingTMP.gameObject.SetActive(false);
            avatarImage.sprite = noAvatar;
            DiscordManager.Instance.Logout();
            SetUIForLogin();
        }

        void SetUIForLogin()
        {
            usernameTMP.text = "Not logged in";

            signInDiscordBtn.gameObject.SetActive(EnableDiscordAuth);
            signInItchBtn.gameObject.SetActive(EnableItchAuth);
            
            signOutBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false);
        }

        void SetUIForLoading()
        {
            loadingIcon.gameObject.SetActive(true);

            signInDiscordBtn.gameObject.SetActive(false);
            signInItchBtn.gameObject.SetActive(false);
            signOutBtn.gameObject.SetActive(false);
        }

        void SetUIForLogout()
        {
            signOutBtn.gameObject.SetActive(true);

            signInDiscordBtn.gameObject.SetActive(false);
            signInItchBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false);
        }

        #region Discord auth
        void SignInWithDiscord()
        {
            if (DiscordManager.Instance == null)
            {
                Game.Console.Log("Login failed: DiscordManager unavailable");
                Debug.LogError("Login failed: DiscordManager unavailable");
                SetUIForLogin();
                return;
            }

            DiscordManager.Instance.RequestOAuth2Token(OnDiscordAuthReceived);

            Game.Console.Log($"Signing in with Discord...");
            Debug.Log($"Signing in with Discord...");
        }

        void OnDiscordAuthReceived(CallbackInfo<string> info)
        {
            Game.Console.Log($"Auth received");
            Debug.Log($"Auth received");

            if (info.ResultCode == ResultCode.Success)
            {
                var token = info.Value;
                DiscordManager.Instance.FetchUserInfo(token, OnSuccessfulLogin);
            }            
            else if (info.ResultCode == ResultCode.Failed)
            {
                Game.Console.Log("Login failed. Unable to get Discord OAuth2 token");
                Debug.LogError("Login failed. Unable to get Discord OAuth2 token");
                SetUIForLogin();
                return;
            }
        }

        void OnSuccessfulLogin(CallbackInfo<UserInfo> info)
        {
            var userInfo = info.Value;

            Game.Console.Log("Successfully fetched user info");
            Game.Console.Log($"User info: {userInfo.username}#{userInfo.discriminator}");
            Game.Console.Log("Now logging in...");

            Debug.Log("Successfully fetched user info");
            Debug.Log($"User info: {userInfo.username}#{userInfo.discriminator}");
            Debug.Log("Now logging in...");
            
            DiscordManager.Instance.SetLocalUser(userInfo);
            SetDisplayedAccount(userInfo);
            SetUIForLogout();
            
            Game.Console.Log("Successfully logged in!");
            Debug.Log("Successfully logged in!");
        }

        public void SetDisplayedAccount(UserInfo userInfo)
        {
            usernameTMP.text = userInfo.username;
            DiscordManager.Instance.FetchUserAvatar(userInfo, OnFetchAvatarCompleted);
            greetingTMP.gameObject.SetActive(true);
        }

        void OnFetchAvatarCompleted(CallbackInfo<Sprite> info)
        {
            if (info.ResultCode == ResultCode.Success)
            {
                avatarImage.sprite = info.Value;
            }
            else
            {
                avatarImage.sprite = null;
                Game.Console.Log("Failed to fetch user avatar");
                Debug.LogError("Failed to fetch user avatar");
            }
        }
        #endregion

        #region Itch auth
        void SignInWithItch()
        {
            if (ItchManager.Instance == null)
            {
                Game.Console.Log("Login failed: ItchManager unavailable");
                Debug.LogError("Login failed: ItchManager unavailable");
                SetUIForLogin();
                return;
            }

            ItchManager.Instance.RequestOAuthToken(OnItchAuthReceived);

            Game.Console.Log($"Signing in with Itch...");
            Debug.Log($"Signing in with Itch...");
        }

        void OnItchAuthReceived(CallbackInfo<string> info)
        {
            if (info.ResultCode == ResultCode.Success)
            {
                var token = info.Value;
                // ItchManager.Instance.FetchUserInfo(token, OnSuccessfulLogin);
            }
            else if (info.ResultCode == ResultCode.Failed)
            {
                Game.Console.Log("Login failed: Unable to get Itch OAuth token");
                Debug.LogError("Login failed: Unable to get Itch OAuth token");
                SetUIForLogin();
                return;
            }
        }
        #endregion
    }
}