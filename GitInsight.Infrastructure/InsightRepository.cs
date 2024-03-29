﻿using LibGit2Sharp;
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

        public async Task UpdateInsight(ILocalGitRepoInsight repoInsight)
        {
            var gitRepo = context.Repositories.FirstOrDefault(r => r.Url == repoInsight.Url);
            if (gitRepo != null)
            {
                UpdateExsistingInsight(gitRepo, repoInsight);
            }
            else
            {
                gitRepo = AddNewInsight(repoInsight);
            }
            await UpdateForks(gitRepo, repoInsight);
        }

        private async Task UpdateForks(GitRepo gitRepo, ILocalGitRepoInsight repoInsight)
        {
            var forks = await repoInsight.GetForks();
            foreach (var forkDTO in forks)
            {
                AddForkRecursive(forkDTO, null, gitRepo);
            }
            context.SaveChanges();
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
                        var udc = context.UserDateCounts.FirstOrDefault(udc => udc.Date == date & udc.Email == user.Email);
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

        private GitRepo AddNewInsight(ILocalGitRepoInsight repoInsight)
        {
            var gitRepo = new GitRepo { Name = repoInsight.Name, Url = repoInsight.Url, Version = repoInsight.Version };
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

            context.SaveChanges();
            return gitRepo;
        }

        private void AddUserDateCount(GitRepo gitRepo, User user, DateCount dateCount)
        {
            var udc = new UserDateCount
            {
                GitRepoId = gitRepo.Id,
                UserName = user.Name,
                Email = user.Email,
                Date = DateTime.SpecifyKind(dateCount.Date, DateTimeKind.Unspecified),
                Count = dateCount.Count
            };
            context.UserDateCounts.Add(udc);
        }

        private void AddForkRecursive(ForkDTO forkDTO, Fork? parent, GitRepo gitRepo)
        {
            var fork = gitRepo.Forks?.FirstOrDefault(f => f.Name == forkDTO.Fullname);
            if (fork == null)
            {
                fork = new Fork
                {
                    Name = forkDTO.Fullname,
                    GitRepoId = gitRepo.Id,
                    Parent = parent
                };
                context.Add(fork);
            }
            foreach (var child in forkDTO.ChildForks)
            {
                AddForkRecursive(child, fork, gitRepo);
            }
        }
    }
}
