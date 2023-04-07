using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using GFin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers
{
    public class ControleFinanceiroController : GenericController
    {
        //
        // GET: /ControleFinanceiro/Compartilhar
        [HttpGet]
        [UsuarioFilter(TipoPerfilAcessoUsuarioEnum.Administrador, TipoPerfilAcessoUsuarioEnum.Responsavel)]
        public ActionResult Compartilhar()
        {
            ConviteCompartilhamentoNegocio negocio = new ConviteCompartilhamentoNegocio(UsuarioLogadoConfig.Instance);
            if (negocio.HasConvidados())
            {
                ListaCompartilhamentoModel modelLista = new ListaCompartilhamentoModel();
                modelLista.Compartilhamentos = negocio.ListarConvidados();
                return View("ListarCompartilhamento", modelLista);
            }

            CompartilharControleModel model = new CompartilharControleModel();
            model.DropboxPermissoesCompartilhamento = Helpers.DropboxHelper.DropboxPermissoesCompartilhamento();
            model.CodigoPermissao = (short)TipoPermissaoCompartilhamentoEnum.Visualizacao;
            return View(model);
        }

        //
        // POST: /ControleFinanceiro/Compartilhar
        [HttpPost]
        [UsuarioFilter(TipoPerfilAcessoUsuarioEnum.Administrador, TipoPerfilAcessoUsuarioEnum.Responsavel)]
        public ActionResult Compartilhar(CompartilharControleModel model)
        {

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Identificamos erros no preenchimento da solicitação, por favor verifique.");
                model.DropboxPermissoesCompartilhamento = Helpers.DropboxHelper.DropboxPermissoesCompartilhamento();
                return View("Compartilhar", model);
            }

            ConviteCompartilhamentoNegocio negocio = new ConviteCompartilhamentoNegocio(UsuarioLogadoConfig.Instance);
            List<ConviteCompartilhamento> listConvites = new List<ConviteCompartilhamento>();
            var listaEmails = UtilNegocio.SepararEmails(model.Emails);
            foreach (var email in listaEmails.Keys)
            {
                ConviteCompartilhamento convite = new ConviteCompartilhamento();
                convite.IdEntidade = UsuarioLogadoConfig.Instance.UsuarioLogado.IdEntidade;
                convite.EmailConvidado = email;
                convite.NomeConvidado = listaEmails[email];
                convite.CodigoPermissaoCompartilhamento = model.CodigoPermissao;
                convite.MensagemConvite = model.Mensagem;
                listConvites.Add(convite);
            }

            try { 

                negocio.RegistrarConvites(listConvites);

                //Exibir os convites...
                ListaCompartilhamentoModel modelLista = new ListaCompartilhamentoModel();
                modelLista.Compartilhamentos = negocio.ListarConvidados();
                return View("ListarCompartilhamento", modelLista);
                
            }
            catch (Exception erro)
            {
                TratarErroNegocio(erro, "Compartilhar");
                return View("Compartilhar");
            }
            
        }

        public ActionResult RemoverCompartilhamento(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult MudarPermissaoCompartilhamento(int id, short permissao)
        {
            throw new NotImplementedException();
        }

        public ActionResult ReenviarConvite(int id)
        {
            throw new NotImplementedException();
        }

        public ActionResult CancelarConvite(int id)
        {
            throw new NotImplementedException();
        }
    }
}
