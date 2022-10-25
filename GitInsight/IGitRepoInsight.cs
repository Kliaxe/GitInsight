using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight
{
    internal interface IGitRepoInsight
    {
        string GetCommitsOverTime();
        string GetCommitsOverTimeByUser(string user);
    }
}
