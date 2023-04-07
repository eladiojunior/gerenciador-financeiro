USE [DB_GFIN]

/****** Object:  Table [dbo].[LogEntry] ******/
SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON
CREATE TABLE [dbo].[LogEntry](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdLogStatistic] [int] NOT NULL,
	[TipoLog] [varchar](15) NOT NULL,
	[LocalErro] [varchar](200) NULL,
	[MetodoErro] [varchar](150) NULL,
	[Mensagem] [varchar](500) NOT NULL,
	[InnerException] [text] NULL,
	[StackTrace] [text] NULL,
	[DataHora] [datetime] NOT NULL,
	[LoginUsuario] [varchar](50) NULL,
	[NomeUsuario] [varchar](100) NULL,
	[PerfilUsuario] [varchar](30) NULL,
	[InfoLogavel] [text] NULL,
	CONSTRAINT [PK_LogEntry] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
ON [PRIMARY]) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Armazena os logs das aplicacoes que utilizam o applogger.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LogEntry'

/****** Object:  Table [dbo].[LogStatistic] ******/
SET ANSI_PADDING ON
CREATE TABLE [dbo].[LogStatistic](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyAppLogger] [varchar](50) NOT NULL,
	[DataHoraPrimeiroErro] [datetime] NULL,
	[QtdErrosLog] [int] NOT NULL,
	[QtdInfosLog] [int] NOT NULL,
	[UltimoErro] [int] NULL,
	[QtdNotificacao] [int] NOT NULL,
	[QtdErrosNotificacao] [int] NOT NULL,
	[DataHoraUltimaNotificacao] [datetime] NULL,
	CONSTRAINT [PK_LogStatistic] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
	ON [PRIMARY]) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Armazena as informacoes do log por keyLogger.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LogStatistic'

ALTER TABLE [dbo].[LogEntry]  WITH CHECK ADD  CONSTRAINT [FK_LogEntry_LogStatistic] FOREIGN KEY([IdLogStatistic]) REFERENCES [dbo].[LogStatistic] ([Id])
ALTER TABLE [dbo].[LogEntry] CHECK CONSTRAINT [FK_LogEntry_LogStatistic]

ALTER TABLE [dbo].[LogStatistic]  WITH CHECK ADD  CONSTRAINT [FK_LogStatistic_LogEntry] FOREIGN KEY([UltimoErro]) REFERENCES [dbo].[LogEntry] ([Id])
ALTER TABLE [dbo].[LogStatistic] CHECK CONSTRAINT [FK_LogStatistic_LogEntry]
ALTER TABLE [dbo].[LogStatistic] ADD  CONSTRAINT [DF_LogStatistic_QtdErrosLog]  DEFAULT ((0)) FOR [QtdErrosLog]
ALTER TABLE [dbo].[LogStatistic] ADD  CONSTRAINT [DF_LogStatistic_QtdInfosLog]  DEFAULT ((0)) FOR [QtdInfosLog]
ALTER TABLE [dbo].[LogStatistic] ADD  CONSTRAINT [DF_LogStatistic_QtdNotificacao]  DEFAULT ((0)) FOR [QtdNotificacao]
ALTER TABLE [dbo].[LogStatistic] ADD  CONSTRAINT [DF_LogStatistic_QtdErrosNotificacao]  DEFAULT ((0)) FOR [QtdErrosNotificacao]

/****** Object:  Table [dbo].[LogHistorico]    Script Date: 09/05/2011 09:54:08 ******/
SET ANSI_PADDING ON
CREATE TABLE [dbo].[LogHistorico](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[KeyAppLogger] [varchar](50) NOT NULL,
	[DataHoraExpurgo] [datetime] NOT NULL,
	[PeriodoExpurgo] [varchar](20) NOT NULL,
	[QtdErrosExpurgados] [int] NOT NULL,
	[QtdInfosExpurgados] [int] NOT NULL,
	[NomeArquivoExpurgo] [varchar](100) NOT NULL,
	CONSTRAINT [PK_LogHistorico] PRIMARY KEY CLUSTERED ([Id] ASC)
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
	ON [PRIMARY]) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Armazena os historicos de logs apos expurgo.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'LogHistorico'
ALTER TABLE [dbo].[LogHistorico] ADD CONSTRAINT [DF_LogHistorico_QtdErrosExpurgados] DEFAULT ((0)) FOR [QtdErrosExpurgados]
ALTER TABLE [dbo].[LogHistorico] ADD CONSTRAINT [DF_LogHistorico_QtdInfosExpurgados] DEFAULT ((0)) FOR [QtdInfosExpurgados]

/****** Object:  Table [dbo].[ItemHistorico]    Script Date: 09/05/2011 09:50:05 ******/
SET ANSI_PADDING ON
CREATE TABLE [dbo].[ItemHistorico](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[IdLogHistorico] [int] NOT NULL,
	[TipoOcorrencia] [varchar](10) NOT NULL,
	[DescricaoOcorrencia] [varchar](200) NOT NULL,
	[QtdOcorrencia] [int] NOT NULL,
	CONSTRAINT [PK_ItemHistorico] PRIMARY KEY CLUSTERED ( [Id] ASC )
	WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
ON [PRIMARY]) ON [PRIMARY]
SET ANSI_PADDING OFF
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Armazena as informacoes do item de maior ocorrencia.' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'ItemHistorico'
ALTER TABLE [dbo].[ItemHistorico]  WITH CHECK ADD  CONSTRAINT [FK_ItemHistorico_LogHistorico] FOREIGN KEY([IdLogHistorico]) REFERENCES [dbo].[LogHistorico] ([Id])
ALTER TABLE [dbo].[ItemHistorico] CHECK CONSTRAINT [FK_ItemHistorico_LogHistorico]
ALTER TABLE [dbo].[ItemHistorico] ADD  CONSTRAINT [DF_ItemHistorico_QtdOcorrencia]  DEFAULT ((0)) FOR [QtdOcorrencia]
