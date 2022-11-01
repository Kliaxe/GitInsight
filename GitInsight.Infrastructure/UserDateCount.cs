using System.ComponentModel.DataAnnotations;

namespace GitInsight.Infrastructure
{
    public class UserDateCount
    {
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public int GitRepoId { get; set; }
    }
}