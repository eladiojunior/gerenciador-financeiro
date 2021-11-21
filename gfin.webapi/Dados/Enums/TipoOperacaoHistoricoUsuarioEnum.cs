using System.ComponentModel;

namespace gfin.webapi.Dados.Enums
{
    public enum TipoOperacaoHistoricoUsuarioEnum
	{
        [Description("Não Informado")]
        NaoInformado = 0,

        [Description("Registrar usuário")]
        RegistrarUsuario = 1,

        [Description("Confirmar e-mail de usuário")]
		ConfirmarEmailUsuario = 2,
        
        [Description("Reenviar confirmação e-mail de usuário")]
        ReenviarConfirmacaoEmailUsuario = 3,

        [Description("Autenticar usuário")]
		AutenticarUsuario = 4,

        [Description("Alterar senha de usuário")]
		AlterarSenhaUsuario = 5,

        [Description("Recuperar senha de acesso")]
        RecuperarSenhaAcesso = 6,

        [Description("Convidar usuário externo")]
        ConvidarUsuarioExterno = 7,
        
        [Description("Mudar foto do usuário")]
		MudarFotoUsuario = 8,

        [Description("Alterar dados do usuário")]
        AlterarDadosUsuario = 9,

        [Description("Alterar dados do entidade de controle do usuário")]
        AlterarDadosEntidadeControleUsuario = 10


    }
}
