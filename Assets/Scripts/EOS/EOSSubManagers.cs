using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Damath.EOS
{
    /// <summary>
    /// Wrapper for EOS sub managers.
    /// </summary>
    public class EOSSubManagers : MonoBehaviour
    {
        EOSFriendsManager friendsManager;
        public EOSFriendsManager FriendsManager => friendsManager;
        EOSLobbyManager lobbyManager;
        public EOSLobbyManager LobbyManager => lobbyManager;

        internal void Initialize()
        {
            lobbyManager ??= Game.EOS.GetOrCreateManager<EOSLobbyManager>();
            friendsManager ??= Game.EOS.GetOrCreateManager<EOSFriendsManager>();
        }
    }
}