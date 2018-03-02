using System;

namespace Miya.Core.Entities.Session
{
    [Serializable()]
    public class SessionUserClaimModel
    {
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
