using System;
using System.Security.Policy;

namespace GitInsight.Tests;

public class GitRepoTests
{
    private readonly IGitRepoInsight repoInsight;

    private Commit CreateCommit(Signature signature)
    {
        var commit = Substitute.For<Commit>();
        commit.Author.Returns(signature);
        return commit;
    }

    public GitRepoTests()
    {
        var person1 = new Signature("Lukas", "lu@mail", DateTimeOffset.Now.Date);
        var person2 = new Signature("Adrian", "ad@mail", DateTimeOffset.Now.Date);
        var person3 = new Signature("Adrian", "ad@mail", DateTimeOffset.Now.AddDays(1).Date);



        List<Commit> commits = new()
        {
            CreateCommit(person3),
            CreateCommit(person1),
            CreateCommit(person1),
            CreateCommit(person2),
            CreateCommit(person2),
            CreateCommit(person2),
        };

        var repo = Substitute.For<IRepository>();
        var querylog = Substitute.For<IQueryableCommitLog>();
        querylog.GetEnumerator().Returns(commits.GetEnumerator());
        repo.Commits.Returns(querylog);


        var network = Substitute.For<Network>();
        var remotes = Substitute.For<RemoteCollection>();
        var remote = Substitute.For<Remote>();

        repo.Network.Returns(network);
        network.Remotes.Returns(remotes);
        remotes.GetEnumerator().Returns(new List<Remote>() { remote }.GetEnumerator());
        remote.Url.Returns("https://github.com/itu-bdsa/project-description");


        repoInsight = new GitRepoInsight(repo, GitHubClient.Client);
    }

    [Fact]
    public void GetCommitsOverTime()
    {
        IEnumerable<DateCount> expected = new List<DateCount>()
        {
            new DateCount(DateTimeOffset.Now.Date, 5),
            new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1)
        };
        var actual = repoInsight.GetCommitHistory();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void GetCommitsOverTimeByUser()
    {
        var expected = new List<(User, IEnumerable<DateCount>)>()
        {
            (
                new User("Lukas", "lu@mail"),
                new List<DateCount>()
                {
                    new DateCount(DateTimeOffset.Now.Date, 2),
                }
            ),
            (
                new User("Adrian", "ad@mail"),
                new List<DateCount>()
                {
                    new DateCount(DateTimeOffset.Now.Date, 3),
                    new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1),
                }
            ),
        };
        var actual = repoInsight.GetCommitHistoryByUser();
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetForks()
    {
        var forks = await repoInsight.GetForks();

        forks.Should().Contain(f => f.Fullname == "Lukski175/project-description");
    }
}