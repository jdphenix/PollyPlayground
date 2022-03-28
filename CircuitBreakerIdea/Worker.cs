using Polly;
using Polly.CircuitBreaker;
using System.Collections.Concurrent;

namespace CircuitBreakerIdea
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IFooService _fooService;
        private readonly OperationSource _source;
        private readonly ISyncPolicy _executionPolicy;
        private readonly ConcurrentQueue<IOperation> _opQueue;

        public Worker(ILogger<Worker> logger, IFooService fooService, OperationSource source, ISyncPolicy executionPolicy)
        {
            _logger = logger;
            _fooService = fooService;
            _source = source;
            _executionPolicy = executionPolicy;
            _opQueue = new();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                while (_opQueue.TryDequeue(out IOperation? queuedOp))
                {
                    await QueueOperation(queuedOp, stoppingToken);
                }

                var newOperations = _source.GetNewOperations();
                foreach (var op in newOperations)
                {
                    await QueueOperation(op, stoppingToken);
                }
            }
        }

        private async Task QueueOperation(IOperation op, CancellationToken stoppingToken)
        {
            try
            {
                _executionPolicy.Execute(() => _fooService.QueueOperation(op));
            }
            catch (IOException)
            {
                _logger.LogWarning("Error during queue operation, will retry.");
            }
            catch (BrokenCircuitException)
            {
                _logger.LogError("Circuit breaker is open.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                _opQueue.Enqueue(op);
            }
        }
    }
}