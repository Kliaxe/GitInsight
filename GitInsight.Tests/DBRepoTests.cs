using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class DBRepoTests
    {
        private readonly GitInsightContext _context;
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
            UserDateCount adrianUserCountDate2 = new() { Email = "adrian@kari.dk", Date = DateTime.Now.Date, UserName = "AdrianStein-cloud", Count = 5, GitRepoId = 2 };
            UserDateCount darlingUserCountDate1 = new() { Email = "mathi-2721@outlook.dk", Date = DateTime.Now.Date, UserName = "FisseDarling", Count = 6, GitRepoId = 1 };
            UserDateCount andréUserCountDate1 = new() { Email = "dinmor@gmail.com", Date = DateTime.Now.Date, UserName = "André", Count = 7, GitRepoId = 2 };
            context.UserDateCounts.AddRange(adrianUserCountDate1, adrianUserCountDate2, darlingUserCountDate1, andréUserCountDate1);
            context.SaveChanges();

            _context = context;
            _repository = new DBRepoInsight(_context);
        }

        [Fact]
        public void Test_test_test()
        {
            var commits = _repository.GetCommitsOverTime();

            List<DateCount> expected = new List<DateCount>();

            Assert.Equal(expected, commits);
        }


    }
}
