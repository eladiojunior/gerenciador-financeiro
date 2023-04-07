using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GFin.Negocio;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Web;

namespace GFin.Tests.Negocio
{
    [TestClass]
    public class EmailNegocioTest
    {

        [TestMethod]
        public void TestEmailNegocio_EnviarEmail()
        {
            
            //Recuperar mensagem para o envio.
            var pathEmailConfirmacao = ConfigurationManager.AppSettings["emailConfirmacaoUsuario"];
            var mensagemEmail = File.ReadAllText(pathEmailConfirmacao);

            //Alterar parâmetros da mensagem.
            mensagemEmail = mensagemEmail.Replace("#DATA_ENVIO_EMAIL#", DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy"));
            mensagemEmail = mensagemEmail.Replace("#NOME#", "Eladio Lima Magalhães Júnior");
            string linkConfirmacaoEmail = ConfigurationManager.AppSettings["linkConfirmacaoUsuario"];
            linkConfirmacaoEmail = string.Format(linkConfirmacaoEmail, UtilNegocio.Criptografar("99"));
            mensagemEmail = mensagemEmail.Replace("#LINK_CONFIRMCAO_EMAIL#", linkConfirmacaoEmail);
            
            //Anexar imagem 'embedder'...
            List<EmailAnexo> anexos = null;
            string cidAnexo = "CID_IMG_CABECALHO_EMAIL";
            mensagemEmail = mensagemEmail.Replace("#IMG_CABECALHO_EMAIL#", cidAnexo);
            var pathImageCabecalhoEmail = ConfigurationManager.AppSettings["imagemCabecalhoEmail"];
            FileInfo fileImagem = new FileInfo(pathImageCabecalhoEmail);
            if (fileImagem.Exists)
            {
                anexos = new List<EmailAnexo>();
                byte[] bytesImgCabecalhoEmail = File.ReadAllBytes(fileImagem.FullName);
                anexos.Add(new EmailAnexo(bytesImgCabecalhoEmail, cidAnexo, fileImagem.Name));
            }

            EmailNegocio.Instance.EnviarEmail("eladiojunior@gmail.com", "Eladio Júnior", "Confirmação de Usuário", mensagemEmail, anexos);

            Console.WriteLine("Qtd e-mail Fila t1: {0}", EmailNegocio.Instance.QtdEmailNaFila());

            Thread.Sleep(60000*2); //Dormir por 2 minutos...
            
            Console.WriteLine("Qtd e-mail Fila t2: {0}", EmailNegocio.Instance.QtdEmailNaFila());
            
            Assert.IsTrue(true);

        }

    }
}
