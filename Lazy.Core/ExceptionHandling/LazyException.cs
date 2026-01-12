namespace Lazy.Core.ExceptionHandling;

public class LazyException : Exception, IUserFriendlyException
{
    public virtual string Code { get; set; }
    public string ResponseMessage { get; set; }

    public LazyException()
    {

    }

    public LazyException(string message) : base(message)
    {

    }


    public LazyException(string message, string responseMessage) : base(message)
    {
        this.ResponseMessage = responseMessage;
    }

}
