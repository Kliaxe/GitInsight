using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private GitInsightContext context;

        public AnalysisRepository(GitInsightContext context)
        {
            this.context = context;
        }

        public bool HasUpToDateAnalysis(IRepository repo)
        {
            throw new NotImplementedException();
        }

        public void UpdateAnalysis(GitRepoInsight gitRepoInsight)
        {
            throw new NotImplementedException();
        }
    }
}
