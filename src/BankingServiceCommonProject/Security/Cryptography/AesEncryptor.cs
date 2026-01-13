using System.Security.Cryptography;
using System.Text;

namespace BankingServiceProject.CommonProject.Security.Cryptography;

public class AesEncryptor
{
    public AesEncryptedData Encrypt(string plain, byte[] key)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.GenerateIV();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] bytes = Encoding.UTF8.GetBytes(plain);
        byte[] cipherBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length);

        return new AesEncryptedData(cipherBytes, aes.IV);
    }

    public string Decrypt(AesEncryptedData encryptedData, byte[] key)
    {
        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.IV = encryptedData.Iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] cipherBytes = encryptedData.EncryptedData;
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
}