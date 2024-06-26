#if !(UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS)
#define DISABLEDISCORD
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace Damath.Network
{
    [DisallowMultipleComponent]
    public class DiscordManager : MonoBehaviour
    {
    #if !DISABLEDISCORD
        protected static DiscordManager s_instance;
        public static DiscordManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    return new GameObject("DiscordManager").AddComponent<DiscordManager>();
                }
                else
                {
                    return s_instance;
                }
            }
        }

        protected bool m_bInitialized = false;
        public static bool Initialized
        {
            get
            {
                return Instance.m_bInitialized;
            }
        }

    #if UNITY_2019_3_OR_NEWER
        // In case of disabled Domain Reload, reset static members before entering Play Mode.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void InitOnPlayMode()
        {
            s_instance = null;
        }
    #endif

        const long clientId = 1255192173842796544;
        const string clientSecret = "cizf2Kvnpy5fis0gEXrGvrkjIoPv8zYO";
        const int listenPort = 54545;
        
        UserInfo localUser;
        public UserInfo LocalUser => localUser;

        protected virtual void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);
            m_bInitialized = true;
        }

        protected virtual void OnEnable()
        {
            if (s_instance == null)
            {
                s_instance = this;
            }

            if (!m_bInitialized)
            {
                return;
            }
        }

        protected virtual void OnDestroy()
        {
            s_instance = null;
        }

        string cachedToken = null;
        DateTime tokenExpiration;

        event Action<string> onTokenReceived;
        event Action<UserInfo> onUserInfoFetched;

        public void SetLocalUser(UserInfo userInfo)
        {
            localUser = userInfo;
        }

        public void RequestOAuth2Token(Action<string> callback)
        {
            onTokenReceived += callback;
            if (cachedToken != null && DateTime.Now < tokenExpiration)
            {
                onTokenReceived?.Invoke(cachedToken);
                onTokenReceived = null;
                return;
            }
            else
            {
                cachedToken = null;
            }

            StartCoroutine(GetTokenCoroutine());
        }

        IEnumerator GetTokenCoroutine()
        {
            string redirectURL = $"http://127.0.0.1:{listenPort}/discord/";
            string url = $"https://discord.com/oauth2/authorize?client_id={clientId}&response_type=code&redirect_uri={Uri.EscapeDataString(redirectURL)}&scope=identify";
            Application.OpenURL(url);

            string authCode = null;
            yield return StartCoroutine(GetDiscordAuthCodeCoroutine(code => authCode = code));

            if (authCode == null)
            {
                onTokenReceived?.Invoke(null);
                onTokenReceived = null;
                yield break;
            }

            var form = new WWWForm();
            form.AddField("grant_type", "authorization_code");
            form.AddField("code", authCode);
            form.AddField("redirect_uri", redirectURL);

            using (UnityWebRequest www = UnityWebRequest.Post("https://discord.com/api/v10/oauth2/token", form))
            {
                string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(clientId + ":" + clientSecret));
                www.SetRequestHeader("Authorization", "Basic " + encodedCredentials);
                www.SetRequestHeader("Accept", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    TokenReader tokenContent = JsonUtility.FromJson<TokenReader>(www.downloadHandler.text);
                    if (!string.IsNullOrEmpty(tokenContent.access_token))
                    {
                        cachedToken = tokenContent.access_token;
                        tokenExpiration = DateTime.Now + TimeSpan.FromSeconds(tokenContent.expires_in - 60);
                        onTokenReceived?.Invoke(cachedToken);
                    }
                    else
                    {
                        onTokenReceived?.Invoke(null);
                    }
                }
                else
                {
                    onTokenReceived?.Invoke(null);
                }
                onTokenReceived = null;
            }
        }

        IEnumerator GetDiscordAuthCodeCoroutine(Action<string> callback)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{listenPort}/discord/");
            listener.Start();

            HttpListenerContext ctx = null;
            while (ctx == null)
            {
                if (listener.IsListening)
                {
                    ctx = listener.GetContext();
                }
                yield return null;
            }

            string code = ctx.Request.QueryString.Get("code");
            listener.Stop();
            callback(code);
        }

        public void FetchUserInfo(string token, Action<UserInfo> callback)
        {
            onUserInfoFetched += callback;
            StartCoroutine(FetchUserInfoCoroutine(token));
        }

        IEnumerator FetchUserInfoCoroutine(string token)
        {
            var www = UnityWebRequest.Get("https://discord.com/api/v10/users/@me");
            www.SetRequestHeader("Authorization", $"Bearer {token}");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(www.downloadHandler.text);
                onUserInfoFetched?.Invoke(userInfo);
            }
            else
            {
                Debug.LogError("Failed to fetch user info.");
                onUserInfoFetched?.Invoke(null);
            }
            onUserInfoFetched = null;
        }

    #else
        public static DiscordManager Instance
        {
            get
            {
                return null;
            }
        }

        public void RequestOAuth2Token(Action<string> callback)
        {
            callback?.Invoke(null);
        }

        public void FetchUserInfo(string token, Action<UserInfo> callback)
        {
            callback?.Invoke(null);
        }
    #endif
    }
}