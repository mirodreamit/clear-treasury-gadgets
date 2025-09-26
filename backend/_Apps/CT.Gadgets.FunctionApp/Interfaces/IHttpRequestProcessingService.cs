using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using CT.Application.Abstractions.Interfaces;
using System;

namespace CT.Gadgets.FunctionApp.Interfaces;

public interface IHttpRequestProcessingService
{
    Task<IActionResult> ProcessHttpRequestAsync(Func<Task<IBaseOutput>> f, ILogger log);
}
