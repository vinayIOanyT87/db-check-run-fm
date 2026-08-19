SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [erv].[tblEntitySegmentTemplate]'
PRINT ''

DECLARE @ErvEntitySegmentInserted bigint
DECLARE @ErvEntitySegmentUpdated bigint
DECLARE @ErvEntitySegmentDeleted bigint

SET @ErvEntitySegmentInserted = 0
SET @ErvEntitySegmentUpdated = 0
SET @ErvEntitySegmentDeleted = 0


DECLARE @tblEntitySegmentTemplateRefData TABLE
(
	[ActionType] VARCHAR (50),
	[OldAppTableName] NVARCHAR(100),
	[AppTableName] NVARCHAR(100),
	[OldEntityIndexFieldName] NVARCHAR(100),
	[EntityIndexFieldName] NVARCHAR(100),
	[OldEntityTypeId] NVARCHAR(100),
	[EntityTypeId] NVARCHAR(100),
	[OldEntityTypeDisplayName] NVARCHAR(100),
	[EntityTypeDisplayName] NVARCHAR(100),
	[OldFilterFieldName] NVARCHAR(100),
	[FilterFieldName] NVARCHAR(100),
	[OldFilterDisplayName] NVARCHAR(100),
	[FilterDisplayName] NVARCHAR(100),
	[OldFilterValuesStoredProc] NVARCHAR(100),
	[FilterValuesStoredProc] NVARCHAR(100),
	[OldFieldLevelConfigSegment] BIT,
	[FieldLevelConfigSegment] BIT,
	[OldLocationBasedConstraintSegment] BIT,
	[LocationBasedConstraintSegment] BIT,
	[OldSystemSegment] BIT,
	[SystemSegment] BIT,
	[OldEntityAssignmentTableName] NVARCHAR(100),
	[EntityAssignmentTableName] NVARCHAR(100),
	[OldCreatedDate] DATETIMEOFFSET(7),
	[CreatedDate] DATETIMEOFFSET(7),
	[OldCreatedBy] [dbo].[udtUserID],
	[CreatedBy] [dbo].[udtUserID],
	[OldUpdatedDate] DATETIMEOFFSET(7),
	[UpdatedDate] DATETIMEOFFSET(7),
	[OldUpdatedBy] [dbo].[udtUserID],
	[UpdatedBy] [dbo].[udtUserID]
);

