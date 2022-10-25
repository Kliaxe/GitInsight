using LibGit2Sharp;

namespace GitInsight
{
    internal class GitRepoInsight : IGitRepoInsight
    {
        private readonly IRepository repo;
        public GitRepoInsight(IRepository repo)
        {
            this.repo = repo;
        }

        public IOrderedEnumerable<DateCount> GetCommitsOverTime()
        {
            return FormatCommits(repo.Commits);
        }
        public Dictionary<string, IOrderedEnumerable<DateCount>> GetCommitsOverTimeByUser()
        {
            return repo.Commits.GroupBy(c => c.Author.Name).ToDictionary(g => g.Key, g => FormatCommits(g));
        }

        private IOrderedEnumerable<DateCount> FormatCommits(IEnumerable<Commit> commits)
        {
            return commits.GroupBy(c => c.Author.When).Select(g => new DateCount(g.Key, g.Count())).OrderBy(t => t.Date);
        }

        public string GetCommitsOverTimeByUserFormatted()
        {
            throw new NotImplementedException();
        }

        public string GetCommitsOverTimeFormatted()
        {
            throw new NotImplementedException();
        }
    }
}

internal record DateCount(DateTimeOffset Date, int Count);
