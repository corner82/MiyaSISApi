using Miya.Core.Token.Abstract;

namespace Miya.Core.Http.HttpRequest.Abstract.Token
{
    public interface IReguestTokenProvider
    {
        RequestBuilderBase AddTokenCreator(ITokenCreater tokenCreator);
    }
}
