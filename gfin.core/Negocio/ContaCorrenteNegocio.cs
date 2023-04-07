using GFin.Dados;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GFin.Negocio
{
    public class ContaCorrenteNegocio : GenericNegocio
    {
        public ContaCorrenteNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }
        /// <summary>
        /// Registra uma conta corrente no sistema;
        /// </summary>
        /// <param name="contaCorrente">Informações da conta corrente a ser registrada.</param>
        public ContaCorrente RegistrarContaCorrente(ContaCorrente contaCorrente)
        {

            ValidarContaCorrente(contaCorrente);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                contaCorrente.DataHoraRegistroContaCorrente = DateTime.Now;
                contaCorrente = uofw.ContaCorrente.Incluir(contaCorrente);
                uofw.SalvarAlteracoes();
            }
            return contaCorrente;

        }

        /// <summary>
        /// Realiza a validação das informações da conta corrente antes de seu registro ou alteração.
        /// </summary>
        /// <param name="contaCorrente">Informações da conta corrente a ser validada.</param>
        /// <returns></returns>
        private void ValidarContaCorrente(ContaCorrente contaCorrente)
        {
            if (contaCorrente == null)
            {
                throw new NegocioException("Conta Corrente informada nula.");
            }
            if (contaCorrente.IdEntidade == 0)
            {
                contaCorrente.IdEntidade = base.ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (contaCorrente.NumeroBanco == 0)
            {
                throw new NegocioException("Número do banco não informado.");
            }

            if (String.IsNullOrEmpty(contaCorrente.NomeBanco))
            {
                throw new NegocioException("Nome do banco não informado.");
            }

            if (String.IsNullOrEmpty(contaCorrente.NumeroAgencia))
            {
                throw new NegocioException("Número da agência não informado.");
            }

            if (String.IsNullOrEmpty(contaCorrente.NumeroContaCorrente))
            {
                throw new NegocioException("Número da conta corrente não informado.");
            }

            if (String.IsNullOrEmpty(contaCorrente.NomeTitularConta))
            {
                throw new NegocioException("Nome do titular da conta corrente não informado.");
            }

        }

        /// <summary>
        /// Recupera a lista de contas corrente.
        /// </summary>
        /// <param name="hasSomenteAtivas">Indicador de recuperação das contas corrente, somente ativas?</param>
        /// <returns></returns>
        public List<ContaCorrente> ListarContaCorrente(bool hasSomenteAtivas)
        {
            List<ContaCorrente> listaContasCorrentes = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (hasSomenteAtivas)
                {
                    listaContasCorrentes = uofw.ContaCorrente.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && cc.IsContaCorrenteAtiva == true);
                }
                else
                {
                    listaContasCorrentes = uofw.ContaCorrente.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
                }
            } 
            return listaContasCorrentes;
        }

        /// <summary>
        /// Recupera uma conta corrente da base pelo Id informado.
        /// </summary>
        /// <param name="id">Identificador da conta corrente.</param>
        /// <returns></returns>
        public ContaCorrente ObterContaCorrente(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.ContaCorrente.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na conta corrente em banco.
        /// </summary>
        /// <param name="contaCorrente">Objeto com as informações da conta corrente.</param>
        public void GravarContaCorrente(ContaCorrente contaCorrente)
        {
            ValidarContaCorrente(contaCorrente);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.ContaCorrente.Alterar(contaCorrente);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Remove uma conta corrente, verificando se existe alguma despesa mensal vinculada a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, removendo 
        /// o registro do banco de dados.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente.</param>
        public void RemoverContaCorrente(int idContaCorrente)
        {
            if (idContaCorrente == 0)
            {
                throw new NegocioException("Identificador da conta corrente não informado.");
            }

            //Verificar se existem Cheques vinculados a Conta Corrente.
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                ContaCorrente contaCorrente = uofw.ContaCorrente.ObterPorId(idContaCorrente);
                if (contaCorrente == null)
                {
                    throw new NegocioException(string.Format("Não foi possível encontrar a conta corrente com o Id [{0}] informado.", idContaCorrente));
                }
                var qtd = uofw.Cheque.QuantRegistros(c => c.IdContaCorrente == contaCorrente.Id);
                if (qtd == 0)
                {//Nenhuma despesa mensal vinculada... exclusão física.
                    uofw.ContaCorrente.Excluir(contaCorrente);
                }
                else
                {//Exclusão lógica.
                    contaCorrente.IsContaCorrenteAtiva = false; //Desativando
                    uofw.ContaCorrente.Alterar(contaCorrente);
                }
                uofw.SalvarAlteracoes();
            }
        }
    }

}