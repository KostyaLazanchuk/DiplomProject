namespace DataAccess.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public DateTime CreatedOn { get; set; }
        public int Position { get; set; }
        public List<Relationship>? Relationship { get; set; }
    }
}
