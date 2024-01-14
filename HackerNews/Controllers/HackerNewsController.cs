using HackerNews.HackerService;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HackerNewsController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<HackerNewsController> _logger;
        private readonly IHackerNewsService  _hackerNewsService;

        public HackerNewsController(ILogger<HackerNewsController> logger,IHackerNewsService hackerNewsService, HttpClient httpClient)
        {
            _logger = logger;
            _hackerNewsService = hackerNewsService;
            _httpClient = httpClient;
        }

        [HttpGet("{numberofStories}")]
        public async Task<ActionResult<IEnumerable<Models.HackerNews>>> GetAsync(int numberofStories)
        {
            _logger.LogInformation("HackerNewsContoller.Get numberofStories parameter {0}", numberofStories);

            if (numberofStories <= 0)
            {
                // Return a bad request response if id is not valid
                return BadRequest("Invalid numberofStories. The numberofStories must be a positive integer.");
            }
            try
            {
                var stories = await _hackerNewsService.RetrieveBestStories(numberofStories);

                return Ok(stories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }     
    }
}