#if !(UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS)
#define DISABLEDISCORD
#endif

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

using JsonUtility = PlayEveryWare.EpicOnlineServices.Utility.JsonUtility;

namespace Aritche.EOS.Managers
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

        /// <summary>
        /// Set this to the client ID specified in the Discord dev portal for your application.
        /// </summary>
        const long clientId = 1255192173842796544;
        /// <summary>
        /// Set this to the client secret received from the OAuth2 section of the Discord dev portal for your application.
        /// FOR TESTING ONLY: Ideally the secret would be stored on a seperate web server
        /// and Discord login requests would be passed through there.
        /// </summary>
        const string clientSecret = "cizf2Kvnpy5fis0gEXrGvrkjIoPv8zYO";
        /// http://127.0.0.1:<port>/discord must be added as a redirect in the Discord dev portal
        /// This opens a listen server to receive the oauth2 callback on localhost,
        /// which may not work consistently across browsers and should be used for testing only.
        const int listenPort = 54545;

        protected virtual void Awake()
        {
            // Only one instance of DiscordManager at a time!
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;

            // We want our DiscordManager Instance to persist across scenes.
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

        class TokenReader
        {
            public string access_token;
            public float expires_in;
        }

        string cachedToken = null;
        DateTime tokenExpiration;

        event Action<string> onTokenReceived;

        public void RequestOAuth2Token(Action<string> callback)
        {
            /// Return cached token or begin login flow to acquire new one
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

            GetTokenAsync().ContinueWith(GetTokenAsyncComplete);
        }

        void GetTokenAsyncComplete(Task<TokenReader> tokenTask)
        {
            TokenReader tokenContent = null;
            if (tokenTask.Status == TaskStatus.RanToCompletion)
            {
                tokenContent = tokenTask.Result;
            }

            if (tokenContent != null)
            {
                cachedToken = tokenContent.access_token;
                //in a real application the refresh token would be stored as well and used to refresh the auth token
                tokenExpiration = DateTime.Now + TimeSpan.FromSeconds(tokenContent.expires_in-60);
            }
            else
            {
                cachedToken = null;
            }

            onTokenReceived?.Invoke(cachedToken);
            onTokenReceived = null;
        }

        async Task<string> GetDiscordAuthCode()
        {
            /// Create a local listen server to receive redirect callback from Discord authentication
            using var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{listenPort}/discord/");
            listener.Start();

            HttpListenerContext ctx = await listener.GetContextAsync();
            using HttpListenerResponse resp = ctx.Response;

            string code = ctx.Request.QueryString.Get("code");

            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";
            listener.Stop();

            return code;
        }

        async Task<TokenReader> GetTokenAsync()
        {
            string redirectURL = $"http://127.0.0.1:{listenPort}/discord/";
            /// Use web auth to begin Discord login flow
            string url = "https://discord.com/oauth2/authorize?client_id=1255192173842796544&response_type=code&redirect_uri=http%3A%2F%2F127.0.0.1%3A54545%2Fdiscord%2F&scope=identify" ;
            Application.OpenURL(url);

            string authCode = await Task.Run(GetDiscordAuthCode);

            if (authCode == null)
            {
                return null;
            }

            //use received auth code to request auth token
            var client = new HttpClient();
            string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(clientId + ":" + clientSecret));
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://discord.com/api/v10/oauth2/token"),
                Headers =
                {
                    {
                        HttpRequestHeader.Authorization.ToString(),
                        "Basic "+encodedCredentials
                    },
                    {
                        HttpRequestHeader.Accept.ToString(),
                        "application/json"
                    }
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    {
                        "grant_type",
                        "authorization_code"
                    },
                    {
                        "code",
                        authCode
                    },
                    {
                        "redirect_uri",
                        redirectURL
                    }
                })
            };

            var response = await client.SendAsync(httpRequestMessage);
            TokenReader tokenContent = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                tokenContent = JsonUtility.FromJson<TokenReader>(responseText);
                if (string.IsNullOrEmpty(tokenContent.access_token))
                {
                    tokenContent = null;
                }
            }

            return tokenContent;
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
#endif
    }
}