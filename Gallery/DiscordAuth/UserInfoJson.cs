namespace Gallery.DiscordAuth
{
    public class UserInfoJson
    {
        public string id;
        public string username = "";
        public string discriminator;
        public string avatar = "";
        public bool mfa_enabled = false;
        public string locale;
        public bool verified;
        public int flags = 0;
        public int premium_type = 0;
    }
}
