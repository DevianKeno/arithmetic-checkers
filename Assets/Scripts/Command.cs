using System.Collections.Generic;
using UnityEngine.Events;

namespace Damath
{
    public class Command
    {
        public enum ArgType {Required, Optional}
        public string Name = "";
        public string Syntax = "";
        public string Description = "";
        public List<(string, ArgType)> Arguments;
        public List<string> Parameters;
        public List<string> Aliases;
        public UnityAction<List<string>> Calls;

        public static Command Create(string command, string description = "")
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
            return newCommand;           
        }

        public void AddParameters(List<string> Parameters)
        {
        }

        public void AddAlias(string alias)
        {
            Aliases.Add(alias);
        }
        
        public void AddCallback(UnityAction<List<string>> func = null)
        {
            if (func != null)
            {
                Calls = func;
            }
        }

        public void SetDescription(string value)
        {
            Description = value;
        }

        public void Invoke(List<string> args)
        {
            Calls(args);

            // foreach (var call in Calls)
            // {
            //     call.Invoke();
            // }
        }
    }
}
