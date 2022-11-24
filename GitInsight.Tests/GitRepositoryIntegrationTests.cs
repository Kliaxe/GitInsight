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
        }

        [Fact]
        public void GitRepoInsightTest()
        {
            var repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, _context, GitHubClient.Client);
            var formatter = new Formatter(repoInsight);
            string expected = $"5 26/10/2022";
            repoInsight.Should().BeAssignableTo<GitRepoInsight>();
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        [Fact]
        public async Task DBRepoInsightTest()
        {
            GitInsightRepoFactory.CreateRepoInsight(repo, _context, GitHubClient.Client);
            await GitInsightRepoFactory.datebaseUpdate;
            var repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, _context, GitHubClient.Client);
            var formatter = new Formatter(repoInsight);
            string expected = $"5 26/10/2022";
            repoInsight.Should().BeAssignableTo<DBRepoInsight>();
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        public void Dispose() => _context.Dispose();
    }
}
