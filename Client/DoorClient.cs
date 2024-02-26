using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Text.Json;
using NLog;
using RestSharp;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Doorfail.Core.Client;

public partial class DoorClient :RestClient
{
    #region Fields

    private const int MAX_NO_CONTENT_RETRY = 5;
    private const int MAX_TIMEOUT = 100000;
    private const int MS_TILL_NO_CONTENT_RETRY = 60000;

    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly Func<Type, JsonSerializerOptions>? _optionsGenerator;

    #endregion Fields

    #region Constructors

    /// <summary></summary>
    /// <param name="client">Dependency Injected HttpClient</param>
    /// <param name="url">API's base URL</param>
    /// <param name="timeoutMS">Timeout in MS to wait for a response</param>
    /// <param name="controllerType">example: `PlanController`</param>
    public DoorClient(HttpClient client, int? timeoutMS = null, Type controllerType = null, Func<Type, JsonSerializerOptions>? serializerOptions = null) : base(client, GetClientOptions(client.BaseAddress?.ToString(), timeoutMS))
    {
        if(string.IsNullOrEmpty(client.BaseAddress.ToString()))
            throw new ArgumentException("BaseAddress cannot be empty!", nameof(client.BaseAddress));
        if(controllerType == null)
            return;
        _service = controllerType;
        _optionsGenerator = serializerOptions;
    }

    #endregion Constructors

    #region Properties

    public string Base { get; set; }

    /// <summary>
    /// Cleared after each usage
    /// </summary>
    public int? OverrideTimeout { private get; set; }

    private Type _service { get; set; }

    private int Timeout
    {
        get => OverrideTimeout ?? Options.MaxTimeout;
    }

    #endregion Properties

    #region Methods

    public RestRequest AddJsonBody(RestRequest request, object body = null)
    {
        if(body is null)
            return request;

        //TODO pass in a serializer that can handle special cases instead of doing the conversions here
        var t = body.GetType();
        if(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var key = t.GetGenericArguments()[0];
            if(key.IsGenericType && new HashSet<Type>(new Type[] { typeof(ValueTuple<>), typeof(ValueTuple<,>) }).Contains(key.GetGenericTypeDefinition()))
            {
                Dictionary<string, object> stringDict = [];
                foreach(var k in ((IDictionary)body).Keys)
                {
                    stringDict.Add(JsonSerializer.Serialize(k, new JsonSerializerOptions { IncludeFields = true }), ((IDictionary)body)[k]);
                }
                request.AddJsonBody(stringDict);
                return request;
            }
        }

        request.AddJsonBody(body);
        return request;
    }

    protected RestRequest BuildRequest(string route, Method method, string name, object? value)
                => BuildRequest(route, method, new NameValueCollection { { name, value.ToString() } });

    protected RestRequest BuildRequest(string route, Method method, NameValueCollection parameters = default)
    {
        // Get TimeoutAttribute of the calling method
        //var timeoutAttribute = _service.GetMethods().Where(m => m.Name == action)
        //    .Max(callingMethod => ((TimeoutAttribute)Attribute.GetCustomAttribute(callingMethod, typeof(TimeoutAttribute)))?.Timeout);
        int? specifiedTimeout = null;

        //if(timeoutAttribute > 0)
        //{
        //    specifiedTimeout = timeoutAttribute * 1000;
        //}
        var request = new RestRequest(GetRoute(route), method)
        {
            RequestFormat = DataFormat.Json,
            //Timeout = specifiedTimeout ?? Timeout
        };
        request.AddHeader("content-type", "application/json");

        //if(specifiedTimeout is not null) // Add timeout to headers
        //{ request.AddHeader("timeout", specifiedTimeout.Value!.ToString()); }

        //Timeout: client seconds/database seconds, Method (GET) PensionPro/Plan/Get
        var logMessage = $"Timeout: c{request.Timeout / 1000}{(specifiedTimeout != null ? "/db" + specifiedTimeout / 1000 : string.Empty)}s, {request.Method} {request.Resource} {Environment.NewLine}";

        parameters ??= [];

        if((parameters?.Count ?? 0) != 0)
        {
            var keyList = parameters?.AllKeys.Where(w => w != null).ToList();
            keyList?.ForEach(p => request.AddQueryParameter(p, parameters[p]));

            logMessage += string.Join(", ", keyList.Select(s => $"\"{s}:{parameters[s]}\""));
        }

        OverrideTimeout = null; // Always clear the override per call, so the next call starts out fresh

        _logger.Debug(logMessage);

        return request;
    }

    /// <summary>
    /// Returns route if not empty. Returns controllers specified Route otherwise.
    ///
    ///
    /// </summary>
    /// <param name="route">Ignores Base routing if you supply a route starting with / example: /api/pensionpro/[controller]/[action]</param>
    /// <param name="action"></param>
    /// <returns></returns>
    protected string GetRoute(string route = default)
    {
        //TODO use
        if(route.StartsWith('|'))// Overwrite with controller info
        {
            route = RouteReader.GetRouteInformation(_service, route[1..]).RouteTemplate;
        }

        if(route.StartsWith('/'))//Overwrite defaults if starting with root '/'
            return route;
        return Base + route;

        //var routeDict = new RouteValueDictionary
        //{
        //    { "controller", controller },
        //    { "action" },
        //};
        //
        //return routeBinder.BindValues(routeDict);
    }

    private static RestClientOptions GetClientOptions(string url, int? timeoutMS)
    {

        return new(new Uri(url))
        {
            UseDefaultCredentials = true,
            MaxTimeout = (timeoutMS ?? 0) > 0 ? timeoutMS!.Value : MAX_TIMEOUT,
            //FollowRedirects = false,//
        };
    }

    [Obsolete]
    private static HttpClient GetHttpClient() => new(new HttpClientHandler()
    {
        Credentials = CredentialCache.DefaultNetworkCredentials,
    });

    private async Task<T> HandleResponse<T>(Task<RestResponse<T>> restResponse)
        => HandleResponse(await restResponse);

    private T HandleResponse<T>(RestResponse<T> restResponse)
    {
        //Content is always expected of type T
        if(restResponse.StatusCode == HttpStatusCode.NoContent)
            throw LogUnsucessfulResponse(restResponse);
        if(restResponse.IsSuccessful && restResponse.ErrorMessage is null)
            return restResponse.Data;//RestSharp parses Content into Data of type `T?`
        //Failed to parse with RestSharp, try JsonSerializer
        else if(_optionsGenerator is not null && restResponse.Content is not null)
        {
            try
            {
                var objResponse = JsonSerializer.Deserialize<T>(restResponse.Content, _optionsGenerator.Invoke(typeof(T)));
                return objResponse;
            } catch(JsonException)
            {
            }
        }

        throw LogUnsucessfulResponse(restResponse);
    }

    private async Task HandleResponse(Task<RestResponse> restResponse) => HandleResponse(await restResponse);

    private void HandleResponse(RestResponse restResponse)
    {
        if(!restResponse.IsSuccessful)
            throw LogUnsucessfulResponse(restResponse);
    }

    private Exception LogUnsucessfulResponse(RestResponse restResponse)
    {//TODO
        _logger.Debug($"{restResponse.Request.Method} {Options.BaseUrl}/{restResponse.Request.Resource} Response:{Environment.NewLine}{restResponse.Content}");
        // var ex = restResponse.AsException();
        // _logger.Error(ex);
        return null;//return ex;
    }

    #endregion Methods
}