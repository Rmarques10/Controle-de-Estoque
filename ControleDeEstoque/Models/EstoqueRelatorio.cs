namespace ControleDeEstoque.Models {
    public class EstoqueRelatorio {
        public string NomeItem { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string TipoOperacao { get; set; }
        public DateTime DataOperacao { get; set; }
        public decimal ValorTotalOperacao { get; internal set; }
        public string? NomeColaborador { get; set; }
        public string? Status { get; set; }
    }
}
