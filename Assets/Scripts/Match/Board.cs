using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Damath.UI;
using System;
using Damath.Players;

namespace Damath
{
    public class Board : MonoBehaviour
    {
        [Header("Board Game.Main.Settings")]
        [SerializeField] int MaxColumns = 8;
        [SerializeField] int MaxRows = 8;

        Ruleset Ruleset { get; set; }
        public Themes Theme;

        List<Cell> _cells = new();
        Dictionary<Vector2, Cell> _cellCoordinates = new();
        List<Piece> _pieces = new();

        [SerializeField] Cell selectedCell = null;
        public Piece SelectedPiece = null;
        public Piece MovedPiece = null;
        public List<Move> ValidMoves = new();
        public MoveType MovesToGet = MoveType.All;
        public bool TurnRequiresCapture = false;
        public bool IsFlipped { get; private set; }
        public int TurnNumber = -1;
        public Side TurnOf;

        public event EventHandler<Side> OnTurnChange;
        public event EventHandler<Move> OnPieceCapture;

        private bool SymbolVisible;
        private bool PieceVisible;

        [Header("Objects")]
        [SerializeField] GameObject cellGroup;
        [SerializeField] GameObject pieceGroup;
        [SerializeField] GameObject coordinates;
        [SerializeField] GameObject graveyardB;
        [SerializeField] GameObject graveyardT;
        [SerializeField] GameObject boardPrefab;
        [SerializeField] Cell cellPrefab;
        [SerializeField] Piece piecePrefab;
        RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Theme = transform.Find("Theme").GetComponent<Themes>();
        }

        void Start()
        {

        }

        void OnEnable()
        {
            Game.Events.OnMatchBegin += BeginMatch;
            Game.Events.OnPlayerSelectPiece += SelectPiece;
            Game.Events.OnPlayerSelectMovecell += SelectMovecell;
            Game.Events.OnDeselect += ClearValidMoves;
            Game.Events.OnMoveTypeRequest += UpdateMoveType;
            Game.Events.OnRequireCapture += UpdateRequireCaptureState;
            // Game.Events.OnPieceDone += CheckForKing;
            Game.Events.OnBoardFlip += Flip;
        }

        void OnDisable()
        {
            Game.Events.OnMatchBegin -= BeginMatch;
            Game.Events.OnPlayerSelectPiece -= SelectPiece;
            Game.Events.OnPlayerSelectMovecell -= SelectMovecell;
            Game.Events.OnDeselect -= ClearValidMoves;
            Game.Events.OnMoveTypeRequest -= UpdateMoveType;
            Game.Events.OnRequireCapture -= UpdateRequireCaptureState;
            // Game.Events.OnPieceDone -= CheckForKing;
            Game.Events.OnBoardFlip -= Flip;
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

                    if (Ruleset.SymbolMap.ContainsKey(coords))
                    {
                        newCell.SetOperation(Ruleset.SymbolMap[coords]);
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
                Vector2Int coords = entry.Key;
                PieceData pieceData = entry.Value;
                Cell cell = GetCell(coords);    

                Piece newPiece = Instantiate(piecePrefab, pieceGroup.transform);
                newPiece.name = $"Piece ({pieceData.Value}) {pieceData.Side}";

                // Transform piece position
                newPiece.transform.localScale = new (Constants.PieceScale, Constants.PieceScale);
                newPiece.transform.position = cell.transform.position;

                // Initialize piece values
                newPiece.SetCell(cell);
                newPiece.SetValue(pieceData.Value);
                newPiece.SetSide(pieceData.Side);
                newPiece.SetKing(pieceData.IsKing);
                cell.SetPiece(newPiece);
                _pieces.Add(newPiece);
            }
        }

        #endregion

        public void BeginMatch()
        {
            Reset();
            ToggleSymbolVisibility(true);
            TogglePieceVisibility(true);
        }

        public void Reset()
        {
            TurnNumber = 0;
            ChangeTurnTo(Ruleset.GetValue<Side>("FirstTurn"));
        }

        public void ChangeTurns()
        {
            if (TurnOf == Side.Blue)
            {
                TurnOf = Side.Orange;
            } else if (TurnOf == Side.Orange)
            {
                TurnOf = Side.Blue;
            }
            TurnNumber++;
            ClearValidMoves();
            OnTurnChange?.Invoke(this, TurnOf);
            Game.Events.TurnChanged(TurnOf);
        }

        public void ChangeTurnTo(Side side)
        {
            TurnOf = side;
            TurnNumber++;
            ClearValidMoves();
            Game.Events.TurnChanged(side);
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
            } else
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
        
