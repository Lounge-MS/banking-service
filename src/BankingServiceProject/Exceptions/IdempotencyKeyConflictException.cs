namespace BankingServiceProject.Exceptions;

public class IdempotencyKeyConflictException : BankingServiceException
{
    public IdempotencyKeyConflictException() : base("Idempotency key already exists in the database")
    {
    }
}