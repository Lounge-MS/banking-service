namespace BankingServiceProject.Entities.Dto;

public record PaymentCreationResponse(
    string ConfirmationUrl,
    IMetainfo Metainfo)
{
}