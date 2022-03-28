using System.Collections.Concurrent;

namespace CircuitBreakerIdea
{
    public class OperationSource
    {
        private readonly Random _random = new Random();
        private int _opCount = 0;
        private Timer _timer;
        private ConcurrentQueue<IOperation> _queue;

        public OperationSource()
        {
            _queue = new();
            _timer = new Timer(CreateNewOperations, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void CreateNewOperations(object? state)
        {
            for (var i = 0; i < _random.Next(0, 5); i++)
            {
                _queue.Enqueue(new Operation(_opCount++.ToString()));
            }
        }

        public IEnumerable<IOperation> GetNewOperations()
        {
            while (_queue.TryDequeue(out var operation))
            {
                yield return operation;
            }
        }
    }
}