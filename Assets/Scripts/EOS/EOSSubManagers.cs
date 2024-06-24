using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayEveryWare.EpicOnlineServices;

using Damath.EOS.Managers;

namespace Damath.EOS
{
    /// <summary>
    /// Wrapper for EOS sub managers.
    /// </summary>
    public class EOSSubManagers : MonoBehaviour
    {
        EOSUserInfoManager userInfoManager;
        public EOSUserInfoManager UserInfoManager => userInfoManager;
        EOSFriendsManager friendsManager;
        public EOSFriendsManager FriendsManager => friendsManager;
        EOSLobbyManager lobbyManager;
        public EOSLobbyManager LobbyManager => lobbyManager;

        internal void Initialize()
        {
            userInfoManager ??= Game.EOS.GetOrCreateManager<EOSUserInfoManager>();
            friendsManager ??= Game.EOS.GetOrCreateManager<EOSFriendsManager>();
            lobbyManager ??= Game.EOS.GetOrCreateManager<EOSLobbyManager>();
        }
    }
}