using PS.LinkShortenerMinimalApi.Web.Storage.Interfaces;
using StackExchange.Redis;

namespace PS.LinkShortenerMinimalApi.Web.Storage
{
    public class RedisLinkStorage : ILinkStorage
    {
        private readonly IDatabase _db;
        private const string KeyPrefix = "shorturl:";

        public RedisLinkStorage(string connectionString)
        {
            var redis = ConnectionMultiplexer.Connect(connectionString);
            _db = redis.GetDatabase();
        }


        public void Save(string shortUrl, string longUrl)
        {
            _db.StringSet(KeyPrefix + shortUrl, longUrl);
        }

        public bool TryGet(string shortUrl, out string longUrl)
        {
            var value = _db.StringGet(KeyPrefix + shortUrl);
            longUrl = value.HasValue ? value.ToString() : null!;

            return value.HasValue;
        }
    }
}
