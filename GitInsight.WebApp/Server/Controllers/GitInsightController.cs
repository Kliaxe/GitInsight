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
        public async Task<ActionResult<RepoAnalysis>> Get(string owner, string repositoryName)
        {
            var (repo, exists) = GetLocalRepository(owner, repositoryName);
            if (!exists)
            {
                return NotFound();
            }
            IGitRepoInsight repoInsight = GetRepoInsight(repo);

            var dateCounts = repoInsight.GetCommitHistory();
            var userDateCounts = repoInsight.GetCommitHistoryByUser().Select(t => new UserDateCounts(t.Item1, t.Item2));
            var forks = (await repoInsight.GetForks()).Select(f => new Fork(f));

            return new RepoAnalysis(dateCounts, userDateCounts, forks);
        }

        //[HttpGet("{owner}/{repositoryName}/{user}")]
        //public async Task<(IEnumerable<UserDateCounts>, IEnumerable<string>)> Get(string owner, string repositoryName, string user)
        //{
        //    if (user is null)
        //    {

        //    }
        //    else if (user == "user")
        //    {
        //        var repo = GetLocalRepository(owner, repositoryName);
        //        IGitRepoInsight repoInsight = new GitRepoInsight(repo);
        //        var testFormat = repoInsight.GetCommitHistoryByUser().Select(t => new UserDateCounts(t.Item1, t.Item2));
        //        return (testFormat, await repoInsight.GetForks());
        //    }
        //    return (null, null);
        //}


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
                    return (null!, false);
                }
            }
            else
            {
                repo = new Repository(path);
                var result = Commands.Pull(repo, new Signature("GitInsight", "none", DateTime.Now), new PullOptions { });
                Console.WriteLine(result.Status);
            }
            return (repo, true);
        }

        private IGitRepoInsight GetRepoInsight(IRepository repo)
        {
            try
            {
                var context = new GitInsightContextFactory().CreateDbContext(Array.Empty<string>());

                return GitInsightRepoFactory.CreateRepoInsight(repo, context, GitHubClient.Client);
            }
            catch (Exception e)
            {
                return new GitRepoInsight(repo, GitHubClient.Client);
            }
        }
    }
}