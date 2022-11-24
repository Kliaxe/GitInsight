namespace GitInsight;
using GitInsight.Infrastructure;
using GitInsight.Database;
using LibGit2Sharp;
using System.Linq;

public class Program
{
    public static bool UserMode { get; private set; }

    private static async Task Main(string[] args)
    {
        UserMode = string.Join("", args).Contains("-user");
        try
        {
            IGitRepoInsight repoInsight;
            var path = string.Join(" ", args.Skip(UserMode ? 1 : 0));
            var repo = new Repository(path);
            try
            {
                var context = new GitInsightContextFactory().CreateDbContext(Array.Empty<string>());
                repoInsight = GitInsightRepoFactory.CreateRepoInsight(repo, context, GitHubClient.Client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                repoInsight = new GitRepoInsight(repo, GitHubClient.Client);
            }
            var formatter = new Formatter(repoInsight);
            var output = UserMode ? formatter.GetCommitsOverTimeByUserFormatted() : formatter.GetCommitsOverTimeFormatted();
            Console.WriteLine(output);
            await GitInsightRepoFactory.datebaseUpdate;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine(e.StackTrace);
        }
    }
}