# PollyPlayground
Code samples and my written examples with Polly. 

## CircuitBreakerIdea
Using circuit breaker and retry to wrap a service that's purposefully faulty. The
service exposes a `Reset()` operation.

The service fails operations 2% of the time, incrementing by 1% each operation 
until reset. 
