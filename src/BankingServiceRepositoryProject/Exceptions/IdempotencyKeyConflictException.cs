namespace BankingServiceProject.RepositoryProject.Exceptions;

public class IdempotencyKeyConflictException : BankingServiceRepositoryException
{
    public IdempotencyKeyConflictException() : base("Idempotency key already exists in the database")
    {
    }
}