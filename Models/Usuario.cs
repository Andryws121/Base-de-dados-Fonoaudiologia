using Microsoft.AspNetCore.Identity;

namespace Base_de_dados_Fonoaudiologia.Models
{
    // Herdamos tudo do IdentityUser (email, senha, etc) e adicionamos o Nome!
    public class Usuario : IdentityUser
    {
        public string NomeCompleto { get; set; } = string.Empty;
    }
}