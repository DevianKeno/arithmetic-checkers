using System;
using System.Collections.Generic;

using UnityEngine;

using Aritche.Rules;

namespace Aritche.Systems
{
    public partial class Console : MonoBehaviour
    {
        public Dictionary<string, Command> Commands = new();

        void InitCommands()
        {
            RegisterCommand(Command.Create(
                "auth <set> <provider>",
                "Set external auth provider.")
                .AddCallback(Command_Auth));

            RegisterCommand(Command.Create(
                "chat <message>",
                "Send a message.")
                .AddCallback(Command_Chat));

            RegisterCommand(Command.Create(
                "clear",
                "Clears console.")
                .AddCallback(Command_Clear));

            RegisterCommand(Command.Create(
                "connect <address>",
                "Connect to a match.")
                .AddCallback(Command_Connect));

            RegisterCommand(Command.Create(
                "debug <scene>",
                "Insert command description.")
                .AddCallback(Command_Debug));

            RegisterCommand(Command.Create(
                "draw",
                "Offer a draw.")
                .AddCallback());

            RegisterCommand(Command.Create(
                "flip",
                "Flips the board.")
                .AddCallback(Command_Flip));

            RegisterCommand(Command.Create(
                "forfeit",
                "Forfeit match.")
                .AddCallback());

            RegisterCommand(Command.Create(
                "help <command>",
                "Insert command description.")
                .AddCallback(Command_Help));

            RegisterCommand(Command.Create(
                "host",
                "Host current match.")
                .AddCallback(Command_Host));

            RegisterCommand(Command.Create(
                "leave",
                "Leave to title screen.")
                .AddCallback(Command_Leave));

            RegisterCommand(Command.Create(
                "lobby <create|info>",
                "Insert command description.")
                .AddCallback(Command_Lobby));

            RegisterCommand(Command.Create(
                "match <create|start> <standard|speed|custom>",
                "Insert command description.")
                .AddCallback(Command_Match));

            RegisterCommand(Command.Create(
                "move <col> <row> <toCol> <toRow>",
                "Insert command description.")
                .AddCallback(Command_Move));

            RegisterCommand(Command.Create(
                "name <name> [side]",
                "Change player name.")
                .AddCallback(Command_Name));

            RegisterCommand(Command.Create(
                "piece <add|remove> <col> <row> [value] [side] [king]",
                "Piece actions. (Cheats only)")
                .AddCallback(Command_Piece));

            RegisterCommand(Command.Create(
                "sandbox",
                "Enter sandbox.")
                .AddCallback(Command_Sandbox)
                .AddAlias("sb"));

            RegisterCommand(Command.Create(
                "select <col> <row>",
                "Select a cell.")
                .AddCallback(Command_Select));

            RegisterCommand(Command.Create(
                "start",
                "Start current match.")
                .AddCallback(Command_Start));
        }

        #region Command actions

        void Command_Auth(List<string> args)
        {
            // ExternalCredentialType auth = args[2] switch
            // {
            //     "itch" => ExternalCredentialType.I
            // };

            
        }

        void Command_Chat(List<string> args)
        {
            /// Removes "chat" argument
            args.RemoveAt(0);
            string message = string.Join(" ", args.ToArray());
            string data = $"{Game.Main.Nickname};{message}";

            /// Invoke console command as player
            if (true)
            {
                // Game.Events.NetworkSend(Parser.Pack(data, Pack.Chat));
            } else
            {
                /// Send chat locally
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
                
                // Network.Main.Connect(args[1]);

            } else
            {
                // Network.Main.Connect("localhost");
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
                RulesetPreset ruleset = args[1] switch
                {
                    "classic" or "1" => RulesetPreset.Classic,
                    "speed" or "2" => RulesetPreset.Speed,
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
            Game.Main.LoadScene("Title", playTransition: true);
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
                        RulesetPreset ruleset = args[2] switch
                        {
                            "classic" or "1"
                                => RulesetPreset.Classic,
                            "classicrandom" or "cr" or "2"
                                => RulesetPreset.ClassicRandom,
                            "speed" or "3"
                                => RulesetPreset.Speed,
                            "100" or "full" or "4"
                                => RulesetPreset.HundredPercent,
                            "custom" or "0"
                                => RulesetPreset.Custom,
                            _
                                => throw new Exception()
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
                        RulesetPreset ruleset = args[1] switch
                        {
                            "classic" or "1" => RulesetPreset.Classic,
                            "speed" or "2" => RulesetPreset.Speed,
                            // "custom" or "3" => RulesetPreset.Custom,
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
            int col = int.Parse(args[2]);
            int row = int.Parse(args[3]);

            if (args[1] == "add")
            {
                var data = new PieceData
                {
                    Value = args[4].ToString(),
                    Side = args[5] == "blue" ? Side.Blue : Side.Orange,
                    IsKing = bool.Parse(args[6])
                };
                Match.Board.AddPiece(col, row, data);

            } else if (args[1] == "remove")
            {
                Match.Board.RemovePiece(col, row);

            } else if (args[1] == "get")
            {
                Log(Match);
            }
        }

        void Command_Sandbox(List<string> args)
        {
            var ruleset = Ruleset.CreateFromPreset(RulesetPreset.Classic);
            ruleset.SetValue("EnableCheats", true);
            Game.Main.CreateMatch(ruleset);
            Game.Main.LoadScene("Match", playTransition: true);
        }

        void Command_Select(List<string> args)
        {
            (int col, int row) = (int.Parse(args[1]), int.Parse(args[2]));
            
            Game.Events.CellSelect(Cellmap[new (col, row)]);
        }

        void Command_Start(List<string> arg0)
        {
            if (Game.Main.Ruleset == null)
            {
                Log("No match created. Create one with /match create <ruleset>");
                return;
            }
            
            if (!Game.Main.Match.IsPlaying)
            {
                Game.Main.StartMatch();
                return;
            }
        }

        #endregion
    }
}