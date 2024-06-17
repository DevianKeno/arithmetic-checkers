using System;
using System.Collections.Generic;
using UnityEngine;

namespace Damath.Systems
{
    public partial class Console : MonoBehaviour
    {
        public Dictionary<string, Command> Commands = new();

        void InitCommands()
        {
            CreateCommand("chat <message>",
                          "Send a message.")
                          .AddCallback(Command_Chat);

            CreateCommand("clear",
                          "Clears console.")
                          .AddCallback(Command_Clear);

            CreateCommand("connect <address>",
                          "Connect to a match.")
                          .AddCallback(Command_Connect);

            CreateCommand("debug <scene>")
                          .AddCallback(Command_Debug);

            CreateCommand("draw",
                          "Offer a draw.")
                          .AddCallback();

            CreateCommand("flip",
                          "Flips the board.")
                          .AddCallback(Command_Flip);
                        
            CreateCommand("forfeit",
                          "Forfeit match.")
                          .AddCallback();

            CreateCommand("help <command>")
                          .AddCallback(Command_Help);

            CreateCommand("host",
                          "Host current match.")
                          .AddCallback(Command_Host);
                          
            CreateCommand("leave",
                          "Leave to title screen.")
                          .AddCallback(Command_Leave);

            CreateCommand("lobby <create|info>")
                          .AddCallback(Command_Lobby);

            CreateCommand("match <create|start> <standard|speed|custom>")
                          .AddCallback(Command_Match);
            
            CreateCommand("move <col> <row> <toCol> <toRow>",
                          "")
                          .AddCallback(Command_Move);

            CreateCommand("name <name> [side]",
                          "Change player name.")
                          .AddCallback(Command_Name);

            CreateCommand("piece <s|s> <col> <row> [value]",
                          "Piece actions.")
                          .AddCallback(Command_Piece);

            CreateCommand("sandbox",
                          "Enter sandbox.")
                          .AddCallback(Command_Sandbox);

            CreateCommand("select <col> <row>",
                          "Select a cell.")
                          .AddCallback(Command_Select);
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
            _UI.Refresh();
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
                /// Run help command
                _UI.WriteLine("List of available commands: ");
                string message = "";
                foreach (var c in Commands)
                {
                    /// This will have an extra comma, pls fix
                    message += $"{c.Value.Name}, ";
                }
                _UI.Write(message);
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
                        Game.Main.LoadScene("Match", playTransition: true);

                    } catch
                    {
                        PromptInvalid(args[0]);
                        return;
                    }
                    break;
                    
                case "start":
                    if (Game.Main.Ruleset == null)
                    {
                        Log("No match created. Create one with /match create <ruleset>");
                    }
                    
                    if (!Game.Main.Match.IsPlaying)
                    {
                        Game.Main.StartMatch();
                        return;
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
                        Game.Main.LoadScene("Match", playTransition: true);
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

        void Command_Sandbox(List<string> args)
        {
            var ruleset = Ruleset.Create(RulesetType.Standard);
            if (ruleset.TryGetRule("EnableCheats", out Rule<bool> rule))
            {
                rule.SetValue(true);
            }

            Game.Main.CreateMatch(ruleset);
            Game.Main.LoadScene("Match", playTransition: true);
        }

        void Command_Select(List<string> args)
        {
            (int col, int row) = (int.Parse(args[1]), int.Parse(args[2]));
            
            Game.Events.CellSelect(Cellmap[new (col, row)]);
        }

        #endregion
    }
}