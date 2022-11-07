using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class InsightRepository : IInsightRepository
    {
        private GitInsightContext context;

        public InsightRepository(GitInsightContext context)
        {
            this.context = context;
        }

        public (bool, GitRepo) HasUpToDateAnalysis(IRepository repo)
        {
            throw new NotImplementedException();
        }

        public void UpdateAnalysis(ILocalGitRepoInsight gitRepoInsight)
        {
            throw new NotImplementedException();
        }
    }
}
