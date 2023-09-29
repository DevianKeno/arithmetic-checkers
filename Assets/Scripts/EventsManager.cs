using System;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace Damath
{
    public class EventsManager : MonoBehaviour
    {
        public bool EnableDebugMode = false;

        #pragma warning disable 0169
        #region Global events

        public event Action<MatchController> OnMatchCreate;
        public event Action OnMatchBegin;
        public event Action OnMatchEnd;
        public event Action OnBoardCreate;
        public event Action<Ruleset> OnRulesetDistribute;

        #endregion

        #region Player events
        
        public event Action<Player> OnPlayerJoin;
        public event Action<Player> OnPlayerCreate;
        public event Action<Player> OnPlayerLeftClick;
        public event Action<Player> OnPlayerRightClick;
        public event Action<Player> OnPlayerHold;
        public event Action<Player> OnPlayerRelease;
        public event Action<Player> OnPlayerDeselect;
        public event Action<Player, string> OnPlayerSendMessage;
        public event Action<Player, string> OnPlayerCommand;
        public event Action<Player, Cell> OnPlayerSelectCell;
        public event Action<Player, Piece> OnPlayerSelectPiece;
        public event Action<Player, Piece> OnPlayerHoldPiece;
        public event Action<Player, Cell> OnPlayerSelectMovecell;
        public event Action<Player, Move> OnPlayerSelectMove;

        #endregion

        #region Network events
        
        public event Action<string> OnServerStart;
        public event Action<Player> OnClientStart;
        public event Action<string> OnNetworkSend;
        public event Action<NetworkConnection, string> OnChatSend;
        public event Action<string> OnObserverSend;
        public event Action<NetworkObject, NetworkConnection> OnOwnershipRequest;
        public event Action<Lobby> OnLobbyCreate;
        public event Action<Lobby> OnLobbyHost;
        public event Action<int, Lobby> OnLobbyJoin;
        public event Action<Lobby> OnLobbyStart;
        public event Action<MatchController> OnMatchHost;

        #endregion
        
        #region Match events
        public event Action OnDeselect;
        public event Action<Cell> OnCellSelect;
        public event Action<Cell> OnCellDeselect;
        public event Action<Cell> OnCellReturn;
        public event Action<Cell> OnMoveSelect;
        public event Action<Piece> OnPieceSelected;
        public event Action<Piece> OnPieceDeselect;
        public event Action<Piece> OnPieceWait;
        public event Action<Move> OnPieceMove;
        public event Action<Piece> OnPieceDone;
        public event Action<Move> OnPieceCapture;
        public event Action<List<Move>> OnBoardUpdateValidMoves;
        public event Action<List<Move>> OnBoardUpdateCaptureables;
        public event Action<Cellmap<Cell>> OnBoardUpdateCellmap;
        public event Action<MoveType> OnMoveTypeRequest;
        public event Action<bool> OnRequireCapture;
        public event Action OnRefresh;
        public event Action<Side> OnTurnChanged;
        public event Action OnBoardFlip;

        #endregion

        // Methods
        #region Global event methods
        
        /// <summary>
        /// Fired when a match is created.
        /// </summary>
        public void MatchCreate(MatchController match)
        {
            OnMatchCreate?.Invoke(match);
        }

        /// <summary>
        /// Fired when a match begins.
        /// </summary>
        public void MatchBegin()
        {
            OnMatchBegin?.Invoke();
        }

        /// <summary>
        /// Fired when a match receives a ruleset. 
        /// </summary>
        /// <param name="value"></param>
        public void RulesetDistribute(Ruleset value)
        {
            OnRulesetDistribute?.Invoke(value);
        }

        #endregion

        #region Player event methods

        /// <summary>
        /// Fired when a new player joins.
        /// </summary>
        public void PlayerJoin(Player player)
        {
            OnPlayerJoin?.Invoke(player);
        }

        /// <summary>
        /// Fired when a new player is created.
        /// </summary>
        public void PlayerCreate(Player player)
        {
            OnPlayerCreate?.Invoke(player);
        }

        public void PlayerLeftClick(Player player)
        {
            OnPlayerLeftClick?.Invoke(player);
        }

        public void PlayerRightClick(Player player)
        {
            OnPlayerRightClick?.Invoke(player);
        }

        /// <summary>
        /// Fired every frame a player holds left click.
        /// </summary>
        public void PlayerHold(Player player)
        {
            OnPlayerHold?.Invoke(player);
        }

        /// <summary>
        /// Fired when a player releases left click.
        /// </summary>
        public void PlayerRelease(Player player)
        {
            OnPlayerRelease?.Invoke(player);
        }

        /// <summary>
        /// Fired when a player selects a cell.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="cell"></param>
        public void PlayerSelectCell(Player actor, Cell cell)
        {
            OnPlayerSelectCell?.Invoke(actor, cell);
        }

        /// <summary>
        /// Fired when a player selects a valid piece.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="piece"></param>
        public void PlayerSelectPiece(Player actor, Piece piece)
        {
            OnPlayerSelectPiece?.Invoke(actor, piece);
        }
        
        public void PlayerHoldPiece(Player actor, Piece piece)
        {
            OnPlayerHoldPiece?.Invoke(actor, piece);
        }

        public void PlayerSelectMovecell(Player player, Cell cell)
        {
            OnPlayerSelectMovecell?.Invoke(player, cell);
        }

        public void PlayerDeselect(Player player)
        {
            OnPlayerDeselect?.Invoke(player);
        }

        /// <summary>
        /// Fired when a player performs a valid move.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="piece"></param>
        public void PlayerSelectMove(Player player, Move move)
        {
            OnPlayerSelectMove?.Invoke(player, move);
        }

        public void PlayerCommand(Player player, string command)
        {
            OnPlayerCommand?.Invoke(player, command);
        }

        #endregion

        #region Network event methods

        public void ClientStart(Player player)
        {
            OnClientStart?.Invoke(player);
        }

        public void NetworkSend(string data)
        {
            Debug.Log($"Test server RPC {data}");
            OnNetworkSend?.Invoke(data);
        }

        public void ChatSend(NetworkConnection conn, string data)
        {
            OnChatSend?.Invoke(conn, data);
        }
        public void ObserverSend(string data)
        {
            Debug.Log($"Test observer RPC {data}");
            OnObserverSend?.Invoke(data);
        }

        public void OwnershipRequest(NetworkObject networkObject, NetworkConnection connection)
        {
            OnOwnershipRequest?.Invoke(networkObject, connection);
        }

        public void LobbyCreate(Lobby lobby)
        {
            OnLobbyCreate?.Invoke(lobby);
        }

        public void LobbyHost(Lobby lobby)
        {
            OnLobbyHost?.Invoke(lobby);
        }

        public void LobbyJoin(int clientId, Lobby lobby)
        {
            OnLobbyJoin?.Invoke(clientId, lobby);
        }
        
        public void LobbyStart(Lobby lobby)
        {
            OnLobbyStart?.Invoke(lobby);
        }
        
        public void MatchHost(MatchController match)
        {
            OnMatchHost?.Invoke(match);
        }

        #endregion

        #region Match event methods

        /// <summary>
        /// Fired upon deselection.
        /// </summary>
        public void Deselect()
        {
            OnDeselect?.Invoke();
        }

        /// <summary>
        /// Fired when a cell is selected.
        /// </summary>
        public void CellSelect(Cell cell)
        {
            OnCellSelect?.Invoke(cell);
        }

        /// <summary>
        /// Fired when a cell is deselected.
        /// </summary>
        public void CellDeselect(Cell cell)
        {
            OnCellDeselect?.Invoke(cell);
        }
                
        public void CellReturn(Cell cell)
        {
            OnCellReturn?.Invoke(cell);
        }

        public void PieceSelected(Piece piece)
        {
            OnPieceSelected?.Invoke(piece);
        }

        /// <summary>
        /// Fired when a piece is deselected.
        /// </summary>
        public void PieceDeselect(Piece piece)
        {
            OnPieceDeselect?.Invoke(piece);
        }

        /// <summary>
        /// Fired when piece is waiting for an action.
        /// </summary>
        public void PieceWait(Piece piece)
        {
            OnPieceWait?.Invoke(piece);
        }

        /// <summary>
        /// Fired when a piece is moved.
        /// </summary>
        public void PieceMove(Move move)
        {
            OnPieceMove?.Invoke(move);
        }

        /// <summary>
        /// Fired when a piece has no more actions to take.
        /// </summary>
        public void PieceDone(Piece piece)
        {
            OnPieceDone?.Invoke(piece);
        }
        
        /// <summary>
        /// Fired when the Board updates all its valid moves.
        /// </summary>
        public void BoardUpdateValidMoves(List<Move> moves)
        {
            OnBoardUpdateValidMoves?.Invoke(moves);
        }

        public void BoardUpdateCaptureables(List<Move> moves)
        {
            OnBoardUpdateCaptureables?.Invoke(moves);
        }
        /// <summary>
        /// Fired when the Board updates all its valid moves.
        /// </summary>
        public void BoardUpdateCellmap(Cellmap<Cell> cellmap)
        {
            OnBoardUpdateCellmap?.Invoke(cellmap);
        }
        
        public void MoveTypeRequest(MoveType moveType)
        {
            if (OnMoveTypeRequest != null)
            {
                OnMoveTypeRequest(moveType);
            }
        }

        public void PieceCapture(Move move)
        {
            OnPieceCapture?.Invoke(move);
        }

        /// <summary>
        /// Fired when the Board requests for a turn that has a mandatory capture.
        /// </summary>
        public void RequireCapture(bool value)
        {
            OnRequireCapture?.Invoke(value);
        }

        public void Refresh()
        {
            if (OnRefresh != null)
            {
                OnRefresh();
            }
        }

        /// <summary>
        /// Fired every time the turns are changed.
        /// The side is who the current turn is.
        /// </summary>
        /// <param name="side"></param>
        public void TurnChanged(Side side)
        {
            OnTurnChanged?.Invoke(side);
        }

        /// <summary>
        /// Fired everytime the board is flipped.
        /// </summary>
        public void BoardFlip()
        {
            OnBoardFlip?.Invoke();
        }

        #endregion
        #pragma warning restore 0169
    }

}
