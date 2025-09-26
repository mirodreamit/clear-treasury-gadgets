using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;

namespace CT.Gadgets.FunctionApp.Extensions;

public static class HttpRequestExtensions
{
    public static T? GetQueryParameter<T>(this HttpRequest req, string parameterName)
    {
        if (!req.Query.TryGetValue(parameterName, out var valueStr) || string.IsNullOrWhiteSpace(valueStr))
            return default;

        var value = valueStr.ToString();

        if (typeof(T) == typeof(string))
            return (T)(object)value;

        if (typeof(T) == typeof(int) && int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var intVal))
            return (T)(object)intVal;

        if (typeof(T) == typeof(double) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var doubleVal))
            return (T)(object)doubleVal;

        return JsonConvert.DeserializeObject<T>(value, new JsonSerializerSettings());
    }

    public static async Task<T> GetModelFromRequestBodyAsync<T>(this HttpRequest req)
    {
        using var sr = new StreamReader(req.Body);

        var body = await sr.ReadToEndAsync();
        var model = JsonConvert.DeserializeObject<T>(body);

        return model!;
    }
}
