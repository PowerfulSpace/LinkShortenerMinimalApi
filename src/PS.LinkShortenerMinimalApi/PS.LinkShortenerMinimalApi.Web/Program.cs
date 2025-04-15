using Microsoft.OpenApi.Models;
using PS.LinkShortenerMinimalApi.Web.Models;
using PS.LinkShortenerMinimalApi.Web.Services.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Services;
using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Storage;
using PS.LinkShortenerMinimalApi.Web.Utils.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Utils;




var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LinkShortener API", Version = "v1" });
});

// Регистрация сервисов
//builder.Services.AddSingleton<IPathService, PathService>();
//builder.Services.AddSingleton<ILinkStorage>(provider =>
//{
//    var pathService = provider.GetRequiredService<IPathService>();
//    var filePath = pathService.GetDataFilePath("links.json");
//    return new JsonLinkStorage(filePath);
//});
//builder.Services.AddSingleton<ILinkShortenerService, LinkShortenerService>();



builder.Services.AddSingleton<ILinkStorage>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var redisConnection = config.GetSection("Redis")["ConnectionString"];
    return new RedisLinkStorage(redisConnection);
});

builder.Services.AddSingleton<ILinkShortenerService, LinkShortenerService>();


var app = builder.Build();







// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinkShortener API v1");
    });
}

app.UseHttpsRedirection();


app.MapGet("/", () => Results.Redirect("/swagger"));

// POST /shorten
app.MapPost("/shorten", (LinkRequest request, ILinkShortenerService service) =>
{
    var shortUrl = service.Shorten(request.LongUrl);
    return Results.Ok(new LinkResponse { ShortUrl = shortUrl });
})
.WithName("ShortenUrl")
.WithOpenApi();


// GET /expand?shortUrl=...
app.MapGet("/expand", (string shortUrl, ILinkShortenerService service) =>
{
    var longUrl = service.Expand(shortUrl);
    return longUrl is not null
        ? Results.Ok(longUrl)
        : Results.NotFound("Ссылка не найдена");
})
.WithName("ExpandUrl")
.WithOpenApi();


// GET /{code} => redirect
app.MapGet("/{code}", (string code, ILinkShortenerService service) =>
{
    var shortUrl = $"https://localhost:7241/{code}";
    var longUrl = service.Expand(shortUrl);

    return longUrl is not null
        ? Results.Redirect(longUrl)
        : Results.NotFound("Короткая ссылка не найдена");
})
.WithName("RedirectToLongUrl");

app.Run();