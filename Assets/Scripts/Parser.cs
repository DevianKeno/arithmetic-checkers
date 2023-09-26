using System.Linq;

namespace Damath
{
    public enum Pack {Ruleset, Chat, Command, RuleType}
    public interface IParsable
    {

    }

    public class Parser
    {

        /// <summary>
        /// Pack data for network transport.
        /// </summary>
        public static string Pack(object data, Pack type)
        {
            var pack = type switch
            {
                Damath.Pack.Ruleset => "r",
                Damath.Pack.Chat => "m",
                Damath.Pack.Command => "c",
                Damath.Pack.RuleType => "t",
                _ => throw new System.Exception()
            };

            return $"{pack};{data}";
        }

        /// <summary>
        /// Parse data with pack type.
        /// </summary>
        public static Pack Parse(string data, out string[] args)
        {
            args = data.Split(";");

            Pack type = args[0] switch
            {
                "r" => Damath.Pack.Ruleset,
                "m" => Damath.Pack.Chat,
                "c" => Damath.Pack.Command,
                "t" => Damath.Pack.RuleType,
                _ => throw new System.Exception()
            };

            return type;
        }
    }
}