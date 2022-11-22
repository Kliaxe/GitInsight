using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInsight.Database
{
    public class Fork
    {
        public int Id { get; set; }
        public required string Name { get; init; }
        public required int GitRepoId { get; init; }
        public Fork? Parent { get; set; }
        public ICollection<Fork> Children { get; } = new List<Fork>();
    }
}
