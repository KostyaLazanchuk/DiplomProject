namespace DataAccess.Models
{
    public class Relationship
    {
        public Guid Id { get; set; }
        public int Weight { get; set; }
        public Guid? EndNode { get; set; }
    }
}
