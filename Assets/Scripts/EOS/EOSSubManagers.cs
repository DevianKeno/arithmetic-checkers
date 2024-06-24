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
        public EOSUserInfoManager UserInfoManager => Game.EOS.GetOrCreateManager<EOSUserInfoManager>();
        public EOSFriendsManager FriendsManager => Game.EOS.GetOrCreateManager<EOSFriendsManager>();
        public EOSLobbyManager LobbyManager => Game.EOS.GetOrCreateManager<EOSLobbyManager>();
    }
}