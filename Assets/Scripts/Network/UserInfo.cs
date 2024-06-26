using System;

namespace Damath.Network
{
    [Serializable]
    public class UserInfo
    {
        public string id;
        public string username;
        public string avatar;
        public string discriminator;
        public int public_flags;
        public int flags;
        public string banner;
        public int accent_color;
        public string global_name;
        public string avatar_decoration_data;
        public string banner_color;
        public string clan;
        public bool mfa_enabled;
        public string locale;
        public int premium_type;
    }
}