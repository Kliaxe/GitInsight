using Bunit;
using RichardSzalay.MockHttp;

namespace GitInsight.Client.Tests;

public class CommitsTests
{
    [Fact]
    public void Test1()
    {
        using TestContext context = new();
        var mock = context.Services.AddMockHttpClient();
        mock.When("GitInsight/Lukski175/ChittyChat").RespondJson(async () => new List<DateCount>()
        {
            new(DateTime.Now, 2),
        });

        var index = context.RenderComponent<Index>();
        var textBoxes = index.WaitForElements("RadzenTextBox");
        textBoxes[0].Change("Kliaxe");
        textBoxes[1].Change("https://github.com/Lukski175/ChittyChat");

        var commits = context.RenderComponent<Commits>();
        var ths = commits.WaitForElements("th");
        ths[0].TextContent.MarkupMatches("Date");
        ths[1].TextContent.MarkupMatches("Count");
    }

    [Fact]
    public void CounterShouldIncrementWhenSelected()
    {
        // Arrange
        using var context = new TestContext();
        var component = context.RenderComponent<Counter>();
        var p = component.Find("p");

        // Act
        component.Find("button").Click();

        // Assert
        p.TextContent.MarkupMatches("Current count: 1");
    }
}