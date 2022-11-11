using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
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
            return repoInsight.GetCommitHistoryByUser().OrderBy(t => t.Item1.name).Select(t => t.Item1.name + "\n" + FormatDateCount(t.Item2)).Aggregate((s1, s2) => s1 + "\n\n" + s2);
        }

        public string GetCommitsOverTimeFormatted()
        {
            return FormatDateCount(repoInsight.GetCommitHistory());
        }

        private string FormatDateCount(IEnumerable<DateCount> dateCounts)
        {
            return dateCounts.OrderBy(d => d.Date).Select(dc => $"{dc.Count} {dc.Date.Date.ToString("d", CultureInfo.GetCultureInfo("fr-FR"))}").Aggregate((s1, s2) => s1 + "\n" + s2);
        }
    }
}