        public void ClearValidMoves()
        {
            foreach (var move in ValidMoves)
            {
                move.Destination.IsValidMove = false;
            }
            ValidMoves.Clear();
        }

        /// <summary>
        /// Determines the move type to get.
        /// </summary>
        public void UpdateMoveType(MoveType moveType)
        {
            MovesToGet = moveType;
        }

        public void UpdateRequireCaptureState(bool value)
        {
            TurnRequiresCapture = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="piece"></param>
        public void SelectPiece(Player player, Piece piece)
        {        
            if (!TurnRequiresCapture)
            {
                ValidMoves = GetPieceMoves(piece);
            } else
            {
                if (!piece.CanCapture) return;
                
                ValidMoves = GetPieceMoves(piece, MoveType.Capture);
            }
            
            Game.Events.BoardUpdateValidMoves(ValidMoves);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        /// <param name="cell"></param>
        public void SelectMovecell(Player player, Cell cell)
        {
            if (player.Side != TurnOf) return;
            if (ValidMoves.Count == 0) return;

            foreach (Move move in ValidMoves)
            {
                if (cell != move.Destination) continue;

                if (cell == move.Destination)
                {
                    SelectMove(player, move);
                    break;
                }
            }
        }

        public void SelectMove(Player player, Move move)
        {
            move.SetPlayer(player);
            PerformMove(move);
        }

        /// <summary>
        /// Perform a piece move given a Move object.
        /// </summary>
        public void PerformMove(Move move)
        {
            ClearValidMoves();
            Piece movedPiece = move.Piece;
            Game.Events.PieceMove(move);
            Game.Console.Log($"[ACTION]: Moved {movedPiece.Value}: ({move.Origin.Col}, {move.Origin.Row}) -> ({move.Destination.Col}, {move.Destination.Row})");
            AnimateMove(move);

            if (!move.HasCapture)
            {
                DonePiece(movedPiece);
            } else
            {
                CapturePiece(move);
            }
        }
        
        /// <summary>
        /// Perform a piece capture given a move.
        /// </summary>
        public void CapturePiece(Move move)
        {
            OnPieceCapture?.Invoke(this, move);
            Game.Events.PieceCapture(move);
            Game.Audio.PlaySound("capture");
            Debug.Log($"[ACTION]: {move.CapturingPiece} captured {move.CapturedPiece}");
            AnimateCapture(move);

            if (Ruleset.GetValue<bool>("AllowChainCapture"))
            {            
                /// Check for chain captures
                ValidMoves = GetPieceMoves(move.Piece, MoveType.Capture);

                if (ValidMoves.Count != 0) /// Has succeeding capture
                {
                    Game.Events.BoardUpdateCaptureables(ValidMoves);
                    Game.Events.RequireCapture(true);
                    return;
                }
            }
            DonePiece(move.Piece);
        }

        /// <summary>
        /// Marks a piece "done" to end piece actions.
        /// </summary>
        public void DonePiece(Piece piece)
        {
            ValidMoves = CheckIfCaptureable(piece);

            if (ValidMoves.Count == 0) /// Piece is NOT captureable
            {
                Game.Events.RequireCapture(false);
                ClearValidMoves();
            } else /// Piece is captureable
            {
                Game.Events.BoardUpdateCaptureables(ValidMoves);
                Game.Events.RequireCapture(true);
            }

            CheckForKing(piece);
            ChangeTurns();
            
            Game.Events.PieceDone(piece);
        }

        /// <summary>
        /// Animates the moved piece in the scene.
        /// </summary>
        void AnimateMove(Move move)
        {
            move.Destination.SetPiece(move.Origin.Piece);
            move.Origin.ClearPiece();

            // (move.originCell.Piece, move.destinationCell.Piece) = (move.destinationCell.Piece, move.originCell.Piece);

            if (Game.Main.Settings.EnableAnimations)
            {
                LeanTween.move(move.Destination.Piece.gameObject, move.Destination.transform.position, 0.5f).setEaseOutExpo();
            }
            // else
            // {
            //     move.destinationCell.Piece.transform.position = move.destinationCell.transform.position;
            // }
        }

        /// <summary>
        /// Animates the capture.
        /// </summary>
        void AnimateCapture(Move move)
        {
            Piece capturedPiece = move.CapturedPiece;
            RectTransform rect = capturedPiece.GetComponent<RectTransform>();

            if (capturedPiece.Side == Side.Blue) /// Blue player captured
            {
                capturedPiece.gameObject.transform.SetParent(graveyardT.transform);
                rect.anchorMin = new Vector2(1f, 0.5f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.localScale = new Vector2(1.5f, 1.5f);

                if (Game.Main.Settings.EnableAnimations)
                {
                    if (graveyardB.transform.childCount > 0)
                    {
                        MoveGraveyardChildren(graveyardT);
                    }
                    LeanTween.move(capturedPiece.gameObject, graveyardT.transform.position, 0.5f).setEaseOutExpo();
                }
            } else /// Orange player captured
            {
                capturedPiece.gameObject.transform.SetParent(graveyardB.transform);
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);                
                rect.localScale = new Vector2(1.5f, 1.5f);

                if (graveyardB.transform.childCount > 0)
                {
                    MoveGraveyardChildren(graveyardB);
                }
                if (Game.Main.Settings.EnableAnimations)
                {
                    LeanTween.move(capturedPiece.gameObject, graveyardB.transform.position, 0.5f).setEaseOutExpo();
                } else
                {
                    capturedPiece.gameObject.transform.position = graveyardB.transform.position;
                }
            }

            // move.Player.CapturedPieces.Add(capturedPiece);
            GetCellFromPiece(capturedPiece).ClearPiece();
            move.Player.CapturedPieces.Add(move.CapturedPiece);
            move.CapturingPiece.HasCaptured = true;
            move.CapturingPiece.CanCapture = false;
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
                } else
                {
                    child.transform.position = targetPosition;
                }
            }
        }

