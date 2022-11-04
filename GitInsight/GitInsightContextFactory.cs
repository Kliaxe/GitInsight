using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    internal class GitInsightContextFactory : IDesignTimeDbContextFactory<GitInsightContext>
    {
        public GitInsightContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            var connectionString = configuration.GetConnectionString("GitInsight");

            var optionsBuilder = new DbContextOptionsBuilder<GitInsightContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new GitInsightContext(optionsBuilder.Options);
        }
    }
}
