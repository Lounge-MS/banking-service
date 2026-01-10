namespace BankingServiceProject.RepositoryProject.Exceptions;

public class PrimaryKeyConflictException : BankingServiceRepositoryException
{
    public PrimaryKeyConflictException() : base("Primary key already exists in the database")
    {
    }
}