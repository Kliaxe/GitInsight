using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitInsight;
using LibGit2Sharp;
using Octokit;

namespace GitInsight.Infrastructure
{
    public class GitInsightRepoFactory
    {
        public static Task datebaseUpdate = Task.CompletedTask;
        public static IGitRepoInsight CreateRepoInsight(IRepository repo, GitInsightContext context, IGitHubClient client)
        {
            var insightRepository = new InsightRepository(context);
            (bool upToDate, var gitRepo) = insightRepository.HasUpToDateInsight(repo);
            if (upToDate)
            {
                return new DBRepoInsight(gitRepo);
            }
            else
            {
                var localRepo = new GitRepoInsight(repo, client);
                datebaseUpdate = insightRepository.UpdateInsight(localRepo);
                return localRepo;
            }
        }
    }
}
