using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Models;
using CT.Gadgets.FunctionApp.Interfaces;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;

namespace CT.Gadgets.FunctionApp.Services;

public class HttpRequestProcessingService : IHttpRequestProcessingService
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Converters = { new StringEnumConverter() },
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };
    
    public async Task<HttpResponseData> ProcessHttpRequestAsync( 
        HttpRequestData req,
        Func<Task<IBaseOutput>> f,
        ILogger log)
    {
        try
        {
            var response = await f();

            return response.Result switch
            {
                OperationResult.InternalError => await CreateErrorResult(req, response, log),
                OperationResult.BadRequest => await CreateErrorResult(req, response, log),
                OperationResult.Unauthorized => await CreateErrorResult(req, response, log),
                OperationResult.Forbidden => await CreateErrorResult(req, response, log),
                OperationResult.NotFound => await CreateErrorResult(req, response, log),
                OperationResult.Conflict => await CreateErrorResult(req, response, log),
                _ => await CreateJsonResult(req, response, GetHttpStatusCode(response.Result))
            };
        }
        catch (JsonSerializationException ex)
        {
            return await CreateErrorResult(req,
                new BaseOutput<string>(OperationResult.BadRequest, $"JsonFormat error. [{ex.Message}]", ex),
                log);
        }
        catch (JsonReaderException ex)
        {
            return await CreateErrorResult(req,
                new BaseOutput<string>(OperationResult.BadRequest, $"JsonFormat error. [{ex.Message}]", ex),
                log);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception occurred");

            var response = new BaseOutput<string>(
                OperationResult.InternalError,
                $"Unhandled error. [{ex.Message}]",
                new ApplicationError(ex));

            return await CreateErrorResult(req, response, log);
        }
    }

    private static async Task<HttpResponseData> CreateErrorResult(HttpRequestData req, IBaseOutput response, ILogger log)
    {
        string message = response.Error is IError error
            ? error.GetUserFriendlyMessage()
            : response.Message ?? "An error occurred";

        log.LogError(message);

        var res = new BaseOutput<IBaseOutput>(response.Result, null!, message);

        return await CreateJsonResult(req, res, GetHttpStatusCode(response.Result));
    }

    private static async Task<HttpResponseData> CreateJsonResult(HttpRequestData req, object obj, int statusCode)
    {
        var json = JsonConvert.SerializeObject(obj, JsonSettings);

        var httpResponse = req.CreateResponse((HttpStatusCode)statusCode);
        httpResponse.Headers.Add("Content-Type", "application/json");
        await httpResponse.WriteStringAsync(json);

        return httpResponse;
    }

    private static int GetHttpStatusCode(OperationResult result)
    {
        return result switch
        {
            OperationResult.Ok or OperationResult.Updated or OperationResult.Deleted => (int)HttpStatusCode.OK,
            OperationResult.Created => (int)HttpStatusCode.Created,
            OperationResult.NotFound => (int)HttpStatusCode.NotFound,
            OperationResult.BadRequest => (int)HttpStatusCode.BadRequest,
            OperationResult.Unauthorized => (int)HttpStatusCode.Unauthorized,
            OperationResult.Forbidden => (int)HttpStatusCode.Forbidden,
            OperationResult.InternalError => (int)HttpStatusCode.InternalServerError,
            OperationResult.Conflict => (int)HttpStatusCode.Conflict,
            _ => (int)HttpStatusCode.OK
        };
    }
}
