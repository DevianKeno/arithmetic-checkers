using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace Damath
{
    public struct PlayerData
    {
        // public Image image;
        public string Name;
    }

    public class Actor : NetworkBehaviour
    {
        public string Name = "Actor";
        public RaycastHit2D Hit;
    }

    public class Spectator : Actor
    {
        public bool IsModerator = false;
    }

    public class Player : Actor
    {
        public Side Side;
        public List<Piece> Pieces;
        public List<Piece> CapturedPieces;
        public float Score = 0f;
        public bool IsPlaying = false;
        public bool IsModerator = false;
        public bool IsControllable = false;
        public bool IsAI = false;
        public Cell SelectedCell = null;
        public Piece SelectedPiece = null;
        public Piece HeldPiece = null;
        public Piece MovedPiece = null;
        public bool IsHolding = false;
        public bool HasCapture = false;
        public bool IsTurn = false;
        public int TurnNumber = 0;
        [SerializeField] private float MouseHoldTime = 0f;


        void Start()
        {                
            Game.Events.OnLobbyStart += InitOnline;
            Game.Events.OnPieceDone += Deselect;
            Game.Events.OnChangeTurn += SetTurn;
            Game.Events.OnNetworkSend += NetworkSend;

            if (IsServer)
            {
                //subscribe network commands which can only be accessed by the server
                Game.Events.OnChatSend += ReceiveChatRpc;
                Game.Events.OnObserverSend += ObserverSendRpc;
            }

            Init();
        }

        void Update()
        {
            DetectRaycast();
            
        }

        void OnDisable()
        {
            // Game.Events.OnPieceMove -= IMadeAMove;
            Game.Events.OnLobbyStart -= InitOnline;
            Game.Events.OnPieceDone -= Deselect;
            Game.Events.OnChangeTurn -= SetTurn;
            Game.Events.OnNetworkSend -= NetworkSend;
        }

        public void InitOnline(Lobby lobby)
        {
            // Game.Events.OnPieceMove += IMadeAMove;
        }

        void NetworkSend(string data)
        {
            NetworkSendRpc(data);
        }
        public void Init()
        {
            Name = Game.Main.Nickname;
            name = $"{Game.Main.Nickname} (Player)";
            Game.Events.PlayerCreate(this);
        }

        public void SetTurn(Side currentTurn)
        {
            if (currentTurn == Side)
            {
                IsTurn = true;
            } else
            {
                IsTurn = false;
            }
        }

        [ServerRpc]
        public void NetworkSendRpc(string data, NetworkConnection conn = default)
        {
            Pack type = Parser.Parse(data, out string[] args);

            switch (type)
            {
                case Pack.Ruleset:
                {
                    //
                    break;
                }
                
                case Pack.Chat:
                {
                    Game.Events.ChatSend(conn, data);
                    break;
                }

                case Pack.Command:
                {
                        //
                    break;
                }

                case Pack.RuleType:
                    
                    Game.Events.ObserverSend(conn, data);
                    break;
            }
        }
        
        [ObserversRpc(ExcludeOwner = false)]
        void ObserverSendRpc(NetworkConnection conn, string data)
        {
            Pack type = Parser.Parse(data, out string[] args);

            if (type == Damath.Pack.RuleType)
            {
                switch (args[1])
                {
                    case "0":
                        Game.Main.SetRuleset(Ruleset.Standard);
                        break;
                    case "1":
                        Game.Main.SetRuleset(Ruleset.Speed);
                        break;
                }
            }
        }

        [ObserversRpc(ExcludeOwner = false)]
        void ReceiveChatRpc(NetworkConnection target, string data)
        {
            Pack type = Parser.Parse(data, out string[] args);

            if(IsOwner) Game.Console.Log($"<{args[1]}> {args[2]}");
        }

/*        void SetConsoleOperator(Player player)
        {
            if (!IsOwner) return;

            if (Settings.EnableDebugMode) Game.Console.Log($"Set {player.Name} as console operator");
            
            Game.Console.SetOperator(this);
        }*/

        public string SetName(string value)
        {
            name = $"Player {value}";
            Name = value;
            return value;
        }

        public void SetPlaying(bool value)
        {
            IsPlaying = value;
        }

        public void SetSide(Side value)
        {
            Side = value;
        }

        public void SetScore(float value)
        {
            Score = value;
        }

        void DetectRaycast()
        {
            if (Input.GetMouseButtonDown(0))
            {
                CastRay();
                Game.Events.PlayerLeftClick(this);    
            }
            
            if (Input.GetMouseButtonDown(1))
            {
                CastRay();
                Game.Events.PlayerRightClick(this);    
            }

            if (Input.GetMouseButton(0))
            {
                MouseHoldTime += 1 * Time.deltaTime;

                if (MouseHoldTime >= Settings.PieceGrabDelay)
                {
                    IsHolding = true;

                    if (HeldPiece != null)
                    {
                        HeldPiece.transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, 1));
                    } else
                    {
                        HeldPiece = SelectedPiece;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                MouseHoldTime = 0f;
                CastRay();

                if (IsHolding)
                {
                    IsHolding = false;
                    Game.Events.PlayerRelease(this);
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                // CastRay();
            }
        }
        
        /// <summary>
        /// Casts a ray then caches hit object.
        /// </summary>
        void CastRay()
        {
            if (!IsControllable) return;

            Hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);  

            if (Hit.collider == null) return;

            if (Hit.collider.CompareTag("Cell"))
            {
                SelectedCell = Hit.collider.gameObject.GetComponent<Cell>();
                SelectCell(SelectedCell);

            } else if (Hit.collider.CompareTag("Background"))
            {
                Debug.Log("Left clicked background");
                Game.Events.CellDeselect(SelectedCell);
            }
        }

        public void SelectCell(Cell cell)
        {
            SelectedCell = cell;

            if (SelectedCell.HasPiece)
            {
                SelectPiece(SelectedCell.Piece);
            } else
            {
                SelectMovecell(SelectedCell);
            }
        }

        public void SelectPiece(Piece piece)
        {
            if (SelectedCell.Piece.Side == Side)
            {
                if (SelectedPiece != null)
                {
                    if (SelectedPiece == piece)
                    {
                        ReleaseHeldPiece();
                        return;
                    } else
                    {
                        Deselect();
                        return;
                    }
                }

                if (MovedPiece != null)
                {
                    if (piece != MovedPiece)
                    {
                        Deselect();
                        return;
                    }
                } else
                {
                    SelectedPiece = piece;
                    Game.Events.PlayerSelectPiece(this, SelectedPiece);
                    Game.Audio.PlaySound("Select");
                    return;
                }
            }

        }

        public void SelectMovecell(Cell cell = null)
        {
            if (cell != null) SelectedCell = cell;

            if (SelectedCell.IsValidMove)
            {
                if (!IsTurn) return;

                Debug.Log("move");
                Game.Events.PlayerSelectMovecell(this, SelectedCell);
                Game.Audio.PlaySound("Move");
                return;
            }

            Deselect();
        }

        public void ReleaseHeldPiece()
        {
            HeldPiece?.ResetPosition();       
            HeldPiece = null;
        }

        public void Deselect()
        {
            SelectedPiece = null;
            ReleaseHeldPiece();

            Game.Events.Deselect();
            Game.Events.PlayerDeselect(this);
        }

        public void Deselect(Piece piece)
        {
            Deselect();
        }

        // private void IMadeAMove(Move move)
        // {
        //     Debug.Log($"I made a move");
            
        //     int[] moveData = new int[]
        //     {
        //         move.originCell.Col,
        //         move.originCell.Row,
        //         move.destinationCell.Col,
        //         move.destinationCell.Row
        //     };
          
        //     Game.Console.Log("Sending move data to server...");
        //     SendMoveDataServerRpc(moveData);
        // }

        // [ServerRpc(RequireOwnership = false)]
        // public void SendMoveDataServerRpc(int[] moveData, ServerRpcParams serverRpcParams = default)
        // {
        //     Game.Console.Log("Received move data");
        //     var senderClientId = serverRpcParams.Receive.SenderClientId;

        //     Game.Console.Log("Sending move data to clients...");
        //     ReceiveMoveDataClientRpc(moveData, GetClientsExcept(senderClientId));
        // }

        // private ClientRpcParams GetClientsExcept(ulong exceptedClientId)
        // {
        //     var target = new ClientRpcParams()
        //     {
        //         Send = new ClientRpcSendParams()
        //         {
        //             // This should get players from the lobby tho (?)
        //             TargetClientIds = Network.Main.ConnectedClientsIds.Where(x => x != exceptedClientId).ToArray()
        //         }
        //     };
        //     return target;
        // }

        // [ClientRpc]
        // public void ReceiveMoveDataClientRpc(int[] command, ClientRpcParams clientRpcParams)
        // {
        //     Game.Console.Log("Received client rpc");
        //     Game.Console.Log($"Someone made a move {command}");
        // }
    }
}