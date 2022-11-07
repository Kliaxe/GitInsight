﻿using LibGit2Sharp;
using System.Text;

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
}
