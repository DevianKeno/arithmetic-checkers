using Epic.OnlineServices.Presence;

namespace Aritche.EOS.Friends
{
    /// <summary>
    /// Class <c>PresenceInfo</c> stores Player Presence data.
    /// </summary>
    public class PresenceInfo
    {
        public Status Status = Status.Offline;
        public string RichText;
        public string Application;
        public string Platform;
        public string JoinInfo;
    }
}