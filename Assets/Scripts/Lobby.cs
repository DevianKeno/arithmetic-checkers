using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.Netcode;
using UnityEngine;

namespace Damath
{
    /// <summary>
    /// Lobby before the match begins.
    /// </summary>
    public class Lobby : INetworkSerializable
    {
        public bool IsPrivate { get; private set; }
        public string Password { get; private set; }
        public List<ulong> ConnectedClients = new();
        public ulong Host;
        public ulong Opponent;
        public bool HasOpponent = false;
        public bool IsHost = false;
        public bool IsOpponent = false;

        public Ruleset Ruleset { get; private set; }
        public bool OpponentIsReady;

        public Lobby(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }
        public void SetPrivacy(bool value)
        {
            IsPrivate = value;
        }

        public void SetHost(ulong clientId)
        {
            Host = clientId;
            IsHost = true;
            ConnectedClients.Add(clientId);
            Game.Events.LobbyJoin(clientId, this);
        }

        public void SetOpponent(ulong clientId)
        {
            Opponent = clientId;
            IsHost = false;
            HasOpponent = true;
            ConnectedClients.Add(clientId);
            Game.Events.LobbyJoin(clientId, this);
        }

        public void ConnectPlayer(ulong clientId)
        {
            ConnectedClients.Add(clientId);
            Game.Events.LobbyJoin(clientId, this);
        }
        
        public bool HasPlayer(ulong clientId)
        {
            if (ConnectedClients.Contains(clientId)) return true;
            return false;
        }

        public void SetOpponentReady(bool isReady)
        {
            OpponentIsReady = isReady;
        }

        public void Start()
        {
            Game.Events.LobbyStart(this);
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            
        }
    }
}
