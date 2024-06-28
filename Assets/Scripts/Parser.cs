using System.Linq;

namespace Aritche
{
    public enum Pack {Ruleset, Chat, Command}
    public interface IParsable
    {

    }

    public class Parser
    {

        /// <summary>
        /// Pack data for network transport.
        /// </summary>
        public static string Pack(string data, Pack type)
        {
            var pack = type switch
            {
                Aritche.Pack.Ruleset => "r",
                Aritche.Pack.Chat => "m",
                Aritche.Pack.Command => "c",
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
                "r" => Aritche.Pack.Ruleset,
                "m" => Aritche.Pack.Chat,
                "c" => Aritche.Pack.Command,
                _ => throw new System.Exception()
            };

            return type;
        }
    }
}