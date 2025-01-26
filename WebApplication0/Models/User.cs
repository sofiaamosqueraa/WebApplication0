namespace WebApplication0.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserCompany> UserCompanies { get; set; } = new List<UserCompany>();
    }
}
