using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Utility.DataContracts.Models;

namespace Api.Options;

public class ConfigureExceptionHandlingApiBehavior : IConfigureOptions<ApiBehaviorOptions>
{
    public void Configure(ApiBehaviorOptions options)
    {
        options.InvalidModelStateResponseFactory = actionContext =>
            new BadRequestObjectResult(new ExceptionModel
            {
                Status = HttpStatusCode.BadRequest.ToString(),
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = string.Join(',',
                    actionContext.ModelState.SelectMany(x => x.Value?.Errors.Select(err => err.ErrorMessage)))
            });
    }
}