using GitInsight.WebApp.Shared;
using GitInsight.Infrastructure;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace GitInsight.WebApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GitInsightController : ControllerBase
    {
        private readonly ILogger<GitInsightController> _logger;
        private readonly string directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/GitInsight Repositories";

        public GitInsightController(ILogger<GitInsightController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{owner}/{repositoryName}")]
        public async Task<ActionResult<IEnumerable<DateCount>>> Get(string owner, string repositoryName)
        {
            await Task.Yield();
            (var repo, bool exists) = GetLocalRepository(owner, repositoryName);
            if (!exists)
            {
                return NotFound();
            }
            IGitRepoInsight repoInsight = GetRepoInsight(repo);
            return Ok(repoInsight.GetCommitHistory());
        }

        [HttpGet("{owner}/{repositoryName}/{user}")]
        public async Task<ActionResult<IEnumerable<UserDateCounts>>> Get(string owner, string repositoryName, string user)
        {
            await Task.Yield();
            if (user == "user")
            {
                (var repo, bool exists) = GetLocalRepository(owner, repositoryName);
                if (!exists)
                {
                    return NotFound();
                }
                IGitRepoInsight repoInsight = GetRepoInsight(repo);
                var testFormat = repoInsight.GetCommitHistoryByUser().Select(t => new UserDateCounts(t.Item1, t.Item2));
                return Ok(testFormat);
            }
            return BadRequest();
        }

        private (IRepository, bool) GetLocalRepository(string owner, string repositoryName)
        {
            Repository repo;
            string path = directory + $"/{owner}_{repositoryName}";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (!Repository.IsValid(path))
            {
                Directory.CreateDirectory(path);
                string remoteUrl = $"https://github.com/{owner}/{repositoryName}";
                try
                {
                    Repository.Clone(remoteUrl, path, new CloneOptions());
                    repo = new Repository(path);
                }
                catch (Exception e)
                {
                    Directory.Delete(path);
                    Console.WriteLine(e.Message);
                    return (null, false)!;
                }
            }
            else
            {
                repo = new Repository(path);
                Commands.Pull(repo, new Signature("GitInsight", "none", DateTime.Now), new PullOptions { });
            }
            return (repo, true);
        }

        private IGitRepoInsight GetRepoInsight(IRepository repo)
        {
            IGitRepoInsight repoInsight;
            try
            {
                var context = new GitInsightContextFactory().CreateDbContext(Array.Empty<string>());
                repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                repoInsight = new GitRepoInsight(repo);
            }
            return repoInsight;
        }
    }
}