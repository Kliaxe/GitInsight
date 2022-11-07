namespace GitInsight.Tests
{
    public class DBRepoTests
    {
        private readonly DBRepoInsight _repository;

        public DBRepoTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            var context = new GitInsightContext(builder.Options);
            context.Database.EnsureCreated();
            GitRepo testRepo1 = new()
            {
                Id = 1,
                Name = "Mega sejt repository",
                Version = DateTime.Now.Date
            };
            GitRepo testRepo2 = new()
            {
                Id = 2,
                Name = "Mindre sejt repository",
                Version = DateTime.Now.Date
            };
            context.Repositories.AddRange(testRepo1, testRepo2);
            UserDateCount adrianUserCountDate1 = new() { Email = "adrian@kari.dk", Date = DateTime.Now.Date, UserName = "AdrianStein-cloud", Count = 2, GitRepoId = 1 };
            UserDateCount adrianUserCountDate2 = new() { Email = "adrian@kari.dk", Date = DateTime.Now.Date.AddDays(1), UserName = "AdrianStein-cloud", Count = 5, GitRepoId = 2 };
            UserDateCount darlingUserCountDate1 = new() { Email = "mathi-2721@outlook.dk", Date = DateTime.Now.Date.AddDays(2), UserName = "FisseDarling", Count = 6, GitRepoId = 1 };
            UserDateCount andréUserCountDate1 = new() { Email = "dinmor@gmail.com", Date = DateTime.Now.Date.AddDays(3), UserName = "André", Count = 7, GitRepoId = 2 };
            context.UserDateCounts.AddRange(adrianUserCountDate1, adrianUserCountDate2, darlingUserCountDate1, andréUserCountDate1);
            context.SaveChanges();
            _repository = new DBRepoInsight(testRepo1);
        }

        [Fact]
        public void Commits_Over_Time_Returns_Commits_Over_Time()
        {
            List<DateCount> expected = new() {
                new(DateTime.Now.Date, 2),
                new(DateTime.Now.Date.AddDays(2), 6),
                new(DateTime.Now.Date.AddDays(1), 5),
                new(DateTime.Now.Date.AddDays(3), 7)
            };

            var actual = _repository.GetCommitsOverTime();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Commits_Over_Time_By_User_Return_Commits_Over_Time_By_User()
        {
            List<(User, List<DateCount>)> expected = new()
            {
                ( new User("AdrianStein-cloud", "adrian@kari.dk"), new List<DateCount>() { 
                    new(DateTime.Now.Date, 2), 
                    new(DateTime.Now.Date.AddDays(2), 6) 
                }),
                ( new User("FisseDarling", "mathi-2721@outlook.dk"), new List<DateCount>() { 
                    new(DateTime.Now.Date.AddDays(1), 5) 
                }),
                ( new User("André", "dinmor@gmail.com"), new List<DateCount>() { 
                    new(DateTime.Now.Date.AddDays(3), 7) 
                })
            };

            var actual = _repository.GetCommitsOverTimeByUser();

            actual.Should().BeEquivalentTo(expected);
        }

    }
}
