namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_USUARIO_SISTEMA")]
    public partial class UsuarioSistema
    {
        [Key]
        [Column("ID_USUARIO")]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("NM_USUARIO")]
        public string NomeUsuario { get; set; }

        [Column("BY_FOTO_PERFIL_USUARIO", TypeName = "image")]
        public byte[] FotoUsuario { get; set; }

        [Required]
        [StringLength(80)]
        [Column("TX_EMAIL_USUARIO")]
        public string EmailUsuario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TX_SENHA_USUARIO")]
        public string SenhaUsuario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TX_SALT_SENHA_USUARIO")]
        public string SaltSenhaUsuario { get; set; }

        [Column("IN_ALTERACAO_SENHA_USUARIO")]
        public bool IsAlterarSenhaUsuario { get; set; }

        [Column("IN_CONFIRMACAO_EMAIL_USUARIO")]
        public bool IsConfirmacaoEmailUsuario { get; set; }

        [Column("CD_TIPO_SITUACAO_USUARIO")]
        public short CodigoTipoSituacaoUsuario { get; set; }

        [Column("DH_ULTIMO_ACESSO_USUARIO")]
        public DateTime? DataUltimoAcessoUsuario { get; set; }

        [Column("DH_REGISTRO_USUARIO")]
        public DateTime DataHoraRegistroUsuario { get; set; }

    }
}
