using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using Polly;
using RestSharp;

namespace Doorfail.Client;

public class RateLimitedClient :RestClient
{
    private Dictionary<double, int> _rateLimitThresholds;
    private int maxRetryAttempts = 3;

    public int RateLimit { get; private set; }
    public int RateLimitRemaining { get; private set; }

    public RateLimitedClient(RestClientOptions options, Dictionary<double, int> rateLimitThresholds, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) : base(options, configureDefaultHeaders, configureSerialization, useClientFactory)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(Dictionary<double, int> rateLimitThresholds, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) : base(configureRestClient, configureDefaultHeaders, configureSerialization, useClientFactory)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(string baseUrl, Dictionary<double, int> rateLimitThresholds, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null) : base(baseUrl, configureRestClient, configureDefaultHeaders, configureSerialization)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(HttpClient httpClient, RestClientOptions? options, Dictionary<double, int> rateLimitThresholds, bool disposeHttpClient = false, ConfigureSerialization? configureSerialization = null) : base(httpClient, options, disposeHttpClient, configureSerialization)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(HttpClient httpClient, Dictionary<double, int> rateLimitThresholds, bool disposeHttpClient = false, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null) : base(httpClient, disposeHttpClient, configureRestClient, configureSerialization)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(HttpMessageHandler handler, Dictionary<double, int> rateLimitThresholds, bool disposeHandler = true, ConfigureRestClient? configureRestClient = null, ConfigureSerialization? configureSerialization = null) : base(handler, disposeHandler, configureRestClient, configureSerialization)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    public RateLimitedClient(Uri baseUrl, Dictionary<double, int> rateLimitThresholds, ConfigureRestClient? configureRestClient = null, ConfigureHeaders? configureDefaultHeaders = null, ConfigureSerialization? configureSerialization = null, bool useClientFactory = false) : base(baseUrl, configureRestClient, configureDefaultHeaders, configureSerialization, useClientFactory)
    {
        _rateLimitThresholds = rateLimitThresholds;
        _rateLimitThresholds.TryAdd(-1.0, 5);
    }

    private void EnforceRateLimit(RestResponse response)
    {
        RateLimitRemaining = int.Parse(response.Headers.FirstOrDefault(h => h.Name == "X-RateLimit-Remaining")?.Value.ToString());
        RateLimit = int.Parse(response.Headers.FirstOrDefault(h => h.Name == "X-RateLimit-Limit")?.Value.ToString());

        foreach(var threshold in _rateLimitThresholds.OrderByDescending(t => t.Key))
        {
            if((double)RateLimitRemaining / RateLimit < threshold.Key)
            {
                // Pause execution
                Thread.Sleep(threshold.Value);
                break;
            }
        }
    }

    public async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken cancellationToken = default)
    {
        RestResponse response = null;
        var retryPolicy = Policy.HandleResult<HttpResponseMessage>(message => message.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(maxRetryAttempts, retryAttempt => TimeSpan.FromSeconds(_rateLimitThresholds[-1.0]));

        await retryPolicy.ExecuteAsync(async () =>
        {
            response = await base.ExecuteAsync(request, cancellationToken);

            var httpResponseMessage = new HttpResponseMessage(response.StatusCode)
            {
                Content = new StringContent(response.Content, Encoding.UTF8, response.ContentType),
                ReasonPhrase = response.StatusDescription,
                RequestMessage = new HttpRequestMessage(new HttpMethod(request.Method.ToString()), response.ResponseUri),
                StatusCode = response.StatusCode,
            };

            foreach(var header in response.Headers)
            {
                httpResponseMessage.Headers.TryAddWithoutValidation(header.Name, header.Value.ToString());
            }

            return httpResponseMessage;
        });

        EnforceRateLimit(response);

        return response;
    }
}

