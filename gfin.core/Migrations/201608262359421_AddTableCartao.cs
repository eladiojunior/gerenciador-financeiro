namespace GFin.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableCartao : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TB_CARTAO_CREDITO",
                c => new
                    {
                        ID_CARTAO_CREDITO = c.Int(nullable: false, identity: true),
                        ID_CONTA_CORRENTE = c.Int(),
                        NR_CARTAO_CREDITO = c.String(nullable: false, maxLength: 50),
                        NM_CARTAO_CREDITO = c.String(nullable: false, maxLength: 80),
                        DT_VALIDADE_CARTAO_CREDITO = c.DateTime(nullable: false),
                        VL_LIMITE_CARTAO_CREDITO = c.Decimal(nullable: false, precision: 10, scale: 2),
                        DD_VENCIMENTO_CARTAO_CREDITO = c.Short(nullable: false),
                        NM_PROPRIETARIO_CARTAO_CREDITO = c.String(nullable: false, maxLength: 80),
                        CD_SITUACAO_CARTAO_CREDITO = c.Short(nullable: false),
                        DH_REGISTRO_CARTAO_CREDITO = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID_CARTAO_CREDITO)
                .ForeignKey("dbo.TB_CONTA_CORRENTE", t => t.ID_CONTA_CORRENTE)
                .Index(t => t.ID_CONTA_CORRENTE);
            
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NM_BANCO", c => c.String(nullable: false, maxLength: 80));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NR_AGENCIA", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NR_CONTA_CORRENTE", c => c.String(nullable: false, maxLength: 30));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NM_CORRENTISTA", c => c.String(nullable: false, maxLength: 80));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "TX_OBSERVACAO", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TB_CARTAO_CREDITO", "ID_CONTA_CORRENTE", "dbo.TB_CONTA_CORRENTE");
            DropIndex("dbo.TB_CARTAO_CREDITO", new[] { "ID_CONTA_CORRENTE" });
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "TX_OBSERVACAO", c => c.String());
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NM_CORRENTISTA", c => c.String(nullable: false));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NR_CONTA_CORRENTE", c => c.String(nullable: false));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NR_AGENCIA", c => c.String(nullable: false));
            AlterColumn("dbo.TB_AGENDA_CORRENTISTA", "NM_BANCO", c => c.String(nullable: false));
            DropTable("dbo.TB_CARTAO_CREDITO");
        }
    }
}
