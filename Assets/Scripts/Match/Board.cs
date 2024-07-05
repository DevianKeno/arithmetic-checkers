using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Aritche.Players;
using Aritche.Rules;
using Aritche.UI;

namespace Aritche
{
    public class Board : MonoBehaviour
    {
        [Header("Board Game.Main.Settings")]
        public const int MaxColumns = 8;
        public const int MaxRows = 8;

        Ruleset Ruleset { get; set; }
        public Themes Theme;

        public List<Piece> LiveBluePieces = new();
        public List<Piece> LiveOrangePieces = new();
        public List<Piece> CapturedBluePieces = new();
        public List<Piece> CapturedOrangePieces = new();

        List<Cell> _cells = new();
        List<Piece> _pieces = new();
        List<MovePerformInfo> _validMoves = new();
        Dictionary<Vector2, Cell> _cellCoordinates = new();
        public bool IsFlipped { get; private set; }

        #region Events
        public event EventHandler<MovePerformInfo> OnMovePerform;
        public event EventHandler<List<MovePerformInfo>> OnUpdateMoves;
        public event EventHandler<List<MovePerformInfo>> OnUpdateValidMoves;
        public event EventHandler<List<MovePerformInfo>> OnUpdateCaptureMoves;

        #endregion

        bool SymbolVisible;
        bool PieceVisible;


        [Header("Objects")]
        [SerializeField] GameObject cellGroup;
        [SerializeField] GameObject pieceGroup;
        [SerializeField] GameObject coordinates;
        [SerializeField] GameObject graveyardB;
        [SerializeField] GameObject graveyardT;
        [SerializeField] GameObject boardPrefab;
        
        [Header("Prefabs")]
        [SerializeField] Cell cellPrefab;
        [SerializeField] Piece piecePrefab;

        RectTransform rectTransform;

        void Awake()
        {
            

            Theme = transform.Find("Theme").GetComponent<Themes>();

            
        }


        #region Initializing methods
        
        internal void SetRuleset(Ruleset rules)
        {            
            if (Game.Main.Settings.EnableDebugMode) Game.Console.Log("[Debug: Board]: Received ruleset");
            Ruleset = rules;
        }

        internal void Initialize()
        {
            if (Ruleset == null)
            {
                Game.Console.Log("[Debug: Board]: Failed to initialize board, no ruleset");
                return;
            }

            if (Game.Main.Settings.EnableDebugMode) Game.Console.Log("[Debug: Board]: Initializing Board");

            GenerateCells();
            GeneratePieces();
            ToggleSymbolVisibility(false);
            TogglePieceVisibility(false);

            if (Game.Main.Settings.EnableDebugMode) Game.Console.Log("[Debug: Board]: Initialized Board");
        }

        /// <summary>
        /// Generate default board cells.
        /// </summary>
        void GenerateCells()
        {
            for (int row = 0; row < MaxRows; row++)
            {
                for (int col = 0; col < MaxColumns; col++)
                {
                    Vector2Int coords = new (col, row);

                    Cell newCell = Instantiate(cellPrefab, cellGroup.transform).GetComponent<Cell>();
                    RectTransform rect = newCell.GetComponent<RectTransform>();
                    newCell.name = $"Cell ({col}, {row})";
                    
                    // Transform cell position
                    float positionX = col * Constants.CellSize + Constants.CellOffset;
                    float positionY = row * Constants.CellSize + Constants.CellOffset;
                    rect.anchoredPosition3D = new (positionX - 0.25f,
                                                   positionY - 0.25f,
                                                   0); // Idk why, but I didn't have to subtract .25 from this before
                    rect.localScale = new (Constants.CellSize, Constants.CellSize);
                    
                    // Cell initialization
                    newCell.SetColRow(col, row);

                    if (Ruleset.SymbolMap.ContainsKey((coords.x, coords.y)))
                    {
                        newCell.SetOperation(Ruleset.SymbolMap[(coords.x, coords.y)]);
                    }

                    _cells.Add(newCell);
                    _cellCoordinates[coords] = newCell;
                }
            }

            Game.Events.BoardUpdateCellmap(Cellmap<Cell>.ToCellmap(_cellCoordinates));
        }

