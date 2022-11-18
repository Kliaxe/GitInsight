using System.ComponentModel.DataAnnotations;

namespace GitInsight.Database
{
    public class UserDateCount
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset Date { get; set; }
        public int Count { get; set; }
        public int GitRepoId { get; set; }
    }
}