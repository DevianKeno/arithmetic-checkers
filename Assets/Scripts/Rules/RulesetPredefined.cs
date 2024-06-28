using System.Collections.Generic;

namespace Aritche.Rules
{
    public partial class Ruleset
    {
        /// <summary>
        /// These data is initialized from files at every start of the program (game).
        /// </summary>
        #region Predefined data

        static Dictionary<(int, int), Operation> SymbolMapStandard { get; set; }
        static Dictionary<(int, int), PieceData> PieceMapStandard { get; set; }
        
        #endregion

        /// <summary>
        /// Initializing function. This runs when the game is first being loaded.
        /// </summary>
        public static void CreatePredefined()
        {
            Game.Console.LogDebug("Initializing rulesets");
            
            /// These should read from a file that's easier to read and handle
            /// Idk maybe this is better(?)
            /// If in file it can be changed outside code
            /// But here it's more secure idk tho
            /// Maybe if have time implement read in file as well?
            // foreach (var entry in data)
            // {
            //     dict.Add(entry.cell, entry.data);
            // }

            /// Standard arrangement of symbols
            /// This is based from the original Damath game
            SymbolMapStandard = new()
            {
                { (1, 0), Operation.Addition},
                { (3, 0), Operation.Subtraction},
                { (5, 0), Operation.Division},
                { (7, 0), Operation.Multiplication},
                { (0, 1), Operation.Subtraction},
                { (2, 1), Operation.Addition},
                { (4, 1), Operation.Multiplication},
                { (6, 1), Operation.Division},
                { (1, 2), Operation.Division},
                { (3, 2), Operation.Multiplication},
                { (5, 2), Operation.Addition},
                { (7, 2), Operation.Subtraction},
                { (0, 3), Operation.Multiplication},
                { (2, 3), Operation.Division},
                { (4, 3), Operation.Subtraction},
                { (6, 3), Operation.Addition},
                { (1, 4), Operation.Addition},
                { (3, 4), Operation.Subtraction},
                { (5, 4), Operation.Division},
                { (7, 4), Operation.Multiplication},
                { (0, 5), Operation.Subtraction},
                { (2, 5), Operation.Addition},
                { (4, 5), Operation.Multiplication},
                { (6, 5), Operation.Division},
                { (1, 6), Operation.Division},
                { (3, 6), Operation.Multiplication},
                { (5, 6), Operation.Addition},
                { (7, 6), Operation.Subtraction},
                { (0, 7), Operation.Multiplication},
                { (2, 7), Operation.Division},
                { (4, 7), Operation.Subtraction},
                { (6, 7), Operation.Addition}
            };

            /// Standard arrangement of pieces
            /// This is based from the original Damath game
            PieceMapStandard = new()
            {
                /// Bottom pieces
                { (1, 0), new ("-11", Side.Blue, false)},
                { (3, 0), new ("8", Side.Blue, false)},
                { (5, 0), new ("-5", Side.Blue,false)},
                { (7, 0), new ("2", Side.Blue, false)},
                { (0, 1), new ("0", Side.Blue, false)},
                { (2, 1), new ("-3", Side.Blue, false)},
                { (4, 1), new ("10", Side.Blue, false)},
                { (6, 1), new ("-7", Side.Blue, false)},
                { (1, 2), new ("-9", Side.Blue, false)},
                { (3, 2), new ("6", Side.Blue, false)},
                { (5, 2), new ("-1", Side.Blue, false)},
                { (7, 2), new ("4", Side.Blue, false)},

                /// Top pieces
                { (0, 5), new ("4", Side.Orange, false)},
                { (2, 5), new ("-1", Side.Orange, false)},
                { (4, 5), new ("6", Side.Orange, false)},
                { (6, 5), new ("-9", Side.Orange, false)},
                { (1, 6), new ("-7", Side.Orange, false)},
                { (3, 6), new ("10", Side.Orange, false)},
                { (5, 6), new ("-3", Side.Orange, false)},
                { (7, 6), new ("0", Side.Orange, false)},
                { (0, 7), new ("2", Side.Orange, false)},
                { (2, 7), new ("-5", Side.Orange, false)},
                { (4, 7), new ("8", Side.Orange, false)},
                { (6, 7), new ("-11", Side.Orange, false)}
            };

            Game.Console.LogDebug("Initialized rulesets");
        }
    }
}