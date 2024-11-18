using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Stock.Asx.DataCenter.EFCore;
using Stock.Asx.DataCenter.EFCore.Model;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Stock.DataCenter.SocialMedia
{
    public static class YoutubeVideo
    {
        private const string URL_LIST = "https://www.googleapis.com/youtube/v3/search?part=snippet&q={{keywords}}&publishedAfter={{releaseAfter}}&type=video&order=relevance&key=AIzaSyAB9SzUtFRiws3RS-U2QEZkWqIVdcAcUGY&maxResults={{maxNumberOfVideos}}";

        // The command you want to execute
        private const string SUBTITLE_COMMAND = "youtube_transcript_api";

        // Arguments to pass to the command
        private const string ARGUMENTS = "{{videoId}} --languages de en";

        public static void GetAllVideoList(ILogger logger)
        {
            using (var context = new CompanyContext())
            {
                var _searchSettings = context.YoutubeVideoCollectionSettings.ToList();
                var _existingVideos = context.SocialMediaYoutubeVideos.ToList();
                var httpClient = new HttpClient();
                var videoToUpload = new List<SocialMediaYoutubeVideo>();

                foreach (var _searchSetting in _searchSettings)
                {
                    var searchUrl = URL_LIST.Replace("{{keywords}}", _searchSetting.SearchFilterKeywords).
                                             Replace("{{releaseAfter}}", DateTime.UtcNow.AddDays(_searchSetting.DateFrom * -1).ToString("yyyy-MM-ddTdd:mm:ssZ")).
                                             Replace("{{maxNumberOfVideos}}", _searchSetting.MaxNumberOfVideos.ToString());

                    HttpResponseMessage response = httpClient.GetAsync(searchUrl).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        var jsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        var videoList = JsonConvert.DeserializeObject<YoutubeSearchResponse>(jsonString);

                        if (videoList.Items == null)
                        {
                            logger.LogWarning($"search criteria of key works {_searchSetting.SearchFilterKeywords} and Id {_searchSetting.Id} return nothing");
                            continue;
                        }

                        foreach (var video in videoList.Items)
                        {
                            try
                            {
                                string releaseDateString = Convert.ToString(video.Snippet.PublishedAt);

                                logger.LogInformation(releaseDateString);

                                var youtube_video = new SocialMediaYoutubeVideo()
                                {
                                    Catergory = _searchSetting.Catergory,
                                    SubCatergory = _searchSetting.SubCatergory,
                                    SectorName = _searchSetting.SectorName,
                                    Symbol = _searchSetting.Symbol,
                                    Title = video.Snippet.Title,
                                    Description = video.Snippet.Description,
                                    VideoId = video.Id.VideoId,
                                    Subtitle = string.Empty,
                                    Enrich = string.Empty,
                                    Sentiment = string.Empty,
                                    IsActive = true,
                                    ReleaseDate = video.Snippet.PublishedAt,
                                    CreateDate = DateTime.UtcNow
                                };

                                if (context.SocialMediaYoutubeVideos.Any(v => v.Symbol == youtube_video.Symbol && v.Catergory == youtube_video.Catergory && v.SubCatergory == youtube_video.SubCatergory && v.SectorName == youtube_video.SectorName && v.VideoId == youtube_video.VideoId))
                                {
                                    // logger.LogWarning($"video with id {youtube_video.VideoId} existed in the sub category {youtube_video.SubCatergory}");
                                    continue;
                                }

                                if (youtube_video.VideoId != string.Empty)
                                {
                                    context.SocialMediaYoutubeVideos.Add(youtube_video);
                                    context.SaveChanges();

                                    logger.LogInformation($"Add video with id {youtube_video.VideoId}");
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogInformation($"Add video with id {video.Id.VideoId} [Failed]");
                                logger.LogError(ex.InnerException.ToString());

                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    //public static string GetVideoSubtitle(string command, string arguments)
    //{
    //    // Initialize the ProcessStartInfo with the command and arguments
    //    ProcessStartInfo startInfo = new ProcessStartInfo
    //    {
    //        FileName = command,
    //        Arguments = arguments,
    //        RedirectStandardOutput = true,  // Redirects the standard output to be read
    //        RedirectStandardError = true,   // Redirects the standard error to be read
    //        UseShellExecute = false,        // Required to redirect output
    //        CreateNoWindow = true           // Prevents creating a command window
    //    };

    //    // Create a new process and assign the start info
    //    using (Process process = new Process())
    //    {
    //        process.StartInfo = startInfo;

    //        try
    //        {
    //            // Start the process
    //            process.Start();

    //            // Read the output and error streams
    //            string output = process.StandardOutput.ReadToEnd();
    //            string error = process.StandardError.ReadToEnd();

    //            // Wait for the process to exit
    //            process.WaitForExit();

    //            // Combine output and error for full capture
    //            return string.IsNullOrWhiteSpace(error) ? output : $"{output}\nError: {error}";
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle any exceptions
    //            return $"Exception: {ex.Message}";
    //        }
    //    }
    //}
}
