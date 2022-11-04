using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public interface IAnalysisRepository
    {
        bool HasUpToDateAnalysis(IRepository repo);

        void UpdateAnalysis(GitRepoInsight gitRepoInsight);
    }
}
