using Microsoft.AspNetCore.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.Use(async (context, next) =>
{
    if (HttpMethods.IsGet(context.Request.Method)
        && context.Request.Path == "/"
        && !ClientAcceptsHtml(context.Request))
    {
        WriteNoCacheHeaders(context.Response);
        context.Response.ContentType = "text/plain; charset=utf-8";
        await context.Response.WriteAsync($"{Guid.NewGuid()}\n");
        return;
    }

    await next();
});

app.UseRouting();

app.MapRazorPages();

app.Run();

static bool ClientAcceptsHtml(HttpRequest request)
{
    RequestHeaders headers = request.GetTypedHeaders();

    if (headers.Accept is null || headers.Accept.Count == 0)
    {
        return false;
    }

    return headers.Accept.Any(accept =>
        accept.MediaType.Equals("text/html", StringComparison.OrdinalIgnoreCase)
        || accept.MediaType.Equals("application/xhtml+xml", StringComparison.OrdinalIgnoreCase));
}

static void WriteNoCacheHeaders(HttpResponse response)
{
    response.Headers.CacheControl = "no-store, no-cache, max-age=0";
    response.Headers.Pragma = "no-cache";
    response.Headers.Expires = "0";
}
