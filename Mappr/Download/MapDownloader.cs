namespace Mappr.Download
{
    public class MapDownloader
    {
        FileDownloader downloader = new FileDownloader();
        string GetUri(int z, int x, int y)
        {
            return $"https://s.rsg.sc/sc/images/games/GTAV/map/render/{z}/{x}/{y}.jpg";
        }

        string GetFilePath(int z, int x, int y)
        {
            return $"maps/gta5/{z}/{x}x{y}.jpg";
        }

        public async Task Download()
        {
            for (int z = 0; z < 8; z++)
            {
                int max = 1 << z;
                for (int y = 0; y < max; y++)
                {
                    for (int x = 0; x < max; x++)
                    {
                        var uri = GetUri(z, x, y);
                        var file = GetFilePath(z, x, y);
                        await downloader.DownloadFileAsync(uri, file);
                    }
                }
            }
        }
    }
}