using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Database
{
    public class GitRepo
    {
        public int Id { get; set; }
        public required string Name { get; init; }
        public required string Url { get; init; }
        public required DateTime Version { get; set; }

        public virtual ICollection<UserDateCount> UserDateCounts { get; } = new List<UserDateCount>();

        public virtual ICollection<Fork> Forks { get; } = new List<Fork>();
    }
}
