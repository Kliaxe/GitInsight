using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    public interface IGitRepoInsight
    {
        string GetCommitsOverTimeFormatted();
        string GetCommitsOverTimeByUserFormatted();

        IOrderedEnumerable<DateCount> GetCommitsOverTime();

        Dictionary<string, IOrderedEnumerable<DateCount>> GetCommitsOverTimeByUser();
    }

    public record DateCount(DateTimeOffset Date, int Count);
}
