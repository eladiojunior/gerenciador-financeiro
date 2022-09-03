using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GFin.Negocio;
using GFin.Negocio.Erros;

namespace GFin.Tests.Negocio
{
    [TestClass]
    public class UtilNegocioTest
    {
        [TestMethod]
        public void TestUtilNegocio_ObterNomeReduzido()
        {
            var nomeReduzido = UtilNegocio.ObterNomeReduzido("Eladio Lima Magalhães Júnior");
            Assert.AreEqual(nomeReduzido, "Eladio");
            nomeReduzido = UtilNegocio.ObterNomeReduzido("joão");
            Assert.AreEqual(nomeReduzido, "João");
        }

        [TestMethod]
        public void TestUtilNegocio_ObterNomeMesValido()
        {
            var nomeMesSetembro = UtilNegocio.ObterNomeMes(9);
            Assert.AreEqual(nomeMesSetembro, "Setembro");
        }
        
        [TestMethod]
        public void TestUtilNegocio_ObterNomeMesInvalido()
        {
            try
            {
                UtilNegocio.ObterNomeMes(31);
                Assert.Fail("Deveria ser um mês inválido.");
            }
            catch (Exception erro)
            {
                Assert.IsInstanceOfType(erro, typeof(NegocioException));
            }
        }

        [TestMethod]
        public void TestUtilNegocio_ValidarSenhaValida()
        {
            //Possui maiúsculo, minúsculo, caracter especial e números;
            string senhaValida = "el@dio123"; 
            bool isValido = UtilNegocio.ValidarSenha(senhaValida);
            Assert.IsTrue(isValido, "Senha válida.");
        }

        [TestMethod]
        public void TestUtilNegocio_ValidarSenhaInvalida()
        {
            string senhaInvalida = "123";
            bool isValido = UtilNegocio.ValidarSenha(senhaInvalida);
            Assert.IsFalse(isValido, "Senha inválida.");
        }

        [TestMethod]
        public void TestUtilNegocio_ValidarEmailValido()
        {
            string emailValido = "eladiojunior@gmail.com";
            bool isValido = UtilNegocio.ValidarEmail(emailValido);
            Assert.IsTrue(isValido, "Email válido.");
        }

        [TestMethod]
        public void TestUtilNegocio_ValidarEmailInvalido()
        {
            string emailInvalido = "eladio junior.com.br";
            bool isValido = UtilNegocio.ValidarEmail(emailInvalido);
            Assert.IsFalse(isValido, "E-mail inválido.");
        }

        [TestMethod]
        public void TestUtilNegocio_Criptografar()
        {
            string valor = "ist@rtup2022";
            string result_valor = "aQBzAHQAQAByAHQAdQBwADIAMAAyADIA";
            string result = UtilNegocio.Criptografar(valor);
            Assert.AreEqual(result, result_valor);
        }

        [TestMethod]
        public void TestUtilNegocio_Descriptografar()
        {
            string valor = "aQBzAHQAQAByAHQAdQBwADIAMAAyADIA";
            string result_valor = "ist@rtup2022";
            string result = UtilNegocio.Descriptografar(valor);
            Assert.AreEqual(result, result_valor);
        }

        [TestMethod]
        public void TestUtilNegocio_ConvertImagemBase64()
        {
            var binaryData = System.IO.File.ReadAllBytes("D:\\Projetos\\GFin\\GFin.Web\\Content\\site\\imgs\\logo-inFourSys-email-topo.png");
            string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
            Console.WriteLine("data:image/png;base64,{0}", base64String);
            Assert.IsNotNull(base64String);
        }
    }
}
