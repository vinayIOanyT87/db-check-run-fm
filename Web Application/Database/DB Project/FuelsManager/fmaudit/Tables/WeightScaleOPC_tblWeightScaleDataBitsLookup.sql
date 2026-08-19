CREATE TABLE [fmaudit].[WeightScaleOPC_tblWeightScaleDataBitsLookup]
(
	[DataBitsIndex] [int] NOT NULL,
	[DataBitsDescription] [varchar](200) NOT NULL,
	[_AuditEventType] [char](1) NULL,
	[_AuditEventSequence] [tinyint] NULL DEFAULT ((0)) ,
	[_AuditSiteGuid] [uniqueidentifier] NULL,
	[_AuditSessionGuid] [uniqueidentifier] NULL,
	[_AuditUserID] [dbo].[udtUserID] NULL,
	[_AuditSessionTokenID] [uniqueidentifier] NULL,
	[_AuditCreatedDate] [datetimeoffset](7) NULL DEFAULT (sysdatetimeoffset()),
	[_AuditGUID] [uniqueidentifier] NOT NULL DEFAULT (newid()) ,
	[_AuditRowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
	[_AuditContext] [varbinary](128) NULL
)
