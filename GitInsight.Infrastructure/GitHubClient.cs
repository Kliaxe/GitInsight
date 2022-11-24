using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class GitHubClient
    {
        public static readonly Octokit.GitHubClient Client = GitHubApiClient();

        private static Octokit.GitHubClient GitHubApiClient()
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("GitInsight"));
            var config = new ConfigurationBuilder().AddUserSecrets<GitHubClient>().Build();
            var apiKey = config["GitHubApiKey"];
            if (apiKey != null) 
            {
                client.Credentials = new Octokit.Credentials(apiKey);
            }
            return client;
        }
    }
}
