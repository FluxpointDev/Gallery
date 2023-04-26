using Newtonsoft.Json;
using System;

namespace Gallery.Database
{
    public class ApiUser
    {
        [JsonProperty("id")]
        public string ID;
        private string RealToken;
        private string EncryptedToken;
        public string Token { get { return EncryptedToken; } set { EncryptedToken = value; RealToken = APICrypt.DecryptString(EncryptedToken); } }
        public string GetRealToken()
        {
            return RealToken;
        }
        private string RealPublicToken;
        private string EncryptedPublicToken;
        public string PublicToken { get { return EncryptedPublicToken; } set { EncryptedPublicToken = value; RealPublicToken = APICrypt.DecryptString(EncryptedPublicToken); } }
        public string GetRealPublicToken()
        {
            return RealPublicToken;
        }

        public DateTime Created;
        public bool Disabled = false;

        public bool IsOwner()
        {
            return ID == "190590364871032834";
        }
    }
}
