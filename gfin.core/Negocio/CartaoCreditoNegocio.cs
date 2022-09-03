using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio.Erros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Negocio
{
    public class CartaoCreditoNegocio : GenericNegocio
    {
        public CartaoCreditoNegocio(GFin.Negocio.Listeners.IClienteListener clienteListener) : base(clienteListener) { }

        /// <summary>
        /// Recupera a lista de cartões de créditos registrados.
        /// </summary>
        /// <param name="situacao">Situação que devem ser listados os cartões.</param>
        /// <returns></returns>
        public List<CartaoCredito> ListarCartaoCredito(TipoSituacaoCartaoCreditoEnum situacao)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (situacao == TipoSituacaoCartaoCreditoEnum.NaoInformado)
                    return uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
                else
                    return uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && cc.SituacaoCartaoCredito == (short)situacao);
            }
        }

        /// <summary>
        /// Realiza o registro de um cartão de crédito.
        /// </summary>
        /// <param name="cartaoCredito">Informações do cartão de crédito.</param>
        public CartaoCredito RegistrarCartaoCredito(CartaoCredito cartaoCredito)
        {
            Validar(cartaoCredito);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                cartaoCredito.DataHoraRegistro = DateTime.Now;
                cartaoCredito = uofw.CartaoCredito.Incluir(cartaoCredito);
                uofw.SalvarAlteracoes();
            }
            return cartaoCredito;
        }

        /// <summary>
        /// Realiza a validação das informações do cartão de crédito antes de seu registro ou alteração.
        /// </summary>
        /// <param name="cartaoCredito">Informações do cartão de crédito.</param>
        /// <returns></returns>
        private void Validar(CartaoCredito cartaoCredito)
        {
            if (cartaoCredito == null)
            {
                throw new NegocioException("Cartão de Crédito informado nulo.");
            }
            if (cartaoCredito.IdEntidade == 0)
            {
                cartaoCredito.IdEntidade = ClienteListener.UsuarioLogado.IdEntidade;
            }
            if (String.IsNullOrEmpty(cartaoCredito.NumeroCartaoCredito))
            {
                throw new NegocioException("Número do Cartão de Crédito não informado.");
            }

            if (String.IsNullOrEmpty(cartaoCredito.NomeCartaoCredito))
            {
                throw new NegocioException("Nome do Cartão de Crédito não informado.");
            }
            if (cartaoCredito.HasCartaoCredito && cartaoCredito.DiaVencimentoCartaoCredito == 0)
            {
                throw new NegocioException("Dia de Vencimento não informado para Cartão de Crédito.");
            }
            if (cartaoCredito.DiaVencimentoCartaoCredito != 0 && cartaoCredito.DiaVencimentoCartaoCredito < 1 && cartaoCredito.DiaVencimentoCartaoCredito > 31)
            {
                throw new NegocioException("Dia de Vencimento do Cartão de Crédito inválido, informar um dia entre 1 a 31.");
            }
            if (cartaoCredito.HasCartaoCredito && cartaoCredito.ValorLimiteCartaoCredito == 0)
            {
                throw new NegocioException("Valor Limite do do Cartão de Crédito não informado.");
            }
            if (String.IsNullOrEmpty(cartaoCredito.NomeProprietarioCartaoCredito))
            {
                throw new NegocioException("Nome do Proprietário do Cartão de Crédito não informado.");
            }

        }

        /// <summary>
        /// Recupera um cartaoCredito pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do cartaoCredito para recuperação.</param>
        /// <returns></returns>
        public CartaoCredito ObterCartaoCredito(int id)
        {
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                return uofw.CartaoCredito.ObterPorId(id);
            }
        }

        /// <summary>
        /// Grava as alterações realizadas na cartaoCredito em banco.
        /// </summary>
        /// <param name="cartaoCredito">Objeto com as informações do cartaoCredito.</param>
        public void GravarCartaoCredito(CartaoCredito cartaoCredito)
        {
            Validar(cartaoCredito);
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                uofw.CartaoCredito.Alterar(cartaoCredito);
                uofw.SalvarAlteracoes();
            }
        }

        /// <summary>
        /// Remove um cartaoCredito do banco de dados.
        /// </summary>
        /// <param name="idCartaoCredito">Identificador do cartão de crédito.</param>
        public void RemoverCartaoCredito(int idCartaoCredito)
        {
            if (idCartaoCredito == 0)
            {
                throw new NegocioException("Identificador do Cartão de Crédito não informado.");
            }

            //Verificar se existem Contas a Pagar vinculadas ao Cartão de Crédito.
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                CartaoCredito cartaoCredito = uofw.CartaoCredito.ObterPorId(idCartaoCredito);
                var qtd = uofw.DespesaMensal.QuantRegistros(d => d.IdEntidade == cartaoCredito.IdEntidade && d.CodigoTipoFormaLiquidacao == (short)TipoFormaLiquidacaoEnum.CartaoCreditoDebito && d.CodigoVinculoFormaLiquidacao == cartaoCredito.Id);
                if (qtd == 0)
                {//Nenhuma despesa mensal vinculada... exclusão física.
                    uofw.CartaoCredito.Excluir(cartaoCredito);
                }
                else
                {//Exclusão lógica.
                    cartaoCredito.SituacaoCartaoCredito = (short)TipoSituacaoCartaoCreditoEnum.Cancelado; //Cancelar cartão;
                    uofw.CartaoCredito.Alterar(cartaoCredito);
                }
                uofw.SalvarAlteracoes();
            }

        }

        /// <summary>
        /// Recupera a lista de cartões de créditos, de uma conta corrente específica.
        /// </summary>
        /// <param name="idContaCorrente">Identificador da conta corrente para listagem dos cartões.</param>
        /// <param name="situacao">Situação que devem ser listados os cartões.</param>
        /// <returns></returns>
        public List<CartaoCredito> ListarCartaoCredito(int idContaCorrente, TipoSituacaoCartaoCreditoEnum situacao)
        {
            List<CartaoCredito> lista = null;
            using (IUnitOfWork uofw = new UnitOfWork())
            {
                if (situacao == TipoSituacaoCartaoCreditoEnum.NaoInformado)
                {
                    if (idContaCorrente == 0)
                    {
                        lista = uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade);
                    }
                    else
                    {
                        lista = uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && cc.IdContaCorrente == idContaCorrente);
                    }
                }
                else
                {
                    if (idContaCorrente == 0)
                    {
                        lista = uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && cc.SituacaoCartaoCredito == (short)situacao);
                    }
                    else
                    {
                        lista = uofw.CartaoCredito.Listar(cc => cc.IdEntidade == ClienteListener.UsuarioLogado.IdEntidade && cc.IdContaCorrente == idContaCorrente && cc.SituacaoCartaoCredito == (short)situacao);
                    }
                }
            }
            return lista;
        }
    }
}
