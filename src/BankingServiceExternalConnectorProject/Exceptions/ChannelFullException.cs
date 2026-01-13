namespace BankingServiceProject.ExternalConnectorProject.Exceptions;

public class ChannelFullException : BankingServiceExternalConnectorException
{
    public ChannelFullException(int channelSize)
        : base($"Channel is full ({channelSize})")
    {
    }
}