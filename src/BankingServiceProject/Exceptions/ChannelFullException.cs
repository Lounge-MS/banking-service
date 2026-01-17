namespace BankingServiceProject.Exceptions;

public class ChannelFullException : BankingServiceException
{
    public ChannelFullException(int channelSize)
        : base($"Channel is full ({channelSize})")
    {
    }
}