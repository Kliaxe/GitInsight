using GitInsight.Infrastructure;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class GitRepositoryIntegrationTests : IClassFixture<DatabaseFixture>, IDisposable
    {
        private IGitRepoInsight repoInsight;
        private readonly GitInsightContext _context;
        private IRepository repo;

        public GitRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            _context = new GitInsightContext(builder.Options);
            _context.Database.EnsureCreated();
            _context.SaveChanges();
            repo = fixture.repo;
            repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, _context, GitHubClient.Client);
        }

        [Fact]
        public void GitRepoInsightTest()
        {
            var formatter = new Formatter(repoInsight);
            string expected = $"5 26/10/2022";
            repoInsight.GetType().Should().Be(typeof(GitRepoInsight));
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        [Fact]
        public void DBRepoInsightTest()
        {
            repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, _context, GitHubClient.Client);

            var formatter = new Formatter(repoInsight);
            string expected = $"5 26/10/2022";
            repoInsight.GetType().Should().Be(typeof(DBRepoInsight));
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        public void Dispose() => _context.Dispose();
    }
}
