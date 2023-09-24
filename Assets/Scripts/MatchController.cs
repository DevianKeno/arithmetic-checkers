using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Unity;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using TMPro;

namespace Damath
{
    /// <summary>
    /// Controls the match.
    /// </summary>
    public class MatchController : MonoBehaviour
    {
        [field: Header("Match")]
        public Ruleset Rules { get; private set; }
        public bool IsPlaying { get; set; }
        public bool IsMultiplayer { get; set; }
        public LobbyManager LobbyManager { get; private set; }

        [SerializeField] private TextMeshProUGUI rulesetText;
        [SerializeField] private GameObject playerPrefab;

        void Awake()
        {
            LobbyManager = FindObjectOfType<LobbyManager>();

            Game.Events.OnLobbyStart += BeginMatch;
        }

        void Start()
        {
            if (Game.Main.Ruleset != null)
            {
                Rules = Game.Main.Ruleset;

                rulesetText.text = Rules.Mode.ToString();
                Game.Events.RulesetDistribute(Rules);
                Game.Events.MatchCreate(this);
                
                Game.Console.Log($"Created {Rules.Mode} match");
            } else
            {
                Game.Console.Log($"No ruleset created. Set one first before starting.");
            }
        }

        void OnDisable() 
        {
            Game.Events.OnLobbyStart -= BeginMatch;
        }

        public void Init()
        {
            if (IsPlaying) return;
            IsPlaying = true;

            StartSolo();

            // if (Game.Network.IsOffline)
            // {
            //     StartSolo();
            // } else
            // {
            //     StartOnline();
            // }
        }

        public Player CreatePlayer(Side side)
        {
            Player newPlayer = Instantiate(playerPrefab).GetComponent<Player>();
            newPlayer.SetSide(side);
            newPlayer.SetPlaying(true);
            Game.Events.PlayerCreate(newPlayer);
            return newPlayer;
        }
        
        void StartOnline()
        {
            // Rules = Network.Network.Lobby.Ruleset;

            // This should be called before the match starts (pre-initialization)
            BeginMatch(true);
        }

        void StartSolo()
        {
            CreatePlayer(Side.Bot).IsControllable = true;
            CreatePlayer(Side.Top).IsControllable = true;
            
            BeginMatch(true);
        }

        public void BeginMatch(bool force = false)
        {
            Game.Events.MatchBegin(this);
            IsPlaying = true;
        }

        public void BeginMatch(Lobby lobby)
        {
            BeginMatch(true);
        }
    }
}