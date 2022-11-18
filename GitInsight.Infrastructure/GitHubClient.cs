using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class GitHubClient
    {
        public static readonly Octokit.GitHubClient Client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("GitInsight"));
    }
}
