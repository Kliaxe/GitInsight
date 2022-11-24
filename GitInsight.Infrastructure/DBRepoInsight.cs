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

        public string Name => gitRepo.Name;
        public string Url => gitRepo.Url;
        public DateTime Version => gitRepo.Version;

        public DBRepoInsight(GitRepo gitRepo)
        {
            this.gitRepo = gitRepo;
        }

        public IEnumerable<DateCount> GetCommitHistory()
        {
            return gitRepo.UserDateCounts.GroupBy(c => c.Date.Date).Select(g => new DateCount(g.Key, g.Sum(u => u.Count)));
        }

        public IEnumerable<(User, IEnumerable<DateCount>)> GetCommitHistoryByUser()
        {
            return gitRepo.UserDateCounts.GroupBy(c => new { c.Email, c.UserName }).Select(g => (new User(g.Key.UserName, g.Key.Email), g.Select(u => new DateCount(u.Date, u.Count))));
        }

        public async Task<IEnumerable<ForkDTO>> GetForks()
        {
            await Task.Yield();
            var rootParents = gitRepo.Forks.Where(f => f.Parent == null);
            return GetForksRecursive(rootParents);
        }

        private IEnumerable<ForkDTO> GetForksRecursive(IEnumerable<Fork> forks)
        {
            if (forks == null) return new List<ForkDTO>();
            return forks.Select(f => new ForkDTO(f.Name, GetForksRecursive(f.Children)));
        }
    }
}
