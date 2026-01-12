namespace Lazy.Core.ExceptionHandling;

public class LazyAuthorizationException : LazyException
{
    public override string Code { get; set; } = "100004";
    public LazyAuthorizationException(string message) : base(message)
    {

    }

    public LazyAuthorizationException(string message, string responseMessage) : base(message)
    {
        this.ResponseMessage = responseMessage;
    }
}
