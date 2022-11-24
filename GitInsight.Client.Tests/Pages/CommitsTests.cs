using AngleSharp.Dom;
using Bunit;
using GitInsight.WebApp.Client;
using Radzen.Blazor;
using RichardSzalay.MockHttp;
using System.Net.Http.Json;

namespace GitInsight.Client.Tests.Pages;

public class CommitsTests
{
    [Fact]
    public void Table_Should_Display_Commit_When_Input_Is_Change()
    {
        using TestContext context = new();
        var mock = context.Services.AddMockHttpClient();
        mock.When("http://localhost/GitInsight/Lukski175/ChittyChat").RespondJson(() => new RepoAnalysis(new List<DateCount>() 
        {
            new(DateTime.Now, 2),
        }, null!, null!));

        var OwnerName = "Lukski175";
        var RepoName = "ChittyChat";

        var index = context.RenderComponent<Index>();
        var owner = index.Find("#owner");
        var repo = index.Find("#repository");
        owner.Input(OwnerName);
        repo.Input(RepoName);

        InputData.NameOfOwner.Should().Be(OwnerName);
        InputData.NameOfRepository.Should().Be(RepoName);

        var commits = context.RenderComponent<Commits>();
        var tds = commits.WaitForElements("td");
        tds[0].MarkupMatches($"<td>{DateTime.Now:dd/MM/yyyy}</td>");
        tds[1].MarkupMatches("<td>2</td>");
    }
}