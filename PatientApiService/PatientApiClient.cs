using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientApiService.Models;
using PatientApiService.Options;
using PatientApiService.Utilities;

namespace PatientApiService;

public class PatientApiClient : IPatientApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<PatientApiOptions> _options;
    private readonly ILogger<PatientApiClient> _logger;
    
    private static JsonSerializerOptions? _serializerOptions;

    public PatientApiClient(IOptions<PatientApiOptions> options, 
        IHttpClientFactory httpClientFactory, 
        ILogger<PatientApiClient> logger)
    {
        _options = options;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// creates or updates a patient with the given model
    /// </summary>
    /// <param name="clientId">client associated to the patientId</param>
    /// <param name="patientId">identifier assigned to the upserted patient record</param>
    /// <returns>patient model associated with given identifier</returns>
    public async Task<Patient?> GetPatientAsync(string clientId, Guid patientId)
    {
        return await SendAsync<Patient?, object>(
            HttpMethod.Post, 
            $"/api/patients/{clientId}/info", 
            new { Id = patientId }
            );
    }
    
    /// <summary>
    /// creates or updates a patient with the given model
    /// </summary>
    /// <param name="clientId">client associated to the patient</param>
    /// <param name="patient">patient model</param>
    /// <returns>identifier assigned to the upserted patient record</returns>
    public async Task<Guid?> UpsertPatientAsync(string clientId, Patient patient)
    {
        return await SendAsync<Guid, Patient>(
            HttpMethod.Post,
            $"/api/patients/{clientId}",
            patient
            );
    }

    /// <summary>
    /// sends an http request to the patient api
    /// </summary>
    /// <param name="httpMethod"></param>
    /// <param name="relativeRequestUrl"></param>
    /// <param name="payload"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception">thrown if the patient api returns an unsuccessful status code or a server error</exception>
    private async Task<TResult?> SendAsync<TResult, TPayload>(HttpMethod httpMethod, string relativeRequestUrl,
        TPayload? payload = default)
    {
        // build request
        var requestString = _options.Value.BaseUrl + relativeRequestUrl;

        using var requestMessage = new HttpRequestMessage(httpMethod, string.Format(requestString));
        if (payload != null)
        {
            var stringContent = new StringContent(JsonSerializer.Serialize(payload, SerializerOptions), Encoding.UTF8, "application/json");
            requestMessage.Content = stringContent;
        }

        // add headers
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($@"{_options.Value.Username}:{_options.Value.Password}")));
        requestMessage.Headers.Add("X-Loyal-ProductId", "outreach-importer");

        // send request
        using var httpClient = _httpClientFactory.CreateClient(nameof(PatientApiClient));
        using var response = await httpClient.SendAsync(requestMessage);

        try
        {
            await ApiHelper.HandleResponseStatus(response, SerializerOptions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unsuccessful response from Patient API");
            throw;
        }
        
        // parse response
        var responseString = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(responseString))
            return default;
        
        var result = JsonSerializer.Deserialize<TResult?>(responseString, SerializerOptions);

        return result;
    }
    
    /// <summary>
    /// create default serializer options for the patient api requests
    /// </summary>
    private static JsonSerializerOptions SerializerOptions
    {
        get
        {
            if (_serializerOptions == null)
            {
                _serializerOptions = new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                _serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            }

            return _serializerOptions;
        }
    }
}