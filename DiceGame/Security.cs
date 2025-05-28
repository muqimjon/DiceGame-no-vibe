namespace DiceGame;

using SHA3.Net;
using System.Security.Cryptography;
using System.Text;

public class Security
{
    private readonly int value;
    private readonly byte[] key;
    private readonly byte[] hmac;

    public Security(int max)
    {
        value = GenerateRandomValue(max);
        key = GenerateKey();
        hmac = ComputeSha3WithKeyAndValue(value, key);
    }

    public string GetHmac()
        => Convert.ToHexString(hmac).ToUpperInvariant();

    public string GetKey()
        => Convert.ToHexString(key).ToUpperInvariant();

    public int GetValue()
        => value;

    private static int GenerateRandomValue(int max)
        => RandomNumberGenerator.GetInt32(0, max);

    private static byte[] ComputeSha3WithKeyAndValue(int value, byte[] key)
    {
        var message = Encoding.UTF8.GetBytes(value.ToString());
        var input = key.Concat(message).ToArray();
        return Sha3.Sha3256().ComputeHash(input);
    }

    private static byte[] GenerateKey()
    {
        var key = new byte[32];
        RandomNumberGenerator.Fill(key);
        return key;
    }
}
