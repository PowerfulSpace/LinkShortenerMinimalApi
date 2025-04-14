using PS.LinkShortenerMinimalApi.Web.Utils.Interfaces;

namespace PS.LinkShortenerMinimalApi.Web.Utils
{
    public class PathService : IPathService
    {
        public string GetDataFilePath(string fileName)
        {
            string basePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\.."));
            return Path.Combine(basePath, "Data", fileName);
        }
    }
}
