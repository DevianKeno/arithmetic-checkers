using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Transporting;
using UnityEngine.SocialPlatforms;

namespace Damath
{
    /// <summary>
    /// Main game controller.
    /// </summary>
    public class Game : MonoBehaviour
    {
        public static Game Main { get; private set; }
        public static EventsManager Events { get; private set; }
        public static Console Console { get; private set; }
        public static Network Network { get; private set; }
        public static UIHandler UI { get; private set; }
        public static AudioManager Audio { get; private set; }

        protected bool IsAlive { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsOnline { get; private set; }
        public bool IsHosting { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool HasRuleset { get; private set; }
        public bool HasMatch { get; private set; }
        public Ruleset Ruleset { get; private set; }
        public string Nickname = "Player";
        public MatchController Match { get; private set; }
        public Scene CurrentScene { get; private set; }


        void Awake()
        {
            DontDestroyOnLoad(this);

            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
                Events = GetComponentInChildren<EventsManager>();
                Console = GetComponentInChildren<Console>();
                UI = GetComponentInChildren<UIHandler>();
                Audio = GetComponentInChildren<AudioManager>();
                Network = GetComponentInChildren<Network>();
            }
        }

        void Start()
        {
            IsAlive = true;
            HasRuleset = false;
            HasMatch = false;

            if (Settings.EnableConsole)
            {
                Console.Enable();
            }
            
            // Initialize
            Ruleset.Init();
            // Themes.Init();
            
            // Events.OnMatchCreate += SetMatch;
            // Network.SceneManager.OnLoadEnd += RegisterScene;
        }

        public void RegisterScene(SceneLoadEndEventArgs args)
        {            
            CurrentScene = args.LoadedScenes[0];
        }
        
        public void Pause(bool value)
        {
            IsPaused = value;
        }

        public void LoadScene(string sceneName, bool playTransition = false, float delayInSeconds = 0f)
        {
            try
            {
                if (playTransition)
                {
                    // Play transition
                    // UI.PlayTransition();
                    StartCoroutine(Load(sceneName, delayInSeconds));
                } else
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                }
            } catch (NullReferenceException e)
            {
                Debug.Log("Scene does not exist" + e);
            }
        }

        IEnumerator Load(string scene, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
            // CurrentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        void CacheRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
            HasRuleset = true;
        }

        public void SetRuleset(Ruleset ruleset)
        {
            CacheRuleset(ruleset);
        }

        /// <summary>
        /// Host a multiplayer match.
        /// </summary>
        public void HostMatch(RulesetType mode)
        {
            CacheRuleset(Ruleset.Create(mode));

            Network.Main.StartHost();
        }

        public void JoinMatch(Lobby lobby, string address)
        {
            IsOnline = true;

            CacheRuleset(lobby.Ruleset);

            Network.Main.Connect(address);
        }
        
        public void LoadGlobalScene(string sceneName)
        {
            if (Settings.EnableDebugMode)
            {
                Console.Log($"Loading scene {sceneName}");
            }

            InstanceFinder.SceneManager.LoadGlobalScenes( new(sceneName) );
        }

        public void UnloadGlobalScene(string sceneName)
        {
            if (Settings.EnableDebugMode)
            {
                Console.Log($"Unloading scene {sceneName}");
            }

            InstanceFinder.SceneManager.UnloadGlobalScenes( new(sceneName) );
        }

        /// <summary>
        /// Creates a match given a ruleset.
        /// </summary>
        public void CreateMatch(RulesetType mode)
        {
            CacheRuleset(Ruleset.Create(mode));

            LoadScene("Match", playTransition: true);
        }      

        private void SetMatch(MatchController match)
        {
            HasMatch = true;
            Match = match;
        }

        /// <summary>
        /// Starts the match.
        /// </summary>
        public void StartMatch()
        {
            if (HasMatch)
            {
                Match.Init();
            }
        }

        public void SetNickname(string value)
        {
            Nickname = value;
        }
        
        #region Debug

        public void Debug_StartStandard()
        {
            
            StartMatch();
        }

        public void Debug_Start()
        {
            StartMatch();
        }

        #endregion
    }
}
