using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    public class Formatter : IFormatter
    {
        private readonly IGitRepoInsight repoInsight;

        public Formatter(IGitRepoInsight repoInsight)
        {
            this.repoInsight = repoInsight;
        }

        public string GetCommitsOverTimeByUserFormatted()
        {
            return repoInsight.GetCommitsOverTimeByUser().Select(pair => pair.Key + "\n" + FormatDateCount(pair.Value)).Aggregate((s1, s2) => s1 + "\n\n" + s2);
        }

        public string GetCommitsOverTimeFormatted()
        {
            return FormatDateCount(repoInsight.GetCommitsOverTime());
        }

        private string FormatDateCount(IEnumerable<DateCount> dateCounts)
        {
            return dateCounts.OrderBy(d => d.Date).Select(dc => $"{dc.Count} {dc.Date.Date}").Aggregate((s1, s2) => s1 + "\n" + s2);
        }
    }
}
