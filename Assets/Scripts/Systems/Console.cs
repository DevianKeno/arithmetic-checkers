using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Aritche.UI;
using Aritche.Players;

namespace Aritche.Systems
{
    public partial class Console : MonoBehaviour
    {
        private Player Operator { get; set; }
        public bool IsEnabled = false;
        public Cell SelectedCell = null;
        public Cellmap<Cell> Cellmap;
        ConsoleWindow _UI;

        MatchController Match { get; set; }

        internal void Initialize()
        {
            Game.Main.OnLateInit += LateInit;
            
            InitializeEvents();

            InitCommands();
            Log($"Started console");
        }

        void LateInit()
        {
            _UI = Game.UI.GetWindowFromId<ConsoleWindow>("console_window");
            _UI.Initialize();
        }

        void ReceiveMatchInstance(MatchController match)
        {
            Match = match;
        }

        void ReceiveCellmap(Cellmap<Cell> cellmap)
        {
            Cellmap = cellmap;
        }

        /// <summary>
        /// Subscription to events is done after the EventManager is initialized.
        /// </summary>
        void InitializeEvents()
        {
            // Game.Events.OnClientStart += SetOperator;
            Game.Events.OnMatchCreate += ReceiveMatchInstance;
            Game.Events.OnBoardUpdateCellmap += ReceiveCellmap;
        }

        /// <summary>
        /// Set console commands to be executed as a player instance.
        /// </summary>
        /// <param name="player"></param>
        public void SetOperator(Player player)
        {
            Operator = player;
        }

        public void RegisterCommand(Command command)
        {
            if (command == null) return;
            
            Commands[command.Name] = command;
            if (command.Aliases.Count > 0)
            {
                foreach (var alias in command.Aliases)
                {
                    if (!Commands.ContainsKey(alias))
                    {
                        Commands[alias] = command;
                    }
                }
            }
        }

        /// <summary>
        /// Prompts invalid command usage.
        /// </summary>
        void PromptInvalid()
        {
            Log("Unknown command. Type /help for a list of available commands");
        }

        void PromptInvalid(string command)
        {
            Log($"Invalid command usage. Try /help {command}");
        }

        public void GetCommand(string command)
        {
            if (command == "") return;

            ExecuteCommand(command);
            _UI.Refresh();
        }

        public void Log(object message)
        {
            try
            {
                _UI.WriteLine($"{message}");
            } catch
            {
                Debug.Log(message);
            }
        }
        
        /// <summary>
        /// Log a debug message into the game's console.
        /// </summary>
        /// <param name="message"></param>
        public void LogDebug(object message)
        {
            if (!Game.Main.Settings.EnableDebugMode) return;
            
            Log(message);
        }
        
        /// <summary>
        /// Execute the current console command set from the UI.
        /// </summary>
        public void Execute()
        {
            ExecuteCommand(_UI.Command);
        }

        /// <summary>
        /// Executes a specific console command.
        /// </summary>
        public void ExecuteCommand(string command)
        {
            if (command.Contains("/")) command = command.Replace("/", "");
            List<string> args = new(command.Split());
            Command toInvoke;

            try
            {
                toInvoke = Commands[args[0]];

                try
                {
                    toInvoke.Invoke(args);
                } catch
                {  
                    PromptInvalid(args[0]);
                    return;
                }
            } catch
            {
                PromptInvalid();
                return;
            }
        }
    }
}
