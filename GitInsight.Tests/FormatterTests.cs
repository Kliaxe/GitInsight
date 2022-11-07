﻿using System;
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
            repoInsightSub.GetCommitHistoryByUser().Returns(new List<(User, IEnumerable<DateCount>)>()
            {
                (
                    new User("Lukas", "lu@mail"),
                    new List<DateCount>()
                    {
                        new DateCount(DateTimeOffset.Now.Date, 2),
                    }
                ),
                (
                    new User("Adrian", "ad@mail"),
                    new List<DateCount>()
                    {
                        new DateCount(DateTimeOffset.Now.Date, 3),
                        new DateCount(DateTimeOffset.Now.AddDays(1).Date, 1),
                    }
                ),
            });

            var formatter = new Formatter(repoInsightSub);
            string expected = $"Adrian\n3 {DateTimeOffset.Now.Date}\n1 {DateTimeOffset.Now.AddDays(1).Date}\n\nLukas\n2 {DateTimeOffset.Now.Date}";
            Assert.Equal(expected, formatter.GetCommitsOverTimeByUserFormatted());
        }
    }
}
