CREATE TABLE [fmcdc].[tblApplicationString](
	[ApplicationStringSKey] [int] IDENTITY(1,1) NOT NULL,
	[ID] [nvarchar](250) NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[StartDate] [datetime] NULL, 
	[EndDate] [datetime] NULL, 
	[ApplicationStringGuid] [uniqueidentifier] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[LookupApplicationStringTypeIndex] [int] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblApplicationString] PRIMARY KEY CLUSTERED
(
	[ApplicationStringSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO