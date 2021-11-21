using gfin.webapi.Dados.Erros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace gfin.webapi.Dados
{
    internal class GenericDAO<T> : IGenericDAO<T> where T : class
    {
        private readonly GFinContext _context;

        internal GenericDAO(GFinContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna o contexto (DbSet&lt;T&gt;) da tabela para realizar operações na entidade (tabela).
        /// </summary>
        /// <returns></returns>
        public DbSet<T> DbContextoSet()
        {
            return _context.Set<T>();
        }

        /// <summary>
        /// Retorna o context de conexão com o banco de dados.
        /// </summary>
        /// <returns></returns>
        public GFinContext DbContextoGFin()
        {
            return _context as GFinContext;
        }

        internal DateTime GetSystemDateTime()
        {
            return new DateTime(DateTime.Now.Date.Ticks);// DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        /// <summary>
        /// Recupera a lista de registro de forma paginada, dentro de uma condição (WHERE) informada, 
        /// retornando a quantidade de registros informado (qtdRegistroPorPaginas), de uma página 
        /// (numeroPagina), possibilitando habilitar a navegação entre os objetos de referência 
        /// (contextoNavegacao);
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a consulta dos dados (utilizando LINQ);</param>
        /// <param name="numeroPagina">Número da página que será retornada na paginação, exemplo de 100 registro, paginados por 10 por página, você teria de 1 a 10 página para retorno.</param>
        /// <param name="qtdRegistrosPorPagina">Quantidade de registro retornado por página, exemplo de 100 registros você pode querer retornar apenas 5 por página.</param>
        /// <param name="ordenarPor">(opcional) Regra de ordenação (ORDER BY) dos registros.</param>
        /// <param name="isOrdenarCrescente">Flag para indicar se a ordenação é crescente (ORDER BY ASC) ou decrescente (ORDER BY DESC).</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns>Retorna classe (ListaPaginada&lt;T&gt;) carregada com o resultado da listagem e paginação.</returns>
        public ListaPaginada<T> Listar(System.Linq.Expressions.Expression<Func<T, bool>> condicao, int numeroPagina, int qtdRegistrosPorPagina, System.Linq.Expressions.Expression<Func<T, object>> ordenarPor = null, bool isOrdenarCrescente = true, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao)
        {
            ListaPaginada<T> resultLista = new ListaPaginada<T>();
            try
            {

                var qtdTotalRegistros = QuantRegistros(condicao, contextoNavegacao);

                var query = DbContextoSet().AsQueryable();

                //Incluir o contexto de navegação...
                AddContextoNavegacao(contextoNavegacao, ref query);

                //Aplicar a condição (Where) de consulta...
                if (condicao != null)
                    query = query.Where(condicao);

                //Aplicar ordenação da consulta...
                if (ordenarPor != null)
                {
                    if (isOrdenarCrescente)
                        query = query.OrderByAsc(ordenarPor);
                    else
                        query = query.OrderByDesc(ordenarPor);
                }

                //Aplicar a paginação do resultado...
                var _skip = (numeroPagina - 1) * qtdRegistrosPorPagina;
                query = query.Skip(_skip).Take(qtdRegistrosPorPagina);

                resultLista.HasProximoPagina = (_skip + qtdRegistrosPorPagina < qtdTotalRegistros);
                resultLista.HasPaginaAnterior = (_skip > 0);
                resultLista.NumeroRegistros = qtdTotalRegistros;
                resultLista.NumeroPaginas = 1; //Padrão...
                if (qtdTotalRegistros > qtdRegistrosPorPagina)
                {
                    int qtdPaginas = (qtdTotalRegistros / qtdRegistrosPorPagina);
                    bool isResto = (qtdTotalRegistros % qtdRegistrosPorPagina != 0);
                    if (isResto) qtdPaginas = qtdPaginas + 1;
                    resultLista.NumeroPaginas = qtdPaginas;
                }
                resultLista.Resultado = query.ToList();

                return resultLista;

            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao listar registros de forma paginada, número da página: {0}, quantidade de registros por página: {1}, entidade: {2}", numeroPagina, qtdRegistrosPorPagina, typeof(T).Name), erro);
            }

        }

        /// <summary>
        /// Recupera a lista de registro dentro de uma condição (WHERE) informada, possibilitando 
        /// habilitar a navegação entre os objetos de referência (contextoNavegacao);
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a consulta dos dados (utilizando LINQ);</param>
        /// <param name="ordenarPor">(opcional) Regra de ordenação (ORDER BY) dos registros.</param>
        /// <param name="isOrdenarCrescente">Flag para indicar se a ordenação é crescente (ORDER BY ASC) ou decrescente (ORDER BY DESC).</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        public List<T> Listar(System.Linq.Expressions.Expression<Func<T, bool>> condicao, System.Linq.Expressions.Expression<Func<T, object>> ordenarPor = null, bool isOrdenarAcendente = true, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao)
        {
            try
            {
                var query = DbContextoSet().AsQueryable();

                //Incluir o contexto de navegação...
                AddContextoNavegacao(contextoNavegacao, ref query);

                //Aplicar a condição (Where) de consulta...
                if (condicao != null)
                    query = query.Where(condicao);

                //Aplicar ordenação da consulta...
                if (ordenarPor != null)
                {
                    if (isOrdenarAcendente)
                        query = query.OrderByAsc(ordenarPor);
                    else
                        query = query.OrderByDesc(ordenarPor);
                }

                return query.ToList();

            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao listar registros, entidade: {0}", typeof(T).Name), erro);
            }
        }

        /// <summary>
        /// Recupera a quantidade de registro dentro de uma condição (WHERE) informada.
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a contagem de registros (utilizando LINQ);</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        public int QuantRegistros(System.Linq.Expressions.Expression<Func<T, bool>> condicao, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao)
        {
            try
            {
                var query = DbContextoSet().AsQueryable();

                //Incluir o contexto de navegação...
                AddContextoNavegacao(contextoNavegacao, ref query);

                //Aplicar a condição (Where) de consulta...
                if (condicao != null)
                    query = query.Where(condicao);

                return query.Count();

            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao obter quantidade de registros, entidade: {0}", typeof(T).Name), erro);
            }
        }

        /// <summary>
        /// Recupera um objeto, definido na generalização &lt;T&gt;, a partir de um ID (Identificador) informado.
        /// </summary>
        /// <param name="id">Identificador único do registro de banco para retorna do objeto.</param>
        /// <returns></returns>
        public T ObterPorId(int id)
        {
            try
            {

                return DbContextoSet().Find(id);

            }
            catch (Exception erro)
            {
                throw new DAOException($"Erro ao obter registro pelo Id: {id}, entidade: {typeof(T).Name}", erro);
            }
        }

        /// <summary>
        /// Recupera um registro dentro de uma condição (WHERE) informada.
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a recuperação do registro (utilizando LINQ), 
        /// identificado que exista mais de um será recuperado o primeiro (FirstOrDefault);</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        public T Obter(Expression<Func<T, bool>> condicao, params Expression<Func<T, object>>[] contextoNavegacao)
        {
            try
            {
                var query = DbContextoSet().AsQueryable();

                //Incluir o contexto de navegação...
                AddContextoNavegacao(contextoNavegacao, ref query);

                //Aplicar a condição (Where) de consulta...
                if (condicao != null)
                    query = query.Where(condicao);

                return query.FirstOrDefault();

            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao obter um registro, entidade: {0}", typeof(T).Name), erro);
            }

        }

        /// <summary>
        /// Realiza a inclusão de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregado para inclusão no repositório de dados.</param>
        /// <returns></returns>
        public T Incluir(T entidade)
        {
            try
            {
                var entity = DbContextoSet().Add(entidade);
                return entity.Entity;
            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao incluir um registro, entidade: {0}", typeof(T).Name), erro);
            }
        }
        /// <summary>
        /// Realiza a exclusão de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregada para exclusão no repositório de dados.</param>
        public void Excluir(T entidade)
        {
            try
            {
                _context.Entry(entidade).State = EntityState.Deleted;
            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao remover um registro, entidade: {0}", typeof(T).Name), erro);
            }
        }
        /// <summary>
        /// Realiza a exclusão de uma entidade (registro), pelo seu ID, definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="id">Identificador da entidade para exclusão no repositório de dados.</param>
        public void Excluir(int id)
        {
            var entity = ObterPorId(id);
            Excluir(entity);
        }
        /// <summary>
        /// Realiza a alteração das informações de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregada para alteração das informações no repositório de dados.</param>
        public void Alterar(T entidade)
        {
            try
            {
                _context.Entry(entidade).State = EntityState.Modified;
            }
            catch (Exception erro)
            {
                throw new DAOException(string.Format("Erro ao alterar um registro, entidade: {0}", typeof(T).Name), erro);
            }
        }

        /// <summary>
        /// Responsável por adicionar os objetos de referencias (navegação) para ser possível acessar as referencias destes objetos.
        /// </summary>
        /// <param name="contextoNavegacao"></param>
        /// <param name="set">Ref para inclusão do contexto de navegtação.</param>
        internal void AddContextoNavegacao(System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao, ref IQueryable<T> set)
        {
            foreach (System.Linq.Expressions.Expression<Func<T, object>> contexto in contextoNavegacao)
                set = set.Include<T, object>(contexto);
        }

    }

}