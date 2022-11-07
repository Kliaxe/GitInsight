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
        private GitInsightContext _context;

        public InsightRepository(GitInsightContext context)
        {
            _context = context;
        }

        public (bool, GitRepo) HasUpToDateInsight(IRepository repo)
        {
            throw new NotImplementedException();
        }

        public void UpdateInsight(ILocalGitRepoInsight gitRepoInsight)
        {
            throw new NotImplementedException();
        }
    }
}
