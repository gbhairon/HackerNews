using System;
using HackerNews.Models;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HackerNewsTests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace HackerNews.HackerService
{

    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsService> _logger;

        public HackerNewsService(ILogger<HackerNewsService> ilogger , HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = ilogger;
        }

        /// <summary>
        ///  Retrieve a list of stories (HackerNews) , with the count of items in the list determined by the numberofStoriesToRetrieve paramter
        /// </summary>
        /// <param name="numberofStoriesToRetrieve"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Models.HackerNews>>  RetrieveBestStories(int numberofStoriesToRetrieve)
        {
            _logger.LogInformation("HackerNewsService.RetrieveIdsOfBestStories parameter {0}", numberofStoriesToRetrieve);
            var bestStoriesIdLst = await GetBestStoriesIds();
            List<Models.HackerNews> bestStoriesFromIds =new List<Models.HackerNews>();
            if (bestStoriesIdLst != null && bestStoriesIdLst.Count > 0)
            {
                bestStoriesFromIds = await  GetBestStoriesFromIds(bestStoriesIdLst, numberofStoriesToRetrieve);
            }

            return bestStoriesFromIds;
        }

        /// <summary>
        /// Given a list of best story ids,  fetch the details (HackerNews) and return the number of items required
        /// </summary>
        /// <param name="bestStoriesIdLst"></param>
        /// <param name="numberofStoriesToRetrieve"></param>
        /// <returns></returns>
        internal virtual async Task<List<Models.HackerNews>> GetBestStoriesFromIds(List<string> bestStoriesIdLst, int numberofStoriesToRetrieve)
        {
            object lockObject = new object(); 
            var apiUrl = "https://hacker-news.firebaseio.com/v0/item/x.json";
            List<Models.HackerNews> hackerNewsList = new List<Models.HackerNews>();
            Task parallelTask = Task.Run( () =>
                {
                    Parallel.ForEach(bestStoriesIdLst, (id) =>
                    {
                        try
                        {
                            var uriForStory = apiUrl.Replace("x", id);

                            var response = _httpClient.GetAsync(uriForStory.ToString()).GetAwaiter().GetResult();

                            response.EnsureSuccessStatusCode();

                            // Read the response content as a list of strings
                            string data = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                            var deserialisedObj = JsonConvert.DeserializeObject<Models.HackerNews>(data);

                            Models.HackerNews hackerNewsObj = (deserialisedObj == null ? new Models.HackerNews() : (Models.HackerNews)deserialisedObj);

                            // Ensure thread safety when adding to the list
                            lock (lockObject)
                            {
                                hackerNewsList.Add(hackerNewsObj);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Failed calling HackerNewsService.GetBestStoriesFromIds id parameter {0}", id);
                            _logger.LogError(ex.Message);
                            _logger.LogError(ex.StackTrace);
                            throw;
                        }
                    });              
                }
            );

           await parallelTask;

           return hackerNewsList.OrderByDescending(x=>x.score).Take(numberofStoriesToRetrieve).ToList<Models.HackerNews>();
        }

        /// <summary>
        ///  Fetch the ids of the best stories from the hacker news relevant website
        /// </summary>
        /// <returns></returns>
        internal virtual async Task<List<string>?> GetBestStoriesIds()
        {
            var hackerNews = new List<Models.HackerNews>();
            var apiUrl = "https://hacker-news.firebaseio.com/v0/beststories.json";

            var response = await _httpClient.GetAsync(apiUrl);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsStringAsync();

            return data?.Substring(1, data.Length - 2).Split(',').ToList<string>();
        }
    }
}
