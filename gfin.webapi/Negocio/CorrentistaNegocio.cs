using gfin.webapi.Dados;
using gfin.webapi.Dados.Models;
using gfin.webapi.Negocio.Erros;
using System;
using System.Collections.Generic;
using gfin.webapi.Negocio.Listeners;

namespace gfin.webapi.Negocio
{
    public class CorrentistaNegocio : GenericNegocio
    {
        public CorrentistaNegocio(IClienteListener cliente, GFinContext context) : base(cliente, context) { }
        /// <summary>
        /// Recupera a lista de correntistas registrados.
        /// </summary>
        /// <returns></returns>
        public List<Correntista> ListarCorrentistas()
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.Correntista.Listar(c => c.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
            }
        }

        /// <summary>
        /// Recupera a lista de nome de bancos dos correntistas sem repetição.
        /// </summary>
        /// <returns></returns>
        public List<string> ListarNomeBancos()
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.Correntista.ListarNomeBanco(ClienteListener.UsuarioLogado.IdEntidade);
            }
        }

        /// <summary>
        /// Realiza o registro de um correntista na agenda.
        /// </summary>
        /// <param name="correntista">Informações do correntista.</param>
        public Correntista RegistrarCorrentista(Correntista correntista)
        {
            Validar(correntista);
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                correntista.DataHoraRegistro = DateTime.Now;
                correntista = uofw.Correntista.Incluir(correntista);
                uofw.SalvarAlteracoes();
                return correntista;
            }
        }

        /// <summary>
        /// Realiza a validação das informações do correntista antes de seu registro ou alteração.
        /// </summary>
        /// <param name="correntista">Informações do correntista.</param>
        /// <returns></returns>
        private void Validar(Correntista correntista)
        {
            if (correntista == null)
            {
                throw new NegocioException("Correntista informado nulo.");
            }
            if (correntista.IdEntidade == 0)
            {
                correntista.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(correntista.NomeBanco))
            {
                throw new NegocioException("Nome do banco não informado.");
            }

            if (String.IsNullOrEmpty(correntista.NumeroAgencia))
            {
                throw new NegocioException("Número da agência não informado.");
            }

            if (String.IsNullOrEmpty(correntista.NumeroContaCorrente))
            {
                throw new NegocioException("Número da conta corrente não informado.");
            }

            if (String.IsNullOrEmpty(correntista.NomeCorrentista))
            {
                throw new NegocioException("Nome do correntista não informado.");
            }

        }

        /// <summary>
        /// Recupera um correntista pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do correntista para recuperação.</param>
        /// <returns></returns>
        public Correntista ObterCorrentista(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                return uofw.Correntista.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na correntista em banco.
        /// </summary>
        /// <param name="correntista">Objeto com as informações do correntista.</param>
        public void GravarCorrentista(Correntista correntista)
        {
            Validar(correntista);
            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                uofw.Correntista.Alterar(correntista);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Remove um correntista do banco de dados.
        /// </summary>
        /// <param name="idCorrentista">Identificador do correntista.</param>
        public void RemoverCorrentista(int idCorrentista)
        {
            if (idCorrentista == 0)
            {
                throw new NegocioException("Identificador do Correntista não informado.");
            }

            using (IUnitOfWork uofw = new UnitOfWork(GFinContext))
            {
                var correntista = uofw.Correntista.ObterPorId(idCorrentista);
                if (correntista == null)
                {
                    throw new NegocioException("Correntista com Id [{0}] não encontrado.");
                }
                uofw.Correntista.Excluir(correntista);
                uofw.SalvarAlteracoes();
            }
        }

    }
}
