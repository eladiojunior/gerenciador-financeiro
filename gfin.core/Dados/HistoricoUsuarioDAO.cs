using GFin.Dados.Erros;
using GFin.Dados.Models;
using System;
using System.Linq;
using System.Data.Entity;

namespace GFin.Dados
{
    internal class HistoricoUsuarioDAO : GenericDAO<HistoricoUsuario>
    {
        internal HistoricoUsuarioDAO(GFinContext dbContexto) : base(dbContexto) { }

        /// <summary>
        /// Recupera o último histórico de um usuário específico, pelo seu Id, e o tipo de operação realizado.
        /// Será ordenado por data/hora de registro de forma decrescente (do maior para o menor) e recuperado o último registro.
        /// </summary>
        /// <param name="idUsuario">Identificador do usuário para recuperação do último histórico.</param>
        /// <param name="tipoOperacao">Tipo da operação realizada no histório para recuperação do último.</param>
        /// <returns></returns>
        internal HistoricoUsuario ObterUltimoHistoricoUsuario(int idUsuario, Enums.TipoOperacaoHistoricoUsuarioEnum tipoOperacaoHistorico)
        {
            try
            {
                var query = DbContextoSet().AsQueryable();
                query = query.Include(hu => hu.Usuario).Where(hu => hu.IdUsuario == idUsuario && hu.CodigoTipoOperacao == (short)tipoOperacaoHistorico);
                return query.OrderByDescending(hu => hu.DataHoraRegistroHistoricoUsuario).FirstOrDefault();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException erro)
            {
                throw new DAOException(erro);
            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao obter último histórico de um usuário [{0}] de um tipo de operação específico [{1}].", idUsuario, Enums.UtilEnum.GetTextoEnum(tipoOperacaoHistorico)), erro);
            }
        }
    }
}
