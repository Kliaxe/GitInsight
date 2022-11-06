using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class GitInsightRepository : IGitInsightRepository
    {
        private GitInsightContext _context;

        public GitInsightRepository(GitInsightContext context)
        {
            _context = context;
        }

        public bool HasUpToDateGitInsight(IRepository repo)
        {
            throw new NotImplementedException();
        }

        public void UpdateGitInsight(GitRepoInsight gitRepoInsight)
        {
            throw new NotImplementedException();
        }
    }
}
