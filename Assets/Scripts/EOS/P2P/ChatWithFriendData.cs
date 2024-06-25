using System.Collections.Generic;

using Epic.OnlineServices;

namespace Damath.EOS.P2P
{
    /// <summary>
    /// Struct <c>ChatWithFriendData</c> is used to store cached friend chat data in <c>UIPeer2PeerMenu</c>.
    /// </summary>
    public struct ChatWithFriendData
    {
        /// <value> Queue of cached <c>ChatEntry</c> objects </value>
        public Queue<ChatEntry> ChatLines;
        /// <value> <c>FriendId</c> of remote friend </value>
        public ProductUserId FriendId;

        /// <summary> Constructor for creating a new local cache of chat entries.</summary>
        /// <param name="FriendId"><c>ProductUserId</c> of remote friend</param>
        public ChatWithFriendData(ProductUserId FriendId)
        {
            this.FriendId = FriendId;
            ChatLines = new Queue<ChatEntry>();
        }
    }
}