CREATE TABLE [erv].[tblTempFieldLevelConfigMatrix](
	[FLCMatrixIndex] [int] identity NOT NULL,
	[FieldConfigGuid] uniqueidentifier NULL,
	EntitySegmentTemplateGuid uniqueidentifier NOT NULL,
	AppTableName nvarchar(100) NOT NULL,
	EntityTypeId nvarchar(100) NOT NULL,
	EntityTypeDisplayName nvarchar(100) NOT NULL,
	SiteGroupGuid uniqueidentifier NOT NULL,
	SiteGroupId nvarchar(30) NOT NULL,
	HierarchyLevel int NOT NULL,
	FilterFieldName nvarchar(100) NULL,
	FilterDisplayName nvarchar(100) NULL,
	FilterValueGuid uniqueidentifier NULL,
	FilterValueName nvarchar(100) NULL,
	TargetField nvarchar(100) NOT NULL,
	IsExternalAttribute bit NULL,
	InternalFieldName nvarchar(100) NULL,
	InheritedControlMode nvarchar(20) NULL,
	ForwardControlMode nvarchar(20) NOT NULL,
	FLCCreatedDate datetimeoffset(7) NULL,
	FLCCreatedBy nvarchar(100) NULL,
	FLCUpdatedDate datetimeoffset(7) NULL,
	FLCUpdatedBy nvarchar(100) NULL,
	FLCRowVersion int NULL,
	[_CallingReferenceGuid] uniqueidentifier NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblTempFieldLevelConfigMatrix] PRIMARY KEY CLUSTERED 
(
	[FLCMatrixIndex]
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)


GO
/****** Object:  Index [IX_tblTempFieldLevelConfigMatrix_CallingReferenceGuid]    Script Date: 8/31/2012 3:17:48 PM ******/
CREATE NONCLUSTERED INDEX [IX_tblTempFieldLevelConfigMatrix_CallingReferenceGuid] ON [erv].[tblTempFieldLevelConfigMatrix]
(
	[_CallingReferenceGuid] ASC
)
INCLUDE 
( 	[FieldConfigGuid],
	[EntitySegmentTemplateGuid],
	[EntityTypeId],
	[SiteGroupGuid],
	[HierarchyLevel],
	[FilterFieldName],
	[FilterValueGuid],
	[TargetField],
	[InheritedControlMode]
)
WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
ALTER TABLE [erv].[tblTempFieldLevelConfigMatrix] ADD  CONSTRAINT [DF_tblTempFieldLevelConfigMatrix_UpdatedBy]  DEFAULT ('') FOR [UpdatedBy]
GO
ALTER TABLE [erv].[tblTempFieldLevelConfigMatrix] ADD  CONSTRAINT [DF_tblTempFieldLevelConfigMatrix_UpdatedDate]  DEFAULT (sysdatetimeoffset()) FOR [UpdatedDate]
GO
ALTER TABLE [erv].[tblTempFieldLevelConfigMatrix] ADD  CONSTRAINT [DF_tblTempFieldLevelConfigMatrix_CreatedBy]  DEFAULT ('') FOR [CreatedBy]
GO
ALTER TABLE [erv].[tblTempFieldLevelConfigMatrix] ADD  CONSTRAINT [DF_tblTempFieldLevelConfigMatrix_CreatedDate]  DEFAULT (sysdatetimeoffset()) FOR [CreatedDate]
