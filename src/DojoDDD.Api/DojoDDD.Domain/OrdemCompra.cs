using System;

namespace DojoDDD.Api.DojoDDD.Domain
{
    public class OrdemCompra
    {
        public OrdemCompra()
        {
            Status = OrdemCompraStatus.Solicitado;
        }

        public int Id { get; set; }
        public DateTime DataOperacao { get; set; }
        public int ProdutoId { get; set; }
        public string ClienteId { get; set; }
        public decimal ValorOperacao { get; set; }
        public OrdemCompraStatus Status { get; set; }
    }
}