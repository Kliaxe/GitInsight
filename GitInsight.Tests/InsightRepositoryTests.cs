using LibGit2Sharp;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class InsightRepositoryTests : IDisposable
    {
        private readonly GitInsightContext _context;
        private readonly InsightRepository _repository;
        private readonly List<UserDateCount> userDateCounts;

        public InsightRepositoryTests()
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
                Version = DateTime.Now.Date,
                Url = "hej.com",
            };
            GitRepo testRepo2 = new()
            {
                Id = 2,
                Name = "Mindre sejt repository",
                Version = DateTime.Now.Date,
                Url = "farvel.com",
            };
            context.Repositories.AddRange(testRepo1, testRepo2);
            UserDateCount adrianUserCountDate1 = new() { Email = "adrian@kari.dk", Date = DateTime.Now.Date, UserName = "AdrianStein-cloud", Count = 2, GitRepoId = 1 };
            UserDateCount adrianUserCountDate2 = new() { Email = "adrian@kari.dk", Date = DateTime.Now.Date.AddDays(1), UserName = "AdrianStein-cloud", Count = 5, GitRepoId = 2 };
            UserDateCount darlingUserCountDate1 = new() { Email = "mathi-2721@outlook.dk", Date = DateTime.Now.Date.AddDays(2), UserName = "Darling", Count = 6, GitRepoId = 1 };
            UserDateCount andréUserCountDate1 = new() { Email = "dinmor@gmail.com", Date = DateTime.Now.Date.AddDays(3), UserName = "André", Count = 7, GitRepoId = 2 };
            context.UserDateCounts.AddRange(adrianUserCountDate1, adrianUserCountDate2, darlingUserCountDate1, andréUserCountDate1);
            context.SaveChanges();
            userDateCounts = new()
            {
                adrianUserCountDate1,
                darlingUserCountDate1,
            };

            _context = context;
            _repository = new(_context);
        }

        [Fact]
        public void HasUpToDateGitInsight()
        {
            var repo = Substitute.For<IRepository>();
            var network = Substitute.For<Network>();
            var remotes = Substitute.For<RemoteCollection>();
            var remote = Substitute.For<Remote>();

            repo.Network.Returns(network);
            network.Remotes.Returns(remotes);
            remotes.GetEnumerator().Returns(new List<Remote>() { remote }.GetEnumerator());
            remote.Url.Returns("hej.com");

            var commit = Substitute.For<Commit>();
            var querylog = Substitute.For<IQueryableCommitLog>();
            var signature = new Signature("Darling", "mathi-2721@outlook.dk", DateTime.Now.Date.AddDays(2));

            repo.Commits.Returns(querylog);
            querylog.GetEnumerator().Returns(new List<Commit>() { commit }.GetEnumerator());
            commit.Author.Returns(signature);

            var actual = _repository.HasUpToDateInsight(repo);
            Assert.True(actual);
        }

        [Fact]
        public void UpdateGitInsight()
        {
            var gitRepoInsight = Substitute.For<GitRepoInsight>();
            userDateCounts.Add(new UserDateCount { UserName = "Daniel", Count = 3, Date = DateTime.Now.Date.AddDays(5), GitRepoId = 1 });
            Dictionary<string, IEnumerable<DateCount>> dateCounts = new()
            {
                { userDateCounts[0].UserName, userDateCounts.Where(u => u.UserName == userDateCounts[0].UserName).Select(u => new DateCount(u.Date, u.Count)) },
                { userDateCounts[1].UserName, userDateCounts.Where(u => u.UserName == userDateCounts[1].UserName).Select(u => new DateCount(u.Date, u.Count)) },
                { userDateCounts[2].UserName, userDateCounts.Where(u => u.UserName == userDateCounts[2].UserName).Select(u => new DateCount(u.Date, u.Count)) }
            };
            gitRepoInsight.GetCommitsOverTimeByUser().Returns(dateCounts);
            _repository.UpdateInsight(gitRepoInsight);
            Assert.Equal(_context.UserDateCounts.Where(u => u.GitRepoId == 1), userDateCounts);
        }

        public void Dispose() => _context.Dispose();
    }
}
