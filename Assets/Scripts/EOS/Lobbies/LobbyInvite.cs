using Epic.OnlineServices;
using Epic.OnlineServices.Lobby;

namespace Aritche.EOS.Lobbies
{
    /// <summary>
    /// Class represents all Lobby Invite properties
    /// </summary>
    public class LobbyInvite
    {
        public Lobby Lobby = new();
        public LobbyDetails LobbyInfo = new();
        public ProductUserId FriendId;
        public EpicAccountId FriendEpicId;
        public string FriendDisplayName;
        public string InviteId;

        public bool IsValid()
        {
            return Lobby.IsValid();
        }

        public void Clear()
        {
            Lobby.Clear();
            LobbyInfo.Release();
            FriendId = new ProductUserId();
            FriendEpicId = new EpicAccountId();
            FriendDisplayName = string.Empty;
            InviteId = string.Empty;
        }
    }
}