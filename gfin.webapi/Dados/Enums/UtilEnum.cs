using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public class UtilEnum
    {
        /// <summary>
        /// Recupera o texto do enum;
        /// </summary>
        /// <param name="enumTipo">Enum, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoEnum(object enumTipo)
        {
            string result = "";
            var memInfo = enumTipo.GetType().GetMember(enumTipo.ToString());
            if (memInfo != null && memInfo.Length != 0)
            {
                var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                result = ((DescriptionAttribute)attributes[0]).Description;
            }
            return result;
        }

        /// <summary>
        /// Recupera a abreviacao da Forma de Liquidação;
        /// </summary>
        /// <param name="enumTipo">Enum Forma de Liquidação, para extração da abreviação.</param>
        /// <returns></returns>
        public static string GetAbreviacaoFormaLiquidacaoEnum(TipoFormaLiquidacaoEnum enumTipo)
        {
            string abreviacao = "";
            switch (enumTipo)
            {
                case TipoFormaLiquidacaoEnum.Dinheiro:
                    abreviacao = "DN";
                    break;
                case TipoFormaLiquidacaoEnum.CartaoCreditoDebito:
                    abreviacao = "CC";
                    break;
                case TipoFormaLiquidacaoEnum.ChequeAVista:
                    abreviacao = "CV";
                    break;
                case TipoFormaLiquidacaoEnum.ChequePreDatado:
                    abreviacao = "CP";
                    break;
                case TipoFormaLiquidacaoEnum.BoletoCobranca:
                    abreviacao = "BC";
                    break;
                case TipoFormaLiquidacaoEnum.DebitoEmConta:
                    abreviacao = "DC";
                    break;
                case TipoFormaLiquidacaoEnum.FaturaMensal:
                    abreviacao = "FT";
                    break;
            }
            return abreviacao;
        }

        /// <summary>
        /// Verifica se um codigo do enum é valido, se existe na estrutura do enum.
        /// </summary>
        /// <param name="enumType">Typo enum a ser verificado.</param>
        /// <param name="codigoTipoEnum">Codigo do enum a ser verificado.</param>
        /// <returns>Caso identifique o codigo no objeto enum, retorna TRUE, caso não retorna FALSE;</returns>
        public static bool IsEnumValido(Type enumType, short codigoTipoEnum)
        {
            bool isValid = false;
            foreach (var value in Enum.GetValues(enumType))
            {
                if (codigoTipoEnum == (int)value)
                {//Código identificando no enum;
                    isValid = true;
                    break;
                }
            }
            return isValid;
        }

        /// <summary>
        /// Retorna a lista de opçõe do Enum, com código e descrição.
        /// </summary>
        /// <param name="enumType">Enum para retorno da lista.</param>
        /// <returns></returns>
        public static List<string> ListaEnums(Type enumType)
        {
            var lista = new List<string>();
            if (enumType == null) return lista;
            foreach (var value in Enum.GetValues(enumType))
            {
                int codigo = (int)value;
                string descricao = GetTextoEnum(value);
                lista.Add($"{codigo}={descricao}");
            }
            return lista;
        }
        
        /// <summary>
        /// Recupera o texto do tipo de lançamento da conta;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de lançamento da conta, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoTipoLancamentoConta(int codigoTipo)
        {
            return GetTextoEnum(GetTipoLancamentoConta(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo de perfil acesso;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de perfil de acesso, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoTipoPerfilAcessoUsuario(int codigoTipo)
        {
            return GetTextoEnum(GetTipoPerfilAcessoUsuario(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoPerfilAcessoUsuarioEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de perfil acesso.</param>
        /// <returns></returns>
        public static TipoPerfilAcessoUsuarioEnum GetTipoPerfilAcessoUsuario(int codigoTipo)
        {
            return (TipoPerfilAcessoUsuarioEnum)Enum.Parse(typeof(TipoPerfilAcessoUsuarioEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoLancamentoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de lançamento.</param>
        /// <returns></returns>
        public static TipoLancamentoEnum GetTipoLancamentoConta(int codigoTipo)
        {
            return (TipoLancamentoEnum)Enum.Parse(typeof(TipoLancamentoEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo forma liquidação;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo forma liquidação, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoFormaLiquidacao(short codigoTipo)
        {
            return GetTextoEnum(GetFormaLiquidacao(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo forma recebimento;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo forma recebimento, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoFormaRecebimento(short codigoTipo)
        {
            return GetTextoEnum(GetFormaRecebimento(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da FormaLiquidacaoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do forma liquidação.</param>
        /// <returns></returns>
        public static TipoFormaLiquidacaoEnum GetFormaLiquidacao(short codigoTipo)
        {
            return (TipoFormaLiquidacaoEnum)Enum.Parse(typeof(TipoFormaLiquidacaoEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da FormaRecebimentoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do forma recebimento.</param>
        /// <returns></returns>
        public static TipoFormaRecebimentoEnum GetFormaRecebimento(short codigoTipo)
        {
            return (TipoFormaRecebimentoEnum)Enum.Parse(typeof(TipoFormaRecebimentoEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo de situação do cheque;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de situação do cheque, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoTipoSituacaoCheque(short codigoTipo)
        {
            return GetTextoEnum(GetTipoSituacaoCheque(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoSituacaoChequeEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de Situação.</param>
        /// <returns></returns>
        public static TipoSituacaoChequeEnum GetTipoSituacaoCheque(short codigoTipo)
        {
            return (TipoSituacaoChequeEnum)Enum.Parse(typeof(TipoSituacaoChequeEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo de situação do cartão de crédito;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de situação do cartão de crédito, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoTipoSituacaoCartaoCredito(short codigoTipo)
        {
            return GetTextoEnum(GetTipoSituacaoCartaoCredito(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoSituacaoCartaoCreditoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de Situação.</param>
        /// <returns></returns>
        public static TipoSituacaoCartaoCreditoEnum GetTipoSituacaoCartaoCredito(short codigoTipo)
        {
            return (TipoSituacaoCartaoCreditoEnum)Enum.Parse(typeof(TipoSituacaoCartaoCreditoEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo de permissão de compartilhamento do controle financeiro;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de permissão do compartilhamento, para extração do texto.</param>
        /// <returns></returns>
        public static string GetTextoTipoPermissaoCompartilhamento(short codigoTipo)
        {
            return GetTextoEnum(GetTipoPermissaoCompartilhamento(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoPermissaoCompartilhamentoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de permissão de compartilhamento.</param>
        /// <returns></returns>
        public static TipoPermissaoCompartilhamentoEnum GetTipoPermissaoCompartilhamento(short codigoTipo)
        {
            return (TipoPermissaoCompartilhamentoEnum)Enum.Parse(typeof(TipoPermissaoCompartilhamentoEnum), Convert.ToString(codigoTipo));
        }

        /// <summary>
        /// Recupera o texto do tipo de situação do processo automático;
        /// </summary>
        /// <param name="codigoTipo">Identificador do tipo de situação do processo automático.</param>
        /// <returns></returns>
        public static string GetTextoTipoSituacaoProcesso(short codigoTipo)
        {
            return GetTextoEnum(GetTipoSituacaoProcesso(codigoTipo));
        }

        /// <summary>
        /// Retorna o Enum da TipoSituacaoProcessoEnum pelo identificador do tipo.
        /// </summary>
        /// <param name="codigoTipo">Codigo do tipo de situação do processo automático.</param>
        /// <returns></returns>
        public static TipoSituacaoProcessoEnum GetTipoSituacaoProcesso(short codigoTipo)
        {
            return (TipoSituacaoProcessoEnum)Enum.Parse(typeof(TipoSituacaoProcessoEnum), Convert.ToString(codigoTipo));
        }
    }
}
