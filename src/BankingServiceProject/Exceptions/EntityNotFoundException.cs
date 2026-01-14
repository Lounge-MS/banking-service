namespace BankingServiceProject.Exceptions;

public class EntityNotFoundException : BankingServiceException
{
    public EntityNotFoundException()
        : base("Entity not found")
    {
    }
}