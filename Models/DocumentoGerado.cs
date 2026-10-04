using System;
using System.ComponentModel.DataAnnotations;

namespace Base_de_dados_Fonoaudiologia.Models
{
    public class DocumentoGerado
    {
        [Key]
        public int Id { get; set; }
        
        public int AlunoId { get; set; }
        
        [Required]
        public string Categoria { get; set; } // Ex: Anamnese, Laudo, Planejamento
        
        [Required]
        public string ConteudoTexto { get; set; }
        
        // NOVO CAMPO: Para guardar a resposta da Inteligência Artificial
        public string? ParecerIA { get; set; } 
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}