namespace HackerNews.HackerService
{
    public interface IHackerNewsService
    {
        public  Task<IEnumerable<Models.HackerNews>> RetrieveBestStories(int numberofStoriesToRetrieve);
    }
}
