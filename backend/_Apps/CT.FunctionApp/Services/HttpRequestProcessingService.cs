using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.Interfaces;
using CT.Application.Abstractions.Models;
using CT.Application.Models;
using CT.FunctionApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CT.FunctionApp.Services;

public class HttpRequestProcessingService : IHttpRequestProcessingService
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        Converters = 
        { 
            new StringEnumConverter() 
        },
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None
    };

    public async Task<IActionResult> ProcessHttpRequestAsync(Func<Task<IBaseOutput>> f, ILogger log)
    {
        try
        {
            var response = await f();

            return response.Result switch
            {
                OperationResult.InternalError => CreateErrorResult(response, log),
                OperationResult.BadRequest => CreateErrorResult(response, log),
                OperationResult.Unauthorized => CreateErrorResult(response, log),
                OperationResult.Forbidden => CreateErrorResult(response, log),
                OperationResult.NotFound => CreateErrorResult(response, log),
                _ => CreateJsonResult(response, GetHttpStatusCode(response.Result))
            };
        }
        catch (JsonSerializationException ex)
        {
            return CreateErrorResult(new BaseOutput<string>(OperationResult.BadRequest, $"JsonFormat error. [{ex.Message}]", ex), log);
        }
        catch (JsonReaderException ex)
        {
            return CreateErrorResult(new BaseOutput<string>(OperationResult.BadRequest, $"JsonFormat error. [{ex.Message}]", ex), log);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception occurred");
            var response = new BaseOutput<string>(OperationResult.InternalError, $"Unhandled error. [{ex.Message}]", new ApplicationError(ex));
            return CreateErrorResult(response, log);
        }
    }

    private static ContentResult CreateErrorResult(IBaseOutput response, ILogger log)
    {
        string message = response.Error is IError error
            ? error.GetUserFriendlyMessage()
            : response.Message ?? "An error occurred";

        log.LogError(message);

        var res = new BaseOutput<IBaseOutput>(response.Result, null, message);

        return CreateJsonResult(res, GetHttpStatusCode(response.Result));
    }

    private static ContentResult CreateJsonResult(object obj, int statusCode)
    {
        var json = JsonConvert.SerializeObject(obj, JsonSettings);

        return new ContentResult
        {
            Content = json,
            ContentType = "application/json",
            StatusCode = statusCode
        };
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
            _ => (int)HttpStatusCode.OK
        };
    }
}
