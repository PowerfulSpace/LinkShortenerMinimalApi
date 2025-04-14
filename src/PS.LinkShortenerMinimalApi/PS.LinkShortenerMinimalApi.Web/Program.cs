
using Microsoft.OpenApi.Models;
using PS.LinkShortenerMinimalApi.Web.Models;
using PS.LinkShortenerMinimalApi.Web.Services.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Services;
using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Storage;
using PS.LinkShortenerMinimalApi.Web.Utils.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LinkShortener API", Version = "v1" });
});

builder.Services.AddSingleton<IPathService, PathService>();

// Временно JSON, потом заменю на настройки Redis
builder.Services.AddSingleton<ILinkStorage>(provider =>
{
    var pathService = provider.GetRequiredService<IPathService>();
    var filePath = pathService.GetDataFilePath("links.json");
    return new JsonLinkStorage(filePath);
});

builder.Services.AddSingleton<ILinkShortenerService, LinkShortenerService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShortener API v1");
    });
}

app.UseHttpsRedirection();

app.MapPost("/shorten", (LinkRequest request, ILinkShortenerService service) =>
{
    var shortUrl = service.Shorten(request.LongUrl);
    return Results.Ok(new LinkResponse { ShortUrl = shortUrl });
})
.WithName("ShortenUrl")
.WithOpenApi();

app.MapGet("/expand", (string shortUrl, ILinkShortenerService service) =>
{
    var longUrl = service.Expand(shortUrl);
    return longUrl is not null
        ? Results.Ok(longUrl)
        : Results.NotFound("Ссылка не найдена");
})
.WithName("ExpandUrl")
.WithOpenApi();

app.Run();