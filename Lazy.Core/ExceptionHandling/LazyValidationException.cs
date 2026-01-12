namespace Lazy.Core.ExceptionHandling;

public class LazyValidationException : LazyException
{
    public override string Code { get; set; } = "100003";

    public LazyValidationException(string message) : base(message)
    {

    }


    public LazyValidationException( string message, string responseMessage) : base(message)
    {
        this.ResponseMessage = responseMessage;
    }
}
