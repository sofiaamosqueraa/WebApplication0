namespace WebApplication0.Models
{
    public class Company
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public List<User> Users { get; set; } = new();
    }
}
