using DojoDDD.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace DojoDDD.Api.Controllers
{
    /// <summary>
    /// Gerencia os clientes
    /// </summary>
    [ApiController]
    [Route("clientes")]
    public class ClienteController : Controller
    {
        private readonly IClienteRepositorio _clienteRepositorio;
        private readonly ICustomLogger _logger;

        public ClienteController(IClienteRepositorio clienteRepositorio, ICustomLogger logger)
        {
            _clienteRepositorio = clienteRepositorio;
            _logger = logger;
        }

        /// <summary>
        /// Retorna todos os clientes cadastrados
        /// </summary>
        /// <returns>Uma lista com todos os clientes</returns>
        /// <response code="200">Clientes retornados com sucesso</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<IEnumerable<Cliente>>> Get()
        {
            try
            {
                var clientes = await _clienteRepositorio.ConsultarTodosCliente();
                if (clientes == null)
                    clientes = new List<Cliente>();

                return Ok(clientes);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao listar clientes. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }

        /// <summary>
        /// Retorna os dados de um(a) cliente específico(a)
        /// </summary>
        /// <param name="idCliente">O ID do(a) cliente a ser consultado(a)</param>
        /// <returns>Os dados do(a) cliente</returns>
        /// <response code="200">Cliente encontrado e retornado com sucesso</response>
        /// <response code="404">Cliente não encontrado</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = null)]
        [HttpGet]
        [Route("{idCliente}")]
        public async Task<ActionResult<Cliente>> GetById([FromRoute] string idCliente)
        {
            try
            {
                var cliente = await _clienteRepositorio.ConsultarPorId(idCliente).ConfigureAwait(false);
                if (cliente == null)
                    return NotFound();

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                _logger.Log($"Erro ao obter cliente. {ex}", EventLevel.Error);
                throw; //deixar subir o erro para retornar 500
            }
        }
    }
}
