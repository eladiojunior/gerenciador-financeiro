﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;

namespace gfin.webapi.Negocio
{
    /// <summary>
    /// Classe resposável por gerenciar a comunicação por e-mail.
    /// Resolvemos separar o envio de e-mail em uma classe de negócio apartada, 
    /// pois ela irá enviar e-mails diretamente (transacional), ou seja tentar 
    /// enviar e caso ocorra erro será retornada a exceção para o chamador. 
    /// E também controla-la os envios de e-mail em fila, ou seja, ao acionar
    /// o envio de um e-mail em fila, o email será enviado normalmente, mas caso
    /// ocorra algum erro ele tentará reenviar por 3 (vez), no intervado de 5 mim.
    /// Não sendo possível enviar o e-mail este será colocado em uma lista para 
    /// tentativa manual de envio, ou retirada.
    /// @autor Eladio Júnior
    /// @data 07/11/2016 
    /// </summary>
    public class EmailNegocio
    {
        private static EmailNegocio _instancia = null;
        private readonly IConfiguration _configuration;
        
        private SmtpClient _smtp = null;
        private string _remetenteEmail = null;
        private string _remetenteNome = null;
        private List<Email> _listEmailForaDaFila = null; //Fora da fila, aguardando intervensão manual.
        private List<Email> _listEmailNaFila = null; //Na fila aguardando envio.
        private const short _qtdMaxTentativasEnvio = 3; //Tres tentativas de enviar o e-mail.
        private const short _tempoEspesaProximoEnvio = 5; //Cinco minutos de espesa para o proximo envio.
        private System.Timers.Timer _timerFilaEmail = null;
        private EmailNegocio(IConfiguration configuration)
        {
            _configuration = configuration;
            _remetenteEmail = _configuration.GetValue<String>("ConfigSMTP:smtp_emailRemetente");
            _remetenteNome = _configuration.GetValue<String>("ConfigSMTP:smtp_nomeRemetente");
            _listEmailNaFila = new List<Email>();
            _listEmailForaDaFila = new List<Email>();
            //Definição do relógio para envio de e-mails na fila...
            _timerFilaEmail = new System.Timers.Timer(5000);//Cinco segundos...
            _timerFilaEmail.Elapsed += OnVerificarFilaEmailEvent;
            _timerFilaEmail.Enabled = true;
        }

        /// <summary>
        /// Evento do processo de Timer para verificar os e-mails da fila e tentar envia-los novamente.
        /// O intervalo de verificação entre um e-mail e outro será de 5 min.
        /// </summary>
        private void OnVerificarFilaEmailEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var _newArray = _listEmailNaFila.ToArray();
            foreach (var item in _newArray)
            {
                DateTime dataHoraVerificacao = item.DataHoraUltimaTentativaEnvio.AddMinutes(_tempoEspesaProximoEnvio);
                if (DateTime.Now.CompareTo(dataHoraVerificacao) > 0)
                {//Aguardar 5 minutos para tentar reenviar o e-mail.
                    EnviarEmailSMTP(item);
                }
            }
        }

        /// <summary>
        /// Recupera a instância do objeto (Singleton);
        /// </summary>
        public static EmailNegocio Instance(IConfiguration configuration)
        {
            return _instancia ?? (_instancia = new EmailNegocio(configuration));
        }

        /// <summary>
        /// Envia um e-mail para o destinatário informado, com o assunto informado e mensagem definida.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário da comunicação.</param>
        /// <param name="nomeDestinatario">Nome do destinatário da comunicação (opcional).</param>
        /// <param name="assuntoEmail">Assunto do e-mail enviado.</param>
        /// <param name="mensagemEmail">Mensagem HTML enviada para o destinatário.</param>
        public void EnviarEmail(string emailDestinatario, string nomeDestinatario, string assuntoEmail, string mensagemEmail)
        {
            List<EmailAnexo> anexos = null;
            EnviarEmail(emailDestinatario, nomeDestinatario, assuntoEmail, mensagemEmail, anexos);
        }

