using LibGit2Sharp;
using System.Collections;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;

namespace GitInsight.Infrastructure;

public class GitRepoInsight : ILocalGitRepoInsight
{
    private readonly IRepository repo;
    private readonly Octokit.IGitHubClient client;

    public string Name => repo.Name();
    public string Url => repo.Url();
    public DateTime Version => repo.Version();

    public GitRepoInsight(IRepository repo, Octokit.IGitHubClient client)
    {
        this.repo = repo;
        this.client = client;
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

    public async Task<IEnumerable<ForkDTO>> GetForks()
    {
        var (owner, name) = repo.OwnerAndName();
        var forks = await client.Repository.Forks.GetAll(owner, name);

        return forks.Select(async r => new ForkDTO(r.FullName, await GetForkRecursive(r))).Select(t => t.Result);
    }

    private async Task<IEnumerable<ForkDTO>> GetForkRecursive(Octokit.Repository repo)
    {
        string owner = repo.FullName.Split("/")[0];
        string name = repo.FullName.Split("/")[1];
        if (repo.ForksCount == 0) return new List<ForkDTO>();

        var forks = await client.Repository.Forks.GetAll(owner, name);
        return forks.Select(async r => new ForkDTO(r.FullName, await GetForkRecursive(r))).Select(t => t.Result);
    }
}
