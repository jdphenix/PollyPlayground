using CircuitBreakerIdea;
using Polly;

static ISyncPolicy GetCircuitBreakerPolicy<TService>(IServiceProvider c) where TService : notnull, IService
{
    return Policy
        .Handle<IOException>()
        .CircuitBreaker(
            exceptionsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(10),
            onBreak: (e, ts) => c.GetRequiredService<TService>().Reset(),
            onReset: () => c.GetRequiredService<ILogger<TService>>().LogInformation("Reset circuit breaker.")
        );
}

static ISyncPolicy GetRetryPolicy<TService>(IServiceProvider c) where TService: notnull, IService
{
    return Policy
        .Handle<IOException>()
        .WaitAndRetry(
            retryCount: 3,
            sleepDurationProvider: i => TimeSpan.FromSeconds(i),
            onRetry: (e, ts) => c.GetRequiredService<ILogger<TService>>().LogInformation("Operation failed, retrying."));
        
}

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();

        services.AddSingleton<ISyncPolicy>(c => GetRetryPolicy<IFooService>(c).Wrap(GetCircuitBreakerPolicy<IFooService>(c)));
        services.AddSingleton<IFooService, FaultyService>();
        services.AddSingleton<OperationSource>();
    })
    .Build();

await host.RunAsync();
