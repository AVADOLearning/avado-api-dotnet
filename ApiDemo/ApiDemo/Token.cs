using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace ApiDemo
{
    [DataContract]
    public sealed class Token
    {
        public const string CREDENTIALS_HEADER = "Authenticate";
        public const string CREDENTIALS_NAMESPACE = "";
        public const string APIKEY_HEADER = "apikey";

        [DataMember]
        public Guid AuthToken { get; set; }

        public MessageHeader ToMessageHeader()
        {
            var lHeader = MessageHeader.CreateHeader(CREDENTIALS_HEADER, CREDENTIALS_NAMESPACE, this);
            return lHeader;
        }
    }
}
