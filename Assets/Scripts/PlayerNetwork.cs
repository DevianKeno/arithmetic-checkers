using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

namespace Damath
{
    public class PlayerNetwork : NetworkBehaviour
    {

        public Player PlayerObject;
        // Start is called before the first frame update
        void Start()
        {
            Game.Events.OnLobbyStart += InitOnline;

            Init();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDisable()
        {
            // Game.Events.OnPieceMove -= IMadeAMove;
            Game.Events.OnLobbyStart -= InitOnline;
        }

        public void InitOnline(Lobby lobby)
        {
            // Game.Events.OnPieceMove += IMadeAMove;
        }

        public void Init()
        {

        }
    }
}
