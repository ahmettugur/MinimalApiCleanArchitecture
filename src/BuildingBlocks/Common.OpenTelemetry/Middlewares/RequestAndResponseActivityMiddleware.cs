using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace Common.OpenTelemetry.Middlewares;

public class RequestAndResponseActivityMiddleware(RequestDelegate next)
{
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new();

    public async Task InvokeAsync(HttpContext context)
    {
        await AddRequestBodyContentToActivityTags(context);
        await AddResponseBodyContentToActivityTags(context);
    }
    
    private async Task AddRequestBodyContentToActivityTags(HttpContext context)
    {
        context.Request.EnableBuffering();
        var requestBodyStreamReader = new StreamReader(context.Request.Body);
        var requestBodyContent = await requestBodyStreamReader.ReadToEndAsync();

        Activity.Current?.SetTag("http.request.body", requestBodyContent);
        context.Request.Body.Position = 0;
    }
    private async Task AddResponseBodyContentToActivityTags(HttpContext context)
    {
        var originalResponse = context.Response.Body;
        await using var responseBodyMemoryStream = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBodyMemoryStream;
        
        await next(context);

        responseBodyMemoryStream.Position = 0;

        var responseBodyStreamReader = new StreamReader(responseBodyMemoryStream);
        var responseBodyContent = await responseBodyStreamReader.ReadToEndAsync();
        Activity.Current?.SetTag("http.response.body", responseBodyContent);
        
        responseBodyMemoryStream.Position = 0;

        await responseBodyMemoryStream.CopyToAsync(originalResponse);
    }
}