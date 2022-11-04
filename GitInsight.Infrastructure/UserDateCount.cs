using System.ComponentModel.DataAnnotations;

namespace GitInsight.Infrastructure
{
    public class UserDateCount
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public int GitRepoId { get; set; }
    }
}