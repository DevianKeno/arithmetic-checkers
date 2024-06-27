using System;
using System.Net;
using System.Threading.Tasks;

using UnityEngine;

namespace Damath.Network.Itch
{
    public class ItchManager : MonoBehaviour
    {
        protected static ItchManager s_instance;
        public static ItchManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    return new GameObject("ItchManager").AddComponent<ItchManager>();
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

        const string clientId = "c59e78190a7fa9cadfc87158dff309cf";
        const string clientSecret = "d9d9eb31a15c0381b728dc54d2e014fdac46654e282c57dda9f922b6e80d2d20";
        const int listenPort = 34567;

        string cachedToken = null;
        DateTime tokenExpiration;

        event Action<CallbackInfo<string>> onTokenReceived;
        event Action<CallbackInfo<UserInfo>> onUserInfoFetched;
        event Action<CallbackInfo<Sprite>> onFetchAvatarCompleted;

        public void RequestOAuthToken(Action<CallbackInfo<string>> callback)
        {
            onTokenReceived += callback;
            if (cachedToken != null && DateTime.Now < tokenExpiration)
            {
                onTokenReceived?.Invoke(new CallbackInfo<string>
                {
                    Value = cachedToken,
                    ResultCode = ResultCode.Success
                });
                onTokenReceived = null;
                return;
            }
            else
            {
                cachedToken = null;
            }

            GetTokenAsync().ContinueWith(GetTokenAsyncComplete);
        }

        async void GetTokenAsyncComplete(Task<TokenResponse> response)
        {
            TokenResponse tokenContent = null;
            if (response.Status == TaskStatus.RanToCompletion)
            {
                tokenContent = response.Result;
            }

            if (tokenContent != null)
            {
                cachedToken = tokenContent.access_token;
                tokenExpiration = DateTime.Now + TimeSpan.FromSeconds(tokenContent.expires_in - 60);
            }
            else
            {
                cachedToken = null;
            }

            onTokenReceived?.Invoke(new()
            {
                Value = cachedToken,
                ResultCode = ResultCode.Success
            });
            onTokenReceived = null;
        }

        async Task<TokenResponse> GetTokenAsync()
        {
            // string redirectUri = $"http://localhost:{listenPort}/callback/";
            string authUrl = $"https://itch.io/user/oauth?client_id=c59e78190a7fa9cadfc87158dff309cf&scope=profile%3Ame&response_type=token&redirect_uri=http%3A%2F%2F127.0.0.1%3A34567";
            Application.OpenURL(authUrl);

            string accessToken = await CaptureAccessToken();

            if (accessToken == null)
            {
                return null;
            }

            cachedToken = accessToken;
            tokenExpiration = DateTime.Now.AddHours(1); // Adjust expiration as needed

            return new TokenResponse { access_token = accessToken };
        }

        async Task<string> CaptureAccessToken()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:34567");
            listener.Start();

            HttpListenerContext ctx = await listener.GetContextAsync();
            HttpListenerResponse resp = ctx.Response;

            string fragment = ctx.Request.Url.Fragment;
            listener.Stop();

            if (fragment.StartsWith("#"))
            {
                var queryParams = System.Web.HttpUtility.ParseQueryString(fragment.Substring(1));
                string accessToken = queryParams.Get("access_token");
                return accessToken;
            }
            else
            {
                return null;
            }
        }

        public void Logout()
        {
            cachedToken = null;
            tokenExpiration = default;

            onTokenReceived = null;
            onUserInfoFetched = null;
            onFetchAvatarCompleted = null;
        }

        [Serializable]
        public class UserInfo
        {
            public string username;
            public string email;
            // Add other fields as needed
        }

        [Serializable]
        public class TokenResponse
        {
            public string access_token;
            public string token_type;
            public int expires_in;
            public string refresh_token;
        }
    }
}
