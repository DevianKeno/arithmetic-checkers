using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Damath
{
    public class Board : MonoBehaviour
    {
        [Header("Board Settings")]
        [SerializeField] int maximumColumns = 8;
        [SerializeField] int maximumRows = 8;
        public Themes Theme;

        public Ruleset Rules { get; private set; }
        public Dictionary<(int, int), Cell> Cellmap = new Dictionary<(int, int), Cell>();
        public Dictionary<Side, Player> Players = new Dictionary<Side, Player>();
        public Piece SelectedPiece = null;
        public List<Move> ValidMoves = new List<Move>();
        public MoveType MovesToGet = MoveType.All;
        public bool TurnRequiresCapture = false;

        [Header("Objects")]
        [SerializeField] GameObject grid;
        [SerializeField] GameObject coordinates;
        [SerializeField] GameObject graveyardB;
        [SerializeField] GameObject graveyardT;
        [SerializeField] GameObject boardPrefab;
        [SerializeField] Cell cellPrefab;
        [SerializeField] Piece piecePrefab;

        RectTransform rectTransform;

        void Awake()
        {
            rectTransform = this.GetComponent<RectTransform>();
            Theme = transform.Find("Theme").GetComponent<Themes>();
        }

        void Start()
        {
            //
        }

        void OnEnable()
        {
            Game.Events.OnRulesetCreate += ReceiveRuleset;
            Game.Events.OnMatchBegin += Init;
            Game.Events.OnPieceSelect += SelectPiece;
            Game.Events.OnMoveSelect += SelectMove;
            Game.Events.OnMoveTypeRequest += UpdateMoveType;
            Game.Events.OnRequireCapture += UpdateRequireCaptureState;
            Game.Events.OnBoardUpdateMoves += UpdateMoves;
            Game.Events.OnPieceMove += MovePiece;
            Game.Events.OnPlayerCreate += AddPlayer;
            Game.Events.OnPieceCapture += CapturePiece;
            Game.Events.OnPieceDone += CheckForKing;
            Game.Events.OnCellRequest += ReturnCell;
        }

        void OnDisable()
        {
            Game.Events.OnRulesetCreate -= ReceiveRuleset;
            Game.Events.OnMatchBegin -= Init;
            Game.Events.OnPieceSelect -= SelectPiece;
            Game.Events.OnMoveSelect -= SelectMove;
            Game.Events.OnMoveTypeRequest -= UpdateMoveType;
            Game.Events.OnRequireCapture -= UpdateRequireCaptureState;
            Game.Events.OnBoardUpdateMoves -= UpdateMoves;
            Game.Events.OnPieceMove -= MovePiece;
            Game.Events.OnPlayerCreate -= AddPlayer;
            Game.Events.OnPieceCapture -= CapturePiece;
            Game.Events.OnPieceDone -= CheckForKing;
            Game.Events.OnCellRequest -= ReturnCell;
        }


        #region Initializing methods
        public void Init(MatchController match)
        {
            GenerateCells();
            GeneratePieces();

            if (Settings.EnableDebugMode)
            {
                Game.Console.Log("[DEBUG]: Board initialized");
            }
        }

        /// <summary>
        /// Generates board cells from the map.
        /// </summary>
        Dictionary<(int, int), Cell>  GenerateCells()
        {
            for (int row = 0; row < maximumRows; row++)
            {
                for (int col = 0; col < maximumColumns; col++)
                {
                    var newCell = Instantiate(cellPrefab, new Vector3(col, row, 0), Quaternion.identity);
                    newCell.name = $"Cell ({col}, {row})";
                    newCell.transform.SetParent(grid.transform);
                    
                    var rect = newCell.GetComponent<RectTransform>();
                    float cellPositionX = col * Constants.cellSize + Constants.cellOffset;
                    float cellPositionY = row * Constants.cellSize + Constants.cellOffset;
                    rect.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(cellPositionX - 0.25f,
                                                                                        cellPositionY - 0.25f,
                                                                                        0); // Idk why, but I didn't have to subtract .25 from this before
                    rect.GetComponent<RectTransform>().localScale = new Vector2(Constants.cellSize, Constants.cellSize);
                    
                    newCell.SetColRow(col, row);
                    // if (Rules.SymbolMap.Symbols.ContainsKey((col, row)))
                    // {
                    //     newCell.SetOperation(Rules.SymbolMap.Symbols[(col, row)]);
                    // }
                    Cellmap[(col, row)] = newCell;
                }
            }

            Game.Events.BoardUpdateCellmap(Cellmap);
            return Cellmap;
        }

        /// <summary>
        /// Generates the pieces and assigns it to its respective cell.
        /// </summary>
        void GeneratePieces()
        {
            GameObject pieceGroup = new("Pieces");
            pieceGroup.transform.SetParent(grid.transform);

            // foreach (var pieceData in Rules.PieceMap.Pieces)
            // {
            //     int col = pieceData.Key.Item1;
            //     int row = pieceData.Key.Item2;
            //     Side side = pieceData.Value.Item1;
            //     string value = pieceData.Value.Item2;
            //     bool IsKing = pieceData.Value.Item3;
                
            //     Cell cell = GetCell(col, row);
    
            //     Piece newPiece = Instantiate(piecePrefab, new Vector3(col, row, 0), Quaternion.identity);
            //     newPiece.name = $"Piece ({value})";
            //     newPiece.transform.SetParent(pieceGroup.transform);
            //     newPiece.transform.position = cell.transform.position;

            //     newPiece.SetCell(cell);
            //     newPiece.SetOwner(Players[side]);
            //     newPiece.SetTeam(side);
            //     if (side == Side.Bot)
            //     {
            //         newPiece.SetColor(Theme.botChipColor, Theme.botChipShadow);
            //     } else
            //     {
            //         newPiece.SetColor(Theme.topChipColor, Theme.topChipShadow);
            //     }
            //     newPiece.SetValue(value);
            //     newPiece.SetKing(IsKing);

            //     cell.SetPiece(newPiece);
            // }
        }
        #endregion

        public void AddPlayer(Player player)
        {
            if (Settings.EnableDebugMode)
            {
                Game.Console.Log($"[BOARD]: Created {player}");
            }
            Players.Add(player.Side, player);
        }

        public void UpdateMoves(List<Move> moves)
        {
            this.ValidMoves = moves;
        }

        public void ReturnCell(int col, int row)
        {
            Game.Events.CellReturn(Cellmap[(col, row)]);
        }

        public void ReceiveRuleset(Ruleset rules)
        {
            Rules = rules;
        }

        /// <summary>
        /// Determines the move type to get.
        /// </summary>
        public void UpdateMoveType(MoveType moveType)
        {
            this.MovesToGet = moveType;
        }

        public void UpdateRequireCaptureState(bool value)
        {
            this.TurnRequiresCapture = value;
        }

        public void SelectPiece(Piece piece)
        {
            SelectedPiece = piece;

            if (TurnRequiresCapture)
            {
                GetPieceMoves(piece, MoveType.Capture);
            } else
            {
                GetPieceMoves(piece);
            }
        }

        public void SelectMove(Cell cell)
        {
            if (ValidMoves.Count == 0) return;

            foreach (Move move in ValidMoves)
            {
                if (cell != move.destinationCell) continue;

                if (cell == move.destinationCell)
                {
                    Game.Events.PieceMove(move);
                    break;
                }
            }
        }

        /// <summary>
        /// Move the piece to destination cell in the scene.
        /// </summary>
        public void MovePiece(Move move)
        {
            Game.Console.Log($"[ACTION]: Moved {SelectedPiece.value}: ({move.originCell.col}, {move.originCell.row}) -> ({move.destinationCell.col}, {move.destinationCell.row})");

            Piece pieceToMove = move.originCell.piece;

            AnimateMovedPiece(move);

            if (move.HasCapture) // Piece has capture
            {
                Game.Events.PieceCapture(move);
            } else // Piece no capture
            {
                DonePiece(pieceToMove);
            }
        }

        /// <summary>
        /// Animates the moved piece.
        /// </summary>
        public void AnimateMovedPiece(Move move)
        {
            // Swap pieces
            (move.originCell.piece, move.destinationCell.piece) = (move.destinationCell.piece, move.originCell.piece);
            // Reinitialization
            move.originCell.HasPiece = false;
            move.destinationCell.HasPiece = true;

            move.destinationCell.piece.col = move.destinationCell.col;
            move.destinationCell.piece.row = move.destinationCell.row;

            LeanTween.move(move.destinationCell.piece.gameObject, move.destinationCell.transform.position, 0.5f).setEaseOutExpo();
        }
        
        /// <summary>
        /// Perform a piece capture given a move.
        /// </summary>
        public void CapturePiece(Move move)
        {
            Debug.Log($"[ACTION]: {move.capturingPiece} captured {move.capturedPiece}");

            AnimateCapturedPiece(move);
            // Check for chain captures
            GetPieceMoves(SelectedPiece, MoveType.Capture);

            if (ValidMoves.Count != 0) // Has chain capture
            {
                Game.Events.BoardUpdateMoves(ValidMoves);
                Game.Events.RequireCapture(true);
            } else // No chain capture
            {
                DonePiece(SelectedPiece);
            }              
        }

        /// <summary>
        /// Animates the capture.
        /// </summary>
        void AnimateCapturedPiece(Move move)
        {
            Piece capturedPiece = move.capturedPiece;

            if (capturedPiece.side == Side.Bot)
            {
                RectTransform rect = capturedPiece.GetComponent<RectTransform>();

                capturedPiece.gameObject.transform.SetParent(graveyardT.transform);
                rect.anchorMin = new Vector2(0.5f, 1f);
                rect.anchorMax = new Vector2(0.5f, 1f);

                LeanTween.move(capturedPiece.gameObject, graveyardT.transform.position, 0.5f).setEaseOutExpo();
            } else // Side.Top
            {
                RectTransform rect = capturedPiece.GetComponent<RectTransform>();

                capturedPiece.gameObject.transform.SetParent(graveyardB.transform);
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);

                LeanTween.move(capturedPiece.gameObject, graveyardB.transform.position, 0.5f).setEaseOutExpo();
            }

            GetCell(capturedPiece).RemovePiece();
            move.capturingPiece.HasCaptured = true;
            move.capturingPiece.CanCapture = false;
        }

        /// <summary>
        /// Checks the piece for any possible actions.
        /// </summary>
        public void CheckPiece(Piece piece)
        {
            List<Move> moves = new();

            CheckForKing(piece);
            // Check if moved piece is able to be captured
            moves = CheckIfCaptureable(piece);
            if (moves.Count != 0) // Piece is captureable
            {
                Game.Events.BoardUpdateMoves(moves);
                Game.Events.RequireCapture(true);
            } else // Piece is NOT captureable
            {
                DonePiece(SelectedPiece);
            }
        }

        /// <summary>
        /// Marks a piece "done" to end piece actions.
        /// </summary>
        public void DonePiece(Piece piece)
        {
            List<Move> moves = new();

            moves.AddRange(CheckIfCaptureable(piece));

            Game.Events.BoardUpdateMoves(moves);

            if (moves.Count != 0)
            {
                Game.Events.RequireCapture(true);
            } else
            {
                Game.Events.RequireCapture(false);
            }
            
            Game.Events.PieceDone(piece);
        }

        /// <summary>
        /// This checks the 4 surrounding cells of the given piece (SE, SW, NE, NW).
        /// Returns all found moves with captures.
        /// </summary>
        public List<Move> CheckIfCaptureable(Piece piece)
        {
            List<Move> moves = new();

            for (int col = piece.col - 1 ;  col < piece.col + 2 ; col += 2)
            {
                if (col < 0 || col > 7) continue;

                for (int row = piece.row - 1 ;  row < piece.row + 2 ; row += 2)
                {
                    if (row < 0 || row > 7) continue;

                    Cell cellToCheck = GetCell(col, row);

                    if (cellToCheck.piece != null)
                    {
                        moves.AddRange(GetPieceMoves(cellToCheck.piece, MoveType.Capture));
                    }
                }
            }
            return moves;
        }

        public void CheckForKing(Piece piece)
        {
            if (piece.side == Side.Bot)
            {
                if (piece.row == 7) piece.Promote();
            } else
            {
                if (piece.row == 0) piece.Promote();
            }
        }

        /// <summary>
        /// Returns the valid moves of the piece.
        /// </summary>
        public void GetPieceMoves(Piece piece)
        {
            List<Move> moves = new List<Move>();
            int up = 1;
            int down = -1;
            int above = piece.row + 1;
            int below = piece.row - 1;

            if (piece.side == Side.Bot)
            {
                // Forward check
                moves.AddRange(CheckLeft(piece, above, up, MovesToGet));
                moves.AddRange(CheckRight(piece, above, up, MovesToGet));
                // Backward check
                moves.AddRange(CheckLeft(piece, below, down, MovesToGet));
                moves.AddRange(CheckRight(piece, below, down, MovesToGet));
            } else if (piece.side == Side.Top)
            {
                // Forward check
                moves.AddRange(CheckLeft(piece, below, down, MovesToGet));
                moves.AddRange(CheckRight(piece, below, down, MovesToGet));
                // Backward check
                moves.AddRange(CheckLeft(piece, above, up, MovesToGet));
                moves.AddRange(CheckRight(piece, above, up, MovesToGet));
            }

            ValidMoves = moves;
            Game.Events.BoardUpdateMoves(ValidMoves);
            Game.Events.PieceWait(piece);
        }

        /// <summary>
        /// Returns the valid moves of the piece.
        /// </summary>
        public List<Move> GetPieceMoves(Piece piece, MoveType moveType = default)
        {
            List<Move> moves = new List<Move>();
            int up = 1;
            int down = -1;
            int above = piece.row + 1;
            int below = piece.row - 1;

            if (piece.side == Side.Bot)
            {
                // Forward check
                moves.AddRange(CheckLeft(piece, above, up, moveType));
                moves.AddRange(CheckRight(piece, above, up, moveType));
                // Backward check
                moves.AddRange(CheckLeft(piece, below, down, moveType));
                moves.AddRange(CheckRight(piece, below, down, moveType));
            } else if (piece.side == Side.Top)
            {
                // Forward check
                moves.AddRange(CheckLeft(piece, below, down, moveType));
                moves.AddRange(CheckRight(piece, below, down, moveType));
                // Backward check
                moves.AddRange(CheckLeft(piece, above, up, moveType));
                moves.AddRange(CheckRight(piece, above, up, moveType));
            }

            ValidMoves = moves;
            Game.Events.BoardUpdateMoves(ValidMoves);
            Game.Events.PieceWait(piece);
            return moves;
        }

        /// <summary>
        /// 
        /// </summary>
        List<Move> CheckLeft(Piece piece, int startingRow, int direction, MoveType moveType)
        {
            List<Move> moves = new List<Move>();
            List<Move> captureMoves = new List<Move>();
            Cell cellToCapture = null;
            int nextEnemyPiece = 0;
            int left = piece.col - 1;

            for (int row = startingRow ; row < maximumRows ; row += direction)
            {
                if (left < 0 || left > 7) break;    //
                if (row < 0 || row > 7) break;      // Out of bounds
                if (nextEnemyPiece > 1) break;      // Two successive pieces

                Cell cellToCheck = GetCell(left, row);

                if (cellToCheck.piece == null)  // Next cell is empty cell
                {
                    if (cellToCapture != null)  // There's a captureable cell
                    {
                        piece.CanCapture = true;
                        captureMoves.Add(new Move(GetCell(piece.col, piece.row), cellToCheck, cellToCapture.piece));
                        if (piece.IsKing) moves.Clear();
                    } else
                    {
                        if (piece.forward != direction)
                        {
                            if (!piece.IsKing) break;
                        }
                        moves.Add(new Move(GetCell(piece.col, piece.row), cellToCheck));
                    }

                    if (!piece.IsKing) break;

                } else if (cellToCheck.piece.side == piece.side)    // Next cell has allied piece
                {
                    break;
                } else  // Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                left -= 1;  // Move selector diagonally
            }

            // Return moves
            switch (moveType)
            {
                case MoveType.Normal:
                    return moves;
                case MoveType.Capture:
                    return captureMoves;
                default:
                    moves.AddRange(captureMoves);
                    return moves;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        List<Move> CheckRight(Piece piece, int startingRow, int direction, MoveType moveType)
        {
            List<Move> moves = new List<Move>();
            List<Move> captureMoves = new List<Move>();
            int nextEnemyPiece = 0;
            Cell cellToCapture = null;
            int right = piece.col + 1;

            for (int row = startingRow; row < maximumRows ; row += direction)
            {
                if (right < 0 || right > 7) break;      //
                if (row < 0 || row > 7) break;          // Out of bounds
                if (nextEnemyPiece > 1) break;          // Two successive pieces

                Cell cellToCheck = GetCell(right, row);

                if (cellToCheck.piece == null)  // Next cell is empty cell
                {
                    if (cellToCapture != null)  // There's a captureable cell
                    {
                        piece.CanCapture = true;
                        captureMoves.Add(new Move(GetCell(piece.col, piece.row), cellToCheck, cellToCapture.piece));
                        if (piece.IsKing) moves.Clear();
                    } else
                    {
                        if (piece.forward != direction)
                        {
                            if (!piece.IsKing) break;
                        }
                        moves.Add(new Move(GetCell(piece.col, piece.row), cellToCheck));
                    }

                    if (!piece.IsKing) break;

                } else if (cellToCheck.piece.side == piece.side)    // Next cell has allied piece
                {
                    break;
                } else  // Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                right += 1;  // Move selector diagonally
            }

            // Return moves
            switch (moveType)
            {
                case MoveType.Normal:
                    return moves;
                case MoveType.Capture:
                    return captureMoves;
                default:
                    moves.AddRange(captureMoves);
                    return moves;
            }
        }

        public Cell GetCell(int col, int row)
        {
            return Cellmap[(col, row)];
        }

        public Cell GetCell(Piece piece)
        {
            return Cellmap[(piece.col, piece.row)];
        }

        public Dictionary<(int, int), Cell> GetCells()
        {
            return Cellmap;
        }
    }
}