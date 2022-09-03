using System;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;

namespace GFin.Dados.Erros
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
        public DAOException(System.Data.Entity.Validation.DbEntityValidationException erro)
            : base(DAOException.ObterMensagemValidationException(erro), erro)
        {
        }

        /// <summary>
        /// Verifica a exceção: DbEntityValidationException, recuperando as informações de ocorrência de erro, para melhorar a identificação do erro.
        /// </summary>
        /// <param name="erro">Objeto de exceção carregado.</param>
        /// <returns></returns>
        private static string ObterMensagemValidationException(System.Data.Entity.Validation.DbEntityValidationException erro)
        {
            StringBuilder stringErroValidation = new StringBuilder();
            foreach (var validationErrors in erro.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    string mensagemErro = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                    stringErroValidation.AppendLine(mensagemErro);
                }
            }
            return stringErroValidation.ToString();
        }

    }
}
