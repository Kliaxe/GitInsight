using LibGit2Sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    internal class DBRepoInsight : IGitRepoInsight
    {
        private readonly GitInsightContext context;
        public DBRepoInsight(GitInsightContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<DateCount> GetCommitsOverTime()
        {
            return context.UserDateCounts.GroupBy(c => c.Date.Date).Select(g => new DateCount(g.Key, g.Sum(u => u.Count)));
        }

        public Dictionary<string, IEnumerable<DateCount>> GetCommitsOverTimeByUser()
        {
            return context.UserDateCounts.GroupBy(c => new { c.Email, c.UserName })
                .ToDictionary(g => g.Key.UserName, g => g.Select(u => new DateCount(u.Date.Date, u.Count)));
        }
    }
}