        /// <summary>
        /// Envia um e-mail para o destinatário informado, com o assunto informado e mensagem definida.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário da comunicação.</param>
        /// <param name="nomeDestinatario">Nome do destinatário da comunicação (opcional).</param>
        /// <param name="assuntoEmail">Assunto do e-mail enviado.</param>
        /// <param name="mensagemEmail">Mensagem HTML enviada para o destinatário.</param>
        /// <param name="anexo">Arquivo de anexo no e-mail.</param>
        public void EnviarEmail(string emailDestinatario, string nomeDestinatario, string assuntoEmail, string mensagemEmail, EmailAnexo anexo)
        {
            List<EmailAnexo> anexos = null;
            if (anexo != null)
            {
                anexos = new List<EmailAnexo>();
                anexos.Add(anexo);
            }
            EnviarEmail(emailDestinatario, nomeDestinatario, assuntoEmail, mensagemEmail, anexos);
        }

        /// <summary>
        /// Envia um e-mail para o destinatário informado, com o assunto informado e mensagem definida.
        /// </summary>
        /// <param name="emailDestinatario">E-mail do destinatário da comunicação.</param>
        /// <param name="nomeDestinatario">Nome do destinatário da comunicação (opcional).</param>
        /// <param name="assuntoEmail">Assunto do e-mail enviado.</param>
        /// <param name="mensagemEmail">Mensagem HTML enviada para o destinatário.</param>
        /// <param name="anexos">Lista de anexo para envio do e-mail ao destinatário.</param>
        /// <returns></returns>
        public void EnviarEmail(string emailDestinatario, string nomeDestinatario, string assuntoEmail, string mensagemEmail, List<EmailAnexo> anexos)
        {

            if (string.IsNullOrEmpty(emailDestinatario))
                throw new Erros.NegocioException("E-mail do destinatário não informado.");
            
            if (!UtilNegocio.ValidarEmail(emailDestinatario))
                throw new Erros.NegocioException("E-mail do destinatário está inválido, por favor verifique.");

            if (string.IsNullOrEmpty(assuntoEmail))
                throw new Erros.NegocioException("Assunto do e-mail não informado.");

            if (string.IsNullOrEmpty(mensagemEmail))
                throw new Erros.NegocioException("Mensagem do e-mail não informada.");

            var email = new Email();
            email.DestinatarioEmail = emailDestinatario;
            email.DestinatarioNome = nomeDestinatario;
            email.Assunto = assuntoEmail;
            email.Mensagem = mensagemEmail;
            if (anexos != null && anexos.Count > 0)
                email.Anexos = anexos;
            //Retirar o '_tempoEspesaProximoEnvio' para que o e-mail seja enviado na hora.
            email.DataHoraUltimaTentativaEnvio = DateTime.Now.AddMinutes(-_tempoEspesaProximoEnvio);
            //Incluir na fila de envio...
            _listEmailNaFila.Add(email);

        }

        /// <summary>
        /// Responsável pelo envio de e-mail;
        /// </summary>
        /// <param name="email">Informações para envio do e-mail.</param>
        private async Task EnviarEmailSMTP(Email email)
        {

            //Registrar a data da tentativa para evitar que seja enviada novamente.
            email.DataHoraUltimaTentativaEnvio = DateTime.Now;

            MailMessage _email = new MailMessage();
            _email.From = new MailAddress(_remetenteEmail, _remetenteNome);
            var emailsDestinatarios = new MailAddressCollection();
            if (string.IsNullOrEmpty(email.DestinatarioNome))
                _email.To.Add(new MailAddress(email.DestinatarioEmail));
            else
                _email.To.Add(new MailAddress(email.DestinatarioEmail, email.DestinatarioNome));

            _email.Subject = email.Assunto;
            _email.IsBodyHtml = true;
            _email.Body = email.Mensagem;

            try
            {

                if (email.Anexos != null)
                {
                    foreach (var anexo in email.Anexos)
                    {
                        Attachment _anexo = new Attachment(new MemoryStream(anexo.BytesAnexo), anexo.NomeAnexo);
                        if (!string.IsNullOrEmpty(anexo.CidAnexo))
                            _anexo.ContentId = anexo.CidAnexo;
                        _email.Attachments.Add(_anexo);
                    }
                }

                //Recupera a conexão SMTP aberta ou cria uma nova.
                await GetSMTP().SendMailAsync(_email);
                
                //Remover da lista de e-mail.
                _listEmailNaFila.Remove(email);

            }
            catch (Exception erro)
            {
                email.ExceptionTentativaEnvio = erro;
                email.QtdTentativaEnvio += 1; 
                if (email.QtdTentativaEnvio >= _qtdMaxTentativasEnvio)
                {
                    _listEmailForaDaFila.Add(email);
                    _listEmailNaFila.Remove(email);
                }
            }
 	        
        }

