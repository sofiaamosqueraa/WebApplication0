namespace WebApplication0.Models
{
    public class UserCompany
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}
