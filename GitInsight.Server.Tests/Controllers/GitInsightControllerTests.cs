using GitInsight.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Server.Tests.Controllers
{
    public class GitInsightControllerTests
    {
        private readonly GitInsightController _sut;

        private const string _gitOwner = "Lukski175";
        private const string _gitRepositoryName = "assignment-04";
        private const string _gitUser = "Moedefeis";

        public GitInsightControllerTests()
        {
            var logger = Substitute.For<ILogger<GitInsightController>>();
            _sut = new GitInsightController(logger);
        }

        [Fact]
        public async Task Get_NonExisting()
        {
            var expected = new List<DateCount>() { new DateCount(new DateTime(2022, 10, 4), 1) };
            var result = await _sut.Get(_gitOwner, _gitRepositoryName);
            result.Should().NotBeEquivalentTo(expected);
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

            var result = await _sut.Get(_gitOwner, _gitRepositoryName);
            result.Should().BeEquivalentTo(expected);
        }

        //[Fact]
        //public async Task Get_User_NonExisting()
        //{
        //    var expected = new List<DateCount>() { new DateCount(new DateTime(2022, 10, 4), 1) };
        //    var result = await _sut.Get(_gitOwner, _gitRepositoryName);
        //    result.Should().NotBeEquivalentTo(expected);
        //}

        [Fact]
        public async Task Get_User_Existing()
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

            var result = await _sut.Get(_gitOwner, _gitRepositoryName, _gitUser);
            result.Should().BeEquivalentTo(expected);
        }


    }
}
