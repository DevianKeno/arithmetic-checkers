using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace Damath
{
    public class Console : MonoBehaviour
    {
        private Player Operator { get; set; }
        public Dictionary<string, Command> Commands = new();
        public static Console Main { get; private set; }
        public bool IsEnabled = false;
        public string command;
        public Cell SelectedCell = null;
        public Cellmap<Cell> Cellmap;
        [SerializeField] private Window Window;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TextMeshProUGUI messages;

        private MatchController Match { get; set; }

        void Awake()
        {
            if (Main != null && Main != this)
            {
                Destroy(this);
            } else
            {
                Main = this;
                Operator = null;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(Settings.KeyBinds.OpenDeveloperConsole))
            {
                if (!Game.Settings.EnableConsole) return;
                Window.Toggle();
                if (Window.IsVisible) input.Select();
            }
        }
        
        public void OnEnable()
        {
            input.Select();
        }

        public void OnDisable()
        {
            IsEnabled = false;
            
            Game.Events.OnClientStart -= SetOperator;
            Game.Events.OnMatchCreate -= ReceiveMatchInstance;
            Game.Events.OnBoardUpdateCellmap -= ReceiveCellmap;
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
        void SubscribeToEvents()
        {
            Game.Events.OnClientStart += SetOperator;
            Game.Events.OnMatchCreate += ReceiveMatchInstance;
            Game.Events.OnBoardUpdateCellmap += ReceiveCellmap;
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

        /// <summary>
        /// Enables the console. Is disabled by default.
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
            Init();
        }

        /// <summary>
        /// Set console commands to be executed as a player instance.
        /// </summary>
        /// <param name="player"></param>
        public void SetOperator(Player player)
        {
            Operator = player;
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

            CreateCommand("clear",
                          "Clears console.").AddCallback(Command_Clear);

            CreateCommand("connect <address>",
                          "Connect to a match.").AddCallback(Command_Connect);

            CreateCommand("debug <scene>").AddCallback(Command_Debug);

            CreateCommand("draw",
                          "Offer a draw.").AddCallback();

            CreateCommand("flip",
                          "Flips the board.").AddCallback(Command_Flip);
                        
            CreateCommand("forfeit",
                          "Forfeit match.").AddCallback();

            CreateCommand("help <command>").AddCallback(Command_Help);

            CreateCommand("host",
                          "Host current match.").AddCallback(Command_Host);
                          
            CreateCommand("leave",
                          "Leave to title screen.").AddCallback(Command_Leave);

            CreateCommand("lobby <create|info>").AddCallback(Command_Lobby);

            CreateCommand("match <create> <classic|speed|custom>").AddCallback(Command_Match);

            CreateCommand("move <col> <row> <toCol> <toRow>",
                          "").AddCallback(Command_Move);

            CreateCommand("name <name> [side]",
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

        public void GetCommand(string command)
        {
            if (command == "") return;

            Run(command);
            Refresh();
        }

        public void Refresh()
        {
            input.text = "";
            input.Select();
        }

        public void Write(string value)
        {
            messages.text += $"{value}";
        }

        public void WriteLine(string value)
        {
            messages.text += $"\n{value}";
        }

        public void Log(object message)
        {
            try
            {
                WriteLine($"{message}");
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
            if (!Game.Settings.EnableDebugMode) return;
            
            Log(message);
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

        #region Console commands list

        void Command_Chat(List<string> args)
        {
            // Remove "chat" argument
            args.RemoveAt(0);
            string message = string.Join(" ", args.ToArray());
            string data = $"{Game.Main.Nickname};{message}";

            // Invoke console command as player
            if (true)
            {
                Game.Events.NetworkSend(Parser.Pack(data, Pack.Chat));
            } else
            {
                // Send chat locally
            }
        }

        void Command_Clear(List<string> args)
        {
            messages.text = "";
        }

        void Command_Connect(List<string> args)
        {
            if (args[1] != null)
            {
                if (args[1] == "localhost") args[1] = "127.0.0.1";
                
                Network.Main.Connect(args[1]);

            } else
            {
                Network.Main.Connect("localhost");
            }
        }

        void Command_Debug(List<string> args)
        {
            if (args[1] == "scene")
            {
                Game.Console.Log($"Current scene: {Game.Main.CurrentScene.name}");
            }
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
                string message = "";
                foreach (var c in Commands)
                {
                    // This will have an extra comma, pls fix
                    message += $"{c.Value.Name}, ";
                }
                
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
            try
            {
                RulesetType ruleset = args[1] switch
                {
                    "standard" or "1" => RulesetType.Standard,
                    "speed" or "2" => RulesetType.Speed,
                    // "custom" or "3" => Ruleset.Type.Custom,
                    _ => throw new Exception()
                };

                Game.Main.HostMatch(ruleset);
                return;

            } catch
            {
                PromptInvalid(args[0]);
                return;
            }
        }        
        
        void Command_Leave(List<string> args)
        {
            Game.Main.LoadScene("Title");
        }

        void Command_Lobby(List<string> args)
        {
            if (!Game.Main.HasRuleset)
            {
                Game.Console.Log("Create a match first with /match create <mode>");
                return;
            }

            if (args.Count != 1)
            {
                if (args[1] ==  "info")
                {

                    return;
                }
            }
        }

        void Command_Match(List<string> args)
        {
            switch (args[1])
            {
                case "create":
                    try
                    {
                        RulesetType ruleset = args[2] switch
                        {
                            "standard" or "1" => RulesetType.Standard,
                            "speed" or "2" => RulesetType.Speed,
                            // "custom" or "3" => RulesetType.Custom,
                            _ => throw new Exception()
                        };

                        Game.Main.CreateMatch(ruleset);

                    } catch
                    {
                        PromptInvalid(args[0]);
                        return;
                    }
                    break;
                case "start":
                    if (!Game.Main.IsPlaying)
                    {
                        Game.Main.StartMatch();
                        return;
                    }

                    if (Game.Main.Ruleset == null)
                    {
                        Log("No match created. Create one with /match create <ruleset>");
                    }
                    break;
                default:
                    try
                    {
                        RulesetType ruleset = args[1] switch
                        {
                            "standard" or "1" => RulesetType.Standard,
                            "speed" or "2" => RulesetType.Speed,
                            // "custom" or "3" => RulesetType.Custom,
                            _ => throw new Exception()
                        };
                        
                        Game.Main.CreateMatch(ruleset);
                    } catch
                    {
                        PromptInvalid(args[0]);
                        return;
                    }
                    break;
            }
        }
        
        void Command_Move(List<string> args)
        {
            (int col, int row) = (int.Parse(args[1]), int.Parse(args[2]));
            (int toCol, int toRow) = (int.Parse(args[3]), int.Parse(args[4]));
            
            Game.Events.CellSelect(Cellmap[new (col, row)]);
            Game.Events.CellSelect(Cellmap[new (toCol, toRow)]);
        }

        void Command_Name(List<string> args)
        {
            args.RemoveAt(0);
            Game.Console.Log("Broken command, pls fix.");
            // var name = string.Join(" ", args.ToArray());
            // // Game.Main.SetNickname(name);
            // Game.Console.Log($"Set name to \"{name}\"");
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
            
            Game.Events.CellSelect(Cellmap[new (col, row)]);
        }

        #endregion
    }
}
