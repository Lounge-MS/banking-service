namespace BankingServiceProject.Exceptions;

public class PrimaryKeyConflictException : BankingServiceException
{
    public PrimaryKeyConflictException() : base("Primary key already exists in the database")
    {
    }
}