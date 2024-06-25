using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Damath.EOS.Managers;

namespace Damath.EOS
{
    /// <summary>
    /// Wrapper for EOS sub managers.
    /// </summary>
    public class EOSSubManagers : MonoBehaviour
    {
        public DiscordManager Discord => DiscordManager.Instance;
        public EOSUserInfoManager UserInfo => Game.EOS.GetOrCreateManager<EOSUserInfoManager>();
        public EOSFriendsManager Friends => Game.EOS.GetOrCreateManager<EOSFriendsManager>();
        public EOSLobbyManager Lobbies => Game.EOS.GetOrCreateManager<EOSLobbyManager>();
        public EOSPeer2PeerManager P2P => Game.EOS.GetOrCreateManager<EOSPeer2PeerManager>();
    }
}