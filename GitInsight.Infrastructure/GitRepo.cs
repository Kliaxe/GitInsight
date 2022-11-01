﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Infrastructure
{
    internal class GitRepo
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTimeOffset Version { get; set; }

        public ICollection<UserDateCount> UserDateCount { get; set; }
    }
}
