using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
            repoInsightSub.GetCommitsOverTime().Returns(new List<DateCount>()
            {
                new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1),
                new DateCount(DateTimeOffset.Now.Date, 5)
            });
            var formatter = new Formatter(repoInsightSub);
            string expected = $"5 {DateTimeOffset.Now.Date}\n1 {DateTimeOffset.Now.AddDays(1).Date}";
            Assert.Equal(expected, formatter.GetCommitsOverTimeFormatted());
        }

        [Fact]
        public void GetCommitsOverTimeByUserFormatted()
        {
            repoInsightSub.GetCommitsOverTimeByUser().Returns(new Dictionary<string, IEnumerable<DateCount>>
            {
                { "Adrian", new List<DateCount>
                    {
                        new DateCount(DateTimeOffset.Now.Date, 3),
                        new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1)
                    }
                },
                { "Lukas", new List<DateCount>
                    {
                        new DateCount(DateTimeOffset.Now.Date, 2)
                    }
                }
            });

            var formatter = new Formatter(repoInsightSub);
            string expected = $"Adrian\n3 {DateTimeOffset.Now.Date}\n1 {DateTimeOffset.Now.AddDays(1).Date}\n\nLukas\n2 {DateTimeOffset.Now.Date}";
            Assert.Equal(expected, formatter.GetCommitsOverTimeByUserFormatted());
        }
    }
}
