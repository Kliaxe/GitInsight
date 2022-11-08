using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GitInsight;
using LibGit2Sharp;

namespace GitInsight.Infrastructure
{
    public class GitInsightRepoFactory
    {
        
        public static IGitRepoInsight CreateRepoInsight(IRepository repo)
        {
            var context = new GitInsightContextFactory().CreateDbContext(Array.Empty<string>());
            var insightRepository = new InsightRepository(context);

            (bool upToDate, var gitRepo) = insightRepository.HasUpToDateInsight(repo);
            if (upToDate)
            {
                return new DBRepoInsight(gitRepo);
            }
            else
            {
                var localRepo = new GitRepoInsight(repo);
                insightRepository.UpdateInsight(localRepo);
                return localRepo;
            }
        }
    }
}
