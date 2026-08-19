/*

	DROP TABLE [dbo].[tblETLAudit]

*/
CREATE TABLE [dbo].[tblETLAudit](
	[AuditKey] [bigint] IDENTITY(1,1) NOT NULL,
	[ParentAuditKey] [bigint] NULL,
	[Operation] [varchar](50) NULL,
	[PkgName] [varchar](50) NULL,
	[PkgGUID] [uniqueidentifier] NULL,
	[PkgVersionGUID] [uniqueidentifier] NULL,
	[PkgVersionMajor] [smallint] NULL,
	[PkgVersionMinor] [smallint] NULL,
	[ExtractRowCnt] [bigint] NULL,
	[InsertRowCnt] [bigint] NULL,
	[UpdateRowCnt] [bigint] NULL,
	[ErrorRowCnt] [bigint] NULL,
	[TableInitialRowCnt] [bigint] NULL,
	[TableFinalRowCnt] [bigint] NULL,
	[ExecStartDT] [datetimeoffset](7) NULL,
	[ExecStopDT] [datetimeoffset](7) NULL,
	[SuccessfulProcessingInd] [char](1) NULL,
	[ProcessNote] [nvarchar](500) NULL,
	[AuditNote] [nvarchar](250) NULL,
	[LoadDate] [datetimeoffset](7) NULL,

 CONSTRAINT [PK_tblETLAudit] PRIMARY KEY CLUSTERED 
(
	[AuditKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
