using DojoDDD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace DojoDDD.Api.Controllers
{
    /// <summary>
    /// Gerencia os produtos
    /// </summary>
    [ApiController]
    [Route("produtos")]
    public class ProdutoController : Controller
    {
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly ICustomLogger _logger;

        public ProdutoController(IProdutoRepositorio produtoRepositorio, ICustomLogger logger)
        {
            _produtoRepositorio = produtoRepositorio;
            _logger = logger;
        }

        /// <summary>
        /// Lista todos os produtos disponíveis
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Produtos retornados com sucesso</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            try
            {
                var produtos = await _produtoRepositorio.Consultar();
                if (produtos == null)
                    produtos = new List<Produto>();

                return Ok(produtos);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao listar produtos. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }

        /// <summary>
        /// Obtem os dados de um produto pelo seu ID
        /// </summary>
        /// <param name="id">O ID do produto</param>
        /// <returns>Os dados do produto</returns>
        /// <response code="200">Produtos retornados com sucesso</response>
        /// <response code="404">Produto não encontrado</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Produto>> GetById([FromRoute] string id)
        {
            try
            {
                var produto = await _produtoRepositorio.ConsultarPorId(int.Parse(id)).ConfigureAwait(false);
                if (produto == null)
                    return NotFound();

                return Ok(produto);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao obter produto. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }
    }
}