        /// <summary>
        /// Check if this piece is captureable.
        /// This checks the 4 surrounding diagonal cells (SE, SW, NE, NW).
        /// </summary>
        /// <returns> All moves with captures. </returns>
        public List<Move> CheckIfCaptureable(Piece piece)
        {
            List<Move> moves = new();

            for (int col = piece.Col - 1 ;  col < piece.Col + 2 ; col += 2)
            {
                if (col < 0 || col > 7) continue;

                for (int row = piece.Row - 1 ;  row < piece.Row + 2 ; row += 2)
                {
                    if (row < 0 || row > 7) continue;

                    Cell cellToCheck = GetCellFromRow(col, row);

                    if (cellToCheck.HasPiece)
                    {
                        moves.AddRange(GetPieceMoves(cellToCheck.Piece, MoveType.Capture));
                    }
                }
            }
            return moves;
        }

        public void CheckForKing(Piece piece)
        {
            if (piece.Side == Side.Blue)
            {
                if (piece.Row == 7) piece.Promote();
            } else
            {
                if (piece.Row == 0) piece.Promote();
            }
        }

        /// <summary>
        /// Returns the valid moves of the piece.
        /// </summary>
        public List<Move> GetPieceMoves(Piece piece)
        {
            List<Move> moves = new();
            int up = 1;
            int down = -1;
            int above = piece.Row + 1;
            int below = piece.Row - 1;

            if (piece.Side == Side.Blue)
            {
                /// Forward check
                moves.AddRange(CheckLeft(piece, above, up, MovesToGet));
                moves.AddRange(CheckRight(piece, above, up, MovesToGet));
                /// Backward check
                moves.AddRange(CheckLeft(piece, below, down, MovesToGet));
                moves.AddRange(CheckRight(piece, below, down, MovesToGet));
            } else if (piece.Side == Side.Orange)
            {
                /// Forward check
                moves.AddRange(CheckLeft(piece, below, down, MovesToGet));
                moves.AddRange(CheckRight(piece, below, down, MovesToGet));
                /// Backward check
                moves.AddRange(CheckLeft(piece, above, up, MovesToGet));
                moves.AddRange(CheckRight(piece, above, up, MovesToGet));
            }
            return moves;
        }

        /// <summary>
        /// Returns the valid moves of the piece.
        /// </summary>
        public List<Move> GetPieceMoves(Piece piece, MoveType moveType = default)
        {
            List<Move> moves = new();
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
            return moves;
        }

