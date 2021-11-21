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
    public class DespesaFixaController : ControllerBase
    {
        private readonly ILogger<DespesaFixaController> _logger;
        private readonly GFinContext _context;
        private readonly IClienteListener _cliente;

        public DespesaFixaController(ILogger<DespesaFixaController> logger, GFinContext context,
            IClienteListener cliente)
        {
            _logger = logger;
            _context = context;
            _cliente = cliente;
        }

        /// <summary>
        /// Obtem uma despesa fixa pelo ID.
        /// </summary>
        /// <param name="id">Identificador da despesa fixa.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ResultBase>> Get(int id)
        {
            var result = new ResultBase();
            result.Status = false;
            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da Despesa Fixa não informado.");
                return Ok(result);
            }

            try
            {
                var negocio = new DespesaNegocio(_cliente, _context);

                var despesaFixa = negocio.ObterDespesaFixa(id);
                if (despesaFixa == null || despesaFixa.Id == 0)
                    result.Mensagem = $"Despesa fixa com Id [{id}] não encontrada.";

                result.Status = true;
                result.Data = despesaFixa;
                return Ok(result);
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao recuperar Despesa Fixa com Id [{id}].");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Registro de uma nova despesa fixa.
        /// </summary>
        /// <param name="model">Informações da despesa fixa</param>
        /// <remarks>
        /// Códigos Tipo Forma Liquidação da Despesa
        /// 1 = Dinheiro
        /// 2 = Cartão de Crédito/Débito
        /// 3 = Cheque à Vista
        /// 4 = Cheque Pré-Datado
        /// 5 = Boleto de Cobrança
        /// 6 = Débito em Conta
        /// 7 = Fatura
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ResultBase>> Post(RegistrarDespesaFixaModel model)
        {
            var result = new ResultBase();
            result.Status = false;

            try
            {
                var despesaFixa = new DespesaFixa();
                despesaFixa.IdNaturezaContaDespesaFixa = model.IdNaturezaContaDespesaFixa;
                despesaFixa.DiaVencimentoDespesaFixa = model.NumeroDiaVencimentoDespesaFixa;
                despesaFixa.DescricaoDespesaFixa = model.TextoDescricaoDespesaFixa;
                despesaFixa.ValorDespesaFixa = model.ValorDespesaFixa;
                despesaFixa.CodigoTipoSituacaoDespesaFixa = (short) TipoSituacaoEnum.Ativo;
                despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa = model.CodigoTipoFormaLiquidacao;

                var negocio = new DespesaNegocio(_cliente, _context);
                despesaFixa = negocio.RegistrarDespesaFixa(despesaFixa);

                result.Status = true;
                result.Mensagem = "Registro de despesa fixa efetuado com sucesso.";
                result.Data = despesaFixa;
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
                result.Data = ResultErro.Get().AddErro($"Erro ao registrar Despesa Fixa.");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Alterar as informações da despesa fixa.
        /// </summary>
        /// <param name="id">Identificador da despesa fixa a ser editada.</param>
        /// <param name="model">Informações da despesa fixa para alteração.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ResultBase>> Put(int id, EditarDespesaFixaModel model)
        {
            var result = new ResultBase();
            result.Status = false;

            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da despesa fixa não informado.");
                return Ok(result);
            }
            
            try
            {
                var negocio = new DespesaNegocio(_cliente, _context);
                var despesaFixa = negocio.ObterDespesaFixa(id);
                if (despesaFixa == null || despesaFixa.Id == 0)
                {
                    result.Mensagem = $"Despesa fixa com Id [{id}] não encontrada.";
                    return Ok(result);
                }

                //Verificar se houve alteração do valor da despesa fixa...
                var hasGravarHistorico = (despesaFixa.ValorDespesaFixa != model.ValorDespesaFixa);

                despesaFixa.IdNaturezaContaDespesaFixa = model.IdNaturezaContaDespesaFixa;
                despesaFixa.DiaVencimentoDespesaFixa = model.NumeroDiaVencimentoDespesaFixa;
                despesaFixa.DescricaoDespesaFixa = model.TextoDescricaoDespesaFixa;
                despesaFixa.ValorDespesaFixa = model.ValorDespesaFixa;
                despesaFixa.CodigoTipoFormaLiquidacaoDespesaFixa = model.CodigoTipoFormaLiquidacao;

                negocio.GravarDespesaFixa(despesaFixa, hasGravarHistorico);

                result.Status = true;
                result.Mensagem = "Despesa fixa alterada com sucesso.";
                result.Data = despesaFixa;
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
                result.Data = ResultErro.Get().AddErro($"Erro ao alterar Despesa Fixa.");
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Remover uma despesa fixa.
        /// </summary>
        /// <param name="id">Identificador da despesa fixa</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ResultBase>> Delete(int id)
        {
            var result = new ResultBase();
            result.Status = false;
            
            if (id == 0)
            {
                result.Data = ResultErro.Get().AddErro("id", "Id da despesa fixa não informado.");
                return result;
            }
            
            try
            {
                var negocio = new DespesaNegocio(_cliente, _context);
                
                negocio.RemoverDespesaFixa(id);
                
                result.Status = true;
                result.Mensagem = "Despesa fixa removida com sucesso.";
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
                result.Data = ResultErro.Get().AddErro($"Erro ao remover Despesa Fixa.");
                return BadRequest(result);
            }
        }
        
        /// <summary>
        /// Lista todas as despesas fixas (ativas);
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "ListarDespesaFixa")]
        public async Task<ActionResult<ResultBase>> List()
        {
            var result = new ResultBase();
            result.Status = false;

            try
            {
                var negocio = new DespesaNegocio(_cliente, _context);
                var listaDespesaFixa = negocio.ListarDespesaFixa(true);
                result.Status = true;
                result.Mensagem = $"Lista de despesas fixas [{listaDespesaFixa.Count}].";
                result.Data = listaDespesaFixa;
                return Ok(result);
            }
            catch (Exception erro)
            {
                _logger.LogError(erro.ToString());
                result.Data = ResultErro.Get().AddErro($"Erro ao listar as despesas fixas (Ativas).");
                return BadRequest(result);
            }
        }
        
    }
}