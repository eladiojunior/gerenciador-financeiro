﻿CREATE TABLE [dbo].[TB_CONVITE_COMPARTILHAMENTO] (
        [ID_CONVITE_COMPARTILHAMENTO] [int] NOT NULL IDENTITY,
        [ID_ENTIDADE_CONTROLE] [int] NOT NULL,
        [NM_CONVIDADO_COMPARTILHAMENTO] [varchar](80),
        [TX_EMAIL_CONVIDADO_COMPARTILHAMENTO] [varchar](100) NOT NULL,
        [TX_TOKEN_CONVITE_COMPARTILHAMENTO] [varchar](80) NOT NULL,
		TX_MENSAGEM_CONVITE_COMPARTILHAMENTO [varchar](500) NULL,
        [DH_REGISTRO_CONVITE_COMPARTILHAMENTO] [datetime] NOT NULL,
        [DH_ACEITE_CONVITE_COMPARTILHAMENTO] [datetime],
        [CD_PERMISSAO_USUARIO_COMPARTILHAMENTO] [smallint] NOT NULL,
        CONSTRAINT [PK_dbo.TB_CONVITE_COMPARTILHAMENTO] PRIMARY KEY ([ID_CONVITE_COMPARTILHAMENTO])
    );
    CREATE INDEX [IX_ID_ENTIDADE_CONTROLE] ON [dbo].[TB_CONVITE_COMPARTILHAMENTO]([ID_ENTIDADE_CONTROLE]);
    ALTER TABLE [dbo].[TB_CONVITE_COMPARTILHAMENTO] ADD CONSTRAINT [FK_dbo.TB_CONVITE_COMPARTILHAMENTO_dbo.TB_ENTIDADE_CONTROLE_FINANCEIRO_ID_ENTIDADE_CONTROLE] FOREIGN KEY ([ID_ENTIDADE_CONTROLE]) REFERENCES [dbo].[TB_ENTIDADE_CONTROLE_FINANCEIRO] ([ID_ENTIDADE_CONTROLE]);