using System.Threading;

namespace Mappr.Download
{
    public class FileDownloader
    {
        private readonly HttpClient httpClient;

        public FileDownloader()
        {
            httpClient = new HttpClient();
        }

        public async Task DownloadFileAsync(string url, string localFilePath, CancellationToken cancellationToken = default)
        {
            try
            {
                // Extract the directory path from the local file path.
                string directoryPath = Path.GetDirectoryName(localFilePath) ?? throw new Exception("Couln't get directory from localFilePath");

                // Check if the directory exists; if not, create it.
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    await File.WriteAllBytesAsync(localFilePath, fileBytes, cancellationToken);
                    Console.WriteLine($"File downloaded and saved to {localFilePath}");
                }
                else
                {
                    Console.WriteLine($"HTTP request failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }
}