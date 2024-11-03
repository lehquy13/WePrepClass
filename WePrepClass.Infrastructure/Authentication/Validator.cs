using System.Security.Cryptography;
using System.Text;
using WePrepClass.Application.Interfaces;

namespace WePrepClass.Infrastructure.Authentication;

public class Validator : IValidator
{
    public string GenerateValidationCode()
    {
        using var rg = RandomNumberGenerator.Create();
        var rno = new byte[5];
        rg.GetBytes(rno);
        var randomValue = BitConverter.ToInt32(rno, 0);

        return randomValue.ToString();
    }

    public string HashPassword(string input)
    {
        // Calculate MD5 hash from input
        var inputBytes = Encoding.ASCII.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        // Convert byte array to hex string
        var sb = new StringBuilder();

        foreach (var hashByte in hashBytes)
        {
            sb.Append(hashByte.ToString("X2"));
        }

        return sb.ToString();
    }
}