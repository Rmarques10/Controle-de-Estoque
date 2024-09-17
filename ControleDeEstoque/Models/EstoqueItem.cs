using System.ComponentModel.DataAnnotations;

namespace ControleDeEstoque.Models {
    public class EstoqueItem {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Codigo do Produto")]
        public string CodigoProduto { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser um valor positivo.")]
        public int Quantidade { get; set; }

        [Required]
        
        [Display(Name = "Preço Unitário")]
        [DataType(DataType.Currency)]
        public decimal PrecoUnitario { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data de Entrada")]
        public DateTime DataEntrada { get; set; } = DateTime.Now;

        // Método para adicionar itens ao estoque
        public void AdicionarEstoque(int quantidade) {
            if (quantidade <= 0) {
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
            }

            Quantidade += quantidade;
            DataEntrada = DateTime.Now;
        }

        // Método para remover itens do estoque
        public void RemoverEstoque(int quantidade) {
            if (quantidade <= 0) {
                throw new ArgumentException("A quantidade deve ser maior que zero.", nameof(quantidade));
            }

            if (Quantidade < quantidade) {
                throw new InvalidOperationException("Quantidade insuficiente em estoque.");
            }

            Quantidade -= quantidade;
        }

        // Método para calcular o valor total do estoque
        public decimal CalcularValorTotal() {
            return Quantidade * PrecoUnitario;
        }
    }
}
