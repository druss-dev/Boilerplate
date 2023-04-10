using System.Net;
using System.Text.Json;
using Loyal.Core.Exceptions;

namespace PatientApiService.Utilities;

public static class ApiHelper
{
    /// <summary>
    /// helper method to handle unexpected results from an api call
    /// </summary>
    /// <param name="response">response from http client request</param>
    /// <param name="serializerOptions">configuration options for deserialization in the event of an exception</param>
    /// <exception cref="HttpRequestException">thrown if the api returns an unsuccessful status code or a server error</exception>
    /// <exception cref="JsonException">thrown in the event the server exception is unable to be deserialized</exception>
    public static async Task HandleResponseStatus(HttpResponseMessage response, JsonSerializerOptions serializerOptions)
    {
        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            try
            {
                // if an exception occurred while calling the api, try to parse the exception information from the response
                var payload =
                    JsonSerializer.Deserialize<ExceptionPayload>(await response.Content.ReadAsStringAsync(), serializerOptions);

                if (payload?.Message != null && payload.StackTrace != null)
                    throw new HttpRequestException($"{payload.Message}: {payload.StackTrace}");
            }
            catch (JsonException ex)
            {
                throw new JsonException("Exception occurred while parsing ExceptionPayload", ex);
            }
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"API request failed with status code: {response.StatusCode}");
        }
    }
}