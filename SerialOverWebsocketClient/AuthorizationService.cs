using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace SerialOverWebsocketClient;

public class AuthorizationService
{
    private readonly AuthorizationOptions options;

    public AuthorizationService(IOptions<AuthorizationOptions> _options)
    {
        options = _options.Value;
    }

    public byte[]? CreateVerifyToken(byte[] token)
    {
        var key = Convert.FromBase64String(options.SecretKey);
        using var hash = new HMACSHA256(key);
        return hash.ComputeHash(token);
    }
}