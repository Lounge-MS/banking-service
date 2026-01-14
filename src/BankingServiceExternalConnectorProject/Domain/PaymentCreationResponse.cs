namespace BankingServiceProject.ExternalConnectorProject.Domain;

public record PaymentCreationResponse(
    string ConfirmationUrl,
    IMetainfo Metainfo)
{
}