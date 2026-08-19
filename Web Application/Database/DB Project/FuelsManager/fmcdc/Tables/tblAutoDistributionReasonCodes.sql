CREATE TABLE [fmcdc].[tblAutoDistributionReasonCodes](
	[AutoDistributionReasonCodesSKey] [int] IDENTITY(1,1) NOT NULL,
	[AutoDistributionReasonCodeGuid] [uniqueidentifier] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[ReasonCode] [nvarchar](50) NULL, 
	[Description] [nvarchar](255) NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [nvarchar](50) NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [nvarchar](50) NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblAutoDistributionReasonCodes] PRIMARY KEY CLUSTERED
(
	[AutoDistributionReasonCodesSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO