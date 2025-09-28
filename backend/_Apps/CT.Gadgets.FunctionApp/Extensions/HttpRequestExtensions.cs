using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Web;

namespace CT.Gadgets.FunctionApp.Extensions;

public static class HttpRequestExtensions
{
    public static T? GetQueryParameter<T>(this HttpRequestData req, string parameterName)
    {
        var queryParams = HttpUtility.ParseQueryString(req.Url.Query);
        var value = queryParams[parameterName];

        if (string.IsNullOrWhiteSpace(value))
            return default;

        if (typeof(T) == typeof(string))
            return (T)(object)value;

        if (typeof(T) == typeof(int) && int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var intVal))
            return (T)(object)intVal;

        if (typeof(T) == typeof(double) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleVal))
            return (T)(object)doubleVal;

        // fallback to JSON deserialization for complex types
        return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings());
    }

    public static async Task<T> GetModelFromRequestBodyAsync<T>(this HttpRequestData req)
    {
        using var sr = new StreamReader(req.Body);

        var body = await sr.ReadToEndAsync();
        var model = JsonConvert.DeserializeObject<T>(body);

        return model!;
    }
}
