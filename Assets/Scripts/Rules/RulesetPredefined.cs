using System.Collections.Generic;

namespace Aritche.Rules
{
    public struct PieceValueIndex
    {
        /// <summary>
        /// Determines which value to get from the values list. 
        /// </summary>
        public int ValueIndex;
        public PieceValue PieceValue;
        public PieceData PieceData;
    }

    public partial class Ruleset
    {
        #region Predefined data

        /// Standard arrangement of symbols
        /// This is based from the original Damath game
        static Dictionary<(int, int), Operation> SymbolMapStandard => new()
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
        static Dictionary<(int, int), PieceData> PieceMapStandard => new()
        {
            /// Bottom pieces
            { (1, 0), new (PieceValueType.Integers, 11, Side.Blue, false)},
            { (3, 0), new (PieceValueType.Integers, 8, Side.Blue, false)},
            { (5, 0), new (PieceValueType.Integers, 5, Side.Blue, false)},
            { (7, 0), new (PieceValueType.Integers, 2, Side.Blue, false)},
            { (0, 1), new (PieceValueType.Integers, 0, Side.Blue, false)},
            { (2, 1), new (PieceValueType.Integers, 3, Side.Blue, false)},
            { (4, 1), new (PieceValueType.Integers, 10, Side.Blue, false)},
            { (6, 1), new (PieceValueType.Integers, 7, Side.Blue, false)},
            { (1, 2), new (PieceValueType.Integers, 9, Side.Blue, false)},
            { (3, 2), new (PieceValueType.Integers, 6, Side.Blue, false)},
            { (5, 2), new (PieceValueType.Integers, 1, Side.Blue, false)},
            { (7, 2), new (PieceValueType.Integers, 4, Side.Blue, false)},

            /// Top pieces
            { (0, 5), new (PieceValueType.Integers, 4, Side.Orange, false)},
            { (2, 5), new (PieceValueType.Integers, 1, Side.Orange, false)},
            { (4, 5), new (PieceValueType.Integers, 6, Side.Orange, false)},
            { (6, 5), new (PieceValueType.Integers, 9, Side.Orange, false)},
            { (1, 6), new (PieceValueType.Integers, 7, Side.Orange, false)},
            { (3, 6), new (PieceValueType.Integers, 10, Side.Orange, false)},
            { (5, 6), new (PieceValueType.Integers, 3, Side.Orange, false)},
            { (7, 6), new (PieceValueType.Integers, 0, Side.Orange, false)},
            { (0, 7), new (PieceValueType.Integers, 2, Side.Orange, false)},
            { (2, 7), new (PieceValueType.Integers, 5, Side.Orange, false)},
            { (4, 7), new (PieceValueType.Integers, 8, Side.Orange, false)},
            { (6, 7), new (PieceValueType.Integers, 11, Side.Orange, false)}
        };
        
        #endregion
        
        /// <summary>
        /// Create a ruleset given a ruleset type.
        /// </summary>
        /// <returns> A ruleset object. </returns>
        public static Ruleset CreateFromPreset(RulesetPreset preset)
        {
            return preset switch
            {
                RulesetPreset.Classic => CreateClassic(),
                RulesetPreset.ClassicRandom => CreateClassicRandom(),
                RulesetPreset.Speed => CreateSpeed(),
                RulesetPreset.Custom => CreateCustom(),
                _ => null,
            };
        }

        public static Ruleset CreateClassic()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.Classic,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.Standard,
                SymbolMapType = SymbolMapType.Standard,
            };