        /// <summary>
        /// Generates the pieces and assigns it to its respective cell.
        /// </summary>
        void GeneratePieces()
        {
            foreach (var entry in Ruleset.PieceMap)
            {
                Cell cell = GetCellFromCoordinates(entry.Key.Item1, entry.Key.Item2);    
                PieceData pieceData = entry.Value;

                var newPiece = CreatePiece(pieceData);
                cell.SetPiece(newPiece);

                if (pieceData.Side == Side.Blue)
                {
                    LiveBluePieces.Add(newPiece);
                }
                else if (pieceData.Side == Side.Orange)
                {
                    LiveOrangePieces.Add(newPiece);
                }

                _pieces.Add(newPiece);
            }
        }

        Cell InstantiateCellPrefab()
        {
            var go = Instantiate(cellPrefab, cellGroup.transform);
            return go.GetComponent<Cell>();
        }

        Piece InstantiatePiecePrefab()
        {
            var go = Instantiate(piecePrefab, pieceGroup.transform);
            return go.GetComponent<Piece>();
        }

        #endregion


        public Piece CreatePiece(PieceData data)
        {
            var newPiece = InstantiatePiecePrefab();
            newPiece.SetData(data);
            return newPiece;
        }

        public void BeginMatch()
        {
            // Reset();
            ToggleSymbolVisibility(true);
            TogglePieceVisibility(true);
        }


        #region Board visibility methods
        /// <summary>
        /// Flips the board 180 degrees.
        /// </summary>
        public void Flip()
        {
            if (IsFlipped)
            {
                Game.Console.Log("Unflipping board");
                cellGroup.transform.Rotate(0f, 0f, -180f);
                foreach (Transform c in cellGroup.transform)
                {
                    c.transform.Rotate(0f, 0f, -180f);
                }
                pieceGroup.transform.Rotate(0f, 0f, -180f);
                foreach (Transform c in pieceGroup.transform)
                {
                    c.transform.Rotate(0f, 0f, -180f);
                }
            }
            else
            {
                Game.Console.Log("Flipping board");
                cellGroup.transform.Rotate(0f, 0f, 180f);
                foreach (Transform c in cellGroup.transform)
                {
                    c.transform.Rotate(0f, 0f, 180f);
                }
                pieceGroup.transform.Rotate(0f, 0f, 180f);
                foreach (Transform c in pieceGroup.transform)
                {
                    c.transform.Rotate(0f, 0f, 180f);
                }
            }
            IsFlipped = !IsFlipped;
            Game.Console.Log("Flipped board");
        }

        public void ToggleSymbolVisibility(bool visibility)
        {
            SymbolVisible = visibility;
            
            if (visibility)
            {
                foreach (Transform c in cellGroup.transform)
                {
                    c.gameObject.SetActive(true);
                }
            } else
            {
                foreach (Transform c in cellGroup.transform)
                {
                    c.gameObject.SetActive(false);
                }
            }
        }

        public void TogglePieceVisibility(bool visibility)
        {
            PieceVisible = visibility;
            
            if (visibility)
            {
                foreach (Transform c in pieceGroup.transform)
                {
                    c.gameObject.SetActive(true);
                }
            } else
            {
                foreach (Transform c in pieceGroup.transform)
                {
                    c.gameObject.SetActive(false);
                }
            }
        }
        
        #endregion

        /// <summary>
        /// 
        /// </summary>
        internal void UpdateValidMoves(List<MovePerformInfo> moves)
        {
            _validMoves = moves;
            foreach (var move in _validMoves)
            {
                move.Destination.IsValidMove = true;
            }
        }

