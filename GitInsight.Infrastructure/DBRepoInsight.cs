using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class DBRepoInsight : IGitRepoInsight
    {
        private readonly GitInsightContext context;
        public DBRepoInsight(GitInsightContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<DateCount> GetCommitsOverTime()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, IEnumerable<DateCount>> GetCommitsOverTimeByUser()
        {
            throw new NotImplementedException();
        }
    }
}
