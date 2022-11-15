namespace GitInsight.Tests
{
    public class DBRepoTests : IDisposable
    {
        private readonly DBRepoInsight _repository;
        private readonly GitInsightContext _context;

        public DBRepoTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            _context = new GitInsightContext(builder.Options);
            _context.Database.EnsureCreated();
            GitRepo repo1 = new() { Name = "Repo 1", Url = "url1.com" };
            GitRepo repo2 = new() { Name = "Repo 2", Url = "url2.com" };
            _context.Repositories.AddRange(repo1, repo2);
            _context.SaveChanges();
            UserDateCount u1 = new() { Count = 2, Date = DateTime.Now.Date, Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = repo1.Id };
            UserDateCount u2 = new() { Count = 6, Date = DateTime.Now.Date.AddDays(2), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = repo1.Id };
            UserDateCount u3 = new() { Count = 4, Date = DateTime.Now.Date.AddDays(2), Email = "lukas@gmail.com", UserName = "Lukas", GitRepoId = repo1.Id };
            UserDateCount u4 = new() { Count = 7, Date = DateTime.Now.Date.AddDays(3), Email = "daniel@gmail.com", UserName = "Daniel", GitRepoId = repo1.Id };
            UserDateCount u5 = new() { Count = 2, Date = DateTime.Now.Date.AddDays(3), Email = "mathias@gmail.com", UserName = "Darling", GitRepoId = repo1.Id };
            UserDateCount u6 = new() { Count = 5, Date = DateTime.Now.Date.AddDays(1), Email = "adrian@kari.dk", UserName = "Adrian", GitRepoId = repo2.Id };
            _context.UserDateCounts.AddRange(u1, u2, u3, u4, u5, u6);
            _context.SaveChanges();
            _repository = new DBRepoInsight(repo1);
        }

        [Fact]
        public void GetCommitHistory_Returns_Commits_Over_Time()
        {
            List<DateCount> expected = new() {
                new(DateTime.Now.Date.AddDays(2), 10),
                new(DateTime.Now.Date, 2),
                new(DateTime.Now.Date.AddDays(3), 9),
            };

            var actual = _repository.GetCommitHistory();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void GetCommitHistoryByUser_Returns_Commits_Over_Time_By_User()
        {
            List<(User, List<DateCount>)> expected = new()
            {
                (
                    new("Adrian", "adrian@kari.dk"), new()
                    {
                        new(DateTime.Now.Date, 2)
                    }
                ),
                (
                    new("Darling", "mathias@gmail.com"), new()
                    {
                        new(DateTime.Now.Date.AddDays(2), 6),
                        new(DateTime.Now.Date.AddDays(3), 2)
                    }
                ),
                (
                    new("Lukas", "lukas@gmail.com"), new()
                    {
                        new(DateTime.Now.Date.AddDays(2), 4)
                    }
                ),
                (
                    new("Daniel", "daniel@gmail.com"), new()
                    {
                        new(DateTime.Now.Date.AddDays(3), 7)
                    }
                )
            };

            var actual = _repository.GetCommitHistoryByUser();

            actual.Should().BeEquivalentTo(expected);
        }

        public void Dispose() => _context.Dispose();
    }

}
