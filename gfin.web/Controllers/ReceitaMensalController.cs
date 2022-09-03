using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Negocio.Filtros;
using GFin.Web.Controllers.Helpers;
using GFin.Web.Models;
using GFin.Web.Models.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ReceitaMensalController : GenericController
    {
        //
        // GET: /ReceitaMensal/
        public ActionResult Index()
        {
            ReceitaMensalModel model = new ReceitaMensalModel();
            model.DataRecebimentoReceita = DateTime.Now.Date;
            CarregarModelDefault(model);
            return View(model);
        }

        private void CarregarModelDefault(ReceitaMensalModel model)
        {
            model.DropboxFormaRecebimento = DropboxHelper.DropboxFormaRecebimento();
            model.DropboxNaturezaReceita = DropboxHelper.DropboxNaturezaReceita();
        }

        /// <summary>
        /// Verifica na Dropbox o value informado (dataValue) para seleção no componente.
        /// </summary>
        /// <param name="dataValue">Data para seleção do dropbox;</param>
        /// <param name="dropboxModel">Componente dropbox que será procurado no value.</param>
        private void SetValueDropboxFiltroMesAno(DateTime dataValue, Models.Helpers.DropboxModel dropboxModel)
        {
            foreach (var item in dropboxModel.Itens)
            {
                item.Selected = item.Value.Equals(dataValue.ToString("dd/MM/yyyy"));
                if (item.Selected) break;
            }
        }
        
        //
        // POST: /ReceitaMensal/Registrar
        [HttpPost]
        public ActionResult Registrar(ReceitaMensalModel model)
        {

            try
            {

                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                string strLiquidacao = string.Empty;

                var receitaMensal = new ReceitaMensal();
                receitaMensal.IdNaturezaContaReceita = model.IdNaturezaReceitaMensal;
                receitaMensal.CodigoTipoFormaRecebimento = model.CodigoFormaRecebimentoReceitaMensal;
                receitaMensal.TextoDescricaoReceita = model.TextoDescricaoReceitaMensal;
                receitaMensal.DataRecebimentoReceita = model.DataRecebimentoReceita;
                receitaMensal.ValorReceita = model.ValorReceita;
                if (model.IsReceitaRecebida)
                {
                    receitaMensal.IsReceitaLiquidada = model.IsReceitaRecebida;
                    receitaMensal.DataHoraLiquidacaoReceita = model.DataRecebimentoReceita;
                    receitaMensal.ValorTotalLiquidacaoReceita = receitaMensal.ValorReceita;
                    strLiquidacao = " (já liquidada)";
                }

                negocio.RegistrarReceita(receitaMensal);

                AlertaUsuario(string.Format("Registro de receita mensal{0} efetuado com sucesso.", strLiquidacao), TipoAlertaEnum.Informacao);

                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Registrar Receita Mensal");
                CarregarModelDefault(model);
                return View("Index", model);
            }

        }

        /// <summary>
        /// Totalizadores no filtro de receitas.
        /// </summary>
        /// <param name="filtroModel"></param>
        private void ObterTotaisReceita(FiltroReceitaModel filtroModel)
        {

            var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);

            var dataInicialVencimento = filtroModel.DataInicialFiltro;
            var dataFinalVencimento = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year), 23, 59, 59);

            filtroModel.ValorTotalReceita = negocio.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 0); //Total Geral
            filtroModel.ValorTotalReceitaRecebidas = negocio.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 1); //Total Recebidas
            filtroModel.ValorTotalReceitaAbertas = negocio.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 2); //Total Abertas
            filtroModel.ValorTotalReceitaVencidas = negocio.ObterTotalReceita(dataInicialVencimento, dataFinalVencimento, 3); //Total Vencidas

        }

        /// <summary>
        /// Recupera a lista de receitas (ativas) do negócio.
        /// </summary>
        /// <returns></returns>
        private List<Dados.Models.ReceitaMensal> ListarReceitas(FiltroReceitaModel filtroModel)
        {
            var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
            var filtro = new FiltroReceitaMensal();
            filtro.DataInicialFiltro = filtroModel.DataInicialFiltro;
            filtro.DataFinalFiltro = new DateTime(filtroModel.DataInicialFiltro.Year, filtroModel.DataInicialFiltro.Month, UtilNegocio.ObterDiasMes(filtroModel.DataInicialFiltro.Month, filtroModel.DataInicialFiltro.Year));
            if (filtroModel.IsFiltroTodas)
                filtro.HasTodas = filtroModel.IsFiltroTodas;
            else
            {
                filtro.HasRecebidas = filtroModel.IsFiltroRecebidas;
                filtro.HasAbertas = filtroModel.IsFiltroAbertas;
                filtro.HasVencidas = filtroModel.IsFiltroVencidas;
            }
            return negocio.ListarReceitaMensal(filtro);
        }

        //
        // GET: /ReceitaMensal/Remover/{id}
        [HttpGet]
        public ActionResult Remover(int id)
        {
            var model = new ReceitaMensalModel();
            CarregarModelDefault(model);

            try
            {

                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da receita mensal não informado.");
                    return View("Index", model);
                }

                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverReceitaMensal(id);

                AlertaUsuario("Receita mensal removida com sucesso.", TipoAlertaEnum.Informacao);
                return RedirectToAction("Index");

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Receita Mensal");
                return View("Index", model);
            }

        }

        //
        // GET: /ReceitaMensal/Editar/{id}
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var model = new ReceitaMensalModel();
            CarregarModelDefault(model);

            try
            {

                if (id == 0)
                {
                    ModelState.AddModelError(string.Empty, "Id da receita mensal não informado.");
                    return View("Index", model);
                }

                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                ReceitaMensal receita = negocio.ObterReceitaMensal(id);
                if (receita == null || receita.Id == 0)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Receita mensal com Id [{0}] não encontrada.", id));
                    return View("Index", model);
                }
                
                model.IdReceitaMensal = receita.Id;
                model.IdNaturezaReceitaMensal = receita.IdNaturezaContaReceita;
                model.CodigoFormaRecebimentoReceitaMensal = receita.CodigoTipoFormaRecebimento;
                model.TextoDescricaoReceitaMensal = receita.TextoDescricaoReceita;
                model.DataRecebimentoReceita = receita.DataRecebimentoReceita;
                model.ValorReceita = receita.ValorReceita;

                return View("Editar", model);

            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Remoção Receita Mensal");
                return View("Index", model);
            }

        }

        //
        // POST /ReceitaMensal/Editar
        [HttpPost]
        public ActionResult Editar(ReceitaMensalModel model)
        {

            CarregarModelDefault(model);

            if (!ModelState.IsValid)
                return View("Editar", model);

            var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
            ReceitaMensal receita = negocio.ObterReceitaMensal(model.IdReceitaMensal);
            if (receita == null || receita.Id == 0)
            {
                ModelState.AddModelError(string.Empty, string.Format("Receita mensal com Id [{0}] não encontrada.", model.IdReceitaMensal));
                return View("Editar", model);
            }

            receita.IdNaturezaContaReceita = model.IdNaturezaReceitaMensal;
            receita.NaturezaContaReceita = null;
            receita.CodigoTipoFormaRecebimento = model.CodigoFormaRecebimentoReceitaMensal;
            receita.TextoDescricaoReceita = model.TextoDescricaoReceitaMensal;
            receita.DataRecebimentoReceita = model.DataRecebimentoReceita;
            receita.ValorReceita = model.ValorReceita;

            negocio.GravarReceitaMensal(receita);

            AlertaUsuario("Receita mensal alterada com sucesso.", TipoAlertaEnum.Informacao);

            return RedirectToAction("Index");
            
        }

        //
        // GET /ReceitaMensal/LiquidarReceitaMensal
        [HttpGet]
        public JsonResult JsonLiquidarReceitaMensal(int idReceita)
        {

            try
            {

                if (idReceita == 0)
                    return JsonResultErro("Identificador da Receita Mensal não informado.");

                ReceitaNegocio negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                ReceitaMensal receita = negocio.ObterReceitaMensal(idReceita);
                if (receita == null)
                    return JsonResultErro(string.Format("Receita mensal com o Id [{0}] não encontrada.", idReceita));

                LiquidaReceitaMensalModel model = new LiquidaReceitaMensalModel();

                model.IdReceitaMensal = receita.Id;
                model.DescricaoNaturezaReceitaMensal = receita.NaturezaContaReceita.DescricaoNaturezaConta;
                model.DescricaoTipoFormaRecebimento = UtilEnum.GetTextoFormaRecebimento(receita.CodigoTipoFormaRecebimento);
                model.TextoDescricaoReceitaMensal = receita.TextoDescricaoReceita;
                model.DataRecebimentoReceita = receita.DataRecebimentoReceita.ToString("dd/MM/yyyy");
                model.ValorReceita = receita.ValorReceita.ToString("N");
                model.DataLiquidacaoReceita = DateTime.Now.Date;
                model.ValorTotalLiquidacaoReceita = receita.ValorReceita;

                return JsonResultSucesso(RenderRazorViewToString("_LiquidarReceitaMensalPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonLiquidarReceitaMensal(idReceita)"));
            }

        }

        // POST /ReceitaMensal/JsonLiquidarReceitaMensal
        [HttpPost]
        public JsonResult JsonLiquidarReceitaMensal(LiquidaReceitaMensalModel model)
        {
            
            if (!ModelState.IsValid)
                return JsonResultErro(ModelState.Errors());

            try
            {
                ReceitaNegocio negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.LiquidarReceita(model.IdReceitaMensal, model.DataLiquidacaoReceita, model.ValorTotalLiquidacaoReceita);
                return JsonResultSucesso("Sua receita foi liquidada com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonLiquidarReceitaMensal(LiquidaReceitaMensalModel)"));
            }
        }
        
        //
        // GET /ReceitaMensal/JsonListarReceitaMensal
        [HttpGet]
        public JsonResult JsonListarReceitaMensal(string dataFiltro)
        {
            try
            {

                FiltroReceitaModel model = new FiltroReceitaModel();
                model.IsFiltroTodas = true;
                if (string.IsNullOrEmpty(dataFiltro))
                {
                    model.DataInicialFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                }
                else
                {
                    DateTime dataFiltroReceita = DateTime.Parse(dataFiltro);
                    model.DataInicialFiltro = dataFiltroReceita;
                }
                model.DropboxFiltroMesAno = DropboxHelper.DropboxMesesDoAno(model.DataInicialFiltro.Year);
                SetValueDropboxFiltroMesAno(model.DataInicialFiltro, model.DropboxFiltroMesAno);
                model.ReceitasMensais = ListarReceitas(model);
                ObterTotaisReceita(model);

                return JsonResultSucesso(RenderRazorViewToString("_ListaReceitaPartial", model));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonListarReceitaMensal(dataFiltro::{0})", dataFiltro)));
            }
        }

        //
        // GET /ReceitaMensal/JsonListarReceitaMensalSimples
        [HttpGet]
        public JsonResult JsonListarReceitaMensalSimples()
        {
            try
            {
                FiltroReceitaModel model = new FiltroReceitaModel();
                model.DataInicialFiltro = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                return JsonResultSucesso(RenderRazorViewToString("_ListaReceitaSimplesPartial", ListarReceitas(model)));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonListarReceitaMensalSimples()"));
            }
        }

        //
        // GET: /ReceitaMensal/JsonRegistrarNovaReceita
        public JsonResult JsonRegistrarNovaReceita()
        {
            try
            {
                ReceitaMensalModel model = new ReceitaMensalModel();
                model.DataRecebimentoReceita = DateTime.Now.Date;
                model.DropboxFormaRecebimento = DropboxHelper.DropboxFormaRecebimento();
                model.DropboxNaturezaReceita = DropboxHelper.DropboxNaturezaReceita();
                return JsonResultSucesso(RenderRazorViewToString("_RegistrarReceitaSimplesPartial", model));
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarNovaReceita()"));
            }
        }

        //
        // POST: /ReceitaMensal/JsonRegistrarNovaReceita
        [HttpPost]
        public JsonResult JsonRegistrarNovaReceita(ReceitaMensalModel model)
        {

            if (!ModelState.IsValid)
                return JsonResultErro(ModelState.Errors());

            try
            {

                ReceitaNegocio negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                string strLiquidacao = string.Empty;

                var receitaMensal = new ReceitaMensal();
                receitaMensal.IdNaturezaContaReceita = model.IdNaturezaReceitaMensal;
                receitaMensal.CodigoTipoFormaRecebimento = model.CodigoFormaRecebimentoReceitaMensal;
                receitaMensal.TextoDescricaoReceita = model.TextoDescricaoReceitaMensal;
                receitaMensal.DataRecebimentoReceita = model.DataRecebimentoReceita;
                receitaMensal.ValorReceita = model.ValorReceita;
                if (model.IsReceitaRecebida)
                {
                    receitaMensal.IsReceitaLiquidada = model.IsReceitaRecebida;
                    receitaMensal.DataHoraLiquidacaoReceita = model.DataRecebimentoReceita;
                    receitaMensal.ValorTotalLiquidacaoReceita = receitaMensal.ValorReceita;
                    strLiquidacao = " (já liquidada)";
                }

                negocio.RegistrarReceita(receitaMensal);

                return JsonResultSucesso(string.Format("Registro de receita mensal{0} efetuado com sucesso.", strLiquidacao));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonRegistrarNovaReceita(model)"));
            }
        }

        /// <summary>
        /// Recupera lista de totais de receitas vencidas nos meses anteriores ao corrente;
        /// </summary>
        /// <returns></returns>
        private List<ReceitaMensal> ListarReceitasVencidasMesesAnteriores()
        {
            Negocio.ReceitaNegocio negocio = new Negocio.ReceitaNegocio(UsuarioLogadoConfig.Instance);
            return negocio.ListarTotaisReceitasVencidasMesesAnteriores(DateTime.Now.Month, DateTime.Now.Year);
        }

        [HttpPost]
        public JsonResult JsonRemoverReceitaMensal(int idReceitaMensal)
        {
            try
            {
                var negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.RemoverReceitaMensal(idReceitaMensal);
                return JsonResultSucesso("Receita mensal removida com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, string.Format("JsonRemoverReceitaMensal(IdReceitaMensal::{0})", idReceitaMensal)));
            }
        }

        //GET: /ReceitaMensal/JsonDetalharReceitaLiquidada
        [HttpGet]
        public JsonResult JsonDetalharReceitaLiquidada(int idReceita)
        {

            try
            {

                if (idReceita == 0)
                    return JsonResultErro("Identificador da receita mensal não informado.");

                ReceitaNegocio negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                ReceitaMensal receita = negocio.ObterReceitaMensal(idReceita);
                if (receita == null)
                    return JsonResultErro(string.Format("Receita mensal com o Id [{0}] não encontrada.", idReceita));

                DetalheReceitaLiquidadaModel modelResult = new DetalheReceitaLiquidadaModel();
                if (receita != null && receita.Id != 0)
                {
                    modelResult.IdReceitaMensal = receita.Id;
                    modelResult.DescricaoNaturezaReceitaMensal = receita.NaturezaContaReceita.DescricaoNaturezaConta;
                    modelResult.DescricaoTipoFormaRecebimento = UtilEnum.GetTextoFormaLiquidacao(receita.CodigoTipoFormaRecebimento);
                    modelResult.TextoDescricaoReceitaMensal = receita.TextoDescricaoReceita;
                    modelResult.DataRecebimentoReceita = receita.DataRecebimentoReceita.ToString("dd/MM/yyyy");
                    modelResult.ValorReceita = receita.ValorReceita.ToString("N");
                    modelResult.DataLiquidacaoReceita = receita.DataHoraLiquidacaoReceita.HasValue ? receita.DataHoraLiquidacaoReceita.Value.ToString("dd/MM/yyyy") : "";
                    modelResult.ValorTotalLiquidacaoReceita = receita.ValorTotalLiquidacaoReceita.HasValue ? receita.ValorTotalLiquidacaoReceita.Value.ToString("N") : ((decimal)0).ToString("N");
                }
                return JsonResultSucesso(RenderRazorViewToString("_DetalharReceitaLiquidadaPartial", modelResult));

            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonDetalharReceitaLiquidada(ReceitaMensalModel)"));
            }

        }

        //POST: /ReceitaMensal/JsonEstornarReceitaLiquidada
        [HttpPost]
        public JsonResult JsonEstornarReceitaLiquidada(int idReceita)
        {

            try
            {
                ReceitaNegocio negocio = new ReceitaNegocio(UsuarioLogadoConfig.Instance);
                negocio.EstornarReceitaMensal(idReceita);
                return JsonResultSucesso("Receita mensal estornada com sucesso.");
            }
            catch (Exception erro)
            {
                return JsonResultErro(TratarMensagemErroNegocio(erro, "JsonEstornarReceitaLiquidada(ReceitaMensalModel)"));
            }

        }
    }
}
