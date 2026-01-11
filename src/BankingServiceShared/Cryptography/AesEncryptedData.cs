namespace BankingServiceProject.SharedProject.Cryptography;

public record AesEncryptedData(
    byte[] EncryptedData,
    byte[] Iv);