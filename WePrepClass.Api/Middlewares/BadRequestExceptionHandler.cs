﻿using Matt.SharedKernel;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WePrepClass.Api.Middlewares;

public sealed class BadRequestExceptionHandler(IServiceProvider serviceProvider) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException badRequestException)
        {
            return false;
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<IAppLogger<BadRequestExceptionHandler>>();

            logger.LogError("Validation failed with errors:\n{Errors}",
                JsonConvert.SerializeObject(badRequestException.Errors, Formatting.Indented));
        }

        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest,
            Title = "One or more validation errors occurred.",
            Detail = badRequestException.Message,
            Extensions = new Dictionary<string, object?>
            {
                ["errorMessages"] = badRequestException.Errors
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}