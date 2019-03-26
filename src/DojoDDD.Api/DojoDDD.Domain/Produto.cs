namespace DojoDDD.Api.DojoDDD.Domain
{
    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Estoque { get; set; }
        public decimal ValorUnitario { get; set; }
        public int ValorMinimoDeCompra { get; set; }
    }
}