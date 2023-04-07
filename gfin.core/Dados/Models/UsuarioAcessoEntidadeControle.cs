namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TB_USUARIO_ACESSO_ENTIDADE_CONTROLE")]
    public partial class UsuarioAcessoEntidadeControle
    {
        [Key]
        [Column("ID_USUARIO_ACESSO_ENTIDADE")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("ID_USUARIO")]
        public int IdUsuarioAcesso { get; set; }

        [Column("ID_USUARIO_RESPONSACEL_ACESSO")]
        public int IdUsuarioResponsavelAcesso { get; set; }

        [Column("CD_TIPO_PERFIL_ACESSO_USUARIO")]
        public short CodigoTipoPerfilAcesso { get; set; }

        [Column("DT_INICIAL_VIGENCIA_ACESSO_USUARIO")]
        public DateTime DataInicialVigenciaAcessoUsuario { get; set; }

        [Column("DT_FINAL_VIGENCIA_ACESSO_USUARIO")]
        public DateTime? DataFinallVigenciaAcessoUsuario { get; set; }

        [Column("DH_REGISTRO_USUARIO_ACESSO")]
        public DateTime DataRegistroUsuarioAcesso { get; set; }

        [ForeignKey("IdUsuarioAcesso")]
        public virtual UsuarioSistema UsuarioAcesso { get; set; }

        [ForeignKey("IdUsuarioResponsavelAcesso")]
        public virtual UsuarioSistema UsuarioResponsacelAcesso { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
