using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    internal interface IGitRepoInsight
    {
        string GetCommitsOverTimeFormatted();
        string GetCommitsOverTimeByUserFormatted(string user);

        IOrderedEnumerable<(DateTimeOffset, int)> GetCommitsOverTime();

        IOrderedEnumerable<(DateTimeOffset, int)> GetCommitsOverTimeByUser(string user);
    }
}
