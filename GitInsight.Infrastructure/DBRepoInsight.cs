using LibGit2Sharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class DBRepoInsight : IGitRepoInsight
    {
        private readonly GitRepo gitRepo;
        public DBRepoInsight(GitRepo gitRepo)
        {
            this.gitRepo = gitRepo;
        }

        public IEnumerable<DateCount> GetCommitsOverTime()
        {
            return gitRepo.UserDateCounts.GroupBy(c => c.Date.Date).Select(g => new DateCount(g.Key, g.Sum(u => u.Count)));
        }

        public IEnumerable<(User, IEnumerable<DateCount>)> GetCommitsOverTimeByUser()
        {
            return gitRepo.UserDateCounts.GroupBy(c => new { c.Email, c.UserName }).Select(g => (new User(g.Key.UserName, g.Key.Email), g.Select(u => new DateCount(u.Date, u.Count))));
        }
    }
}
