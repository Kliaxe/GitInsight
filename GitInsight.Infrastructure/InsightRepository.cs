using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    public class InsightRepository : IInsightRepository
    {
        private readonly GitInsightContext context;

        public InsightRepository(GitInsightContext context)
        {
            this.context = context;
        }

        public (bool, GitRepo) HasUpToDateInsight(IRepository repo)
        {
            var url = repo.Url();
            var gitRepo = context.Repositories.FirstOrDefault(r => r.Url == url);
            if (gitRepo != null)
            {
                bool upToDate = gitRepo.Version == repo.Version();
                if(upToDate) return (true, gitRepo);
            }
            return (false, null!);
        }

        public void UpdateInsight(ILocalGitRepoInsight repoInsight)
        {
            var gitRepo = context.Repositories.FirstOrDefault(r => r.Url == repoInsight.Url);
            if (gitRepo != null)
            {
                UpdateExsistingInsight(gitRepo, repoInsight);
            }
            else
            {
                AddNewInsight(repoInsight);
            }
        }

        private void UpdateExsistingInsight(GitRepo gitRepo, ILocalGitRepoInsight repoInsight)
        {
            var insight = repoInsight.GetCommitHistoryByUser();
            var databaseVersionDate = gitRepo.Version.Date;
            gitRepo.Version = repoInsight.Version;
            context.Repositories.Update(gitRepo);
            foreach ((var user, var dateCounts) in insight)
            {
                foreach (var dateCount in dateCounts)
                {
                    var date = dateCount.Date;
                    if (date > databaseVersionDate) AddUserDateCount(gitRepo, user, dateCount);
                    else if (dateCount.Date == databaseVersionDate)
                    {
                        var udc = context.UserDateCounts.First(udc => udc.Date == date & udc.Email == user.Email);
                        if (udc != null)
                        {
                            udc.Count = dateCount.Count;
                            context.UserDateCounts.Update(udc);
                        }
                        else AddUserDateCount(gitRepo, user, dateCount);
                    }
                }
            }
            context.SaveChanges();
        }

        private void AddNewInsight(ILocalGitRepoInsight repoInsight)
        {
            var gitRepo = new GitRepo { Name = repoInsight.Name, Url = repoInsight.Url, Version = repoInsight.Version};
            context.Repositories.Add(gitRepo);
            context.SaveChanges();

            var insight = repoInsight.GetCommitHistoryByUser();
            foreach ((var user, var dateCounts) in insight)
            {
                foreach (var dateCount in dateCounts)
                {
                    AddUserDateCount(gitRepo, user, dateCount);
                }
            }
        }
        private void AddUserDateCount(GitRepo gitRepo, User user, DateCount dateCount)
        {
            var udc = new UserDateCount
            {
                GitRepoId = gitRepo.Id,
                UserName = user.Name,
                Email = user.Email,
                Date = dateCount.Date,
                Count = dateCount.Count
            };
            context.UserDateCounts.Add(udc);
        }
    }
}
