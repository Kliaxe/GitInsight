using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public interface IFormatter
    {
        string GetCommitsOverTimeFormatted();
        string GetCommitsOverTimeByUserFormatted();
    }
}
