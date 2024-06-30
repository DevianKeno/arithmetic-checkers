using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

using Epic.OnlineServices.Logging;

using PlayEveryWare.EpicOnlineServices;

using Aritche.Systems;
using Aritche.Rules;
using Aritche.EOS;

namespace Aritche
{
    /// <summary>
    /// Main game controller.
    /// </summary>
    public class Game : MonoBehaviour
    {
        public static Game Main { get; private set; }
        [field: SerializeField] public Settings Settings { get; private set; }

        public static Lang Lang { get; private set; }
        public static EventsManager Events { get; private set; }
        public static Systems.Console Console { get; private set; }
        public static UIManager UI { get; private set; }
        public static AudioManager Audio { get; private set; }
        /// <summary>
        /// EOS Interface (Singleton).
        /// Just a shortcut to 'EOSManager.Instance'
        /// </summary>
        public static EOSManager.EOSSingleton EOS => EOSManager.Instance;
        public static EOSSubManagers EOSManagers { get; private set; }

        public bool IsAlive { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsOnline
        {
            get
            {
                return EOSManager.Instance.GetEOSPlatformInterface() != null;
            }
        }
        public bool IsHosting { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool HasRuleset { get; private set; }
        public bool HasMatch { get; private set; }
        public Ruleset Ruleset { get; private set; }
        public Dictionary<Side, string> Nickname = new()
        {
            {Side.Blue, "Player"},
            {Side.Orange, "Opponent"}
        };
        public MatchController Match { get; private set; }
        public event Action OnLateInit;
        public Scene CurrentScene
        {
            get
            {
                return SceneManager.GetActiveScene();
            }
        }

        [SerializeField] PlayerInput mainInput;
        public PlayerInput MainInput => mainInput;

        void Awake()
        {
            DontDestroyOnLoad(this);

            if (Main != null && Main != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Main = this;
                Lang = GetComponentInChildren<Lang>();
                Events = GetComponentInChildren<EventsManager>();
                Console = GetComponentInChildren<Systems.Console>();
                UI = GetComponentInChildren<UIManager>();
                Audio = GetComponentInChildren<AudioManager>();
                EOSManagers = GetComponentInChildren<EOSSubManagers>();
            }
            mainInput = GetComponent<PlayerInput>();
        }

        void Start()
        {
            IsAlive = true;
            InitializeManagers();
            InitializeEvents();
            
            HasRuleset = false;
            HasMatch = false;
        }

        void InitializeManagers()
        {
            Console.Initialize();
            UI.Initialize();
            Audio.Initialize();
            OnLateInit?.Invoke();
        }

        void InitializeEvents()
        {
            Events.OnMatchCreate += SetMatch;
            // Network.SceneManager.OnLoadEnd += RegisterScene;
        }

        public InputActionMap GetActionMap(string name)
        {
            return MainInput.actions.FindActionMap(name);
        }

        // void RegisterScene(SceneLoadEndEventArgs args)
        // {            
        //     CurrentScene = args.LoadedScenes[0];
        // }
        
        public void Pause(bool value)
        {
            IsPaused = value;
        }


        event Action OnLoadSceneCompleted;

        public void LoadScene(string sceneName, bool playTransition = true, float delayInSeconds = 0f, Action OnLoadSceneCompletedCallback = null)
        {
            try
            {
                OnLoadSceneCompleted += OnLoadSceneCompletedCallback;
                StartCoroutine(Load(sceneName, playTransition, Game.UI.TransitionOptions.AnimationSpeed / 2f));
            }
            catch (NullReferenceException e)
            {
                Debug.LogError($"Scene does not exist" + e);
            }
        }

        void Update()
        {
            if (asyncOp != null)
            {
                var progress = asyncOp.progress * 100f;
                Debug.Log($"LOADING: {progress}%");
            }
        }

        AsyncOperation asyncOp = null;

        IEnumerator Load(string scene, bool playTransition, float delayInSeconds)
        {
            var asyncOp = SceneManager.LoadSceneAsync(scene);
            asyncOp.allowSceneActivation = false;

            while (asyncOp.progress < 0.9f)
            {
                yield return null;
            }

            if (playTransition)
            {
                Game.UI.PlayTransitionHalf(() =>
                {
                    asyncOp.allowSceneActivation = true;
                    Game.UI.PlayTransitionEnd(() =>
                    {
                        OnLoadSceneCompleted?.Invoke();
                        OnLoadSceneCompleted = null;
                    });
                });
            }
        }

        void CacheRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
            HasRuleset = true;
        }

        void ClearRuleset()
        {
            Ruleset = null;
            HasRuleset = false;
        }

        /// <summary>
        /// Host a multiplayer match.
        /// </summary>
        public void HostMatch(RulesetPreset preset)
        {
            CacheRuleset(Ruleset.CreateFromPreset(preset));

            // Network.Main.StartHost();
        }

        // public void JoinMatch(Lobby lobby, string address)
        // {
        //     IsOnline = true;

        //     CacheRuleset(lobby.Ruleset);

        //     Network.Main.Connect(address);
        // }
        
        public void LoadGlobalScene(string sceneName)
        {
            if (Settings.EnableDebugMode)
            {
                Console.Log($"Loading scene {sceneName}");
            }

            SceneManager.LoadScene(sceneName);
        }

        public void UnloadGlobalScene(string sceneName)
        {
            if (Settings.EnableDebugMode)
            {
                Console.Log($"Unloading scene {sceneName}");
            }

            SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// Creates a match given a ruleset type.
        /// </summary>
        public void CreateMatch(RulesetPreset mode)
        {
            CacheRuleset(Ruleset.CreateFromPreset(mode));
        }
        
        /// <summary>
        /// Creates a match given a ruleset.
        /// </summary>
        public void CreateMatch(Ruleset ruleset)
        {
            CacheRuleset(ruleset);
        }      

        public void SetMatch(MatchController match)
        {
            HasMatch = true;
            Match = match;
        }

        public void StartMatch()
        {
            if (!HasMatch) return;
            if (CurrentScene.name == "Match")
            {
                Match.StartMatch();
            }
            else
            {
                Game.Console.Log($"Unable to start match. You're not in a match!");
                Debug.LogError($"Unable to start match. You're not in a match!");
            }
        }

        internal void EndMatch()
        {
            ClearRuleset();
            Match.EndMatch();
            Match = null;
            HasMatch = false;
        }

        public void SetNickname(Side side, string value)
        {
            Nickname[side] = value;
        }
    }
}