MERGE INTO [erv].[tblEntitySegmentTemplate] AS Target
USING (VALUES
	(N'44642d4c-6cdd-4bde-b246-68edc01a064f', N'[dbo].[tblCompanies]', N'CompanyGuid', N'Company', N'Companies', NULL, NULL, NULL, 1, 1, 0, N'[map].[tblEntityCompanyToSite]', N'10/1/2012 3:59:56 PM -04:00', N'', N'10/1/2012 3:59:56 PM -04:00', N''),
	(N'825f4c39-f7ed-43f5-b35d-ae2e5dad6281', N'[dbo].[tblPersonnel]', N'PersonnelGuid', N'Personnel', N'Personnel', NULL, NULL, NULL, 1, 1, 0, N'[map].[tblEntityPersonnelToSite]', N'10/1/2012 3:59:56 PM -04:00', N'', N'10/1/2012 3:59:56 PM -04:00', N''),
	(N'39e9bb24-0e4a-435a-8b35-bcfd8c9cd44b', N'[dbo].[tblEquipment]', N'EquipmentGuid', N'Equipment', N'Equipment', N'EquipmentTypeGuid', N'Equipment Type', N'[erv].[usp_GetEquipmentTypesForSegmentTemplate]', 1, 1, 0, N'[map].[tblEntityEquipmentToSite]', N'10/1/2012 3:59:56 PM -04:00', N'', N'10/1/2012 3:59:56 PM -04:00', N''),
	(N'e47124d1-80ea-4e4a-9f85-beeb294e08ae', N'[dbo].[tblProducts]', N'ProductGuid', N'Product', N'Products', NULL, NULL, NULL, 1, 1, 0, N'[map].[tblEntityProductToSite]', N'10/1/2012 3:59:56 PM -04:00', N'', N'10/1/2012 3:59:56 PM -04:00', N''),
	(N'a7aae550-f952-41f1-b556-c1533882612b', N'[dbo].[tblTransactionAliases]', N'TransactionAliasGuid', N'Transaction_Alias', N'Transaction Aliases', NULL, NULL, NULL, 1, 1, 0, N'[map].[tblEntityTransactionAliasToSite]', N'10/1/2012 3:59:56 PM -04:00', N'', N'10/1/2012 3:59:56 PM -04:00', N'')
 ) AS Source ([EntitySegmentTemplateGuid], [AppTableName], [EntityIndexFieldName], [EntityTypeId], [EntityTypeDisplayName], [FilterFieldName], [FilterDisplayName], [FilterValuesStoredProc], [FieldLevelConfigSegment], [LocationBasedConstraintSegment], [SystemSegment], [EntityAssignmentTableName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[EntitySegmentTemplateGuid] = Source.[EntitySegmentTemplateGuid])
WHEN MATCHED AND EXISTS (SELECT Target.[AppTableName]
						, Target.[EntityIndexFieldName]
						, Target.[EntityTypeId]
						, Target.[EntityTypeDisplayName]
						, Target.[FilterFieldName]
						, Target.[FilterDisplayName]
						, Target.[FilterValuesStoredProc]
						, Target.[FieldLevelConfigSegment]
						, Target.[LocationBasedConstraintSegment]
						, Target.[SystemSegment]
						, Target.[EntityAssignmentTableName]
						, Target.[CreatedDate]
						, Target.[CreatedBy]
						, Target.[UpdatedDate]
						, Target.[UpdatedBy] 
						EXCEPT 
						SELECT Source.[AppTableName]
						, Source.[EntityIndexFieldName]
						, Source.[EntityTypeId]
						, Source.[EntityTypeDisplayName]
						, Source.[FilterFieldName]
						, Source.[FilterDisplayName]
						, Source.[FilterValuesStoredProc]
						, Source.[FieldLevelConfigSegment]
						, Source.[LocationBasedConstraintSegment]
						, Source.[SystemSegment]
						, Source.[EntityAssignmentTableName]
						, Source.[CreatedDate]
						, Source.[CreatedBy]
						, Source.[UpdatedDate]
						, Source.[UpdatedBy]) THEN
	UPDATE SET [AppTableName] = Source.[AppTableName]
				, [EntityIndexFieldName] = Source.[EntityIndexFieldName]
				, [EntityTypeId] = Source.[EntityTypeId]
				, [EntityTypeDisplayName] = Source.[EntityTypeDisplayName]
				, [FilterFieldName] = Source.[FilterFieldName]
				, [FilterDisplayName] = Source.[FilterDisplayName]
				, [FilterValuesStoredProc] = Source.[FilterValuesStoredProc]
				, [FieldLevelConfigSegment] = Source.[FieldLevelConfigSegment]
				, [LocationBasedConstraintSegment] = Source.[LocationBasedConstraintSegment]
				, [SystemSegment] = Source.[SystemSegment]
				, [EntityAssignmentTableName] = Source.[EntityAssignmentTableName]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([EntitySegmentTemplateGuid], [AppTableName], [EntityIndexFieldName], [EntityTypeId], [EntityTypeDisplayName], [FilterFieldName], [FilterDisplayName], [FilterValuesStoredProc], [FieldLevelConfigSegment], [LocationBasedConstraintSegment], [SystemSegment], [EntityAssignmentTableName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[EntitySegmentTemplateGuid], Source.[AppTableName], Source.[EntityIndexFieldName], Source.[EntityTypeId], Source.[EntityTypeDisplayName], Source.[FilterFieldName], Source.[FilterDisplayName], Source.[FilterValuesStoredProc], Source.[FieldLevelConfigSegment], Source.[LocationBasedConstraintSegment], Source.[SystemSegment], Source.[EntityAssignmentTableName], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[AppTableName],
   inserted.[AppTableName],
   deleted.[EntityIndexFieldName],
   inserted.[EntityIndexFieldName],
   deleted.[EntityTypeId],
   inserted.[EntityTypeId],
   deleted.[EntityTypeDisplayName],
   inserted.[EntityTypeDisplayName],
   deleted.[FilterFieldName],
   inserted.[FilterFieldName],
   deleted.[FilterDisplayName],
   inserted.[FilterDisplayName],
   deleted.[FilterValuesStoredProc],
   inserted.[FilterValuesStoredProc],
   deleted.[FieldLevelConfigSegment],
   inserted.[FieldLevelConfigSegment],
   deleted.[LocationBasedConstraintSegment],
   inserted.[LocationBasedConstraintSegment],
   deleted.[SystemSegment],
   inserted.[SystemSegment],
   deleted.[EntityAssignmentTableName],
   inserted.[EntityAssignmentTableName],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblEntitySegmentTemplateRefData;

SELECT @ErvEntitySegmentInserted = COUNT(*) FROM @tblEntitySegmentTemplateRefData WHERE ActionType IN ( 'INSERT' );
SELECT @ErvEntitySegmentUpdated = COUNT(*) FROM @tblEntitySegmentTemplateRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @ErvEntitySegmentDeleted = COUNT(*) FROM @tblEntitySegmentTemplateRefData WHERE ActionType IN ( 'DELETE' )

IF (@ErvEntitySegmentInserted = 0 AND @ErvEntitySegmentUpdated = 0)
BEGIN
	PRINT '** No Changes Detected for [erv].[tblEntitySegmentTemplate] **'
	PRINT ''
END

IF (@ErvEntitySegmentInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ErvEntitySegmentInserted) + ' NEW RECORDS INSERTED INTO [erv].[tblEntitySegmentTemplate] **'
	PRINT ''
END

IF (@ErvEntitySegmentUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @ErvEntitySegmentUpdated) + ' EXISTING RECORDS UPDATED IN [erv].[tblEntitySegmentTemplate] **'
	PRINT ''
	SELECT * FROM @tblEntitySegmentTemplateRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
