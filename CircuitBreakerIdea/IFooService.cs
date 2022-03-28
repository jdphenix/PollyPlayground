namespace CircuitBreakerIdea
{
    public interface IFooService : IService
    {
        void QueueOperation(IOperation op);
    }
}
