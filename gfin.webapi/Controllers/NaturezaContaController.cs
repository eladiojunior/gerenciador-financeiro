using System;
using System.Threading.Tasks;
using gfin.webapi.Api.Models;
using gfin.webapi.Dados;
using gfin.webapi.Dados.Enums;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio;
using gfin.webapi.Negocio.Erros;
using gfin.webapi.Negocio.Listeners;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace gfin.webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NaturezaContaController : ControllerBase
    {
        private readonly ILogger<NaturezaContaController> _logger;
        private readonly GFinContext _context;
        private readonly IClienteListener _cliente;

        public NaturezaContaController(ILogger<NaturezaContaController> logger, GFinContext context,
            IClienteListener cliente)
        {
            _logger = logger;
            _context = context;
            _cliente = cliente;
        }

        /// <summary>
        /// Obtem uma natureza de contapelo ID.
        /// </summary>
        /// <param name="id">Identificador da natureza da conta.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultBase>> Get(int id)
        {
            var result = new ResultBase();
            result.Status = false;
            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da natureza da conta não informado.");
                return Ok(result);
            }

            try
            {
                var negocio = new NaturezaNegocio(_cliente, _context);

                var natureza = negocio.ObterNaturezaConta(id);
                if (natureza == null || natureza.Id == 0)
                    result.Mensagem = $"Natureza da conta com Id [{id}] não encontrada.";

                result.Status = true;
                result.Data = natureza;
                return Ok(result);
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao recuperar natureza da conta com Id [{id}].");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Registro de uma nova natureza da conta.
        /// </summary>
        /// <param name="model">Informações da natureza da conta</param>
        /// <remarks>
        /// Códigos Tipo Lançamento Conta:
        /// 1 = Despesa
        /// 2 = Receita 
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResultBase>> Post(RegistrarNaturezaContaModel model)
        {
            var result = new ResultBase();
            result.Status = false;

            try
            {
                var naturezaConta = new NaturezaConta();
                naturezaConta.CodigoTipoLancamentoConta = model.CodigoTipoLancamentoConta;
                naturezaConta.DescricaoNaturezaConta = model.DescricaoNaturezaConta;
                naturezaConta.CodigoTipoSituacaoNaturezaConta = (short)TipoSituacaoEnum.Ativo;

                var negocio = new NaturezaNegocio(_cliente, _context);
                naturezaConta = negocio.RegistrarNatureza(naturezaConta);

                result.Status = true;
                result.Mensagem = "Registro de natureza da conta efetuado com sucesso.";
                result.Data = naturezaConta;
                return Ok(result);
            }
            catch (Exception erro)
            {
                if (erro.GetType() == typeof(NegocioException))
                {
                    result.Data = ResultErro.Get().AddErro(erro.Message);
                    return Ok(result);
                }

                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao registrar natureza da conta.");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Alterar as informações da natureza da conta.
        /// </summary>
        /// <param name="id">Identificador da natureza da conta a ser editada.</param>
        /// <param name="model">Informações da natureza da conta para alteração.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultBase>> Put(int id, EditarNaturezaContaModel model)
        {
            var result = new ResultBase();
            result.Status = false;

            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da natureza da conta não informado.");
                return Ok(result);
            }
            
            try
            {
                var negocio = new NaturezaNegocio(_cliente, _context);
                var natureza = negocio.ObterNaturezaConta(id);
                if (natureza == null || natureza.Id == 0)
                {
                    result.Mensagem = $"Natureza da conta com Id [{id}] não encontrada.";
                    return Ok(result);
                }

                natureza.CodigoTipoSituacaoNaturezaConta = model.CodigoTipoLancamentoConta;
                natureza.DescricaoNaturezaConta = model.DescricaoNaturezaConta;
                negocio.GravarNaturezaConta(natureza);

                result.Status = true;
                result.Mensagem = "Natureza da conta alterada com sucesso.";
                result.Data = natureza;
                return Ok(result);
            }
            catch (Exception erro)
            {
                if (erro.GetType() == typeof(NegocioException))
                {
                    result.Data = ResultErro.Get().AddErro(erro.Message);
                    return Ok(result);
                }

                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao alterar natureza da conta.");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Remover natureza da conta.
        /// </summary>
        /// <param name="id">Identificador da natureza da conta.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultBase>> Delete(int id)
        {
            var result = new ResultBase();
            result.Status = false;
            
            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da natureza da conta não informado.");
                return result;
            }
            
            try
            {
                var negocio = new NaturezaNegocio(_cliente, _context);
                
                negocio.RemoverNaturezaConta(id);
                
                result.Status = true;
                result.Mensagem = "Natureza da conta removida com sucesso.";
                return Ok(result);
            }
            catch (Exception erro)
            {
                if (erro.GetType() == typeof(NegocioException))
                {
                    result.Data = ResultErro.Get().AddErro(erro.Message);
                    return Ok(result);
                }

                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao remover natureza da conta.");
                return BadRequest(result);
            }
        }
        
        /// <summary>
        /// Lista todas as naturezada de conta (ativas);
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "ListarNaturezaConta")]
        public async Task<ActionResult<ResultBase>> List()
        {
            var result = new ResultBase();
            result.Status = false;

            try
            {
                var negocio = new NaturezaNegocio(_cliente, _context);
                var lista = negocio.ListarNaturezas();
                result.Status = true;
                result.Mensagem = $"Lista de naturezas de contas [{lista.Count}].";
                result.Data = lista;
                return Ok(result);
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao listar as naturezas de contas (Ativas).");
                return BadRequest(result);
            }
        }
        
    }
}