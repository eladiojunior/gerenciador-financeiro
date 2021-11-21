using gfin.webapi.Dados;
using gfin.webapi.Dados.Enums;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;
using gfin.webapi.Negocio.Listeners;

namespace gfin.webapi.Negocio
{
    public class ChequeNegocio : GenericNegocio
    {
        public ChequeNegocio(IClienteListener cliente, GFinContext context) : base(cliente, context) { }
        /// <summary>
        /// Registra uma Cheque em uma Conta corrente;
        /// </summary>
        /// <param name="cheque">Informações do cheque a ser registrado.</param>
        public Cheque RegistrarCheque(Cheque cheque)
        {

            ValidarCheque(cheque);

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                cheque.DataHoraRegistroCheque = DateTime.Now;
                cheque.CodigoSituacaoCheque = (short)TipoSituacaoChequeEnum.ChequeRegistrado;
                cheque = uofw.Cheque.Incluir(cheque);
                uofw.SalvarAlteracoes();
            }

            return cheque;

        }

        /// <summary>
        /// Realiza a validação das informações do Cheque antes de seu registro ou alteração.
        /// </summary>
        /// <param name="cheque">Informações do cheque a ser validada.</param>
        /// <returns></returns>
        private void ValidarCheque(Cheque cheque)
        {
            if (cheque == null)
            {
                throw new NegocioException("Nenhuma informação de cheque preenchida.");
            }
            if (cheque.IdContaCorrente == 0 && cheque.ContaCorrente == null)
            {
                throw new NegocioException("Conta Corrente do cheque não informada.");
            }
            if (cheque.NumeroCheque == 0)
            {
                throw new NegocioException("Número do cheque não informado.");
            }
        }

