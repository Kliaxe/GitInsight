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
        public string Name { get; set; }
        public int GitRepoId { get; set; }
        public int? ParentId { get; set; }
        public Fork? Parent { get; set; }
        public ICollection<Fork> Children { get; set; }
    }
}
