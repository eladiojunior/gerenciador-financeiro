using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GFin.Dados;
using GFin.Dados.Enums;
using GFin.Dados.Models;
using GFin.Negocio;

namespace GFin.Tests.Negocio
{
    [TestClass]
    public class DespesaNegocioTest: GenericTest
    {
        [TestMethod]
        public void TestRegistrarDespesa()
        {

            var despesa = new DespesaMensal();
            despesa.DescricaoDespesa = "Pagamento de Conta de Luz";
            despesa.DataVencimentoDespesa = new DateTime(2015, 03, 21);
            despesa.CodigoTipoFormaLiquidacao = (int)TipoFormaLiquidacaoEnum.Dinheiro;
            despesa.IdNaturezaContaDespesa = 1; //Moradia
            despesa.ValorDespesa = 152.99m;
            despesa.IsDespesaLiquidada = false;
            despesa.IsDespesaParcelada = false;
            despesa.TextoObservacaoDespesa = "";
            despesa.ValorDescontoLiquidacaoDespesa = 0;
            despesa.ValorMultaJurosLiquidacaoDespesa = 0;
            despesa.ValorTotalLiquidacaoDespesa = 0;

            DespesaNegocio negocio = new DespesaNegocio(this);
            despesa = negocio.RegistrarDespesa(despesa);

            Assert.AreNotEqual(0, despesa.Id);

        }

        [TestMethod]
        public void TestRegistrarDespesaFixa()
        {

            var despesa = new DespesaFixa();
            despesa.DescricaoDespesaFixa = "Conta de Luz";
            despesa.DiaVencimentoDespesaFixa = 31;
            despesa.CodigoTipoSituacaoDespesaFixa = (int)TipoSituacaoEnum.Ativo;
            despesa.IdNaturezaContaDespesaFixa = 1; //Moradia
            despesa.ValorDespesaFixa = 150.00m;

            DespesaNegocio negocio = new DespesaNegocio(this);
            despesa = negocio.RegistrarDespesaFixa(despesa);

            Assert.AreNotEqual(0, despesa.Id);

        }

        [TestMethod]
        public void TestVerificarDespesaFixaDoMes()
        {
            int idEntidade = 1;
            DespesaNegocio negocio = new DespesaNegocio(this);
            int qtdDespesasRegistradas = negocio.VerificarDespesasFixasDoMes(idEntidade);
        }
    }
}