            ruleset.AddRule(0, "AllowCapture", true);
            ruleset.AddRule(1, "AllowMandatoryCapture", true);
            ruleset.AddRule(2, "AllowChainCapture", true);
            ruleset.AddRule(3, "AllowPromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", false);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableGlobalTimer", true);
            ruleset.AddRule(8, "EnableTurnTimer", true);
            ruleset.AddRule(9, "GlobalTimeSeconds", 1200f);
            ruleset.AddRule(10, "TurnTimeSeconds", 60f);
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }
        
        public static Ruleset CreateClassicRandom()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.ClassicRandom,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.Random,
                SymbolMapType = SymbolMapType.Random,
            };

            ruleset.AddRule(0, "AllowCapture", true);
            ruleset.AddRule(1, "AllowMandatoryCapture", true);
            ruleset.AddRule(2, "AllowChainCapture", true);
            ruleset.AddRule(3, "AllowPromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", false);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableGlobalTimer", true);
            ruleset.AddRule(8, "EnableTurnTimer", true);
            ruleset.AddRule(9, "GlobalTimeSeconds", 1200f);
            ruleset.AddRule(10, "TurnTimeSeconds", 60f);
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = RandomizeOperationsSymmetrical(SymbolMapStandard);
            ruleset.PieceMap = RandomizePiecesSymmetrical(PieceMapStandard);

            return ruleset;
        }

        public static Ruleset CreateSpeed()
        {
            Ruleset ruleset = new()
            {
                Preset = RulesetPreset.Speed,
                PieceValueType = PieceValueType.Integers,
                PieceMapType = PieceMapType.Standard,
                SymbolMapType = SymbolMapType.Standard,
            };

            ruleset.AddRule(0, "AllowCapture", true);
            ruleset.AddRule(1, "AllowMandatoryCapture", true);
            ruleset.AddRule(2, "AllowChainCapture", true);
            ruleset.AddRule(3, "AllowPromotion", true);
            ruleset.AddRule(4, "EnableTouchMove", false);
            ruleset.AddRule(5, "EnableScoring", true);
            ruleset.AddRule(6, "EnableTimer", true);
            ruleset.AddRule(7, "EnableGlobalTimer", true);
            ruleset.AddRule(8, "EnableTurnTimer", true);
            ruleset.AddRule(9, "GlobalTimeSeconds", 300f);
            ruleset.AddRule(10, "TurnTimeSeconds", 15f);
            ruleset.AddRule(11, "FirstTurn", Side.Blue);
            ruleset.AddRule(12, "PlayersType", RulesetPlayersType.Solo);
            ruleset.AddRule(13, "AI", "Xena");
            ruleset.AddRule(14, "EnableCheats", false);

            ruleset.SymbolMap = SymbolMapStandard;
            ruleset.PieceMap = PieceMapStandard;

            return ruleset;
        }

        public static Ruleset CreateCustom()
        {
            Ruleset ruleset = CreateClassic();
            ruleset.Preset = RulesetPreset.Custom;

            return ruleset;
        }

        public static string GetValueFromTypeAndIndex(PieceValueType valueType, int index)
        {
            var list = PieceValuesDictionary[valueType];

            if (index < 0 || index > list.Count)
            {
                return "0";
            }
            return list[index];
        }


        #region Values list

        public static Dictionary<PieceValueType, List<string>> PieceValuesDictionary => new()
        {
            {PieceValueType.Naturals, PiecesNaturals},
            {PieceValueType.Integers, PiecesIntegers},
            {PieceValueType.Rationals, PiecesRationals},
            {PieceValueType.Radicals, PiecesRadicals},
            {PieceValueType.Polynomials, PiecesPolynomials},
            {PieceValueType.Binary, PiecesBinary},
            {PieceValueType.Octal, PiecesOctal},
            {PieceValueType.Hexadecimal, PiecesHexadecimal},
        };

        public static List<string> PiecesNaturals => new()
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11",
        };

        public static List<string> PiecesIntegers => new()
        {
            "0", "-1", "2", "-3", "4", "-5", "6", "-7", "8", "-9", "10", "-11",
        };

        public static List<string> PiecesRationals => new()
        {
            "0", "-1/10", "2/10", "-3/10", "4/10", "-5/10", "6/10", "-7/10", "8/10", "-9/10", "10/10", "-11/10",
        };

        public static List<string> PiecesRadicals => new()
        {
            "-49√8", "4√18", "144√8", "-25√18", "16√32", "100√2", "-√8", "64√2", "-81√32", "-9√2", "36√32", "-121√18",
        };

        public static List<string> PiecesPolynomials => new()
        {
            "-21xy²", "6x", "78xy²", "-15x", "10y", "66x²y", "-xy²", "36x²y", "-45y", "-3x²y", "28y", "-55x",
        };
        
        public static List<string> PiecesBinary => new()
        {
            "0", "1", "10", "11", "100", "101", "110", "111", "1000", "1001", "1010", "1011",
        };

        public static List<string> PiecesHexadecimal => new()
        {
            /// literally basic lmao change?
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B",
        };

        public static List<string> PiecesOctal => new()
        {
            /// Does not reflect /// kinda stuipd ngl
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11",
        };

        #endregion
    }
}