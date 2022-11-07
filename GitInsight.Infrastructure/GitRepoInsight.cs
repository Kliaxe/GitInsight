﻿using LibGit2Sharp;
using System.Text;

namespace GitInsight.Infrastructure;

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
    public IEnumerable<(User, IEnumerable<DateCount>)> GetCommitsOverTimeByUser()
    {
        return repo.Commits.GroupBy(c => new { c.Author.Name, c.Author.Email }).Select(g => (new User(g.Key.Name, g.Key.Email), FormatCommits(g)));
    }

    private IEnumerable<DateCount> FormatCommits(IEnumerable<Commit> commits)
    {
        return commits.GroupBy(c => c.Author.When.Date).Select(g => new DateCount(g.Key, g.Count()));
    }
}
