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
        public List<(string, ArgType)> Arguments = new();
        public List<string> Parameters = new();
        public List<string> Aliases = new();
        public UnityAction<List<string>> Calls;

        public static Command Create(string command, string description = "")
        {
            var args = command.Split(" ");
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
                    newCommand.Arguments.Add((substring, ArgType.Required));
                } else if (arg.Contains("["))
                {
                    // Optional
                    string substring = arg[1..^1];
                    newCommand.Arguments.Add((substring, ArgType.Optional));
                }
                i++;
            }
            return newCommand;           
        }

        public Command AddParameters(List<string> Parameters)
        {
            return this;
        }

        public Command AddAlias(string alias)
        {
            Aliases.Add(alias);
            return this;
        }

        public Command AddAlias(string[] alias)
        {
            Aliases.AddRange(alias);
            return this;
        }
        
        public Command AddCallback(UnityAction<List<string>> func = null)
        {
            if (func != null)
            {
                Calls = func;
            }
            return this;
        }

        public Command SetDescription(string value)
        {
            Description = value;
            return this;
        }

        public void Invoke(List<string> args)
        {
            Calls(args);
        }
    }
}
