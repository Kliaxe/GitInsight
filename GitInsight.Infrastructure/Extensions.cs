using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
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
            return repo.Network.Remotes.First().Name;
        }
    }
}
