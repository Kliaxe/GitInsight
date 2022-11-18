using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GitInsight.Database
{
    public class GitInsightContextFactory : IDesignTimeDbContextFactory<GitInsightContext>
    {
        public GitInsightContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<GitInsightContextFactory>().Build();
            var connectionString = configuration.GetConnectionString("GitInsight");

            var optionsBuilder = new DbContextOptionsBuilder<GitInsightContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new GitInsightContext(optionsBuilder.Options);
        }
    }
}
