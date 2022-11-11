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
        public async Task<IEnumerable<DateCount>> Get(string owner, string repositoryName)
        {
                var repo = GetLocalRepository(owner, repositoryName);
                IGitRepoInsight repoInsight = new GitRepoInsight(repo);
                return repoInsight.GetCommitHistory();
        }

        [HttpGet("{owner}/{repositoryName}/{user}")]
        public async Task<IEnumerable<string>> Get(string owner, string repositoryName, string user)
        {
            if (user == "user")
            {
                var repo = GetLocalRepository(owner, repositoryName);
                IGitRepoInsight repoInsight = new GitRepoInsight(repo);
                var formatter = new Formatter(repoInsight);
                //var result = formatter.GetCommitsOverTimeFormatted();
                var testFormat = repoInsight.GetCommitHistoryByUser().SelectMany(u => u.Item2.Select(dc => $"{u.Item1.name} - {dc.Date} - {dc.Count}"));
                return testFormat;
            }
            return null;
        }

        private IRepository GetLocalRepository(string owner, string repositoryName)
        {
            Repository repo;
            string path = directory + $"/{owner}_{repositoryName}";
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            if (!Repository.IsValid(path))
            {
                Directory.CreateDirectory(path);
                string remoteUrl = $"https://github.com/{owner}/{repositoryName}";
                Repository.Clone(remoteUrl, path, new CloneOptions());
                repo = new Repository(path);
            }
            else
            {
                repo = new Repository(path);
                var result = Commands.Pull(repo, new Signature("GitInsight", "none", DateTime.Now), new PullOptions { });
                Console.WriteLine(result.Status);
            }
            return repo;
        }
    }
}