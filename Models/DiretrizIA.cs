using System.ComponentModel.DataAnnotations;

namespace Base_de_dados_Fonoaudiologia.Models
{
    public class DiretrizIA
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Categoria { get; set; }
        
        [Required]
        public string PromptRestricao { get; set; }
    }
}