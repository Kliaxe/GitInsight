using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GitInsight.Infrastructure
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
