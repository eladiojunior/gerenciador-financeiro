namespace GFin.Dados.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TB_CONTA_CORRENTE")]
    public partial class ContaCorrente
    {
        public ContaCorrente()
        {
            ListaCartaoCredito = new HashSet<CartaoCredito>();
            ListaCheque = new HashSet<Cheque>();
            ListaExtratoBancarioConta = new HashSet<ExtratoBancarioConta>();
            ListaLancamentoExtratoBancario = new HashSet<LancamentoExtratoBancario>();
        }

        [Key]
        [Column("ID_CONTA_CORRENTE")]
        public int Id { get; set; }

        [Required]
        [Column("ID_ENTIDADE_CONTROLE")]
        public int IdEntidade { get; set; }

        [Required]
        [Column("NR_BANCO")]
        public int NumeroBanco { get; set; }

        [Required]
        [StringLength(50)]
        [Column("TX_NOME_BANCO")]
        public string NomeBanco { get; set; }

        [Required]
        [StringLength(10)]
        [Column("NR_AGENCIA")]
        public string NumeroAgencia { get; set; }

        [Required]
        [StringLength(20)]
        [Column("NR_CONTA_CORRENTE")]
        public string NumeroContaCorrente { get; set; }

        [Required]
        [StringLength(100)]
        [Column("TX_NOME_TITULAR_CONTA")]
        public string NomeTitularConta { get; set; }

        [Column("VR_LIMITE_CONTA")]
        public decimal ValorLimiteConta { get; set; }

        [Column("IN_CONTA_ATIVA")]
        public bool IsContaCorrenteAtiva { get; set; }

        [Column("DH_REGISTRO_CONTA")]
        public DateTime DataHoraRegistroContaCorrente { get; set; }

        [ForeignKey("IdEntidade")]
        public virtual EntidadeControle EntidadeControle { get; set; }

        public virtual ICollection<CartaoCredito> ListaCartaoCredito { get; set; }
        public virtual ICollection<Cheque> ListaCheque { get; set; }
        public virtual ICollection<ExtratoBancarioConta> ListaExtratoBancarioConta { get; set; }
        public virtual ICollection<LancamentoExtratoBancario> ListaLancamentoExtratoBancario { get; set; }

        [NotMapped]
        public string BancoAgenciaContaCorrente { 
            get {
                return string.Format("{0} | {1} | {2} - {3}", NumeroBanco.ToString("000"), NumeroAgencia, NumeroContaCorrente, NomeBanco);
            } 
        }
    }
}
