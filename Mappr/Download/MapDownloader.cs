using System.Diagnostics;

namespace Mappr.Download
{
    public class MapDownloader
    {
        FileDownloader downloader = new FileDownloader();

        // Define delegate types for the callbacks
        public delegate string UriCallback(int z, int x, int y);
        public delegate string FileCallback(int z, int x, int y);

        // Callback properties
        public UriCallback? GetUriCallback { get; set; }
        public FileCallback? GetFileCallback { get; set; }

        public async Task Download(int levels, CancellationToken cancellationToken = default, IProgress<float> progress = null)
        {
            int totalTiles = 0;
            for (int z = 0; z < levels; z++)
                totalTiles += (1 << z) * (1 << z);

            int downloadedTiles = 0;
            for (int z = 0; z < levels; z++)
            {
                int max = 1 << z;
                for (int y = 0; y < max; y++)
                {
                    for (int x = 0; x < max; x++)
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        if (GetUriCallback == null || GetFileCallback == null)
                            throw new InvalidOperationException("Callback functions not set.");

                        var uri = GetUriCallback(z, x, y);
                        var file = GetFileCallback(z, x, y);

                        downloadedTiles++;
                        if (File.Exists(file))
                        {
                            progress?.Report((float)downloadedTiles / totalTiles);
                            continue;
                        }
                            
                        Debug.WriteLine($"Downloading {uri}");
                        await downloader.DownloadFileAsync(uri, file);
                        progress?.Report((float)downloadedTiles / totalTiles);
                    }
                }
            }
        }
    }
}
