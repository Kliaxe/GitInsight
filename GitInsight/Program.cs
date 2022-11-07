namespace GitInsight;

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
                var insightRepository = new InsightRepository(context);
                (bool upToDate, var gitRepo) = insightRepository.HasUpToDateInsight(repo);
                if (upToDate)
                {
                    repoInsight = new DBRepoInsight(gitRepo);
                }
                else
                {
                    var localRepo = new GitRepoInsight(repo);
                    insightRepository.UpdateInsight(localRepo);
                    repoInsight = localRepo;
                }
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