using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados
{
    /// <summary>
    /// Interface para implementação de um repositório de dados.
    /// </summary>
    /// <typeparam name="T">Defini o obejeto de entidade no repositório de dados.</typeparam>
    internal interface IGenericDAO<T> where T : class
    {
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
        ListaPaginada<T> Listar(System.Linq.Expressions.Expression<Func<T, bool>> condicao, int numeroPagina, int qtdRegistrosPorPagina, System.Linq.Expressions.Expression<Func<T, object>> ordenarPor = null, bool isOrdenarCrescente = true, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao);

        /// <summary>
        /// Recupera a lista de registro dentro de uma condição (WHERE) informada, possibilitando 
        /// habilitar a navegação entre os objetos de referência (contextoNavegacao);
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a consulta dos dados (utilizando LINQ);</param>
        /// <param name="ordenarPor">(opcional) Regra de ordenação (ORDER BY) dos registros.</param>
        /// <param name="isOrdenarCrescente">Flag para indicar se a ordenação é crescente (ORDER BY ASC) ou decrescente (ORDER BY DESC).</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        List<T> Listar(System.Linq.Expressions.Expression<Func<T, bool>> condicao, System.Linq.Expressions.Expression<Func<T, object>> ordenarPor = null, bool isOrdenarCrescente = true, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao);

        /// <summary>
        /// Recupera a quantidade de registro dentro de uma condição (WHERE) informada.
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a contagem de registros (utilizando LINQ);</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        int QuantRegistros(System.Linq.Expressions.Expression<Func<T, bool>> condicao, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao);

        /// <summary>
        /// Recupera um objeto, definido na generalização &lt;T&gt;, a partir de um ID (Identificador) informado.
        /// </summary>
        /// <param name="id">Identificador único do registro de banco para retorna do objeto.</param>
        /// <returns></returns>
        T ObterPorId(int id);

        /// <summary>
        /// Recupera um registro dentro de uma condição (WHERE) informada.
        /// </summary>
        /// <param name="condicao">Condição (WHERE) para realizar a recuperação do registro (utilizando LINQ), 
        /// identificado que exista mais de um será recuperado o primeiro (FirstOrDefault);</param>
        /// <param name="contextoNavegacao">Informa o contexto de navegação entre os objetos de referencia.</param>
        /// <returns></returns>
        T Obter(System.Linq.Expressions.Expression<Func<T, bool>> condicao, params System.Linq.Expressions.Expression<Func<T, object>>[] contextoNavegacao);

        /// <summary>
        /// Realiza a inclusão de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregado para inclusão no repositório de dados.</param>
        /// <returns></returns>
        T Incluir(T entidade);
        
        /// <summary>
        /// Realiza a exclusão de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregada para exclusão no repositório de dados.</param>
        void Excluir(T entidade);
        
        /// <summary>
        /// Realiza a exclusão de uma entidade (registro), pelo seu ID, definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="id">Identificador da entidade para exclusão no repositório de dados.</param>
        void Excluir(int id);
        
        /// <summary>
        /// Realiza a alteração das informações de uma entidade (registro), definido na generalização &lt;T&gt;, no repositório de dados.
        /// </summary>
        /// <param name="entidade">Entidade carregada para alteração das informações no repositório de dados.</param>
        void Alterar(T entidade);
        
        /// <summary>
        /// Retorna o contexto (DbSet&lt;T&gt;) da tabela para realizar operações na entidade (tabela).
        /// </summary>
        /// <returns></returns>
        DbSet<T> DbContextoSet();
        
        /// <summary>
        /// Retorna o contexto de conexão com o banco de dados.
        /// </summary>
        /// <returns></returns>
        GFinContext DbContextoGFin();

    }
}
