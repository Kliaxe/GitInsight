using GitInsight.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GitInsight.Server.Tests.Controllers
{
    public class GitInsightControllerTests
    {
        private readonly GitInsightController _controller;

        private readonly IEnumerable<DateCount> dateCounts;
        private readonly IEnumerable<UserDateCounts> userDateCounts;

        public GitInsightControllerTests()
        {
            var logger = Substitute.For<ILogger<GitInsightController>>();
            _controller = new GitInsightController(logger);

            dateCounts = new List<DateCount>()
            {
                new DateCount(new DateTime(2022, 10, 4), 9),
                new DateCount(new DateTime(2022, 9, 30), 2),
                new DateCount(new DateTime(2022, 9, 29), 1),
                new DateCount(new DateTime(2022, 9, 28), 2),
                new DateCount(new DateTime(2022, 9, 25), 1),
                new DateCount(new DateTime(2020, 9, 23), 3),
                new DateCount(new DateTime(2020, 9, 22), 1),
                new DateCount(new DateTime(2020, 9, 10), 1),
            };

            userDateCounts = new List<UserDateCounts>()
            {
                new UserDateCounts
                (
                    new User("Lukski175", "113596146+Lukski175@users.noreply.github.com"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 10, 4), 1),
                    }
                ),
                new UserDateCounts
                (
                    new User("Moedefeis", "71703802+Moedefeis@users.noreply.github.com"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 10, 4), 2),
                    }
                ),
                new UserDateCounts
                (
                    new User("Mathias Hvolgaard Darling Larsen", "mhvl@itu.dk"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 10, 4), 3),
                    }
                ),
                new UserDateCounts
                (
                    new User("lukas175", "79913864+lukas175@users.noreply.github.com"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 10, 4), 3),
                    }
                ),
            };
        }

        [Fact]
        public async Task Get_NonExisting() {
            var result = await _controller.Get("abcd", "abcd");
            result.Result.Should().BeAssignableTo<NotFound<string>>();
            var actual = (result.Result as NotFound<string>)!.Value;
            actual.Should().Be("https://github.com/abcd/abcd");
        }

        [Fact]
        public async Task Get_Existing()
        {
            var result = await _controller.Get("Lukski175", "assignment-04");
            result.Result.Should().BeAssignableTo<Ok<RepoAnalysis>>();
            var actual = (result.Result as Ok<RepoAnalysis>)!.Value;
            actual.DateCounts.Should().BeEquivalentTo(dateCounts);
            actual.UserDateCounts.Take(4).Should().BeEquivalentTo(userDateCounts);
        }
    }
}
