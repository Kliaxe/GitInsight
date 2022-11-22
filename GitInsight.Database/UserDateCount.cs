using System.ComponentModel.DataAnnotations;

namespace GitInsight.Database
{
    public class UserDateCount
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public required DateTimeOffset Date { get; set; }
        public required int Count { get; set; }
        public required int GitRepoId { get; set; }
    }
}