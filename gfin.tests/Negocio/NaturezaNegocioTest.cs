using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GFin.Tests.Negocio
{
    [TestClass]
    public class NaturezaNegocioTest: GenericTest
    {
        [TestMethod]
        public void TestNaturezaNegocio_RegistrarNatureza_Valida()
        {
            
            var natureza = new NaturezaConta();
            natureza.DescricaoNaturezaConta = "Educação";
            natureza.CodigoTipoLancamentoConta = (int)TipoLancamentoEnum.Despesa;
            natureza.CodigoTipoSituacaoNaturezaConta = (int)TipoSituacaoEnum.Ativo;

            NaturezaNegocio naturezaNegocio = new NaturezaNegocio(this);
            natureza = naturezaNegocio.RegistrarNatureza(natureza);

            Assert.AreNotEqual(0, natureza.Id);

        }

        [TestMethod]
        public void TestNaturezaNegocio_RegistrarNatureza_Invalida()
        {
            try
            {
                NaturezaNegocio naturezaNegocio = new NaturezaNegocio(this);

                var natureza = new NaturezaConta();
                natureza = naturezaNegocio.RegistrarNatureza(natureza);
                Assert.Fail("Não ocorreu exceção de negocio: Descrição não informada.");

            }
            catch (System.Exception erro)
            {
                Assert.IsInstanceOfType(erro, typeof(GFin.Negocio.Erros.NegocioException), string.Format("Exceção de Negocio: {0}", erro.Message));
            }

        }

        [TestMethod]
        public void TestNaturezaNegocio_RegistrarNatureza_TipoLancamentoInvalido()
        {
            try
            {
                NaturezaNegocio naturezaNegocio = new NaturezaNegocio(this);

                var natureza = new NaturezaConta();
                natureza.DescricaoNaturezaConta = "Descrição Certa";
                natureza.CodigoTipoLancamentoConta = 99; //Tipo não definido no enum (GFin.Dados.Enuns.TipoLancamentoEnum);
                natureza = naturezaNegocio.RegistrarNatureza(natureza);
                Assert.Fail("Não ocorreu exceção de negocio: Tipo Lancamento inválido.");

            }
            catch (System.Exception erro)
            {
                Assert.IsInstanceOfType(erro, typeof(GFin.Negocio.Erros.NegocioException), string.Format("Exceção de Negocio: {0}", erro.Message));
            }

        }

        [TestMethod]
        public void TestNaturezaNegocio_ListarNaturezas_Valida()
        {

            NaturezaNegocio naturezaNegocio = new NaturezaNegocio(this);
            var list = naturezaNegocio.ListarNaturezas();
            
            Assert.IsNotNull(list);

        }
    }
}
