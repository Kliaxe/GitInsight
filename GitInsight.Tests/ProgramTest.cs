using System.Reflection;

namespace GitInsight.Tests;

public class ProgramTest
{
    [Fact]
    public void Program_given_args_user_UserMode_equals_true()
    {
        var program = Assembly.Load(nameof(GitInsight));
        program.EntryPoint?.Invoke(null, new[] { new string[] { "-user" } });

        Assert.True(Program.UserMode);
    }

    [Fact]
    public void Program_given_empty_args_UserMode_equals_false()
    {
        var program = Assembly.Load(nameof(GitInsight));
        program.EntryPoint?.Invoke(null, new[] { Array.Empty<string>() });

        Assert.False(Program.UserMode);
    }
}
