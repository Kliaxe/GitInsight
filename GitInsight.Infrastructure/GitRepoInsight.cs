using LibGit2Sharp;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace GitInsight.Infrastructure;

public class GitRepoInsight : ILocalGitRepoInsight
{
    private readonly IRepository repo;

    public string Name => repo.Name();
    public string Url => repo.Url();
    public DateTimeOffset Version => repo.Version();

    public GitRepoInsight(IRepository repo)
    {
        this.repo = repo;
    }

    public IEnumerable<DateCount> GetCommitHistory()
    {
        return FormatCommits(repo.Commits);
    }
    public IEnumerable<(User, IEnumerable<DateCount>)> GetCommitHistoryByUser()
    {
        return repo.Commits.GroupBy(c => new { c.Author.Name, c.Author.Email }).Select(g => (new User(g.Key.Name, g.Key.Email), FormatCommits(g)));
    }

    private IEnumerable<DateCount> FormatCommits(IEnumerable<Commit> commits)
    {
        return commits.GroupBy(c => c.Author.When.Date).Select(g => new DateCount(g.Key, g.Count()));
    }

    public async Task<IEnumerable<Fork>> GetForks()
    {
        client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("GitInsight"));
        var (owner, name) = repo.OwnerAndName();
        var forks = await client.Repository.Forks.GetAll(owner, name);

        return forks.Select(async r => new Fork(r.FullName, await GetForkRecursive(r))).Select(t => t.Result);
    }

    Octokit.GitHubClient client;

    private async Task<IEnumerable<Fork>> GetForkRecursive(Octokit.Repository repo)
    {
        if (repo.ForksCount == 0) return new List<Fork>();

        var forks = await client.Repository.Forks.GetAll(repo.Owner.Name, repo.Name);
        return forks.Select(async r => new Fork(r.FullName, await GetForkRecursive(r))).Select(t => t.Result);
    }
}
