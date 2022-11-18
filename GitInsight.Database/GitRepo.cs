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
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTimeOffset Version { get; set; }

        public ICollection<UserDateCount> UserDateCounts { get; set; }
    }
}
