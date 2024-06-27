using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;
using UnityMainThreadDispatcher;

namespace Damath.Network
{
    public enum ResultCode
    {
        Success, Failed
    }

    public class CallbackInfo<T>
    {
        public T Value { get; set; }
        public ResultCode ResultCode { get; set; }
    }
}

namespace Damath.Network.Discord
{

    [DisallowMultipleComponent]
    public class DiscordManager : MonoBehaviour
    {

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

        class TokenReader
        {
            public string access_token;
            public float expires_in;
        }
        
        string cachedToken = null;
        DateTime tokenExpiration;

        event Action<CallbackInfo<string>> onTokenReceived;
        event Action<CallbackInfo<UserInfo>> onUserInfoFetched;
        event Action<CallbackInfo<Sprite>> onFetchAvatarCompleted;

        public void SetLocalUser(UserInfo userInfo)
        {
            localUser = userInfo;
        }

        public void RequestOAuth2Token(Action<CallbackInfo<string>> callback)
        {
            onTokenReceived += callback;
            if (cachedToken != null && DateTime.Now < tokenExpiration)
            {
                onTokenReceived?.Invoke(new()
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

        async void GetTokenAsyncComplete(Task<TokenReader> tokenTask)
        {
            TokenReader tokenContent = null;
            if (tokenTask.Status == TaskStatus.RanToCompletion)
            {
                tokenContent = tokenTask.Result;
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

        async Task<string> CaptureAccessToken()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add($"http://127.0.0.1:{listenPort}/discord/");
            listener.Start();

            HttpListenerContext ctx = await listener.GetContextAsync();
            HttpListenerResponse resp = ctx.Response;

            string code = ctx.Request.QueryString.Get("code");

            resp.StatusCode = (int)HttpStatusCode.OK;
            resp.StatusDescription = "Status OK";
            listener.Stop();

            return code;
        }

        async Task<TokenReader> GetTokenAsync()
        {
            string redirectURL = $"http://127.0.0.1:{listenPort}/discord/";
            string url = "https://discord.com/oauth2/authorize?client_id=1255192173842796544&response_type=code&redirect_uri=http%3A%2F%2F127.0.0.1%3A54545%2Fdiscord%2F&scope=identify";
            Application.OpenURL(url);

            string authCode = await Task.Run(CaptureAccessToken);

            if (authCode == null)
            {
                return null;
            }

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
                        "Basic " + encodedCredentials
                    },
                    {
                        HttpRequestHeader.Accept.ToString(),
                        "application/json"
                    }
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                    { "grant_type", "authorization_code" },
                    { "code", authCode },
                    { "redirect_uri", redirectURL }
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

        public async void FetchUserInfo(string token, Action<CallbackInfo<UserInfo>> callback)
        {
            onUserInfoFetched += callback;
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var response = await client.GetAsync("https://discord.com/api/v10/users/@me");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string responseText = await response.Content.ReadAsStringAsync();
                UserInfo userInfo = JsonUtility.FromJson<UserInfo>(responseText);

                Dispatcher.Enqueue(() =>
                {
                    onUserInfoFetched?.Invoke(new()
                    {
                        Value = userInfo,
                        ResultCode = ResultCode.Success
                    });
                    onUserInfoFetched = null;
                });
            }
            else
            {
                Dispatcher.Enqueue(() =>
                {
                    onUserInfoFetched?.Invoke(new()
                    {
                        Value = null,
                        ResultCode = ResultCode.Failed
                    });
                    onUserInfoFetched = null;
                });
            }
        }

        public async void FetchUserAvatar(UserInfo info, Action<CallbackInfo<Sprite>> callback)
        {
            onFetchAvatarCompleted += callback;
            string url = $"https://cdn.discordapp.com/avatars/{info.id}/{info.avatar}.png?size=256";
            var client = new HttpClient();

            var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsByteArrayAsync();
                Texture2D texture = new(2, 2);
                if (texture.LoadImage(data))
                {
                    Sprite avatarSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                    Dispatcher.Enqueue(() =>
                    {
                        onFetchAvatarCompleted?.Invoke(new CallbackInfo<Sprite>
                        {
                            Value = avatarSprite,
                            ResultCode = ResultCode.Success
                        });
                        onFetchAvatarCompleted = null;
                    });
                }
            }
            else
            {
                Dispatcher.Enqueue(() =>
                {
                    onFetchAvatarCompleted?.Invoke(new CallbackInfo<Sprite>
                    {
                        Value = null,
                        ResultCode = ResultCode.Failed
                    });
                    onFetchAvatarCompleted = null;
                });
            }
        }

        public void Logout()
        {
            localUser = null;

            cachedToken = null;
            tokenExpiration = default;

            onTokenReceived = null;
            onUserInfoFetched = null;
            onFetchAvatarCompleted = null;
        }
    }
}
