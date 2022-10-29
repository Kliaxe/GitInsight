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
        return commits.GroupBy(c => c.Author.When.Date).Select(g => new DateCount(g.Key, g.Count())).OrderBy(t => t.Date);
    }

    public string GetCommitsOverTimeByUserFormatted()
    {
        return GetCommitsOverTimeByUser().Select(pair => pair.Key + "\n" + FormatDateCount(pair.Value)).Aggregate((s1, s2) => s1 + "\n\n" + s2);
        /*StringBuilder sb = new();
        GetCommitsOverTimeByUser().ToList().ForEach(d => sb.Append(d.Key).Append("\n").Append(d.Value.Select(dc => sb.Append(dc.Count + " " + dc.Date + "\n"))));
        return sb.ToString();  */      
    }

    public string GetCommitsOverTimeFormatted()
    {
        return FormatDateCount(GetCommitsOverTime());
    }

    private string FormatDateCount(IEnumerable<DateCount> dateCounts)
    {
        return dateCounts.Select(dc => $"{dc.Count} {dc.Date.Date}").Aggregate((s1, s2) => s1 + "\n" + s2);
    }
}
