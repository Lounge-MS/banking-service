namespace BankingServiceProject.RepositoryProject.Exceptions;

public class EmptyReaderException : BankingServiceRepositoryException
{
    public EmptyReaderException() : base("Reader is empty")
    {
    }
}