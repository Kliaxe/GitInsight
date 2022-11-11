using GitInsight.Infrastructure;
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

        public GitRepositoryIntegrationTests(DatabaseFixture fixture)
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<GitInsightContext>();
            builder.UseSqlite(connection);
            _context = new GitInsightContext(builder.Options);
            _context.Database.EnsureCreated();
            _context.SaveChanges();

            repoInsight = GitInsightRepoFactory.CreateRepoInsight(fixture.repo, _context);
        }

        [Fact]
        public void GitRepoInsightTest()
        {
            var formatter = new Formatter(repoInsight);
            string expected = $"5 26/10/2022";
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        public void Dispose() => _context.Dispose();
    }
}
