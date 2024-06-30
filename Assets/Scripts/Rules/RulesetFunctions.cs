using System;
using System.Collections.Generic;

namespace Aritche.Rules
{
    public partial class Ruleset
    {
        static void ShuffleList<T>(Random random, List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (list[j], list[i]) = (list[i], list[j]);
            }
        }

        static Dictionary<(int, int), Operation> RandomizeOperationsSymmetrical(Dictionary<(int, int), Operation> basis)
        {
            var random = new Random();
            var symbolMap = new Dictionary<(int, int), Operation>();
            
            var operations = new List<Operation>(basis.Values);
            var coordinates = new List<(int, int)>(basis.Keys);
            
            /// Shuffle
            for (int i = operations.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (operations[j], operations[i]) = (operations[i], operations[j]);
            }
            
            for (int i = 0; i < coordinates.Count / 2; i++)
            {
                symbolMap[coordinates[i]] = operations[i];
                var rotatedCoordinate = (7 - coordinates[i].Item1, 7 - coordinates[i].Item2);
                symbolMap[rotatedCoordinate] = operations[i];
            }

            return symbolMap;
        }

        static Dictionary<(int, int), PieceData> RandomizePiecesSymmetrical(Dictionary<(int, int), PieceData> basis)
        {
            var random = new Random();
            var pieceMap = new Dictionary<(int, int), PieceData>();

            var bluePieces = new List<PieceData>();
            var orangePieces = new List<PieceData>();

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                    bluePieces.Add(kvp.Value);
                else if (kvp.Value.Side == Side.Orange)
                    orangePieces.Add(kvp.Value);
            }

            ShuffleList(random, bluePieces);
            ShuffleList(random, orangePieces);

            int blueIndex = 0;
            int orangeIndex = 0;

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                {
                    pieceMap[kvp.Key] = bluePieces[blueIndex];
                    blueIndex++;
                }
                else if (kvp.Value.Side == Side.Orange)
                {
                    pieceMap[kvp.Key] = orangePieces[orangeIndex];
                    orangeIndex++;
                }
            }

            return pieceMap;
        }

        static Dictionary<(int, int), Operation> RandomizeOperations(Dictionary<(int, int), Operation> basis)
        {
            var random = new Random();
            var symbolMap = new Dictionary<(int, int), Operation>();
            
            var operations = new List<Operation>(basis.Values);
            var coordinates = new List<(int, int)>(basis.Keys);
            
            /// Shuffle
            for (int i = operations.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (operations[j], operations[i]) = (operations[i], operations[j]);
                symbolMap[coordinates[i]] = operations[i];
            }
            
            return symbolMap;
        }
        
        static Dictionary<(int, int), PieceData> RandomizePieces(Dictionary<(int, int), PieceData> basis)
        {
            var random = new Random();
            var pieceMap = new Dictionary<(int, int), PieceData>();

            var bluePieces = new List<PieceData>();
            var orangePieces = new List<PieceData>();

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                    bluePieces.Add(kvp.Value);
                else if (kvp.Value.Side == Side.Orange)
                    orangePieces.Add(kvp.Value);
            }

            ShuffleList(random, bluePieces);
            ShuffleList(random, orangePieces);

            var allBluePositions = new List<(int, int)>();
            var allOrangePositions = new List<(int, int)>();

            foreach (var kvp in basis)
            {
                if (kvp.Value.Side == Side.Blue)
                    allBluePositions.Add(kvp.Key);
                else if (kvp.Value.Side == Side.Orange)
                    allOrangePositions.Add(kvp.Key);
            }

            ShuffleList(random, allBluePositions);
            ShuffleList(random, allOrangePositions);

            for (int i = 0; i < bluePieces.Count; i++)
            {
                pieceMap[allBluePositions[i]] = bluePieces[i];
            }

            for (int i = 0; i < orangePieces.Count; i++)
            {
                pieceMap[allOrangePositions[i]] = orangePieces[i];
            }

            return pieceMap;
        }
    }
}