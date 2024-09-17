using System.ComponentModel.DataAnnotations;

namespace ControleDeEstoque.Models {
    public class EstoqueOperacao {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EstoqueItemId { get; set; }

        public EstoqueItem EstoqueItem { get; set; }

        [Required]
        public int Quantidade { get; set; }

        public decimal PrecoUnitario { get; set; }

        [Required]
        public DateTime DataOperacao { get; set; } = DateTime.Now;

        [Required]
        public string TipoOperacao { get; set; } // "Entrada" ou "Saída"

        public int? ColaboradorId { get; set; }
        public Colaborador? Colaborador { get; set; }

        public decimal ValorTotalOperacao => Quantidade * PrecoUnitario;

        [Required]
        public string? Status { get; set; }
    }
}
