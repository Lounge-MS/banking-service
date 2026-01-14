namespace BankingServiceProject.SharedProject.Security.Cryptography;

public record AesEncryptedData(
    byte[] EncryptedData,
    byte[] Iv);