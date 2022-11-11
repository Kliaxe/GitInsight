using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Tests
{
    public class FormatterTests
    {
        private readonly IGitRepoInsight repoInsightSub;

        public FormatterTests()
        {
            repoInsightSub = Substitute.For<IGitRepoInsight>();
        }

        [Fact]
        public void GetCommitsOverTimeFormatted()
        {
            repoInsightSub.GetCommitHistory().Returns(new List<DateCount>()
            {
                new DateCount(new DateTime(2022, 1, 31), 1),
                new DateCount(new DateTime(2022, 1, 27), 5)
            });
            var formatter = new Formatter(repoInsightSub);
            string expected = $"5 27/01/2022\n1 31/01/2022";
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        [Fact]
        public void GetCommitsOverTimeByUserFormatted()
        {
            repoInsightSub.GetCommitHistoryByUser().Returns(new List<(User, IEnumerable<DateCount>)>()
            {
                (
                    new User("Lukas", "lu@mail"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 1, 27), 2),
                    }
                ),
                (
                    new User("Adrian", "ad@mail"),
                    new List<DateCount>()
                    {
                        new DateCount(new DateTime(2022, 1, 27), 3),
                        new DateCount(new DateTime(2022, 1, 31), 1),
                    }
                ),
            });

            var formatter = new Formatter(repoInsightSub);
            string expected = $"Adrian\n3 27/01/2022\n1 31/01/2022\n\nLukas\n2 27/01/2022";
            Assert.Equal(expected, formatter.GetCommitsOverTimeByUserFormatted());
        }
    }
}
