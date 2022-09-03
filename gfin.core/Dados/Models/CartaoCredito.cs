using GFin.Dados.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GFin.Dados.Models
{
    [Table("TB_CARTAO_CREDITO")]
    public class CartaoCredito
    {

        [Key]
        [Column("ID_CARTAO_CREDITO")]
        public int Id { get; set; }

        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Column("ID_CONTA_CORRENTE")]
        public int? IdContaCorrente { get; set; }

        [Required]
        [StringLength(35)]
        [Column("NR_CARTAO_CREDITO")]
        public string NumeroCartaoCredito { get; set; }

        [Required]
        [StringLength(80)]
        [Column("NM_CARTAO_CREDITO")]
        public string NomeCartaoCredito { get; set; }

        [Required]
        [Column("DT_VALIDADE_CARTAO_CREDITO")]
        public DateTime DataValidadeCartaoCredito { get; set; }

        [Column("VL_LIMITE_CARTAO_CREDITO")]
        public decimal ValorLimiteCartaoCredito { get; set; }

        [Column("IN_FUNCAO_CREDITO_ATIVO")]
        public bool HasCartaoCredito { get; set; }

        [Column("IN_CARTAO_CREDITO_PREPAGO")]
        public bool HasCartaoPrePago { get; set; }

        [Column("IN_FUNCAO_DEBITO_ATIVO")]
        public bool HasCartaoDebito { get; set; }

        [Column("DD_VENCIMENTO_CARTAO_CREDITO")]
        public short DiaVencimentoCartaoCredito { get; set; }

        [Required]
        [StringLength(80)]
        [Column("NM_PROPRIETARIO_CARTAO_CREDITO")]
        public string NomeProprietarioCartaoCredito { get; set; }

        [Column("CD_TIPO_SITUACAO_CARTAO_CREDITO")]
        public short SituacaoCartaoCredito { get; set; }

        [Column("DH_REGISTRO_CARTAO_CREDITO")]
        public DateTime DataHoraRegistro { get; set; }

        [ForeignKey("IdContaCorrente")]
        public virtual ContaCorrente ContaCorrenteCheque { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

    }
}
