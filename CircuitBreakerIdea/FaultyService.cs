namespace CircuitBreakerIdea
{
    /// <summary>
    /// A purpose built "faulty" service, that fails more often until reset.
    /// </summary>
    public class FaultyService : IFooService
    {
        private const double BaseFailRate = .1;
        private readonly Random _random = new Random();
        private readonly ILogger<FaultyService> _logger;
        private double currentFailRate = BaseFailRate;

        public FaultyService(ILogger<FaultyService> logger)
        {
            _logger = logger;
        }

        public void Reset()
        {
            // envision this method initializing a new client, or taking 
            // some other action to attempt to get the service back to a 
            // healthy state.

            currentFailRate = BaseFailRate;
        }

        public void QueueOperation(IOperation op)
        {
            if (_random.NextDouble() < currentFailRate)
            {
                throw new IOException("Service failed.");
            }

            currentFailRate += 0.01;
            // code that queues the op.
            _logger.LogInformation("Op queued, {descriptor}", op.Descriptor);
        }
    }
}
