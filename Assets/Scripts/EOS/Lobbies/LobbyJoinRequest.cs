using Epic.OnlineServices.Lobby;

namespace Aritche.EOS.Lobbies
{
    /// <summary>
    /// Class represents a request to Join a lobby
    /// </summary>
    public class LobbyJoinRequest
    {
        string Id = string.Empty;
        LobbyDetails LobbyInfo = new();

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Id);
        }

        public void Clear()
        {
            Id = string.Empty;
            LobbyInfo = new LobbyDetails();
        }
    }
}