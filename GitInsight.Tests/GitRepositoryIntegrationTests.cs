using GitInsight.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class GitRepositoryIntegrationTests
    {
        [Fact]
        public void GitRepoInsightTest()
        {
            var path = "";
            var repo = new Repository(path);
            var repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo);
            var formatter = new Formatter(repoInsight);
            var output = formatter.GetCommitsOverTimeFormatted();
        }
    }
}
