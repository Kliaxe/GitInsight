using GitInsight.WebApp.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GitInsight.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GitInsightController : ControllerBase
    {
        private readonly ILogger<GitInsightController> _logger;

        public GitInsightController(ILogger<GitInsightController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{owner}/{repositoryName}")]
        public async Task<ActionResult<string>> Get(string owner, string repositoryName)
        {
            throw new NotImplementedException();
        }
    }
}