﻿namespace GitInsight;

public class Program
{
    public static bool UserMode { get; private set; }

    private static void Main(string[] args)
    {
        UserMode = string.Join("", args).Contains("-user");
        try
        {
            IGitRepoInsight repoInsight;
            var repo = new Repository(UserMode ? args[1] : args[0]);
            try
            {
                var context = new GitInsightContextFactory().CreateDbContext(Array.Empty<string>());
                repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                repoInsight = new GitRepoInsight(repo);
            }
            var formatter = new Formatter(repoInsight);
            var output = UserMode ? formatter.GetCommitsOverTimeByUserFormatted() : formatter.GetCommitsOverTimeFormatted();
            Console.WriteLine(output);
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}