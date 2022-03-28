namespace CircuitBreakerIdea
{
    public class Operation : IOperation
    {
        public Operation(string descriptor)
        {
            Descriptor = descriptor;
        }

        public string Descriptor { get; private init; }
    }
}
