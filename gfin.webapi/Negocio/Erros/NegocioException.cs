using System;

namespace gfin.webapi.Negocio.Erros
{
    public class NegocioException : Exception
    {
        public NegocioException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        public NegocioException(string message)
            : base(message)
        {
        }
    }
}