        /// <summary>
        /// 
        /// </summary>
        List<Move> CheckLeft(Piece piece, int startingRow, int yDirection, MoveType moveType)
        {
            List<Move> moves = new();
            List<Move> captureMoves = new();
            Cell cellToCapture = null;
            int nextEnemyPiece = 0;
            int left = piece.Col - 1;

            for (int row = startingRow ; row < MaxRows ; row += yDirection)
            {
                if (left < 0 || left > 7) break;    ///
                if (row < 0 || row > 7) break;      /// Out of bounds
                if (nextEnemyPiece > 1) break;      /// Two successive enemy pieces

                Cell cellToCheck = GetCellFromRow(left, row);

                if (cellToCheck.Piece == null) /// Next cell is empty cell
                {
                    if (cellToCapture != null)  /// There's a captureable cell
                    {
                        piece.CanCapture = true;
                        captureMoves.Add(new Move(GetCellFromRow(piece.Col, piece.Row), cellToCheck, cellToCapture.Piece));
                        if (piece.IsKing) moves.Clear();
                    } else
                    {
                        if (piece.Forward != yDirection)
                        {
                            if (!piece.IsKing) break;
                        }
                        moves.Add(new Move(GetCellFromRow(piece.Col, piece.Row), cellToCheck));
                    }

                    if (!piece.IsKing) break;

                } else if (cellToCheck.Piece.Side == piece.Side) /// Next cell has allied piece
                {
                    break;
                } else /// Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                left -= 1;  /// Move selector diagonally
            }

            return moveType switch
            {
                MoveType.All => moves.Concat(captureMoves).ToList(),
                MoveType.Normal => moves,
                MoveType.Capture => captureMoves,
                _ => null,
            };
        }
        
        /// <summary>
        /// 
        /// </summary>
        List<Move> CheckRight(Piece piece, int startingRow, int yDirection, MoveType moveType)
        {
            List<Move> moves = new();
            List<Move> captureMoves = new();
            int nextEnemyPiece = 0;
            Cell cellToCapture = null;
            int right = piece.Col + 1;

            for (int row = startingRow; row < MaxRows ; row += yDirection)
            {
                if (right < 0 || right > 7) break;      ///
                if (row < 0 || row > 7) break;          /// Out of bounds
                if (nextEnemyPiece > 1) break;          /// Two successive enemy pieces

                Cell cellToCheck = GetCellFromRow(right, row);

                if (cellToCheck.Piece == null)  /// Next cell is empty cell
                {
                    if (cellToCapture != null)  /// There's a captureable cell
                    {
                        piece.CanCapture = true;
                        captureMoves.Add(new Move(GetCellFromRow(piece.Col, piece.Row), cellToCheck, cellToCapture.Piece));
                        if (piece.IsKing) moves.Clear();
                    } else
                    {
                        if (piece.Forward != yDirection)
                        {
                            if (!piece.IsKing) break;
                        }
                        moves.Add(new Move(GetCellFromRow(piece.Col, piece.Row), cellToCheck));
                    }

                    if (!piece.IsKing) break;

                } else if (cellToCheck.Piece.Side == piece.Side)    /// Next cell has allied piece
                {
                    break;
                } else  /// Next cell has enemy piece
                {
                    nextEnemyPiece += 1;
                    cellToCapture = cellToCheck;
                }
                right += 1;  /// Move selector diagonally
            }

            return moveType switch
            {
                MoveType.All => moves.Concat(captureMoves).ToList(),
                MoveType.Normal => moves,
                MoveType.Capture => captureMoves,
                _ => null,
            };
        }

        public Piece AddPiece(int col, int row, Side side, string value, bool isKing)
        {
            var data = new PieceData
            {
                Side = side,
                Value = value,
                IsKing = isKing
            };
            return AddPiece(GetCellFromRow(col, row), data);
        }

        public Piece AddPiece(int col, int row, PieceData pieceData)
        {
            return AddPiece(GetCellFromRow(col, row), pieceData);
        }

        public Piece AddPiece(Cell cell, PieceData pieceData)
        {
            if (cell.HasPiece)
            {
                Game.Console.Log("Can't add a piece to an existing piece.");
                return null;
            }

            Piece newPiece = Instantiate(piecePrefab, pieceGroup.transform);
            newPiece.name = $"Piece ({pieceData.Value}) {pieceData.Side}";

            /// Transform piece position
            newPiece.transform.localScale = new (Constants.PieceScale, Constants.PieceScale);
            newPiece.transform.position = cell.transform.position;

            /// Initialize piece values
            newPiece.SetCell(cell);
            newPiece.SetValue(pieceData.Value);
            newPiece.SetSide(pieceData.Side);
            newPiece.SetKing(pieceData.IsKing);
            cell.SetPiece(newPiece);

            return newPiece;
        }

        public void RemovePiece(int col, int row)
        {
            RemovePiece(GetCellFromRow(col, row));
        }

        public void RemovePiece(Cell cell)
        {
            Destroy(cell.Piece.gameObject);
            cell.ClearPiece();
        }

        public void CapturePiece(int col, int row)
        {
            //
        }

        public void CapturePiece(Cell cell)
        {
            // return cell.RemovePiece();
        }

        public Cell GetCell(Vector2Int coords)
        {
            if (_cellCoordinates.ContainsKey(coords))
            {
                return _cellCoordinates[coords];
            }
            return null;
        }

        public Cell GetCellFromRow(int col, int row)
        {
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