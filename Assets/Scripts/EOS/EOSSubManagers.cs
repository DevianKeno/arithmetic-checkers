using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayEveryWare.EpicOnlineServices;

using Damath.EOS.Managers;
using Epic.OnlineServices.Auth;

namespace Damath.EOS
{
    /// <summary>
    /// Wrapper for EOS sub managers.
    /// </summary>
    public class EOSSubManagers : MonoBehaviour
    {
        public EOSUserInfoManager UserInfo => Game.EOS.GetOrCreateManager<EOSUserInfoManager>();
        public EOSFriendsManager Friends => Game.EOS.GetOrCreateManager<EOSFriendsManager>();
        public EOSLobbyManager Lobbies => Game.EOS.GetOrCreateManager<EOSLobbyManager>();
        public EOSPeer2PeerManager P2P => Game.EOS.GetOrCreateManager<EOSPeer2PeerManager>();
    }
}