using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;
using System.Text.Json;

namespace PS.LinkShortenerMinimalApi.Web.Storage
{
    public class JsonLinkStorage : IBulkLinkStorage
    {
        private readonly string _filePath;
        private Dictionary<string, string> _data;

        public JsonLinkStorage(string filePath)
        {
            _filePath = filePath;

            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);

                _data = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new();
            }
            else
            {
                _data = new Dictionary<string, string>();
            }
        }

        public void Save(string shortUrl, string longUrl)
        {
            _data[shortUrl] = longUrl;
            Persist();
        }

        public bool TryGet(string shortUrl, out string longUrl)
        {
            return _data.TryGetValue(shortUrl, out longUrl);
        }

        public Dictionary<string, string> GetAll() => _data;

        private void Persist()
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
