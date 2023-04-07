using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GFin.Negocio
{
    public class NaturezaNegocio : GenericNegocio
    {
        public NaturezaNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }

        /// <summary>
        /// Registra uma natureza (Despesa ou Receita) valida.
        /// </summary>
        /// <param name="natureza">Informações da natureza a ser registrada.</param>
        /// <returns></returns>
        public NaturezaConta RegistrarNatureza(NaturezaConta natureza)
        {
            ValidarNatureza(natureza);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                //Informar data do registro da natureza.
                natureza.DataHoraRegistroNaturezaConta = DateTime.Now;
                natureza = uofw.NaturezaConta.Incluir(natureza);
                uofw.SalvarAlteracoes();
                return natureza;
            }
        }

        /// <summary>
        /// Valida as informações da natureza.
        /// </summary>
        /// <param name="natureza">Informações da natureza a ser validada.</param>
        private void ValidarNatureza(NaturezaConta natureza)
        {
            if (natureza == null)
            {
                throw new NegocioException("Nenhuma informação da natureza da conta preenchida.");
            }
            if (natureza.IdEntidade == 0)
            {
                natureza.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(natureza.DescricaoNaturezaConta))
            {
                throw new NegocioException("Descrição da natureza da conta não informada.");
            }
            natureza.DescricaoNaturezaConta = natureza.DescricaoNaturezaConta.Trim();

            if (natureza.CodigoTipoLancamentoConta == (int)TipoLancamentoEnum.NaoInformado)
            {
                throw new NegocioException("Tipo da natureza (Despesa ou Receita) não informado.");
            }
            if (!UtilEnum.IsEnumValido(typeof(TipoLancamentoEnum), natureza.CodigoTipoLancamentoConta))
            {
                throw new NegocioException(string.Format("Tipo da natureza [{0}] inválido, informe 1-Despesa ou 2-Receita.", natureza.CodigoTipoLancamentoConta));
            }
            //Verificar a existencia de uma Natureza com o mesmo nome. Se for nova.
            if (natureza.Id == 0)
            {//Nova

                using (IUnitOfWork uofw = new UnitOfWork())
                {
                    var qtdRegs = uofw.NaturezaConta.QuantRegistros(n => n.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && 
                        n.CodigoTipoLancamentoConta == natureza.CodigoTipoLancamentoConta &&
                        n.DescricaoNaturezaConta.Equals(natureza.DescricaoNaturezaConta, StringComparison.CurrentCultureIgnoreCase));
                    if (qtdRegs > 0)
                    {//Encontrado uma natureza com o mesmo nome.
                        throw new NegocioException("Já existe uma natureza da conta com a mesma descrição.");
                    }
                } 
            }
        }

        /// <summary>
        /// Recupera a lista de todas as naturezas;
        /// </summary>
        /// <returns></returns>
        public List<NaturezaConta> ListarNaturezas()
        {
            return ListarNaturezas(TipoLancamentoEnum.NaoInformado);
        }

        /// <summary>
        /// Recupera a lista de naturezas pelo tipo informado (débito ou crédito);
        /// </summary>
        /// <param name="tipoNatureza">Tipo da natureza.</param>
        /// <returns></returns>
        public List<NaturezaConta> ListarNaturezas(TipoLancamentoEnum tipoNatureza)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                int _idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
                if (tipoNatureza == TipoLancamentoEnum.NaoInformado)
                {//Recupera todos os tipos...
                    return uofw.NaturezaConta.Listar(n => n.IdEntidade == _idEntidade && n.CodigoTipoSituacaoNaturezaConta == (short)TipoSituacaoEnum.Ativo, 
                        n => n.DescricaoNaturezaConta);
                }

                //Recuperar informação específica... por tipo de naturaza (débito ou crédito);
                return uofw.NaturezaConta.Listar(n => n.IdEntidade == _idEntidade && n.CodigoTipoLancamentoConta == (short)tipoNatureza && n.CodigoTipoSituacaoNaturezaConta == (short)TipoSituacaoEnum.Ativo, 
                    n => n.DescricaoNaturezaConta);
            }

        }

        /// <summary>
        /// Verifica a existencia da natureza informada.
        /// </summary>
        /// <param name="idNaturezaConta">Identificador da natureza.</param>
        /// <returns></returns>
        public bool IsNaturezaConta(int idNaturezaConta)
        {
            return IsNaturezaConta(ClienteListener.UsuarioLogado.IdEntidade, idNaturezaConta);
        }

        /// <summary>
        /// Verifica a existencia da natureza informada.
        /// </summary>
        /// <param name="idEntidade">Identificador da entidade de controle.</param>
        /// <param name="idNaturezaConta">Identificador da natureza.</param>
        /// <returns></returns>
        public bool IsNaturezaConta(int idEntidade, int idNaturezaConta)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var qtd = uofw.NaturezaConta.QuantRegistros(n => n.IdEntidade == idEntidade && n.Id == idNaturezaConta);
                return (qtd != 0);
            }
        }

        /// <summary>
        /// Recupera uma natureza de conta pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador da natureza da conta para recuperar.</param>
        /// <returns></returns>
        public NaturezaConta ObterNaturezaConta(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.NaturezaConta.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na natureza da conta em banco.
        /// </summary>
        /// <param name="natureza">Objeto com as informações da despesa fixa.</param>
        public void GravarNaturezaConta(NaturezaConta natureza)
        {
            ValidarNatureza(natureza);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.NaturezaConta.Alterar(natureza);
                uofw.SalvarAlteracoes();
            } 
        }

        /// <summary>
        /// Remove uma natureza da conta, verificando se existe algum vínculo a ela, 
        /// caso exista a exclusão será apenas lógica, caso não será uma exclusão física, 
        /// removendo o registro do banco de dados.
        /// </summary>
        /// <param name="idNatureza">Identificador da natureza da conta.</param>
        public void RemoverNaturezaConta(int idNatureza)
        {
            if (idNatureza == 0)
                throw new NegocioException("Identificador da natureza da conta não informado.");

            using (IUnitOfWork uofw = new UnitOfWork())
            {
                var _natureza = uofw.NaturezaConta.ObterPorId(idNatureza);
                if (_natureza == null)
                    throw new NegocioException(string.Format("Não foi possível encontrar a natureza da conta com o Id [{0}].", idNatureza));

                int _idEntidade = ClienteListener.UsuarioLogado.IdEntidade;
                var qtd_despesaFixa = uofw.DespesaFixa.QuantRegistros(d => d.IdEntidade == _idEntidade && d.IdNaturezaContaDespesaFixa == _natureza.Id);
                var qtd_despesaMensal = uofw.DespesaMensal.QuantRegistros(d => d.IdEntidade == _idEntidade && d.IdNaturezaContaDespesa == _natureza.Id);
                var qtd_receitaFixa = uofw.ReceitaFixa.QuantRegistros(d => d.IdEntidade == _idEntidade && d.IdNaturezaContaReceitaFixa == _natureza.Id);
                var qtd_receitaMensal = uofw.ReceitaMensal.QuantRegistros(d => d.IdEntidade == _idEntidade && d.IdNaturezaContaReceita == _natureza.Id);
                
                if (qtd_despesaFixa == 0 && qtd_despesaMensal == 0 && qtd_receitaFixa == 0 && qtd_receitaMensal == 0)
                {//Nenhuma despesa e receita vinculada a natureza... exclusão física.
                    uofw.NaturezaConta.Excluir(_natureza);
                }
                else
                {//Exclusão lógica.
                    _natureza.CodigoTipoSituacaoNaturezaConta = (short)TipoSituacaoEnum.Inativo;
                    uofw.NaturezaConta.Alterar(_natureza);
                }
                uofw.SalvarAlteracoes();
            }

        }
    }
}
