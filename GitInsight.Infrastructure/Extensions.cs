using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public static class Extensions
    {
        public static string Url(this IRepository repo)
        {
            return repo.Network.Remotes.First().Url;
        }

        public static string Name(this IRepository repo)
        {
            var (_, name) = repo.OwnerAndName();
            return name;
        }

        public static DateTime Version(this IRepository repo)
        {
            return repo.Commits.Max(c => c.Author.When).DateTime;
        }

        public static (string, string) OwnerAndName(this IRepository repo)
        {
            string url = repo.Url();
            var match = Regex.Match(url, @"github\.[^/]+/(?<owner>[^/]+)/(?<name>[^/\.]+)");
            return (match.Groups["owner"].Value, match.Groups["name"].Value);
        }
    }
}
