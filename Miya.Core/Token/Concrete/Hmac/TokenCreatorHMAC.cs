using System;
using Miya.Core.Token.Abstract;

namespace Miya.Core.Token.Concrete.Hamc
{
    class TokenCreatorHMAC : ITokenCreater, ITokenArgs
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string Salt { get; set; }

        public string CreateToken()
        {
            throw new NotImplementedException();
        }
    }
}
