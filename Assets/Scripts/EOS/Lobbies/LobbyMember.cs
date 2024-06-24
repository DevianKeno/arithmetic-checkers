using System.Collections.Generic;
using Epic.OnlineServices;

namespace Damath.EOS.Lobbies
{
    /// <summary>
    /// Class represents all Lobby Member properties
    /// </summary>
    public class LobbyMember
    {
        public EpicAccountId AccountId;
        public ProductUserId ProductId;

        public string DisplayName
        {
            get
            {
                MemberAttributes.TryGetValue(DisplayNameKey, out LobbyAttribute nameAttrib);
                return nameAttrib?.AsString ?? string.Empty;
            }
        }

        public const string DisplayNameKey = "DISPLAYNAME";

        public Dictionary<string, LobbyAttribute> MemberAttributes = new();

        public LobbyRTCState RTCState = new();
    }

}