namespace PS.LinkShortenerMinimalApi.Web.Storage.Interfaces
{
    public interface ILinkStorage
    {
        void Save(string shortUrl, string longUrl);
        bool TryGet(string shortUrl, out string longUrl);
    }
}
