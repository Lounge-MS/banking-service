namespace BankingServiceProject.RepositoryProject.Exceptions;

public abstract class BankingServiceRepositoryException : Exception
{
    protected BankingServiceRepositoryException(
        string message = "Exception occured in repository")
        : base(message)
    {
    }
}