        /// <summary>
        /// Clears out all cached valid moves.
        /// </summary>
        internal void ClearValidMoves()
        {
            foreach (var move in _validMoves)
            {
                move.Destination.IsValidMove = false;
            }
            _validMoves.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SelectPiece(Piece piece, bool isCapture = false)
        {        
            if (isCapture)
            {
                /// I don't know why I'm checking this honestly I just thought about it
                /// It's important though
                if (!piece.CanCapture && !piece.HasCaptured) return;
                
                var moves = GetPieceMoves(piece, MoveType.Capture);
                if (moves.Count > 0)
                {
                    UpdateValidMoves(moves);
                    OnUpdateCaptureMoves?.Invoke(this, _validMoves);
                }
                else
                {
                    DonePiece(piece);
                }
            }
            else
            {
                var moves = GetPieceMoves(piece);
                UpdateValidMoves(moves);
            }
            
            OnUpdateValidMoves?.Invoke(this, _validMoves);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SelectCell(Cell cell, Player player)
        {
            if (IsCellInValidMoves(cell, out var move))
            {
                PerformMove(move);
            }
        }

        bool IsCellInValidMoves(Cell cell, out MovePerformInfo info)
        {
            info = null;
            foreach (MovePerformInfo move in _validMoves)
            {
                if (cell == move.Destination)
                {
                    info = move;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Perform a piece move given a Move object.
        /// </summary>
        public void PerformMove(MovePerformInfo move)
        {
            AnimateMove(move);
            Game.Audio.PlaySound("move");

            if (move.HasCapture)
            {
                PerformCapture(move);
            }
            else
            {
                DonePiece(move.CapturingPiece);
            }
            OnMovePerform?.Invoke(this, move);
            Game.Console.Log($"[ACTION]: Moved {move.CapturingPiece.ValueAsString}: ({move.Origin.Col}, {move.Origin.Row}) -> ({move.Destination.Col}, {move.Destination.Row})");
        }
        
        /// <summary>
        /// Perform a piece capture given a move.
        /// </summary>
        public void PerformCapture(MovePerformInfo move)
        {
            Game.Audio.PlaySound("capture");
            AnimateCapture(move);

            var pieces = move.CapturedPiece.Side switch
            {
                Side.Blue => LiveBluePieces,
                Side.Orange => LiveOrangePieces
            };
            var piecesCaptured = move.CapturedPiece.Side switch
            {
                Side.Blue => CapturedBluePieces,
                Side.Orange => CapturedOrangePieces
            };
            var result = pieces.Find((Piece piece) =>
            {
                return piece == move.CapturedPiece;
            });
            if (result != null)
            {
                var temp = result;
                pieces.Remove(result);
                piecesCaptured.Add(temp);
            }

            GetCellFromPiece(move.CapturedPiece).ClearPiece();
            move.Player.CapturedPieces.Add(move.CapturedPiece);
            move.CapturingPiece.HasCaptured = true;
            move.CapturedPiece.Owner.RemoveFromPieces(move.CapturedPiece);
            
            Game.Console.Log($"[ACTION]: {move.CapturingPiece} captured {move.CapturedPiece}");
        }

        /// <summary>
        /// Marks a piece "done" to end piece actions.
        /// </summary>
        public void DonePiece(Piece piece)
        {
            piece.FinishActions();
        }

        /// <summary>
        /// Animates the moved piece in the scene.
        /// </summary>
        void AnimateMove(MovePerformInfo move)
        {
            move.MovePieces();
            move.CapturingPiece.ResetPositionToCell(animate: Game.Main.Settings.EnableAnimations);
        }

        /// <summary>
        /// Animates the capture.
        /// </summary>
        void AnimateCapture(MovePerformInfo move)
        {
            /// TODO: Checks on the MovePerformInfo class if it contains null values
            Piece capturedPiece = move.CapturedPiece;
            RectTransform pieceRect = capturedPiece.GetComponent<RectTransform>();

            if (capturedPiece.Side == Side.Blue) /// Blue player captured
            {
                capturedPiece.gameObject.transform.SetParent(graveyardT.transform);
                pieceRect.anchorMin = new (1f, 0.5f);
                pieceRect.anchorMax = new (1f, 1f);
                pieceRect.localScale = new (1.5f, 1.5f);

                if (Game.Main.Settings.EnableAnimations)
                {
                    if (graveyardB.transform.childCount > 0)
                    {
                        MoveGraveyardChildren(graveyardT);
                    }

                    LeanTween.move(capturedPiece.gameObject, graveyardT.transform.position, 0.5f)
                    .setEase(LeanTweenType.easeOutExpo);
                }
            }
            else if (capturedPiece.Side == Side.Orange)
            {
                capturedPiece.gameObject.transform.SetParent(graveyardB.transform);
                pieceRect.anchorMin = new (0.5f, 0f);
                pieceRect.anchorMax = new (0.5f, 0f);                
                pieceRect.localScale = new (1.5f, 1.5f);

                if (graveyardB.transform.childCount > 0)
                {
                    MoveGraveyardChildren(graveyardB);
                }

                if (Game.Main.Settings.EnableAnimations)
                {
                    LeanTween.move(capturedPiece.gameObject, graveyardB.transform.position, 0.5f)
                    .setEase(LeanTweenType.easeOutExpo);
                }
                else
                {
                    capturedPiece.gameObject.transform.position = graveyardB.transform.position;
                }
            }
        }

        void MoveGraveyardChildren(GameObject graveyard)
        {
            for (int i = 0; i < graveyard.transform.childCount; i++)
            {
                GameObject child = graveyard.transform.GetChild(i).gameObject;
                float childPositionX = child.transform.position.x;

                Vector3 targetPosition = new(
                    childPositionX -= 0.5f,
                    child.transform.position.y,
                    child.transform.position.z
                );

                if (Game.Main.Settings.EnableAnimations)
                {
                    LeanTween.move(child, targetPosition, 0.5f).setEaseOutExpo();
                }
                else
                {
                    child.transform.position = targetPosition;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public bool CheckCapturesAll(Side side)
        {
            var moves = new List<MovePerformInfo>();
            var pieces = side switch
            {
                Side.Blue => LiveBluePieces,
                Side.Orange => LiveOrangePieces
            };
            
            foreach (Piece piece in pieces)
            {
                var pieceCaptureMoves = GetPieceMoves(piece, MoveType.Capture);
                moves.AddRange(pieceCaptureMoves);
            }

            UpdateValidMoves(moves);
            OnUpdateCaptureMoves?.Invoke(this, moves);
            return moves.Count > 0;
        }

        /// <summary>
        /// Check if this piece is captureable.
        /// This checks the 4 surrounding diagonal cells (SE, SW, NE, NW).
        /// </summary>
        /// <returns> All moves with captures. </returns>
        public bool CheckIfCaptureable(Piece piece, out List<MovePerformInfo> moves, bool setAsValid = false)
        {
            moves = new();
            
            for (int col = piece.Col - 1 ;  col < piece.Col + 2 ; col += 2)
            {
                if (col < 0 || col > 7) continue;

                for (int row = piece.Row - 1 ;  row < piece.Row + 2 ; row += 2)
                {
                    if (row < 0 || row > 7) continue;

                    Cell cellToCheck = GetCellFromCoordinates(col, row);

                    if (cellToCheck.HasPiece)
                    {
                        if (piece.IsAllyOf(cellToCheck.Piece)) continue;
                        
                        moves.AddRange(GetPieceMoves(cellToCheck.Piece, MoveType.Capture));
                    }
                }
            }

            if (moves.Count > 0 && setAsValid)
            {
                UpdateValidMoves(moves);
                OnUpdateCaptureMoves?.Invoke(this, moves);
            }
            return moves.Count > 0;
        }
        
        /// <summary>
        /// Same as the original method, but recursive.
        /// This is used for checking if there are king pieces that can capture this piece,
        /// </summary>
        public bool CheckIfCaptureableMax(Piece piece, out List<MovePerformInfo> moves, bool setAsValid = false)
        {
            moves = new List<MovePerformInfo>();
            int up = 1;
            int down = -1;
            int above = piece.Row + 1;
            int below = piece.Row - 1;

            /// Above
            moves.AddRange(CheckLeftCapturer(piece, above, up));
            moves.AddRange(CheckRightCapturer(piece, above, up));
            /// Below
            moves.AddRange(CheckLeftCapturer(piece, below, down));
            moves.AddRange(CheckRightCapturer(piece, below, down));
            
            if (moves.Count > 0 && setAsValid)
            {
                UpdateValidMoves(moves);
                OnUpdateCaptureMoves?.Invoke(this, moves);
            }
            return moves.Count > 0;
        }

        List<MovePerformInfo> CheckLeftCapturer(Piece piece, int startingRow, int yDirection)
        {
            var moves = new List<MovePerformInfo>();
            int left = piece.Col - 1;
            int distance = 0;

            for (int row = startingRow ; row < MaxRows ; row += yDirection)
            {
                if (left < 0 || left > 7) break;    /// Out of bounds
                if (row < 0 || row > 7) break;      /// Out of bounds

                Cell cellToCheck = GetCellFromCoordinates(left, row);

                if (cellToCheck.IsEmpty) continue;
                
                if (distance > 1)
                {
                    /// The piece can't reach our piece
                    if (cellToCheck.Piece.IsPawn) break;
                }

                if (cellToCheck.Piece.IsEnemyOf(piece))
                {
                    var pieceMoves = GetPieceMoves(cellToCheck.Piece, MoveType.Capture);

                    if (pieceMoves.Count > 0 && piece.IsCapturedInMoves(pieceMoves, out var captureMoves))
                    {
                        moves.AddRange(captureMoves);
                    }
                }
                else /// allied piece
                {
                    break;
                }

                left -= 1;  /// Move selector diagonally
                distance++;
            }

            return moves;
        }

        List<MovePerformInfo> CheckRightCapturer(Piece piece, int startingRow, int yDirection)
        {
            var moves = new List<MovePerformInfo>();
            int right = piece.Col + 1;
            int distance = 0;

            for (int row = startingRow ; row < MaxRows ; row += yDirection)
            {
                if (right < 0 || right > 7) break;  /// Out of bounds
                if (row < 0 || row > 7) break;      /// Out of bounds

                Cell cellToCheck = GetCellFromCoordinates(right, row);

                if (cellToCheck.IsEmpty) continue;
                
                if (distance > 1)
                {
                    if (cellToCheck.Piece.IsPawn) continue;
                }

                if (cellToCheck.Piece.IsEnemyOf(piece))
                {
                    var pieceMoves = GetPieceMoves(cellToCheck.Piece, MoveType.Capture);
                    
                    if (pieceMoves.Count > 0 && piece.IsCapturedInMoves(pieceMoves, out var captureMoves))
                    {
                        moves.AddRange(captureMoves);
                    }
                }
                else /// allied piece
                {
                    break;
                }

                right += 1;  /// Move selector diagonally
                distance++;
            }

            return moves;
        }

        public void CheckForKing(Piece piece)
        {
            if (piece.Side == Side.Blue)
            {
                if (piece.Row == 7) piece.Promote();
            }
            else
            {
                if (piece.Row == 0) piece.Promote();
            }
        }

        /// <summary>
        /// Returns the valid moves of the piece.
        /// </summary>
        public List<MovePerformInfo> GetPieceMoves(Piece piece, MoveType moveType = default)
        {
            List<MovePerformInfo> moves = new();
            int up = 1;
            int down = -1;
            int above = piece.Row + 1;
            int below = piece.Row - 1;

            if (piece.Side == Side.Blue)
            {
                /// Forward check
                moves.AddRange(CheckLeft(piece, above, up, moveType));
                moves.AddRange(CheckRight(piece, above, up, moveType));
                /// Backward check
                moves.AddRange(CheckLeft(piece, below, down, moveType));
                moves.AddRange(CheckRight(piece, below, down, moveType));
            } else if (piece.Side == Side.Orange)
            {
                /// Forward check
                moves.AddRange(CheckLeft(piece, below, down, moveType));
                moves.AddRange(CheckRight(piece, below, down, moveType));
                /// Backward check
                moves.AddRange(CheckLeft(piece, above, up, moveType));
                moves.AddRange(CheckRight(piece, above, up, moveType));
            }

            piece.CanCapture = moves.Count > 0;
            return moves;
        }

        /// <summary>
        /// 
        /// </summary>
        List<MovePerformInfo> CheckLeft(Piece piece, int startingRow, int yDirection, MoveType moveType)
        {
            List<MovePerformInfo> moves = new();
            List<MovePerformInfo> captureMoves = new();
            Cell cellToCapture = null;
            int nextEnemyPiece = 0;
            int left = piece.Col - 1;

            for (int row = startingRow ; row < MaxRows ; row += yDirection)
            {
                if (left < 0 || left > 7) break;    /// Out of bounds
                if (row < 0 || row > 7) break;      /// Out of bounds
                if (nextEnemyPiece > 1) break;      /// Two successive enemy pieces

                Cell cellToCheck = GetCellFromCoordinates(left, row);

                if (!cellToCheck.HasPiece) /// Next cell is empty cell
                {
                    if (cellToCapture != null) /// There's a captureable cell
                    {
                        var newMove = new MovePerformInfo(
                            piece: piece,
                            toCapture: cellToCapture.Piece,
                            origin: GetCellFromCoordinates(piece.Col, piece.Row),
                            destination: cellToCheck
                        );
                        
                        captureMoves.Add(newMove);
                        if (piece.IsKing) moves.Clear();
                    }
                    else
                    {
                        if (piece.Forward != yDirection)
                        {
                            if (!piece.IsKing) break;
                        }

                        var newMove = new MovePerformInfo(
                            piece: piece,
                            origin: GetCellFromCoordinates(piece.Col, piece.Row),
                            destination: cellToCheck
                        );
                        moves.Add(newMove);
                    }

                    if (!piece.IsKing) break;

                }
                else if (cellToCheck.Piece.Side == piece.Side) /// Next cell has allied piece
                {
                    break;
                }
                else /// Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                left -= 1;  /// Move selector diagonally
            }

            return moveType switch
            {
                MoveType.All => moves.Concat(captureMoves).ToList(),
                MoveType.Move => moves,
                MoveType.Capture => captureMoves,
                _ => null,
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        List<MovePerformInfo> CheckRight(Piece piece, int startingRow, int yDirection, MoveType moveType)
        {
            List<MovePerformInfo> moves = new();
            List<MovePerformInfo> captureMoves = new();
            int nextEnemyPiece = 0;
            Cell cellToCapture = null;
            int right = piece.Col + 1;

            for (int row = startingRow; row < MaxRows ; row += yDirection)
            {
                if (right < 0 || right > 7) break;      /// Out of bounds
                if (row < 0 || row > 7) break;          /// Out of bounds
                if (nextEnemyPiece > 1) break;          /// Two successive enemy pieces

                Cell cellToCheck = GetCellFromCoordinates(right, row);

                if (!cellToCheck.HasPiece) /// Next cell is empty cell
                {
                    /// There's a piece waiting to be captured
                    /// This is a captureable cell (?)
                    if (cellToCapture != null)  
                    {
                        var newMove = new MovePerformInfo(
                            piece: piece,
                            toCapture: cellToCapture.Piece,
                            origin: GetCellFromCoordinates(piece.Col, piece.Row),
                            destination: cellToCheck
                        );
                        captureMoves.Add(newMove);
                        if (piece.IsKing) moves.Clear();
                    }
                    else
                    {
                        if (piece.Forward != yDirection)
                        {
                            /// Now checking reverse direction (backward moves)
                            /// Only king pieces are allowed to move freely backwards
                            if (!piece.IsKing) break;
                        }

                        var newMove = new MovePerformInfo(
                            piece: piece,
                            origin: GetCellFromCoordinates(piece.Col, piece.Row),
                            destination: cellToCheck
                        );
                        moves.Add(newMove);
                    }

                    if (!piece.IsKing) break;

                }
                else if (cellToCheck.Piece.Side == piece.Side)    /// Next cell has allied piece
                {
                    break;
                }
                else  /// Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                right += 1;  /// Move selector diagonally
            }

            return moveType switch
            {
                MoveType.All => moves.Concat(captureMoves).ToList(),
                MoveType.Move => moves,
                MoveType.Capture => captureMoves,
                _ => null,
            };
        }

        public Piece AddPiece(int col, int row, PieceData pieceData)
        {
            return AddPiece(GetCellFromCoordinates(col, row), pieceData);
        }

        public Piece AddPiece(Cell cell, PieceData pieceData)
        {
            if (cell.HasPiece)
            {
                Game.Console.Log("Can't add piece to a cell that already has one.");
                return null;
            }

            Piece newPiece = CreatePiece(pieceData);
            cell.SetPiece(newPiece);

            return newPiece;
        }

        public void RemovePiece(int col, int row)
        {
            RemovePiece(GetCellFromCoordinates(col, row));
        }

        public void RemovePiece(Cell cell)
        {
            Destroy(cell.Piece.gameObject);
            cell.ClearPiece();
        }

        // public void CapturePiece(int col, int row)
        // {
        //     //
        // }

        // public void CapturePiece(Cell cell)
        // {
        //     // return cell.RemovePiece();
        // }

        public Cell GetCellFromVector2(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x > 8 || coords.y < 0 || coords.y > 8)
            {
                Game.Console.Log("Invalid col row given.");
                throw new ArgumentOutOfRangeException();
            }

            if (_cellCoordinates.ContainsKey(coords))
            {
                return _cellCoordinates[coords];
            }
            return null;
        }

        public Cell GetCellFromCoordinates(int col, int row)
        {
            if (col < 0 || col > 8 || row < 0 || row > 8)
            {
                Game.Console.Log("Invalid col row given.");
                throw new ArgumentOutOfRangeException();
            }

            var coords = new Vector2Int(col, row);
            if (_cellCoordinates.ContainsKey(coords))
            {
                return _cellCoordinates[coords];
            }
            return null;
        }

        public Cell GetCellFromPiece(Piece piece)
        {
            var coords = new Vector2Int(piece.Col, piece.Row);
            if (_cellCoordinates.ContainsKey(coords))
            {
                return _cellCoordinates[coords];
            }
            return null;
        }

        internal void RemoveAllPieces()
        {
            foreach (var cell in _cells)
            {
                cell.RemovePiece();
            }
        }
    }
}