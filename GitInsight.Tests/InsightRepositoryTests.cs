using GitInsight.Infrastructure;

namespace GitInsight.Tests
{
    public class InsightRepositoryTests : IDisposable
    {
        private readonly GitInsightContext _context;
        private readonly InsightRepository _repository;
        private readonly List<UserDateCount> userDateCounts;
        private readonly DateTimeOffset version;

        private readonly GitRepo repo1;

        public InsightRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            var context = new GitInsightContext(builder.Options);
            context.Database.EnsureCreated();
            version = DateTime.Now.AddHours(127);
            repo1 = new() { Name = "Repo 1", Url = "url1.com", Version = version };
            GitRepo repo2 = new() { Name = "Repo 2", Url = "url2.com" };
            context.Repositories.AddRange(repo1, repo2);
            context.SaveChanges();
            UserDateCount u1 = new() { Count = 2, Date = DateTime.Now.Date, Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = repo1.Id };
            UserDateCount u2 = new() { Count = 6, Date = DateTime.Now.Date.AddDays(2), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = repo1.Id };
            UserDateCount u3 = new() { Count = 4, Date = DateTime.Now.Date.AddDays(2), Email = "lukas@gmail.com", UserName = "Lukas", GitRepoId = repo1.Id };
            UserDateCount u4 = new() { Count = 7, Date = DateTime.Now.Date.AddDays(3), Email = "daniel@gmail.com", UserName = "Daniel", GitRepoId = repo1.Id };
            UserDateCount u5 = new() { Count = 2, Date = DateTime.Now.Date.AddDays(3), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = repo1.Id };
            UserDateCount u6 = new() { Count = 5, Date = DateTime.Now.Date.AddDays(1), Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = repo2.Id };
            context.UserDateCounts.AddRange(u1, u2, u3, u4, u5, u6);
            context.SaveChanges();

            userDateCounts = new() { u1, u2 };
            _context = context;
            _repository = new(_context);
        }

        [Fact]
        public void HasUpToDateInsight_Returns_True()
        {
            var repo = CreateIRepositorySubstitute("url1.com", version);

            (bool actual, var gitRepo) = _repository.HasUpToDateInsight(repo);

            actual.Should().Be(true);
            gitRepo.Name.Should().Be("Repo 1");
        }

        [Fact]
        public void HasUpToDateInsight_Returns_False()
        {
            var repo = CreateIRepositorySubstitute("url1.com", version.AddMinutes(127));

            (bool actual, var gitRepo) = _repository.HasUpToDateInsight(repo);

            actual.Should().Be(false);
            gitRepo.Should().Be(null);
        }

        [Fact]
        public void UpdateGitInsight_Adds_New_Entry()
        {
            var gitRepoInsight = Substitute.For<ILocalGitRepoInsight>();

            List<(User, IEnumerable<DateCount>)> dateCounts = new()
            {
                (
                    new("Adrian", "adrian@kari.dk"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date, 2)
                    }
                ),
                (
                    new("Darling", "mathias@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(2), 6),
                        new(DateTime.Now.Date.AddDays(3), 2)
                    }
                ),
                (
                    new("Lukas", "lukas@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(2), 4)
                    }
                ),
                (
                    new("Daniel", "daniel@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(3), 7),
                        new(DateTime.Now.Date.AddDays(5), 3)
                    }
                )
            };

            gitRepoInsight.GetCommitHistoryByUser().Returns(dateCounts);
            gitRepoInsight.Url.Returns("url1.com");

            _repository.UpdateInsight(gitRepoInsight);

            var userDateCounts = repo1.UserDateCounts;
            userDateCounts.Should().ContainSingle(u => u.UserName == "Daniel" && u.Date == DateTime.Now.Date.AddDays(5) && u.Count == 3);
        }

        public void Dispose() => _context.Dispose();

        private IRepository CreateIRepositorySubstitute(string url, DateTimeOffset latestCommit)
        {
            var repo = Substitute.For<IRepository>();
            var network = Substitute.For<Network>();
            var remotes = Substitute.For<RemoteCollection>();
            var remote = Substitute.For<Remote>();

            repo.Network.Returns(network);
            network.Remotes.Returns(remotes);
            remotes.GetEnumerator().Returns(new List<Remote>() { remote }.GetEnumerator());
            remote.Url.Returns(url);

            var commit = Substitute.For<Commit>();
            var querylog = Substitute.For<IQueryableCommitLog>();
            var signature = new Signature("name", "name@mail.com", latestCommit);

            repo.Commits.Returns(querylog);
            querylog.GetEnumerator().Returns(new List<Commit>() { commit }.GetEnumerator());
            commit.Author.Returns(signature);

            return repo;
        }

        private IEnumerable<(User, IEnumerable<DateCount>)> ModifyExsistingUserAndGetUserDateCounts(GitRepo repo, User user, DateCount dateCount)
        {
            var insight = new DBRepoInsight(repo).GetCommitHistoryByUser().ToDictionary(t => t.Item1, t => t.Item2);
            insight[user] = insight[user].Append(dateCount);
            return insight.Select(pair => (pair.Key, pair.Value));
        }
    }
}
