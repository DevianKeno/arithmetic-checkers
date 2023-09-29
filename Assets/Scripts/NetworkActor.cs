using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

namespace Damath
{
    public class NetworkActor : NetworkBehaviour
    {
        void Awake()
        {
            Game.Events.OnNetworkSend += NetworkSend;

            if (IsServer)
            {
                // Subscribe network commands which can only be accessed by the server
                Game.Events.OnChatSend += ReceiveChatRpc;
            }
        }

        void OnDisable()
        {
            Game.Events.OnNetworkSend -= NetworkSend;
        }

        void NetworkSend(string data)
        {
            NetworkSendRpc(data);
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
            }
        }
        
        [ObserversRpc(ExcludeOwner = false)]
        void ReceiveChatRpc(NetworkConnection target, string data)
        {
            Pack type = Parser.Parse(data, out string[] args);

            if(IsOwner) Game.Console.Log($"<{args[1]}> {args[2]}");
        }
    }
}
