using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GFin.Negocio;

namespace GFin.Tests.Negocio
{
    [TestClass]
    public class UsuarioNegocioTest : GenericTest
    {
        [TestMethod]
        public void TestUsuarioNegocio_RegistrarUsuarioAcessoEntidade_Invalido()
        {
            try
            {
                UsuarioNegocio negocioUsuario = new UsuarioNegocio(this);
                GFin.Dados.Models.UsuarioAcessoEntidadeControle usuarioAcessoEntidade = new GFin.Dados.Models.UsuarioAcessoEntidadeControle();
                usuarioAcessoEntidade.CodigoTipoPerfilAcesso = (short)GFin.Dados.Enums.TipoPerfilAcessoUsuarioEnum.Responsavel;
                usuarioAcessoEntidade = negocioUsuario.RegistrarUsuario(usuarioAcessoEntidade);
                Assert.Fail("Não ocorreu exceção de negócio.");
            }
            catch (Exception erro)
            {
                Assert.IsInstanceOfType(erro, typeof(GFin.Negocio.Erros.NegocioException), string.Format("Exceção de Negocio: {0}", erro.Message));
            }
        }

        [TestMethod]
        public void TestUsuarioNegocio_RegistrarUsuarioAcessoEntidade_Valido()
        {
            try
            {
                UsuarioNegocio negocioUsuario = new UsuarioNegocio(this);
                
                GFin.Dados.Models.UsuarioAcessoEntidadeControle usuarioAcessoEntidade = new GFin.Dados.Models.UsuarioAcessoEntidadeControle();

                //Entidade
                usuarioAcessoEntidade.EntidadeControle = new GFin.Dados.Models.EntidadeControle();
                usuarioAcessoEntidade.EntidadeControle.CodigoTipoEntidade = (short)GFin.Dados.Enums.TipoEntidadeControleEnum.Fisica;
                usuarioAcessoEntidade.EntidadeControle.CpfCnpjEntidade = "";
                usuarioAcessoEntidade.EntidadeControle.NomeEntidade = "Minha Casa";
                
                //Usuario
                usuarioAcessoEntidade.UsuarioAcesso = new GFin.Dados.Models.UsuarioSistema();
                usuarioAcessoEntidade.UsuarioAcesso.EmailUsuario = "eladio.junior@cassi.com.br";
                usuarioAcessoEntidade.UsuarioAcesso.NomeUsuario = "Eladio Lima Magalhães Júnior";
                usuarioAcessoEntidade.UsuarioAcesso.SenhaUsuario = "el@dio123";
                
                //Usuario Acesso Entidade
                usuarioAcessoEntidade.CodigoTipoPerfilAcesso = (short)GFin.Dados.Enums.TipoPerfilAcessoUsuarioEnum.Responsavel;
                usuarioAcessoEntidade.DataInicialVigenciaAcessoUsuario = DateTime.Now;

                usuarioAcessoEntidade = negocioUsuario.RegistrarUsuario(usuarioAcessoEntidade);

                Assert.AreNotEqual(0, usuarioAcessoEntidade.Id);

            }
            catch (Exception erro)
            {
                Assert.Fail("Erro ao registrar usuário: {0}", erro.Message);
            }
        }
    }
}
