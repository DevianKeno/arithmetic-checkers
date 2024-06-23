using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Epic.OnlineServices;
using PlayEveryWare.EpicOnlineServices;
using PlayEveryWare.EpicOnlineServices.Samples;

namespace Damath.EOS
{
    public class EOSLobbyUI : MonoBehaviour
    {
        [SerializeField] GameObject lobbyContainer;

        [Header("Prefabs")]
        [SerializeField] GameObject lobbyEntryPrefab;


        void Start()
        {
            Game.EOSManagers.LobbyManager.AddNotifyMemberUpdateReceived(OnMemberUpdate);

        }

        public void CreateLobby()
        {
            
        }

        void OnMemberUpdate(string LobbyId, ProductUserId MemberId)
        {
            
        }

        void DisplayLobbyInfoUI()
        {

        }
    }
}