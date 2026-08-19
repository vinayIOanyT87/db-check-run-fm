CREATE TABLE [dbo].[tblETLAudit](
	[AuditKey] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentAuditKey] [bigint] NULL,
	[Operation] [varchar](50) NULL,
	[PkgName] [varchar](50) NULL,
	[TableName] [varchar](250) NULL,
	[PkgGUID] [uniqueidentifier] NULL,
	[PkgVersionGUID] [uniqueidentifier] NULL,
	[PkgVersionMajor] [smallint] NULL,
	[PkgVersionMinor] [smallint] NULL,
	[ExecStartDT] [datetimeoffset](7) NULL,
	[ExecStopDT] [datetimeoffset](7) NULL,
	[ExtractRowCnt] [int] NULL,
	[BatchKeyStart] [int] NULL,
	[BatchKeyEnd] [int] NULL,
	[BatchSize] [int] NULL,
	[TableInitialRowCnt] [int] NULL,
	[TableFinalRowCnt] [int] NULL,
	[SuccessfulProcessingInd] [char](1) NULL,
	[ProcessNote] [nvarchar](250) NULL,
	[AuditNote] [nvarchar](1000) NULL,
	[LoadDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_tblETLAudit] PRIMARY KEY CLUSTERED 
(
	[AuditKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]