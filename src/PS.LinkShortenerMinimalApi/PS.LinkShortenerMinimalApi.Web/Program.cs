using PS.LinkShortenerMinimalApi.Web.Models;
using PS.LinkShortenerMinimalApi.Web.Services;
using PS.LinkShortenerMinimalApi.Web.Services.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Storage;
using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;




var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
//builder.Services.AddSingleton<IPathService, PathService>();
//builder.Services.AddSingleton<ILinkStorage>(provider =>
//{
//    var pathService = provider.GetRequiredService<IPathService>();
//    var filePath = pathService.GetDataFilePath("links.json");
//    return new JsonLinkStorage(filePath);
//});
//builder.Services.AddSingleton<ILinkShortenerService, LinkShortenerService>();


// Настройка конфигурации
builder.Services.Configure<ShortenerSettings>(builder.Configuration.GetSection("Shortener"));

// Регистрация Redis хранилища
string redisConnection = builder.Configuration.GetSection("Redis")["ConnectionString"]!;
builder.Services.AddSingleton<ILinkStorage>(new RedisLinkStorage(redisConnection));

// Регистрация сервиса сокращения ссылок
builder.Services.AddSingleton<ILinkShortenerService, LinkShortenerService>();


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();
{

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
    app.MapGet("/r/{code}", (string code, ILinkShortenerService service) =>
    {
        string baseUrl = builder.Configuration.GetSection("Shortener")["BaseUrl"]!.TrimEnd('/');

        var shortUrl = $"{baseUrl}/r/{code}";
        var longUrl = service.Expand(shortUrl);

        return longUrl is not null
            ? Results.Redirect(longUrl)
            : Results.NotFound("Короткая ссылка не найдена");
    })
    .WithName("RedirectToLongUrl");

    app.Run();
}






