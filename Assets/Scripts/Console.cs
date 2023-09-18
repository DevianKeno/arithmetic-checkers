using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Data.Common;

namespace Damath
{
    public class Console : MonoBehaviour
    {
        public Player Operator = null;
        public Dictionary<string, Command> Commands = new();
        public static Console Main { get; private set; }
        public bool IsEnabled = false;
        public string command;
        public MatchController Match = null;
        public Cell SelectedCell = null;
        public Dictionary<(int, int), Cell> Cellmap;
        [SerializeField] private Window Window;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TextMeshProUGUI messages;

        void Awake()
        {
            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
            }
            
        }

        void Update()
        {
            if (Input.GetKeyDown(Settings.KeyBinds.OpenDeveloperConsole))
            {
                if (!Settings.EnableConsole) return;
                Window.Toggle();
                if (Window.IsVisible) input.Select();
            }
        }
        
        public void OnDisable()
        {
            IsEnabled = false;
            Game.Events.OnMatchBegin -= ReceiveMatchInstance;
            Game.Events.OnBoardUpdateCellmap -= ReceiveCellmap;
        }

        /// <summary>
        /// Enables the console. Is disabled by default.
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
            Init();
        }

        void ReceiveMatchInstance(MatchController match)
        {
            Match = match;
        }

        public void SetOperator(Player player)
        {
            Operator = player;
        }

        void SubscribeToEvents()
        {
            Game.Events.OnMatchBegin += ReceiveMatchInstance;
            Game.Events.OnBoardUpdateCellmap += ReceiveCellmap;
        }

        void ReceiveCellmap(Dictionary<(int, int), Cell> cellmap)
        {
            Cellmap = cellmap;
        }

        void Init()
        {
            SubscribeToEvents();
            input = Window.transform.Find("Input").GetComponent<TMPro.TMP_InputField>();
            messages = Window.transform.Find("Message").GetComponent<TextMeshProUGUI>();

            input.onSubmit.AddListener(new UnityAction<string>(GetCommand)); 

            InitCommands();
            Log($"Started console");
        }

        public Command CreateCommand(string command, string description = "")
        {
            var args = command.Split(" ");
            string commandName = args[0];
            Command newCommand = new()
            {
                Name = args[0],
                Syntax = command,
                Description = description
            };

            int i = 0;
            foreach (string arg in args)
            {
                if (i == 0) continue;

                if (arg.Contains("|"))
                {
                    string[] splitArgs = arg.Split("|");
                }

                if (arg.Contains("<"))
                {
                    // Required
                    string substring = arg[1..^1];
                    newCommand.Arguments.Add((substring, Command.ArgType.Required));
                } else if (arg.Contains("["))
                {
                    // Optional
                    string substring = arg[1..^1];
                    newCommand.Arguments.Add((substring, Command.ArgType.Optional));
                }
                i++;
            }

            Commands.Add(commandName, newCommand); 
            return newCommand;           
        }

        void InitCommands()
        {
            CreateCommand("chat <message>",
                          "Send a message.").AddCallback(Command_Chat);

            CreateCommand("connect <address>",
                          "Connect to a match.").AddCallback(Command_Connect);

            CreateCommand("draw",
                          "Offer a draw.").AddCallback();

            CreateCommand("flip",
                          "Flips the board.").AddCallback(Command_Flip);
                        
            CreateCommand("forfeit",
                          "Forfeit match.").AddCallback();

            CreateCommand("help <command>").AddCallback(Command_Help);

            CreateCommand("host",
                          "Host current match.").AddCallback(Command_Host);

            CreateCommand("lobby <info>").AddCallback(Command_Lobby);

            CreateCommand("match <create> <classic|speed|custom>").AddCallback(Command_Match);

            CreateCommand("move <col> <row> <toCol> <toRow>",
                          "").AddCallback(Command_Move);

            CreateCommand("name <name>",
                          "Change player name.").AddCallback(Command_Name);

            CreateCommand("piece <s|s> <col> <row> [value]",
                          "Piece actions.").AddCallback(Command_Piece);

            CreateCommand("select <col> <row>",
                          "Select a cell.").AddCallback(Command_Select);
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

        public void GetCommand(string input)
        {
            if (input == "") return;
            command = input;
            Run(command);
            this.input.text = "";
            this.input.Select();
        }

        public void Refresh()
        {
            input.text = "";
            input.Select();
        }

        public void Log(object message)
        {
            if (message.GetType() != typeof(string))
            {
                Debug.Log(message);
            } else
            {
                messages.text += $"\n{message}";
            }
        }

        /// <summary>
        /// Invokes a console command.
        /// </summary>
        public void Run(string command)
        {
            if (command.Contains("/")) command = command.Replace("/", "");
            List<string> args = new(command.Split());
            Command toInvoke;

            // try
            // {
                toInvoke = Commands[args[0]];

                // try
                // {
                    toInvoke.Invoke(args);
                // } catch
                // {  
                //     PromptInvalid(args[0]);
                //     return;
                // }
            // } catch
            // {
            //     PromptInvalid();
            //     return;
            // }
        }

        // Console commands list
        #region 

        void Command_Chat(List<string> args)
        {
            args.RemoveAt(0);
            var message = string.Join(" ", args.ToArray());
            Log($"Sent a message: {message}");
            
            //
        }

        void Command_Connect(List<string> args)
        {
            // Network.Main.JoinLobby(args[1]);
        }

        void Command_Flip(List<string> args)
        {
            Game.Events.BoardFlip();
        }

        void Command_Help(List<string> args)
        {
            if (args.Count == 1)
            {
                // Run help command
                Log("List of available commands: ");
                // Iterate through command list keys
                
            } else
            {
                if (int.TryParse(args[1], out int page))
                {
                    Log($"Showing page {page}");
                } else
                {
                    Log($"Usage: " + Commands[args[1]].Syntax);
                    Log(Commands[args[1]].Description);
                }
            }
        }

        void Command_Host(List<string> args)
        {
            // Network.Main.CreateLobby();
            // Network.Main.Host();
        }

        void Command_Lobby(List<string> args)
        {
            if (args[1] == "info")
            {
                // Log(Game.Main.Lobbies[0].GetLobbyInfo());
            }
        }

        void Command_Match(List<string> args)
        {
            Game.Main.CreateMatch(Ruleset.CreateStandard());
            // if (args[1] == "create")
            // {
            //     try
            //     {
            //         Ruleset.Type mode = args[2] switch
            //         {
            //             "standard" or "1" => Ruleset.Type.Standard,
            //             "speed" or "2" => Ruleset.Type.Speed,
            //             // "custom" or "3" => Ruleset.Type.Custom,
            //             _ => throw new Exception()
            //         };
            //         Game.Main.CreateMatch(mode);
            //     } catch
            //     { 
            //         PromptInvalid(args[0]);
            //     }
            // } else if (args[1] == "start")
            // {
            //     if (Game.Main.Ruleset == null)
            //     {
            //         Log("No match created. Create one with /match create <mode>");
            //     } else
            //     {
            //         Game.Main.StartMatch();
            //     }
            // } else if (args[1] == "get")
            // {
            //     Log($"{Match}");
            // }
        }
        
        void Command_Move(List<string> args)
        {
            (int col, int row) = (int.Parse(args[1]), int.Parse(args[2]));
            (int toCol, int toRow) = (int.Parse(args[3]), int.Parse(args[4]));
            
            Game.Events.CellSelect(Cellmap[(col, row)]);
            Game.Events.CellSelect(Cellmap[(toCol, toRow)]);
        }

        void Command_Name(List<string> args)
        {
            args.RemoveAt(0);
            var name = string.Join(" ", args.ToArray());
            Game.Main.SetNickname(name);
            Game.Console.Log($"Set name to \"{name}\"");
        }

        void Command_Piece(List<string> args)
        {
            if (args[1] == "create")
            {
                //
            } else if (args[1] == "get")
            {
                Log(Match);
            }
        }

        void Command_Select(List<string> args)
        {
            (int col, int row) = (int.Parse(args[1]), int.Parse(args[2]));
            
            Game.Events.CellSelect(Cellmap[(col, row)]);
        }

        #endregion
    }
}
