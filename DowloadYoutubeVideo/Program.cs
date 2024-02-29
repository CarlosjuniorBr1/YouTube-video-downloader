using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;


namespace YouTubeDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {

            List<string> videoUrls = new List<string>{};
            Console.WriteLine("\n\n\t\t\t\t\t\t\tBem vindo!!!\n\n ");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------\n\n");


            Console.Write("Digite o caminho para salvar os videos: ");

            // Set the output directory path here
            string outputDirectory = Console.ReadLine();


            Console.Write("Quantos videos deseja baixar? ");
            int n = int.Parse(Console.ReadLine());
          

            for(int i = 1; i<= n; i++)
            {
                Console.Write("Musica " + i + "\n URL: ");
                string aux = Console.ReadLine(); 
                videoUrls.Add(aux);

            }

            
           
            try
            {
                foreach (var videoUrl in videoUrls)
                {
                    await DownloadYouTubeVideo(videoUrl, outputDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while downloading the videos: " + ex.Message);
            }
        }
        static async Task DownloadYouTubeVideo(string videoUrl, string outputDirectory)
        {
            var youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(videoUrl);

            // Sanitize the video title to remove invalid characters from the file name
            string sanitizedTitle = string.Join("_", video.Title.Split(Path.GetInvalidFileNameChars()));

            // Get all available muxed streams
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
            var muxedStreams = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).ToList();

            if (muxedStreams.Any())
            {
                var streamInfo = muxedStreams.First();
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(streamInfo.Url);
                var datetime = DateTime.Now;

                string outputFilePath = Path.Combine(outputDirectory, $"{sanitizedTitle}.{streamInfo.Container}");
                using var outputStream = File.Create(outputFilePath);
                await stream.CopyToAsync(outputStream);

                Console.WriteLine("Download completed!");
                Console.WriteLine($"Video saved as: {outputFilePath}{datetime}");
            }
            else
            {
                Console.WriteLine($"No suitable video stream found for {video.Title}.");
            }
        }
    }
}