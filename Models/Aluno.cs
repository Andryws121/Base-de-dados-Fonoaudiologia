using System.ComponentModel.DataAnnotations;

namespace Base_de_dados_Fonoaudiologia.Models
{
    public class Aluno
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Nome { get; set; }
        
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        public string Email { get; set; }
        
        public string SemestreAtual { get; set; }
    }
}