using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GFin.Dados.Models
{
    [Table("TB_CONVITE_COMPARTILHAMENTO")]
    public class ConviteCompartilhamento
    {
        [Key]
        [Column("ID_CONVITE_COMPARTILHAMENTO")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }
        
        [StringLength(80)]
        [Column("NM_CONVIDADO_COMPARTILHAMENTO")]
        public string NomeConvidado { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column("TX_EMAIL_CONVIDADO_COMPARTILHAMENTO")]
        public string EmailConvidado { get; set; }
        
        [Required]
        [StringLength(80)]
        [Column("TX_TOKEN_CONVITE_COMPARTILHAMENTO")]
        public string TokenConvite { get; set; }

        [StringLength(500)]
        [Column("TX_MENSAGEM_CONVITE_COMPARTILHAMENTO")]
        public string MensagemConvite { get; set; }

        [Required]
        [Column("DH_REGISTRO_CONVITE_COMPARTILHAMENTO")]
        public DateTime DataHoraRegistroConvite { get; set; }

        [Column("DH_ACEITE_CONVITE_COMPARTILHAMENTO")]
        public DateTime? DataHoraAceiteConvite { get; set; }

        [Required]
        [Column("CD_PERMISSAO_USUARIO_COMPARTILHAMENTO")]
        public short CodigoPermissaoCompartilhamento { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
