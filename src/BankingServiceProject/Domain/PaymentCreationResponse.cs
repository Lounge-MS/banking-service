namespace BankingServiceProject.Domain;

public record PaymentCreationResponse(
    string ConfirmationUrl,
    IMetainfo Metainfo)
{
}