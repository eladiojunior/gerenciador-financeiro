using GFin.Negocio;
using GFin.Dados.Enums;
using GFin.Web.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GFin.Web.Controllers.Helpers
{
    public class DropboxHelper
    {
        private static DropboxModel dropboxDiaVencimento = null;

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Natureza de Conta - Despesa;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxNaturezaDespesa()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            var negocioNatureza = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
            var listaNaturezas = negocioNatureza.ListarNaturezas(Dados.Enums.TipoLancamentoEnum.Despesa);
            foreach (var item in listaNaturezas)
	        {
                dropbox.Itens.Add(new SelectListItem() { Text = item.DescricaoNaturezaConta, Value = item.Id.ToString() });
	        }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Natureza de Conta - Receita;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxNaturezaReceita()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            var negocioNatureza = new NaturezaNegocio(UsuarioLogadoConfig.Instance);
            var listaNaturezas = negocioNatureza.ListarNaturezas(Dados.Enums.TipoLancamentoEnum.Receita);
            foreach (var item in listaNaturezas)
            {
                dropbox.Itens.Add(new SelectListItem() { Text = item.DescricaoNaturezaConta, Value = item.Id.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Dia do Mês (1 a 31);
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxDiasMes()
        {
            if (dropboxDiaVencimento == null)
            {//Criar instância do objeto e manter na memória...
                dropboxDiaVencimento = new DropboxModel();
                dropboxDiaVencimento.Itens.Add(new SelectListItem() { Text = "Dia", Value = "" });
                for (int i = 1; i <= 31; i++)
                {
                    dropboxDiaVencimento.Itens.Add(new SelectListItem() { Text = i.ToString("00"), Value = i.ToString() });
                }
            }
            return dropboxDiaVencimento;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Tipo de Lançamento de Conta;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxTipoLancamentoConta()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            foreach (var item in Enum.GetValues(typeof(TipoLancamentoEnum)))
            {
                short value = (short)((TipoLancamentoEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Tipo Forma de Liquidação;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxFormaLiquidacao()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            foreach (var item in Enum.GetValues(typeof(TipoFormaLiquidacaoEnum)))
	        {
                short value = (short)((TipoFormaLiquidacaoEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
	        }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Tipo Forma de Recebimento;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxFormaRecebimento()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            foreach (var item in Enum.GetValues(typeof(TipoFormaRecebimentoEnum)))
            {
                short value = (short)((TipoFormaRecebimentoEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox com os Meses de um determinado ano.
        /// No Value: será colocado uma data 01/01/2016, no Text: Janeiro/2016;
        /// </summary>
        /// <param name="ano">Ano que será utilizado para listar seus meses, Ex: Janeiro/2016...</param>
        /// <returns></returns>
        internal static DropboxModel DropboxMesesDoAno(int ano, bool hasSelecione = false)
        {
            var dropbox = new DropboxModel();
            if (hasSelecione)
                dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            for (int mes = 1; mes <= 12; mes++)
            {
                DateTime data = new DateTime(ano, mes, 1);
                dropbox.Itens.Add(new SelectListItem() { Text = UtilNegocio.ConverterPrimeiraLetraEmMaiusculo(data.ToString("MMMM/yyyy")), Value = data.ToString("dd/MM/yyyy") });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera a lista de instituições financeiras cadastradas.
        /// No value do dropbox será colocado uma string: NoBanco|NoAgencia|NoContaCorrente - Nome do Banco
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxBancoAgenciaContaCorrente()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "" });
            var negocioContaCorrente = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
            var lista = negocioContaCorrente.ListarContaCorrente(true);
            foreach (var item in lista)
            {
                dropbox.Itens.Add(new SelectListItem() { Text = item.BancoAgenciaContaCorrente, Value = item.Id.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera a lista de instituições financeiras cadastradas.
        /// No value do dropbox será colocado uma string: NoBanco|NoAgencia|NoContaCorrente - Nome do Banco
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxBancoAgenciaContaCorrenteCartao(bool isItemSemConta = true)
        {
            var dropbox = new DropboxModel();
            if (isItemSemConta)
                dropbox.Itens.Add(new SelectListItem() { Text = "[Cartão Sem Conta Corrente]", Value = "0" });
            var negocioContaCorrente = new ContaCorrenteNegocio(UsuarioLogadoConfig.Instance);
            var lista = negocioContaCorrente.ListarContaCorrente(true);
            foreach (var item in lista)
            {
                dropbox.Itens.Add(new SelectListItem() { Text = item.BancoAgenciaContaCorrente, Value = item.Id.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera a lista de banco registrados na agenda de correntista.
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxNomeBancoCorrentista()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "", Value = "" });
            var negocioCorrentista = new CorrentistaNegocio(UsuarioLogadoConfig.Instance);
            var lista = negocioCorrentista.ListarNomeBancos();
            foreach (var item in lista)
            {
                dropbox.Itens.Add(new SelectListItem() { Text = item, Value = item });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Tipo de Situação do Cheque;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxTipoSituacaoCheque()
        {
            var dropbox = new DropboxModel();
            dropbox.Itens.Add(new SelectListItem() { Text = "Selecione", Value = "0" });
            foreach (var item in Enum.GetValues(typeof(TipoSituacaoChequeEnum)))
            {
                short value = (short)((TipoSituacaoChequeEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contente os itens de Tipo de Situação do Cartão de Crédito;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxTipoSituacaoCartaoCredito()
        {
            var dropbox = new DropboxModel();
            foreach (var item in Enum.GetValues(typeof(TipoSituacaoCartaoCreditoEnum)))
            {
                short value = (short)((TipoSituacaoCartaoCreditoEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
            }
            return dropbox;
        }

        /// <summary>
        /// Recupera um objeto Dropbox contendo os itens de Permissão de Compartilhamento do Controle Financeiro;
        /// </summary>
        /// <returns></returns>
        internal static DropboxModel DropboxPermissoesCompartilhamento()
        {
            //Podem Visualizar Podem Editar
            var dropbox = new DropboxModel();
            foreach (var item in Enum.GetValues(typeof(TipoPermissaoCompartilhamentoEnum)))
            {
                short value = (short)((TipoPermissaoCompartilhamentoEnum)item);
                if (value != 0)
                    dropbox.Itens.Add(new SelectListItem() { Text = UtilEnum.GetTextoEnum(item), Value = value.ToString() });
            }
            return dropbox;
        }
    }
}