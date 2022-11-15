using Bunit;
using Radzen.Blazor;
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
        index.Find("#owner").Change("Kliaxe");
        index.Find("#repository").Change("https://github.com/Lukski175/ChittyChat");

        var commits = context.RenderComponent<Commits>();
        //commits.Find("p").MarkupMatches("<em>Please fill in the information on the home page...</em>");
        /*var ths = commits.FindAll("th");
        ths[0].TextContent.MarkupMatches("Date");
        ths[1].TextContent.MarkupMatches("Count");*/
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