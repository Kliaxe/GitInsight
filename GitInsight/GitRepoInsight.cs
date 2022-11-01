using LibGit2Sharp;
using System.Text;

namespace GitInsight;

public class GitRepoInsight : IGitRepoInsight
{
    private readonly IRepository repo;
    public GitRepoInsight(IRepository repo)
    {
        this.repo = repo;
    }

    public IEnumerable<DateCount> GetCommitsOverTime()
    {
        return FormatCommits(repo.Commits);
    }
    public Dictionary<string, IEnumerable<DateCount>> GetCommitsOverTimeByUser()
    {
        return repo.Commits.GroupBy(c => c.Author.Name).ToDictionary(g => g.Key, g => FormatCommits(g));
    }

    private IEnumerable<DateCount> FormatCommits(IEnumerable<Commit> commits)
    {
        return commits.GroupBy(c => c.Author.When.Date).Select(g => new DateCount(g.Key, g.Count())).OrderBy(t => t.Date);
    }
}
