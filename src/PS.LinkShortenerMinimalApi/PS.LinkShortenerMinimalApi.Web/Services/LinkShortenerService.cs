using PS.LinkShortenerMinimalApi.Web.Services.Interfaces;
using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;

namespace PS.LinkShortenerMinimalApi.Web.Services
{
    public class LinkShortenerService : ILinkShortenerService
    {
        private readonly ILinkStorage _storage;
        const string BaseUrl = "http://sho.com/";

        public LinkShortenerService(ILinkStorage storage)
        {
            _storage = storage;
        }

        public string Shorten(string longUrl)
        {
            string code;
            string shortLink;

            do
            {
                code = GenerateShortCode();
                shortLink = $"{BaseUrl}{code}";
            }
            while (_storage.TryGet(shortLink, out _));

            _storage.Save(shortLink, longUrl);

            return shortLink;
        }

        public string? Expand(string shortUrl)
        {
            _storage.TryGet(shortUrl, out var longUrl);
            return longUrl;
        }

        private string GenerateShortCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }
    }
}
