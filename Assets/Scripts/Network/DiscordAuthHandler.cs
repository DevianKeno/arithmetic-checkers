using System.Collections;

using UnityEngine;
using TMPro;

using Damath.UI;

namespace Damath.Network
{
    public class DiscordAuthHandler : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] Button signInBtn;
        [SerializeField] Button signOutBtn;
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
            SignInWithDiscord();
        }

        public void OnSignOutClick()
        {
            SetUIForLoading();
            usernameTMP.text = "Logging out...";
            greetingTMP.gameObject.SetActive(false);
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
            Debug.Log("Setting UI for logout.");
            signOutBtn.gameObject.SetActive(true);
            signInBtn.gameObject.SetActive(false);
            loadingIcon.gameObject.SetActive(false);
        }

        void SignInWithDiscord()
        {
            if (DiscordManager.Instance == null)
            {
                Debug.LogError("Connect Login failed: DiscordManager unavailable");
                SetUIForLogin();
                return;
            }
            DiscordManager.Instance.RequestOAuth2Token(OnDiscordAuthReceived);
        }

        void OnDiscordAuthReceived(string token)
        {
            if (token == null)
            {
                Debug.LogError("Connect Login failed: Unable to get Discord OAuth2 token");
                SetUIForLogin();
                return;
            }
            DiscordManager.Instance.FetchUserInfo(token, OnSuccessfulLogin);
        }

        void OnSuccessfulLogin(UserInfo userInfo)
        {
            Debug.Log("Successfully fetched user info");
            Debug.Log($"User info: {userInfo.username}#{userInfo.discriminator}");
            Debug.Log("Now logging in...");
            
            DiscordManager.Instance.SetLocalUser(userInfo);
            SetDisplayedAccount(userInfo);
            SetUIForLogout();
        }

        public void SetDisplayedAccount(UserInfo userInfo)
        {
            usernameTMP.text = userInfo.username;
            greetingTMP.gameObject.SetActive(true);
        }
    }
}