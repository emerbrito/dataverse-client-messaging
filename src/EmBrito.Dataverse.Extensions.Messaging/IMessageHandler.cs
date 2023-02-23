namespace EmBrito.Dataverse.Extensions.Messaging
{
    public interface IMessageHandler<in TMessage>
    {
        bool CanHandle(TMessage message);
        Task Handle(TMessage message, CancellationToken cancellationToken);
    }
}