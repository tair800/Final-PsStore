using Final.Core.Entities;

namespace Final.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GetToken(string secretKey, string audience, string issuer, User user, IList<string> roles);

    }
}
