using AppLogger.Enums;
using AppLogger.Interfaces;
using AppLogger.Models;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;

namespace GFin.Web
{
    public class AppLoggerConfig : IConfigLog
    {
        public TipoArmazenamentoLoggerEnum GetTipoArmazenamento()
        {
            return TipoArmazenamentoLoggerEnum.DATABASE;
        }

        public DbConnection ConnectionDataBase()
        {
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["connectionStringAppLogger"];
            return new SqlConnection(connectionString);
        }

        public string GetLocalArmazenamentoLog()
        {
            return null;
        }

        public int GetLimiteMaximoMegaBytesArquivoLog()
        {
            return 10;//dez mega de limite
        }

        public TipoPeriodoExpurgoEnum GetPeriodoExpurgoLog()
        {
            return TipoPeriodoExpurgoEnum.NUNCA;
        }

        public string GetLocalArmazenamentoExpurgoLog()
        {
            return null;
        }

        public TipoLoggerEnum GetTipoLogAtivo()
        {
            return TipoLoggerEnum.ERRO;
        }

        public int GetQtdMinimaNotificacao()
        {
            return 0;
        }

        public List<IEmailLog> GetEmailsNotificacaoLog()
        {
            return null;
        }

        public IConfigSMTP GetConfigServidorEmail()
        {
            return null;
        }

        public string GetHostServidorEmail()
        {
            return null;
        }

        public IEmailLog GetEmailRemetenteLog()
        {
            return null;
        }

        public bool RegraNotificacao(LogEntry log)
        {
            return false;
        }
    }
}