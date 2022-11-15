using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public record DateCount(DateTimeOffset Date, int Count);
    public record UserDateCounts(User User, IEnumerable<DateCount> DateCounts);
    public record User(string Name, string Email);
    public record Fork(string Fullname);
    public record RepoAnalysis(IEnumerable<DateCount> DateCounts, IEnumerable<UserDateCounts> UserDateCounts, IEnumerable<Fork> Forks);
}
