namespace Mappr.Download
{
    public class FileDownloader
    {
        private readonly HttpClient httpClient;

        public FileDownloader()
        {
            httpClient = new HttpClient();
        }

        public async Task DownloadFileAsync(string url, string localFilePath)
        {
            try
            {
                // Extract the directory path from the local file path.
                string directoryPath = Path.GetDirectoryName(localFilePath);

                // Check if the directory exists; if not, create it.
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(localFilePath, fileBytes);
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