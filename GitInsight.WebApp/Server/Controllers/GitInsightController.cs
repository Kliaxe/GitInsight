using GitInsight.Infrastructure;
using GitInsight.Database;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

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
        public async Task<Results<Ok<RepoAnalysis>, NotFound<string>>> Get(string owner, string repositoryName)
        {
            var (repo, exists) = GetLocalRepository(owner, repositoryName);
            if (!exists)
            {
                return TypedResults.NotFound(GitHubUrl(owner, repositoryName));
            }
            IGitRepoInsight repoInsight = GetRepoInsight(repo);

            var dateCounts = repoInsight.GetCommitHistory();
            var userDateCounts = repoInsight.GetCommitHistoryByUser().Select(t => new UserDateCounts(t.Item1, t.Item2));
            var forks = (await repoInsight.GetForks());

            return TypedResults.Ok(new RepoAnalysis(dateCounts, userDateCounts, forks));
        }

        private (IRepository, bool) GetLocalRepository(string owner, string repositoryName)
        {
            Repository repo;
            string path = directory + $"/{owner}_{repositoryName}";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (!Repository.IsValid(path))
            {
                Directory.CreateDirectory(path);
                string remoteUrl = GitHubUrl(owner, repositoryName);
                try
                {
                    Repository.Clone(remoteUrl, path, new CloneOptions());
                    repo = new Repository(path);
                }
                catch (Exception)
                {
                    Directory.Delete(path, true);
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
            catch (Exception)
            {
                return new GitRepoInsight(repo, GitHubClient.Client);
            }
        }

        private string GitHubUrl(string owner, string repositoryName) => $"https://github.com/{owner}/{repositoryName}";
    }
}