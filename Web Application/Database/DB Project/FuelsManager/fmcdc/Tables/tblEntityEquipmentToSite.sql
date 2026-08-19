CREATE TABLE [fmcdc].[tblEntityEquipmentToSite](
	[EntityEquipmentToSiteSKey] [int] IDENTITY(1,1) NOT NULL,
	[EquipmentToSiteGuid] [uniqueidentifier] NULL, 
	[EquipmentGuid] [uniqueidentifier] NULL, 
	[SiteGuid] [uniqueidentifier] NULL, 
	[CreatedDate] [datetimeoffset](7) NULL, 
	[CreatedBy] [udtUserID] NULL, 
	[UpdatedDate] [datetimeoffset](7) NULL, 
	[UpdatedBy] [udtUserID] NULL, 
	[SourceRowVersion] [bigint] NULL, 
	[AssignedFromSiteGuid] [uniqueidentifier] NULL, 
	[_ClusterIdx] [bigint] NULL, 
	[RecordUpdatedDate] [datetimeoffset](7) NULL,
	[IsRecordDeleted] [bit] NULL,
	[_RowVersion] [timestamp] NOT NULL,
CONSTRAINT [PK_tblEntityEquipmentToSite] PRIMARY KEY CLUSTERED
(
	[EntityEquipmentToSiteSKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF)
)
GO