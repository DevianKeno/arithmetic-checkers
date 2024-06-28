using Epic.OnlineServices;
using Epic.OnlineServices.Friends;

namespace Aritche.EOS.Friends
{
    /// <summary>
    /// Class <c>FriendData</c> stores Friend data.
    /// </summary>
    public class FriendData
    {
        public EpicAccountId LocalUserId;
        public EpicAccountId UserId;
        public ProductUserId UserProductUserId;
        public string Name;
        public FriendsStatus Status = FriendsStatus.NotFriends;
        public PresenceInfo Presence;

        public bool IsFriend()
        {
            return Status == FriendsStatus.Friends;
        }

        public bool IsOnline()
        {
            return Presence?.Status == Epic.OnlineServices.Presence.Status.Online;
        }
    }
}