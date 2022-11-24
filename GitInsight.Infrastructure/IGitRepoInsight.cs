using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public interface IGitRepoInsight
    {
        string Name { get; }
        string Url { get; }
        DateTime Version { get; }
        IEnumerable<DateCount> GetCommitHistory();

        IEnumerable<(User, IEnumerable<DateCount>)> GetCommitHistoryByUser();

        Task<IEnumerable<ForkDTO>> GetForks();
    }
}
