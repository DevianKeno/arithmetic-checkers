using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;

namespace Damath
{
    public class Network : NetworkBehaviour
    {
        public static Network Main { get; private set; }
        public Lobby Lobby;
        private int[] lobbyList;
        private List<Lobby> lobbies;
        public string localIp; 
        public bool EnableDebug = true;

        private void Awake()
        {
            
        }
        void Start()
        {
            Game.Events.OnServerSend += SendServerData;
            if(IsServer) Game.Events.OnObserverSend += SendObserverRpc;
        }

        void SendServerData(string data)
        {
            SendServerRpc(data);
        }

        //Every client should be able to send message to the server
        //However, only the owner of the object can send the rpc to the server.
        [ServerRpc(RequireOwnership = true)]
        void SendServerRpc(string data, NetworkConnection conn = null)
        {
            //temporary parser
            string[] args = data.Split(';');
            if (args[0].Equals("c"))
            {
                Game.Events.ObserverSend($"c;{conn.ClientId};{args[2]};");
            }
            
            
        }

        //only accessible when the owner is also a server
        [ObserversRpc(ExcludeOwner = false)]
        void SendObserverRpc(string data)
        {
            //only the owner should receive the observerRpc
            if (base.IsOwner) {
                //temporary parser
                string[] args = data.Split(';');
                if (args[0].Equals("c"))
                {
                    Game.Console.Log($"<{args[1]}>: {args[2]}");
                }
                
            }
       
        }

        void onDestroy()
        {
            Game.Events.OnServerSend -= SendServerData;
            if (base.IsServer) Game.Events.OnObserverSend -= SendObserverRpc;
        }

    }


}
