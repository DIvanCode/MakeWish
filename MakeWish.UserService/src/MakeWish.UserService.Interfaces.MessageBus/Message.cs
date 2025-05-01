namespace MakeWish.UserService.Interfaces.MessageBus;

public abstract class Message
{
    public abstract string Type { get; }
    public abstract object Payload { get; }
}