        /// <summary>
        /// Recupera a lista de cheques de uma contas corrente.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente para recuperação dos cheques.</param>
        /// <param name="tipoSituacaoCheque">Indicador do tipo de situação do Cheque, utilizar enum: TipoSituacaoChequeEnum.ChequeRegistrado;</param>
        /// <returns></returns>
        public List<Cheque> ListarCheque(int idContaCorrente, TipoSituacaoChequeEnum tipoSituacaoCheque = TipoSituacaoChequeEnum.NaoInformado)
        {
            List<Cheque> lista = null;
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                if (tipoSituacaoCheque == TipoSituacaoChequeEnum.NaoInformado)
                {//Recuperar todos os cheques da conta corrente informada.
                    lista = uofw.Cheque.Listar(c => c.IdContaCorrente == idContaCorrente);
                }
                else
                {//Recupera cheques de uma Conta Corrente e o Tipo de Situação informado.
                    lista = uofw.Cheque.Listar(c => c.IdContaCorrente == idContaCorrente && c.CodigoSituacaoCheque == (short)tipoSituacaoCheque);
                }
            }
            return lista;
        }

        /// <summary>
        /// Recupera um Cheque pelo Id informado.
        /// </summary>
        /// <param name="id">Identificador do Cheque.</param>
        /// <returns></returns>
        public Cheque ObterCheque(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.Cheque.Obter(s => s.Id == id, c => c.ContaCorrente);
            }

        }

        /// <summary>
        /// Recupera um Cheque pela Conta Corrente e seu Número informado.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da Conta Corrente do Cheque;</param>
        /// <param name="numeroCheque">Número do Cheque da Conta Corrente;</param>
        /// <returns></returns>
        public Cheque ObterCheque(int idContaCorrente, int numeroCheque)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.Cheque.ObterPorContaNumeroCheque(idContaCorrente, numeroCheque);
            }
        }

        /// <summary>
        /// Remove um cheque da conta corrente, verificando se existe alguma movimentação do cheque vinculada a ele, 
        /// caso exista será solicitado que o usuário cancele o cheque e não exclua, caso não será uma exclusão física, 
        /// removendo o registro do banco de dados.
        /// </summary>
        /// <param name="cheque">Objeto com as informações do cheque da conta corrente.</param>
        public void RemoverCheque(Cheque cheque)
        {
            if (cheque == null)
            {
                throw new NegocioException("Cheque informado nulo.");
            }

            if (cheque.Id == 0)
            {
                throw new NegocioException("Identificador do cheque não informado.");
            }

            if (cheque.IdContaCorrente == 0)
            {
                throw new NegocioException("Identificador da conta corrente não informado.");
            }

            //Verificar se existem Movimentações do Cheque vinculados.
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                var qtd = uofw.Cheque.QuantRegistros(c => c.Id == cheque.Id && c.CodigoSituacaoCheque != (short)TipoSituacaoChequeEnum.ChequeRegistrado);
                if (qtd != 0)
                {//Nenhuma despesa mensal vinculada... exclusão física.
                    throw new NegocioException(
                        $"Existem movitentações deste Cheque [{cheque.NumeroCheque}], sua remoção não é permitida. Favor realizar o cancelamento do Cheque.");
                }
                uofw.Cheque.Excluir(cheque);
                uofw.SalvarAlteracoes();
            }

        }

        /// <summary>
        /// Realiza o registro de cancelamento do Cheque.
        /// </summary>
        /// <param name="chequeCancelado">Cheque a ser cancelado.</param>
        public void CancelarCheque(ChequeCancelado chequeCancelado)
        {
            if (chequeCancelado == null)
                throw new NegocioException("Nenhuma informação de cheque preenchida.");

            if (chequeCancelado.IdCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            //Registrar no histório ou registro de cheques cancelados.
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {

                var _cheque = uofw.Cheque.Obter(c => c.Id == chequeCancelado.IdCheque, c => c.ContaCorrente);
                if (_cheque == null)
                {
                    throw new NegocioException(
                        $"Não foi possível encontrar o cheque pelo ID [{chequeCancelado.IdCheque}].");
                }
                if (_cheque.CodigoSituacaoCheque == (short)TipoSituacaoChequeEnum.ChequeCancelado)
                {
                    throw new NegocioException(
                        $"Número do Cheque [{_cheque.NumeroCheque}] da Conta [{_cheque.ContaCorrente.BancoAgenciaContaCorrente}] já cancelado.");
                }

                //Alterar situação do cheques para cancelado...
                _cheque.CodigoSituacaoCheque = (short)TipoSituacaoChequeEnum.ChequeCancelado;
                uofw.Cheque.Alterar(_cheque);

                //Registrar nos cheques cancelados...
                uofw.ChequeCancelado.Incluir(chequeCancelado);

                uofw.SalvarAlteracoes();

            }
        }

        /// <summary>
        /// Realizar o registro de vários cheques (uma faixa).
        /// </summary>
        /// <param name="listaCheques">Lista de Cheques.</param>
        public void RegistrarCheques(List<Cheque> listaCheques)
        {
            if (listaCheques.Count == 0)
            {
                throw new NegocioException("Nenhum cheque informado para registro.");
            }
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                foreach (var item in listaCheques)
                {
                    item.DataHoraRegistroCheque = DateTime.Now;
                    ValidarCheque(item);
                    uofw.Cheque.Incluir(item);
                }
                uofw.SalvarAlteracoes();
            }

        }
        /// <summary>
        /// Realiza a emissão de um cheque registrado.
        /// </summary>
        /// <param name="chequeEmitido">Objeto (ChequeEmitido) para registro de sua emissão.</param>
        internal void EmitirCheque(ChequeEmitido chequeEmitido)
        {

            if (chequeEmitido == null)
                throw new NegocioException("Nenhuma informação de cheque preenchida.");

            if (chequeEmitido.IdCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            //Registrar no histório ou registro de cheques emitido.
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {

                var _cheque = uofw.Cheque.Obter(c => c.Id == chequeEmitido.IdCheque, c => c.ContaCorrente);
                if (_cheque == null)
                {
                    throw new NegocioException(
                        $"Não foi possível encontrar o cheque pelo ID [{chequeEmitido.IdCheque}].");
                }
                if (_cheque.CodigoSituacaoCheque == (short)TipoSituacaoChequeEnum.ChequeEmitido)
                {
                    throw new NegocioException(
                        $"Número do Cheque [{_cheque.NumeroCheque}] da Conta [{_cheque.ContaCorrente.BancoAgenciaContaCorrente}] já emitido.");
                }
                if (_cheque.CodigoSituacaoCheque != (short)TipoSituacaoChequeEnum.ChequeRegistrado)
                {
                    throw new NegocioException(string.Format("Número do Cheque [{0}] da Conta [{1}] está com a situação [{3}], não pode ser emitido.", _cheque.NumeroCheque, _cheque.ContaCorrente.BancoAgenciaContaCorrente, UtilEnum.GetTextoTipoSituacaoCheque(_cheque.CodigoSituacaoCheque)));
                }

                //Alterar situação do cheques para Emitido...
                _cheque.CodigoSituacaoCheque = (short)TipoSituacaoChequeEnum.ChequeEmitido;
                uofw.Cheque.Alterar(_cheque);

                //Registrar nos cheques emitidos...
                uofw.ChequeEmitido.Incluir(chequeEmitido);

                uofw.SalvarAlteracoes();
            }

        }
        /// <summary>
        /// Recupera o histórico de cheque emitido pelo id do cheque.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque.</param>
        /// <returns></returns>
        public List<ChequeEmitido> ListarChequeEmitido(int idCheque)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            List<ChequeEmitido> listResult = new List<ChequeEmitido>();
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                listResult = uofw.ChequeEmitido.Listar(s => s.IdCheque == idCheque).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// Recupera o histórico de cheque compensado pelo id do cheque.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque.</param>
        /// <returns></returns>
        public List<ChequeCompensado> ListarChequeCompensado(int idCheque)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            List<ChequeCompensado> listResult = new List<ChequeCompensado>();
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                listResult = uofw.ChequeCompensado.Listar(s => s.IdCheque == idCheque).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// Recupera o histórico de cheque devolvido pelo id do cheque.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque.</param>
        /// <returns></returns>
        public List<ChequeDevolvido> ListarChequeDevolvido(int idCheque)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            List<ChequeDevolvido> listResult = new List<ChequeDevolvido>();
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                listResult = uofw.ChequeDevolvido.Listar(s => s.IdCheque == idCheque).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// Recupera o histórico de cheque cancelado pelo id do cheque.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque.</param>
        /// <returns></returns>
        public List<ChequeCancelado> ListarChequeCancelado(int idCheque)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            List<ChequeCancelado> listResult = new List<ChequeCancelado>();
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                listResult = uofw.ChequeCancelado.Listar(s => s.IdCheque == idCheque).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// Recupera o histórico de cheque resgatado pelo id do cheque.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque.</param>
        /// <returns></returns>
        public List<ChequeResgatado> ListarChequeResgatado(int idCheque)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");

            List<ChequeResgatado> listResult = new List<ChequeResgatado>();
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                listResult = uofw.ChequeResgatado.Listar(s => s.IdCheque == idCheque).ToList();
            }
            return listResult;
        }

        /// <summary>
        /// Realiza o cancelamento de um cheque pela remoção da despesa que o emitiu.
        /// </summary>
        /// <param name="idCheque">Identificador do cheque que será cancelado.</param>
        /// <param name="descricaoDespesa">Descrição da despesa que emitiu o cheque.</param>
        public void CancelarChequePorRemocaoDespesaMensal(int idCheque, string descricaoDespesa)
        {
            if (idCheque == 0)
                throw new NegocioException("Identificador do cheque não informado.");
            if (string.IsNullOrEmpty(descricaoDespesa))
                descricaoDespesa = "[Não informada]";
            var cheque = ObterCheque(idCheque);
            if (cheque != null)
            {//Cancelar cheque.
                var chequeCancelado = new ChequeCancelado();
                chequeCancelado.IdCheque = cheque.Id;
                chequeCancelado.IsCancelamentoBanco = false;
                chequeCancelado.DataCancelamentoCheque = DateTime.Now;
                chequeCancelado.ObservacaoCancelamentoCheque =
                    $"Cancelamento de cheque pela remoção de despesa que o emitiu. Descrição despesa: {descricaoDespesa}.";
                CancelarCheque(chequeCancelado);
            }
        }

    }

}