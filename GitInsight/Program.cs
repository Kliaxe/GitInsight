namespace GitInsight;

public class Program
{
    public static bool UserMode { get; private set; }

    private static void Main(string[] args)
    {
        UserMode = args[0].Trim().ToLower() == "-user";
        
    }
}