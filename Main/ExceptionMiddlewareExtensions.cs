using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Models;
using Newtonsoft.Json;
using Utils;

namespace Main;

public static class ExceptionMiddlewareExtensions
{
    public static void UseApiExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async context =>
            {
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    LogException(context, contextFeature);

                    var error = await GetErrorDetails(contextFeature.Error, context);

                    context.Response.StatusCode = error.StatusCode;

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
                }
            }
        });
    }

    private static async Task<ErrorDto> GetErrorDetails(Exception contextFeatureError, HttpContext context)
    {
        if (contextFeatureError is IntakerValidationException validationException)
        {
            return new ErrorDto((int)HttpStatusCode.BadRequest, validationException.Code, validationException.Message);
        }

        if (contextFeatureError is IntakerNotFoundException notFoundException)
        {
            return new ErrorDto((int)HttpStatusCode.NotFound, notFoundException.Code, notFoundException.Message);
        }

        if (contextFeatureError is IntakerInternalErrorException internalErrorException)
        {
            return new ErrorDto((int)HttpStatusCode.InternalServerError, internalErrorException.Code, internalErrorException.Message);
        }

        return new ErrorDto((int)HttpStatusCode.InternalServerError, ErrorCodes.General.Code, contextFeatureError.Message);
    }

    private static void LogException(HttpContext context, IExceptionHandlerFeature contextFeature)
    {
        var logger = context.RequestServices.GetService<ILoggerFactory>()!
            .CreateLogger("ExceptionHandler");

        switch (contextFeature.Error)
        {
            case IntakerValidationException validationException:
                logger.LogError(validationException, "Validation error: {Message}", validationException.Message);
                break;
            case IntakerNotFoundException:
                // Skip logging as it's probably user's error
                break;
            case IntakerInternalErrorException internalErrorException:
                logger.LogError(internalErrorException, "Internal error: {Message}", internalErrorException.Message);
                break;
            default:
                logger.LogError(contextFeature.Error, "Unhandled error occurred");
                break;
        }
    }
}