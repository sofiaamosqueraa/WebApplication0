namespace WebApplication0.Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public required string Email { get; set; }  // E-mail do usuário convidado
        public required string Token { get; set; }  // Token único para validação do convite
        public required string EmpresasSelecionadas { get; set; }  // IDs das empresas associadas ao convite (separados por vírgula)
        public DateTime DataCriacao { get; set; } // Data de criação do convite
    }
}
