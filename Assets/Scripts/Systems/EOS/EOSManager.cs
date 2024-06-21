using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;
using Epic.OnlineServices;
using Epic.OnlineServices.Auth;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Platform;
using Damath.Systems.Configuration;

namespace Damath.Systems.EOS
{
    public class EOSManager : MonoBehaviour
    {
        [SerializeField] static bool _isInitialized = false; 

        public string ProductName = "Arithmetic Checkers";
        public string ProductVersion = "1.0";

        EpicAccountId loggedInAccountId;
        
        public LoginCredentialType m_LoginCredentialType = LoginCredentialType.AccountPortal;

        /// These fields correspond to \<see cref="Credentials.Id" /> and \<see cref="Credentials.Token" />,
        /// and their use differs based on the login type. For more information, see \<see cref="Credentials" />
        /// and the Auth Interface documentation.
        public string m_LoginCredentialId = null;
        public string m_LoginCredentialToken = null;

        static PlatformInterface platformInterface;
        const float c_PlatformTickInterval = 0.1f;
        float m_PlatformTickTimer = 0f;
        IntPtr m_LibraryPointer;

        /// If we're in editor, we should dynamically load and unload the SDK between play sessions.
        /// This allows us to initialize the SDK each time the game is run in editor.
        #if UNITY_EDITOR
            [DllImport("Kernel32.dll")]
            private static extern IntPtr LoadLibrary(string lpLibFileName);

            [DllImport("Kernel32.dll")]
            private static extern int FreeLibrary(IntPtr hLibModule);

            [DllImport("Kernel32.dll")]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        #endif

        void Awake()
        {
            #if UNITY_EDITOR
                var libraryPath = "Assets/" + Epic.OnlineServices.Config.LibraryName;

                m_LibraryPointer = LoadLibrary(libraryPath);
                if (m_LibraryPointer == IntPtr.Zero)
                {
                    Debug.LogError("Failed to load library" + libraryPath);
                    return;
                }
                Bindings.Hook(m_LibraryPointer, GetProcAddress);
            #endif
        }

        EOSConfig LoadEOSConfig()
        {
            TextAsset configText = Resources.Load<TextAsset>("eos");
            if (configText == null)
            {
                Debug.LogError("Configuration file not found!");
                return null;
            }

            try
            {
                var config = JsonUtility.FromJson<Configuration.Config>(configText.text);
                return config.EOSConfig;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load config: {e.Message}");
                return null;
            }
        }

        internal void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("EOS Platform is already initialized.");
                return;
            }

            var eosConfig = LoadEOSConfig();
            var initializeOptions = new InitializeOptions()
            {
                ProductName = ProductName,
                ProductVersion = ProductVersion
            };
            var initializeResult = PlatformInterface.Initialize(ref initializeOptions);

            if (initializeResult != Result.Success)
            {
                Debug.LogError("Failed to initialize platform: " + initializeResult);
                return;
            }

            /// The SDK outputs lots of information that is useful for debugging.
            /// Make sure to set up the logging interface as early as possible: after initializing.
            LoggingInterface.SetLogLevel(LogCategory.AllCategories, LogLevel.VeryVerbose);
            LoggingInterface.SetCallback((ref LogMessage logMessage) =>
            {
                Debug.Log(logMessage.Message);
            });
            
            var options = new Options()
            {
                ProductId = eosConfig.ProductId,
                SandboxId = eosConfig.SandboxId,
                DeploymentId = eosConfig.DeploymentId,
                ClientCredentials = new ClientCredentials()
                {
                    ClientId = eosConfig.ClientId,
                    ClientSecret = eosConfig.ClientSecret
                }
            };

            platformInterface = PlatformInterface.Create(ref options);
            if (platformInterface == null)
            {
                Debug.LogError("Failed to create platform");
                return;
            }
            
            _isInitialized = true;
        }

        internal void Login()
        {
            var loginOptions = new LoginOptions()
            {
                Credentials = new Credentials()
                {
                    Type = m_LoginCredentialType,
                    Id = m_LoginCredentialId,
                    Token = m_LoginCredentialToken
                },
                /// Change these scopes to match the ones set up on your product on the Developer Portal.
                ScopeFlags = AuthScopeFlags.BasicProfile | AuthScopeFlags.FriendsList //| AuthScopeFlags.Presence
            };

            try
            {
                /// Ensure platform tick is called on an interval, or this will not callback.
                platformInterface.GetAuthInterface().Login(ref loginOptions, null, OnLoginComplete);
            } catch
            {
                Debug.LogError("Login error.");
                return;
            }
        }

        void OnLoginComplete(ref LoginCallbackInfo callbackInfo)
        {
            if (callbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Login succeeded");
                loggedInAccountId = callbackInfo.LocalUserId;
            }
            else if (Common.IsOperationComplete(callbackInfo.ResultCode))
            {
                Debug.Log("Login failed: " + callbackInfo.ResultCode);
            }
        }

        internal void Logout()
        {
            var logoutOptions = new LogoutOptions()
            {
                LocalUserId = loggedInAccountId
            };
            platformInterface.GetAuthInterface().Logout(ref logoutOptions, null, OnLogoutComplete);
        }

        void OnLogoutComplete(ref LogoutCallbackInfo callbackInfo)
        {
            Debug.Log($"Logout completed with result: {callbackInfo.ResultCode}");

            if (callbackInfo.ResultCode == Result.Success)
            {
                Debug.Log("Persistent auth deleted.");
                loggedInAccountId = null;
            }
            else if (Common.IsOperationComplete(callbackInfo.ResultCode))
            {
                Debug.LogError("Logout failed: " + callbackInfo.ResultCode);
            }
        }

        /// <summary>
        /// Calling tick on a regular interval is required for callbacks to work.
        /// </summary>
        void Update()
        {
            if (platformInterface != null)
            {
                m_PlatformTickTimer += Time.deltaTime;

                if (m_PlatformTickTimer >= c_PlatformTickInterval)
                {
                    m_PlatformTickTimer = 0;
                    platformInterface.Tick();
                }
            }
        }

        void OnApplicationQuit()
        {
            if (platformInterface != null)
            {
                platformInterface.Release();
                platformInterface = null;
                PlatformInterface.Shutdown();
            }

            #if UNITY_EDITOR
                if (m_LibraryPointer != IntPtr.Zero)
                {
                    Debug.Log($"Unhooking bindings...");
                    Bindings.Unhook();
                    Debug.Log($"Bindings unhooked. (?)");
                    // FreeLibraryAsync();
                    _isInitialized = false;
                }
            #endif
        }

        public int FreeLibraryMaxIter = 10;
        /// <summary>
        /// This does not work it just makes the Unity Editor hang. I don't know what is happening and I honestly have no idea
        /// how to fix this. I just commented this for now but it leads me to restart the editor every single time ;;
        /// </summary>
        async void FreeLibraryAsync()
        {
            /// Free the library until the module ref count is 0
            int freeCount = 0;
            while (FreeLibrary(m_LibraryPointer) != 0)
            {
                freeCount++;
                if (freeCount > FreeLibraryMaxIter)
                {
                    Debug.LogWarning($"FreeLibrary called more than {FreeLibraryMaxIter} times, something might be wrong.");
                    break;
                }
                await Task.Yield();
            }
            Debug.Log($"FreeLibrary called {freeCount} times.");

            m_LibraryPointer = IntPtr.Zero;
            Debug.Log("Library freed.");
        }
    }
}