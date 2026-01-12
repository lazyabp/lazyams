namespace Lazy.Core.ExceptionHandling;

public class EntityNotFoundException : LazyException
{
    public override string Code { get; set; } = "100001";
    public EntityNotFoundException(string message) : base(message)
    {

    }

    public EntityNotFoundException(string message, string responseMessage) : base(message)
    {
        this.ResponseMessage = responseMessage;
    }
}
