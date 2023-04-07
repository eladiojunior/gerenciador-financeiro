/*==============================================================*/
/* Table: TB_PROCESSO_AUTOMATICO                                */
/*==============================================================*/
create table TB_PROCESSO_AUTOMATICO (
   ID_PROCESSO_AUTOMATICO int                  identity,
   ID_ENTIDADE_CONTROLE int                  not null,
   CD_TIPO_PROCESSO_AUTOMATICO smallint             not null,
   NM_PROCESSO_AUTOMATICO varchar(80)          not null,
   CD_TIPO_SITUACAO_ATUAL_PROCESSO smallint             not null,
   DH_REGISTRO_PROCESSO_AUTOMATICO datetime             not null,
   constraint PK_TB_PROCESSO_AUTOMATICO primary key nonclustered (ID_PROCESSO_AUTOMATICO)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sys.sp_addextendedproperty 'MS_Description', 
   'Entidade para armazenar os processo autom�tico executados na aplica��o.',
   'user', @CurrentUser, 'table', 'TB_PROCESSO_AUTOMATICO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'C�digo do Tipo de Processamento Autom�tico:
1 - Verificar Contas Fixas Mensal;
2 - Verificar Importa��o de Arquivo de Conta Corrente;',
   'user', @CurrentUser, 'table', 'TB_PROCESSO_AUTOMATICO', 'column', 'CD_TIPO_PROCESSO_AUTOMATICO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'C�digo do tipo de situa��o do processo.
Mantem o �ltimo c�digo tipo situa��o do processo para facilitar o acesso a informa��o.',
   'user', @CurrentUser, 'table', 'TB_PROCESSO_AUTOMATICO', 'column', 'CD_TIPO_SITUACAO_ATUAL_PROCESSO'
go

/*==============================================================*/
/* Index: RL_ENTIDADE_CONTROLE_PROCESSO_AUTOMATICO_FK           */
/*==============================================================*/
create index RL_ENTIDADE_CONTROLE_PROCESSO_AUTOMATICO_FK on TB_PROCESSO_AUTOMATICO (
ID_ENTIDADE_CONTROLE ASC
)
go

alter table TB_PROCESSO_AUTOMATICO
   add constraint FK_ENTIDADE_CONTROLE_PROCESSO foreign key (ID_ENTIDADE_CONTROLE)
      references TB_ENTIDADE_CONTROLE_FINANCEIRO (ID_ENTIDADE_CONTROLE)
go

/*==============================================================*/
/* Table: TB_HISTORICO_SITUACAO_PROCESSO                        */
/*==============================================================*/
create table TB_HISTORICO_SITUACAO_PROCESSO (
   ID_HISTORICO_SITUACAO_PROCESSO int                  identity,
   ID_PROCESSO_AUTOMATICO int                  not null,
   CD_TIPO_SITUACAO_PROCESSO smallint             not null,
   DH_SITUACAO_PROCESSO datetime             not null,
   TX_HISTORICO_SITUACAO_PROCESSO varchar(500)         null,
   constraint PK_TB_HISTORICO_SITUACAO_PROCE primary key nonclustered (ID_HISTORICO_SITUACAO_PROCESSO)
)
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sys.sp_addextendedproperty 'MS_Description', 
   'Entidade que armazena o hist�rico das situa��es de cada processo autom�tico (ciclo de vida do processo), e na entidade Processo Automatico ter� a �ltima situa��o do processo, de forma a facilitar o acesso.',
   'user', @CurrentUser, 'table', 'TB_HISTORICO_SITUACAO_PROCESSO'
go

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', 
   'C�digo do tipo de situa��o do processo.',
   'user', @CurrentUser, 'table', 'TB_HISTORICO_SITUACAO_PROCESSO', 'column', 'CD_TIPO_SITUACAO_PROCESSO'
go

/*==============================================================*/
/* Index: RL_PROCESSO_AUTOMATICO_HISTORICO_SITUACAO_PROCESSO_FK */
/*==============================================================*/
create index RL_PROCESSO_AUTOMATICO_HISTORICO_SITUACAO_PROCESSO_FK on TB_HISTORICO_SITUACAO_PROCESSO (
ID_PROCESSO_AUTOMATICO ASC
)
go

alter table TB_HISTORICO_SITUACAO_PROCESSO
   add constraint FK_PROCESSO_HISTORICO_SITUACAO foreign key (ID_PROCESSO_AUTOMATICO)
      references TB_PROCESSO_AUTOMATICO (ID_PROCESSO_AUTOMATICO)
go
