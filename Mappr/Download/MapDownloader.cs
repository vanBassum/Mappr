using System.Diagnostics;

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

        public async Task Download(int z = 0, int y = 0)
        {
            for (; z < 8; z++)
            {
                int max = 1 << z;
                for (; y < max; y++)
                {
                    for (int x = 0; x < max; x++)
                    {
                        var uri = GetUri(z, x, y);
                        var file = GetFilePath(z, x, y);
                        if (File.Exists(file))
                            continue;
                        Debug.WriteLine($"Downloading {uri}");
                        await downloader.DownloadFileAsync(uri, file);
                    }
                }
            }
        }
    }
}