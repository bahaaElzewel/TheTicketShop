using System.Net;
using Microsoft.Extensions.Http.Resilience;

public static class GrpcClientExtension
{
    public static IHttpStandardResiliencePipelineBuilder AddGrpcClientConfig<TClient>
        (this IServiceCollection services, IConfigurationSection serviceAdress) where TClient : class
    {
        return services.AddGrpcClient<TClient>(options =>
        {
            if (!string.IsNullOrEmpty(serviceAdress.Value))
            {
                options.Address = new Uri(serviceAdress.Value);
            }
        }).ConfigureChannel((sp, channelOptions) =>
        {
            channelOptions.MaxReceiveMessageSize = 10 * 1024 * 1024; // 10MB
            channelOptions.MaxSendMessageSize = 10 * 1024 * 1024; // 10MB
            channelOptions.UnsafeUseInsecureChannelCallCredentials = true;
        }).ConfigurePrimaryHttpMessageHandler(sp =>
        {
            return new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                KeepAlivePingPolicy = HttpKeepAlivePingPolicy.WithActiveRequests
            };
        }).AddCallCredentials((context, metadata, serviceProvider) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext is not null)
            {
                // Forward Authorization header
                if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
                {
                    metadata.Add("Authorization", authHeader.ToString());
                }

                // Forward Origin header
                if (httpContext.Request.Headers.TryGetValue("Origin", out var originHeader))
                {
                    metadata.Add("Origin", originHeader.ToString());
                }
            }

            return Task.CompletedTask;
        }).AddStandardResilienceHandler(options =>
        {
            options.Retry.MaxRetryAttempts = 3;
            options.Retry.Delay = TimeSpan.FromSeconds(2);
            options.Retry.ShouldHandle = args =>
            {
                return ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode == HttpStatusCode.RequestTimeout ||
                    args.Outcome.Result?.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    args.Outcome.Result?.StatusCode == HttpStatusCode.InternalServerError
                );
            };

            // options.Timeout.Timeout = TimeSpan.FromSeconds(10);

            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
            options.CircuitBreaker.MinimumThroughput = 5;
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
        });
    }
}