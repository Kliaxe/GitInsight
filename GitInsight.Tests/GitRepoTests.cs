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
        var person1 = new Signature("Lukas", "mail", DateTimeOffset.Now.Date);
        var person2 = new Signature("Adrian", "mail", DateTimeOffset.Now.Date);
        var person3 = new Signature("Adrian", "mail", DateTimeOffset.Now.AddDays(1).Date);



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


        repoInsight = new GitRepoInsight(repo);
    }

    [Fact]
    public void GetCommitsOverTime()
    {
        List<DateCount> expected = new()
        {
            new DateCount(DateTimeOffset.Now.Date, 5),
            new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1)
        };

        Assert.Equal(expected, repoInsight.GetCommitsOverTime());
    }

    [Fact]
    public void GetCommitsOverTimeByUser()
    {
        var expected = new Dictionary<string, IEnumerable<DateCount>>()
        {
            {
                "Lukas",
                new List<DateCount>()
                {
                    new DateCount(DateTimeOffset.Now.Date, 2),
                }
            },
            {
                "Adrian",
                new List<DateCount>()
                {
                    new DateCount(DateTimeOffset.Now.Date, 3),
                    new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1),
                }
            },
        };

        Assert.Equal(expected, repoInsight.GetCommitsOverTimeByUser());
    }
}