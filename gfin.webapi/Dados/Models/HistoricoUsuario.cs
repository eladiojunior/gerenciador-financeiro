namespace gfin.webapi.Dados.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_HISTORICO_USUARIO")]
    public partial class HistoricoUsuario
    {

        [Key]
        [Column("ID_HISTORICO_USUARIO")]
        public int Id { get; set; }

        [Required]
        [Column("CD_TIPO_OPERACAO")]
        public short CodigoTipoOperacao { get; set; }

        [Required]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("CD_IDENTIFICACAO_MAQUINA_USUARIO")]
        [StringLength(30)]
        public string IpMaquinaUsuario { get; set; }

        [Column("TX_DISPOSITIVO_ACESSO_USUARIO")]
        [StringLength(250)]
        public string DispositivoAcessoUsuario { get; set; }

        [Column("IN_DISPOSITIVO_MOBILE_USUARIO")]
        public bool IsDispositivoMobileUsuario { get; set; }
        
        [Column("TX_HISTORICO_USUARIO")]
        [StringLength(500)]
        public string TextoHistoricoUsuario { get; set; }

        [Required]
        [Column("DH_REGISTRO_HISTORICO_USUARIO")]
        public DateTime DataHoraRegistroHistoricoUsuario { get; set; }

        [ForeignKey("IdUsuario")]
        public virtual UsuarioSistema Usuario { get; set; }


    }
}
