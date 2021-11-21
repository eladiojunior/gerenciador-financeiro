using System;

namespace gfin.webapi.Dados.Erros
{
    /// <summary>
    /// Representa uma exception de banco de dados.
    /// </summary>
    public class DAOException : Exception
    {
        public DAOException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public DAOException(string message)
            : base(message)
        {
        }

    }
}