        /// <summary>
        /// Recupera um instância de SMTP para envio de e-mail.
        /// </summary>
        /// <returns></returns>
        private SmtpClient GetSMTP()
        {
            if (_smtp == null)
            {
                string smtp_usuario = _configuration.GetValue<String>("ConfigSMTP:smtp_usuario");
                string smtp_senha = _configuration.GetValue<String>("ConfigSMTP:smtp_senha");
                string smtp_host = _configuration.GetValue<String>("ConfigSMTP:smtp_host");
                int smtp_port = _configuration.GetValue<int>("ConfigSMTP:smtp_port");
                _smtp = new SmtpClient(smtp_host, smtp_port);
                if (!string.IsNullOrEmpty(smtp_usuario) && !string.IsNullOrEmpty(smtp_senha))
                {
                    _smtp.EnableSsl = true;
                    _smtp.UseDefaultCredentials = true;
                    string smtp_senha_decrypt = UtilNegocio.Descriptografar(smtp_senha);
                    _smtp.Credentials = new NetworkCredential(smtp_usuario, smtp_senha_decrypt);
                }
            }
            return _smtp;
        }

        /// <summary>
        /// Recupera os e-mails pendentes de envio, que não puderam ser enviados por erro.
        /// Após as tentativas de envio.
        /// </summary>
        /// <returns></returns>
        public List<Email> ListarEmailPendentes()
        {
            return _listEmailForaDaFila;
        }

        /// <summary>
        /// Recupera a quantidade de e-mails na fila para envio.
        /// </summary>
        /// <returns></returns>
        public int QtdEmailNaFila()
        {
            return _listEmailNaFila.Count;
        }

    }

    /// <summary>
    /// Classe que representa as informações de e-mail a ser enviado.
    /// </summary>
    public class Email {
        public  string DestinatarioEmail { get; set; }
        public  string DestinatarioNome { get; set; }
        public  string Mensagem { get; set; }
        public  string Assunto { get; set; }
        public List<EmailAnexo> Anexos { get; set; }
        public DateTime DataHoraUltimaTentativaEnvio { get; set; }
        public Exception ExceptionTentativaEnvio { get; set; }
        public short QtdTentativaEnvio { get; set; }
    }
    /// <summary>
    /// Classe que representa um anexo do e-mail que pode ser colocado como 'embedded' imagem, 
    /// utilizado internamente no html do e-mail adicionando o identificador 
    /// &lt;img src="cid:{Identificador}"&gt;.
    /// </summary>
    public class EmailAnexo
    {
        public string NomeAnexo { get; set; }
        public string CidAnexo { get; set; }
        public byte[] BytesAnexo { get; set; }
        public EmailAnexo(byte[] bytes, string cid, string nome)
        {
            CidAnexo = cid;
            NomeAnexo = nome;
            BytesAnexo = bytes;
        }
        public EmailAnexo(byte[] bytes, string nome)
        {
            CidAnexo = string.Empty;
            NomeAnexo = nome;
            BytesAnexo = bytes;
        }

    }
}
