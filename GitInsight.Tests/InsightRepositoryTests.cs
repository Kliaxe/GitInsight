using GitInsight.Infrastructure;
using NSubstitute;

namespace GitInsight.Tests
{
    public class InsightRepositoryTests : IDisposable
    {
        private readonly GitInsightContext _context;
        private readonly InsightRepository _repository;


        private readonly GitRepo repo1;
        private readonly GitRepo repo2;
        private readonly DateTimeOffset version;
        private readonly Dictionary<User, List<DateCount>> dateCounts;

        private readonly UserDateCount u1;
        private readonly UserDateCount u2;
        private readonly UserDateCount u3;
        private readonly UserDateCount u4;
        private readonly UserDateCount u5;
        private readonly UserDateCount u6;

        public InsightRepositoryTests()
        {
            //Sqlite
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            var context = new GitInsightContext(builder.Options);
            context.Database.EnsureCreated();

            //Database insight data
            version = DateTime.Now.AddDays(3);
            repo1 = new() { Name = "Repo 1", Url = "url1.com", Version = version };
            repo2 = new() { Name = "Repo 2", Url = "url2.com" };
            u1 = new() { Count = 2, Date = DateTime.Now.Date, Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = 1 };
            u2 = new() { Count = 6, Date = DateTime.Now.Date.AddDays(2), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = 1 };
            u3 = new() { Count = 4, Date = DateTime.Now.Date.AddDays(2), Email = "lukas@gmail.com", UserName = "Lukas", GitRepoId = 1 };
            u4 = new() { Count = 7, Date = DateTime.Now.Date.AddDays(3), Email = "daniel@gmail.com", UserName = "Daniel", GitRepoId = 1 };
            u5 = new() { Count = 2, Date = DateTime.Now.Date.AddDays(3), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = 1 };
            u6 = new() { Count = 5, Date = DateTime.Now.Date.AddDays(1), Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = 2 };
            _context = context;
            
            

            //Local insight data
            dateCounts = new()
            {
                {
                    new("Adrian", "adrian@kari.dk"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date, 2)
                    }
                },
                {
                    new("Darling", "mathias@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(2), 6),
                        new(DateTime.Now.Date.AddDays(3), 2)
                    }
                },
                {
                    new("Lukas", "lukas@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(2), 4)
                    }
                },
                {
                    new("Daniel", "daniel@gmail.com"), new List<DateCount>()
                    {
                        new(DateTime.Now.Date.AddDays(3), 7)
                    }
                }
            };

            _repository = new(_context);
        }

        [Fact]
        public void HasUpToDateInsight_Returns_True()
        {
            var repo = CreateIRepositorySubstitute("url1.com", version);
            LoadDatabaseData();

            (bool actual, var gitRepo) = _repository.HasUpToDateInsight(repo);

            actual.Should().Be(true);
            gitRepo.Name.Should().Be("Repo 1");
        }

        [Fact]
        public void HasUpToDateInsight_Returns_False()
        {
            var repo = CreateIRepositorySubstitute("url1.com", version.AddMinutes(127));
            LoadDatabaseData();

            (bool actual, var gitRepo) = _repository.HasUpToDateInsight(repo);

            actual.Should().Be(false);
            gitRepo.Should().Be(null);
        }

        [Fact]
        public void UpdateInsight_Updates_Version()
        {
            var repoInsight = CreateRepoInsightSubstitute();

            var expected = version.AddHours(5).AddSeconds(127);
            repoInsight.Version.Returns(expected);
            LoadDatabaseData();

            _repository.UpdateInsight(repoInsight);

            var actual = repo1.Version;
            actual.Should().Be(expected);
        }

        [Fact]
        public void UpdateInsight_Adds_New_UserDateCount()
        {
            var user = new User("Daniel", "daniel@gmail.com");
            dateCounts[user].Add(new DateCount(DateTime.Now.Date.AddDays(5), 3));

            var repoInsight = CreateRepoInsightSubstitute();
            LoadDatabaseData();

            _repository.UpdateInsight(repoInsight);

            var userDateCounts = repo1.UserDateCounts;
            userDateCounts.Should().Contain(u => u.UserName == "Daniel" && u.Date == DateTime.Now.Date.AddDays(5) && u.Count == 3);
        }

        [Fact]
        public void UpdateInsight_Updates_Exsisting_UserDateCount()
        {
            var user = new User("Daniel", "daniel@gmail.com");
            dateCounts[user].Clear();
            dateCounts[user].Add(new DateCount(DateTime.Now.Date.AddDays(3), 10));

            var repoInsight = CreateRepoInsightSubstitute();
            LoadDatabaseData();

            _repository.UpdateInsight(repoInsight);

            var userDateCounts = repo1.UserDateCounts;
            userDateCounts.Should().Contain(u => u.UserName == "Daniel" && u.Date == DateTime.Now.Date.AddDays(3) && u.Count == 10);
            userDateCounts.Should().NotContain(u => u.UserName == "Daniel" && u.Date == DateTime.Now.Date.AddDays(3) && u.Count == 7);
        }

        [Fact]
        public void UpdateInsight_Creates_New_Insight_In_Database()
        {
            var repoInsight = CreateRepoInsightSubstitute();
            repoInsight.Name.Returns("New Repo");
            var expected = new List<UserDateCount>() { u1, u2, u3, u4, u5 };

            _repository.UpdateInsight(repoInsight);

            var gitRepo = _context.Repositories.First(r => r.Name == "New Repo");
            gitRepo.Should().NotBe(null);
            var actual = gitRepo.UserDateCounts;
            actual.Should().BeEquivalentTo(expected);
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

        private ILocalGitRepoInsight CreateRepoInsightSubstitute()
        {
            var gitRepoInsight = Substitute.For<ILocalGitRepoInsight>();
            var formattedData = dateCounts.Select(pair => (pair.Key, (IEnumerable<DateCount>)pair.Value));
            gitRepoInsight.GetCommitHistoryByUser().Returns(formattedData);
            gitRepoInsight.Url.Returns("url1.com");
            gitRepoInsight.Version.Returns(version);
            return gitRepoInsight;
        }

        private void LoadDatabaseData()
        {
            _context.Repositories.AddRange(repo1, repo2);
            _context.SaveChanges();
            _context.UserDateCounts.AddRange(u1, u2, u3, u4, u5, u6);
            _context.SaveChanges();
        }
    }
}
