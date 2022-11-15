using GitInsight.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Server.Tests.Controllers
{
    public class GitInsightControllerTests
    {
        private readonly GitInsightController _controller;

        private const string _gitOwner = "Lukski175";
        private const string _gitRepositoryName = "assignment-04";

        public GitInsightControllerTests()
        {
            var logger = Substitute.For<ILogger<GitInsightController>>();
            _controller = new GitInsightController(logger);
        }

        [Fact]
        public async Task Get_NonExisting()
        {
            var expected = new List<DateCount>() { new DateCount(new DateTime(2022, 10, 4), 1) };
            var result = await _controller.Get(_gitOwner, _gitRepositoryName);
            result.Value.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_Existing()
        {
            var expected = new List<DateCount>()
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

            var result = await _controller.Get(_gitOwner, _gitRepositoryName);
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_User_NonExisting()
        {
            var expected = new List<UserDateCounts>() { 
                new UserDateCounts(new User("Test", "Test@github.com"),
                new List<DateCount>() { new DateCount(new DateTime(0198, 10, 4), 1) })
            };
            var result = await _controller.Get(_gitOwner, _gitRepositoryName, "user");
            result.Value.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_User_Existing()
        {
            var expected = new List<UserDateCounts>()
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

            var result = await _controller.Get(_gitOwner, _gitRepositoryName, "user");
            // Just take first 4
            result.Value!.Take(4).Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Get_With_Non_Existing_Url()
        {
            var result = await _controller.Get("AABBCC", "DDEEFF", "user");
            result.Should().BeAssignableTo<NotFoundResult>();
        }
    }
}
