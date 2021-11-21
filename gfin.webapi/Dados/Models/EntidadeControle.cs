using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gfin.webapi.Dados.Models
{
    [Table("TB_ENTIDADE_CONTROLE_FINANCEIRO")]
    public partial class EntidadeControle
    {

        [Key]
        [Column("ID_ENTIDADE_CONTROLE")]
        public int Id { get; set; }

        [Required]
        [Column("CD_TIPO_ENTIDADE_CONTROLE")]
        public short CodigoTipoEntidade { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NM_ENTIDADE_CONTROLE")]
        public string NomeEntidade { get; set; }

        [StringLength(20)]
        [Column("NR_CPF_CNPJ_ENTIDADE_CONTROLE")]
        public string CpfCnpjEntidade { get; set; }

        [Required]
        [Column("CD_TIPO_SITUACAO_ENTIDADE")]
        public short CodigoTipoSituacaoEntidade { get; set; }

        [Required]
        [Column("DH_REGISTRO_ENTIDADE_CONTROLE")]
        public DateTime DataHoraRegistro { get; set; }
    }
}
