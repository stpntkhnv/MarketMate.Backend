namespace MarketMate.Infrastructure.HttpClients.Handlers;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore;
    private readonly int _requestsPerSecond;
    private readonly TimeSpan _resetInterval;
    private int _requestCount;
    private DateTime _resetTime;

    public RateLimitingHandler(HttpMessageHandler innerHandler, int requestsPerSecond) : base(innerHandler)
    {
        _requestsPerSecond = requestsPerSecond;
        _resetInterval = TimeSpan.FromSeconds(1);
        _semaphore = new SemaphoreSlim(1, 1);
        _resetTime = DateTime.UtcNow.Add(_resetInterval);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (DateTime.UtcNow >= _resetTime)
            {
                _resetTime = DateTime.UtcNow.Add(_resetInterval);
                _requestCount = 0;
            }

            _requestCount++;

            if (_requestCount > _requestsPerSecond)
            {
                var delay = _resetTime - DateTime.UtcNow;
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
