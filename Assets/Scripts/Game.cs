using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        protected bool IsAlive;
        public bool IsPaused;
        public bool HasMatch { get; private set; }
        [field: SerializeField] public bool IsHosting { get; private set; }
        public Ruleset Ruleset { get; private set; }
        public string Nickname = "Player";
        public List<Lobby> Lobbies = new();
        public MatchController Match { get; private set; }

        void Awake()
        {
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
                Network = GameObject.FindGameObjectWithTag("Network").GetComponent<Network>();
            }
        }

        void Start()
        {
            IsAlive = true;
            HasMatch = false;

            if (Settings.EnableConsole)
            {
                Console.Enable();
            }

            Events.OnMatchCreate += SetMatch;
        }
        
        public void Pause(bool value)
        {
            IsPaused = value;
        }

        public void LoadScene(string scene, bool playTransition = false, float delayInSeconds = 0f)
        {
            try
            {
                if (playTransition)
                {
                    // Play transition
                    // UI.PlayTransition();
                    StartCoroutine(Load(scene, delayInSeconds));
                } else
                {
                    SceneManager.LoadScene(scene);
                }
            } catch (NullReferenceException e)
            {
                Debug.Log("Scene does not exist" + e);
            }
        }

        IEnumerator Load(string scene, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            SceneManager.LoadScene(scene);
        }

        /// <summary>
        /// Creates a match given a ruleset.
        /// </summary>
        public void CreateMatch()
        {
            LoadScene("Match", playTransition: true);
            
            Ruleset = Ruleset.CreateStandard();
            Events.RulesetCreate(Ruleset);
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
            CreateMatch();
            StartMatch();
        }

        public void Debug_Start()
        {
            StartMatch();
        }

        #endregion
    }
}
