using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public interface IGitRepoInsight
    {
        IEnumerable<DateCount> GetCommitsOverTime();

        IEnumerable<(User, IEnumerable<DateCount>)> GetCommitsOverTimeByUser();
    }
}
