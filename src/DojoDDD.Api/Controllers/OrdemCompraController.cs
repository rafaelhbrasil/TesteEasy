using DojoDDD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;

namespace DojoDDD.Api.Controllers
{
    /// <summary>
    /// Gerencia as ordens de compra
    /// </summary>
    [ApiController]
    [Route("ordemcompra")]
    public class OrdemCompraController : Controller
    {
        private readonly IOrdemCompraServico _ordemCompraServico;
        private readonly IOrdemCompraRepositorio _ordemCompraRepositorio;
        private readonly ICustomLogger _logger;

        public OrdemCompraController(IOrdemCompraServico ordemCompraServico,
                                     IOrdemCompraRepositorio ordemCompraRepositorio,
                                     ICustomLogger logger)
        {
            _ordemCompraServico = ordemCompraServico;
            _ordemCompraRepositorio = ordemCompraRepositorio;
            _logger = logger;
        }

        /// <summary>
        /// Consulta uma ordem de compra pelo seu ID
        /// </summary>
        /// <param name="idOrdemCompra">O ID da ordem de compra</param>
        /// <returns>Os dados da Ordem de Compra</returns>
        /// <response code="200">Ordem de compra encontrada e retornada com sucesso</response>
        /// <response code="404">Ordem de compra não encontrada</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet]
        [Route("{idOrdemCompra}")]
        public async Task<ActionResult<OrdemCompra>> ConsultarPorId([FromRoute] string idOrdemCompra)
        {
            try
            {
                var ordemCompra = await _ordemCompraRepositorio.ConsultarPorId(idOrdemCompra);
                if (ordemCompra == null)
                    return NotFound();
                return Ok(ordemCompra);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao obter ordem de compra. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }

        /// <summary>
        /// Cria uma nova ordem de compra
        /// </summary>
        /// <param name="ordemCompra">Os dados da ordem de compra</param>
        /// <returns>O ID da nova ordem de compra</returns>
        /// <response code="201">Ordem de compra criada com sucesso</response>
        /// <response code="400">Dados ou campos inválidos</response>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = null)]
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody] OrdemCompra ordemCompra)
        {
            try
            {
                var id = await _ordemCompraServico.RegistrarOrdemCompra(ordemCompra.ClienteId, ordemCompra.ProdutoId, ordemCompra.QuantidadeSolicitada);
                return Created(string.Empty, id);
            }
            catch (InvalidOperationException ex)
            {
                _logger.Log($"Erro ao criar ordem de compra. {ex}", EventLevel.Warning);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao criar ordem de compra. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }
    }
}
