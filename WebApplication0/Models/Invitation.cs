namespace WebApplication0.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public required string Email { get; set; }  
        public required string Token { get; set; }  
        public required string EmpresasSelecionadas { get; set; } 
        public DateTime DataCriacao { get; set; } 
    }
}
