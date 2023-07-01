using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] int maximumColumns = 8;
    [SerializeField] int maximumRows = 8;
    public Dictionary<(int, int), Cell> cellMap = new Dictionary<(int, int), Cell>();
    public Map Map = null;
    public GameObject graveyardB;
    public GameObject graveyardT;

    [Header("Prefabs")]
    [SerializeField] Cell _cellPrefab;
    [SerializeField] Piece _piecePrefab;
    [SerializeField] GameObject _grid;
    [SerializeField] GameObject coordinatesGO;

    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = this.GetComponent<RectTransform>();
    }

    void Start()
    {
        this.name = $"Board";
        rectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
        rectTransform.localScale = new Vector3(0.03023016f, 0.03809f, 0.3809f);

        Init();
    }

    void Init()
    {
        if (Map == null)
        {
            Map = new Map();
        }

        GenerateCells();
        GeneratePieces();
    }

    // Remove this board from the game
    public void Delete()
    {
        //
    }

    // Generates board cells
    void GenerateCells()
    {
        for (int row = 0; row < maximumRows; row++)
        {
            for (int col = 0; col < maximumColumns; col++)
            {
                var newCell = Instantiate(_cellPrefab, new Vector3(col, row, 0), Quaternion.identity);
                newCell.name = $"Cell ({col}, {row})";
                newCell.transform.SetParent(_grid.transform);
                
                var rect = newCell.GetComponent<RectTransform>();
                float cellPositionX = col * Constants.cellSize + Constants.cellOffset;
                float cellPositionY = row * Constants.cellSize + Constants.cellOffset;
                rect.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(cellPositionX - 0.25f,
                                                                                    cellPositionY - 0.25f,
                                                                                    0); // Idk why, but I didn't have to subtract .25 from this before
                rect.GetComponent<RectTransform>().localScale = new Vector2(Constants.cellSize, Constants.cellSize);
                
                newCell.SetColRow(col, row);
                if (Map.symbols.ContainsKey((col, row)))
                {
                    newCell.SetOperation(Map.symbols[(col, row)]);
                }
                cellMap[(col, row)] = newCell;
            }
        }
    }

    void GeneratePieces()
    {
        GameObject pieceGroup = new GameObject("Pieces");
        pieceGroup.transform.SetParent(_grid.transform);

        foreach (var pieceData in Map.pieces)
        {
            int col = pieceData.Key.Item1;
            int row = pieceData.Key.Item2;
            Side side = pieceData.Value.Item1;
            Color color = pieceData.Value.Item2;
            string value = pieceData.Value.Item3;
            bool IsKing = pieceData.Value.Item4;
            
            Cell cell = GetCell(col, row);
   
            Piece newPiece = Instantiate(_piecePrefab, new Vector3(col, row, 0), Quaternion.identity);
            newPiece.name = $"Piece ({value})";
            newPiece.transform.SetParent(pieceGroup.transform);
            newPiece.transform.position = cell.transform.position;

            newPiece.SetCell(cell);
            newPiece.SetTeam(side);
            newPiece.SetColor(color);
            newPiece.SetValue(value);
            newPiece.SetKing(IsKing);

            cell.SetPiece(newPiece);
        }
    }

    public List<Move> GetMoves(Piece piece, MoveType moveType=MoveType.All)
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

        return moves;
    }

    List<Move> CheckLeft(Piece piece, int startingRow, int direction, MoveType moveType)
    {
        List<Move> moves = new List<Move>();
        List<Move> captureMoves = new List<Move>();
        Cell cellToCapture = null;
        int nextEnemyPiece = 0;
        int left = piece.col - 1;

        for (int row = startingRow ; row < maximumRows ; row += direction)
        {
            if (left < 0 || left > 7) break;
            if (row < 0 || row > 7) break;
            if (nextEnemyPiece > 1) break;

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
    
    List<Move> CheckRight(Piece piece, int startingRow, int direction, MoveType moveType)
    {
        List<Move> moves = new List<Move>();
        List<Move> captureMoves = new List<Move>();
        int nextEnemyPiece = 0;
        Cell cellToCapture = null;
        int right = piece.col + 1;

        for (int row = startingRow; row < maximumRows ; row += direction)
        {
            if (right < 0 || right > 7) break;
            if (row < 0 || row > 7) break;
            if (nextEnemyPiece > 1) break;

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
    /// This checks for the possible captures of the piece.
    /// Returns all the moves with captures.
    /// </summary>
    public List<Move> CheckForCaptures(Piece piece)
    {
        return GetMoves(piece, MoveType.Capture);
    }

    /// <summary>
    /// This checks only the 4 surrounding cells of the given piece (SE, SW, NE, NW).
    /// Returns all found moves with captures.
    /// </summary>
    public List<Move> CheckIfCaptureable(Piece piece)
    {
        List<Move> moves = new List<Move>();

        for (int col = piece.col - 1 ;  col < piece.col + 2 ; col += 2)
        {
            if (col < 0 || col > 7) continue;
            for (int row = piece.row - 1 ;  row < piece.row + 2 ; row += 2)
            {
                if (row < 0 || row > 7) continue;
                Cell cellToCheck = GetCell(col, row);

                if (cellToCheck.piece != null)
                {
                    moves.AddRange(GetMoves(cellToCheck.piece, MoveType.Capture));
                }
            }
        }
        return moves;
    }

    public Cell GetCell(int col, int row)
    {
        return cellMap[(col, row)];
    }

    public Cell GetCell(Piece piece)
    {
        return cellMap[(piece.col, piece.row)];
    }

    public Dictionary<(int, int), Cell> GetCells()
    {
        return cellMap;
    }

    /// <summary>
    /// Physically move the piece to destination cell.
    /// </summary>
    public void MovePiece(Piece piece, Cell destinationCell)
    {
        Cell originCell = GetCell(piece);

        var pieceToMove = originCell.piece;
        originCell.piece = destinationCell.piece;
        destinationCell.piece = pieceToMove;

        destinationCell.piece.col = destinationCell.col;
        destinationCell.piece.row = destinationCell.row;

        LeanTween.move(destinationCell.piece.gameObject, destinationCell.transform.position, 0.5f).setEaseOutExpo();
    }

    public void Capture(Move move)
    {
        Piece capturedPiece = move.capturedPiece;

        if (capturedPiece.side == Side.Bot)
        {
            RectTransform rect = capturedPiece.GetComponent<RectTransform>();

            capturedPiece.gameObject.transform.SetParent(graveyardT.transform);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);

            LeanTween.move(capturedPiece.gameObject, graveyardT.transform.position, 0.5f).setEaseOutExpo();
        } else
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
}