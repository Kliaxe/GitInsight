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
        private readonly GitInsightContext _context;
        public DBRepoInsight(GitInsightContext context)
        {
            _context = context;
        }

        public IEnumerable<DateCount> GetCommitsOverTime() => FormatUserDateCounts(_context.UserDateCounts);

        public Dictionary<string, IEnumerable<DateCount>> GetCommitsOverTimeByUser()
            => _context.UserDateCounts.GroupBy(u => u.UserName).ToDictionary(g => g.Key, g => FormatUserDateCounts(g));
        

        private IEnumerable<DateCount> FormatUserDateCounts(IEnumerable<UserDateCount> userDateCounts)
            => userDateCounts.Select(g => new DateCount(g.Date, g.Count)).OrderBy(t => t.Date);
        
    }
}
