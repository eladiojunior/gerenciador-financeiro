using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Tests.Negocio
{
    public class GenericTest : GFin.Negocio.Listeners.IClienteListener
    {
        public string IpMaquinaUsuario
        {
            get { return "25.25.25.25"; }
        }

        public string InfoDispositivoUsuario
        {
            get {
                return "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:51.0) Gecko/20100101 Firefox/51.0";
            }
        }
        public GFin.Negocio.Listeners.IUsuarioLogado UsuarioLogado
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDispositivoMobileUsuario
        {
            get { return false; }
        }


        public string MapPathAppServidor
        {
            get { return "C:\\TEMP\\"; }
        }
    }
    internal class UsuarioLogadoTest : GFin.Negocio.Listeners.IUsuarioLogado
    {
        public bool IsLogado
        {
            get { return true; }
        }
        public int IdUsuario
        {
            get { return 1; }
        }
        public string NomeUsuario
        {
            get { return "Eladio Júnior"; }
        }
        public string EmailUsuario
        {
            get { return "eladiojunior@gmail.com.br"; }
        }
        public DateTime DataUltimoAcessoUsuario
        {
            get { return DateTime.Now; }
        }
        public int IdEntidade
        {
            get { return 1; }
        }
        public string NomeEntidade
        {
            get { return "Minha Casa"; }
        }
        public string FotoBase64
        {
            get { return ""; }
            set { }
        }
        public string NomePerfil
        {
            get { return "Responsável"; }
        }
        public short CodigoPerfil
        {
            get { return (short)GFin.Dados.Enums.TipoPerfilAcessoUsuarioEnum.Responsavel; }
        }

        public bool IsEmpresa
        {
            get
            {
                return false;
            }
        }
    }
}