namespace BankingServiceCallbackHandlerProject.Exceptions;

public class ChannelFullException : BankingServiceCallbackHandlerException
{
    public ChannelFullException(int channelSize)
        : base($"Channel is full ({channelSize})")
    {
    }
}