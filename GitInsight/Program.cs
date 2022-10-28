using NSubstitute;

namespace GitInsight;

public class Program
{
    public static bool UserMode { get; private set; }

    private static void Main(string[] args)
    {
        UserMode = string.Join("", args).Trim().ToLower() == "-user";
        /*var repo = new Repository(UserMode ? args[1] : args[0]);
        var repoInsight = new GitRepoInsight(repo);
        var output = UserMode ? repoInsight.GetCommitsOverTimeByUserFormatted() : repoInsight.GetCommitsOverTimeFormatted();
        Console.WriteLine(output);*/
    }
}