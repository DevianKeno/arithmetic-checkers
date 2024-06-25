namespace Damath.EOS.P2P
{
    /// <summary>
    /// Struct <c>ChatEntry</c> is used to store cached chat data in <c>UIPeer2PeerMenu</c>.
    /// </summary>
    public struct ChatEntry
    {
        /// <value>True if message was from local user</value>
        public bool isOwnEntry;

        /// <value> Cache for message entry </value>
        public string Message;
    }
}