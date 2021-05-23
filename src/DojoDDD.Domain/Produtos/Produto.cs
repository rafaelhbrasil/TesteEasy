namespace DojoDDD.Domain
{
    public class Produto
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public double Estoque { get; set; }
        public decimal PrecoUnitario { get; set; }
        public int ValorMinimoDeCompra { get; set; }
    }
}