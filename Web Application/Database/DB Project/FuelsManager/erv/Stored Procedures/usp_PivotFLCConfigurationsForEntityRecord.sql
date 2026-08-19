/*
	DROP PROCEDURE [erv].[usp_PivotFLCConfigurationsForEntityRecord]

	DECLARE @callingRefGuid uniqueidentifier
	SET @callingRefGuid = NEWID()	
	EXEC [erv].[usp_PivotFLCConfigurationsForEntityRecord] '3A065809-A9FD-45C5-8890-4A9392172352','B85D8705-6B48-41FE-B7A5-69C4BE66992F', '00000000-0000-0000-0000-000000000001', NULL, @CallingRefGuid
	SELECT * FROM erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRefGuid


	DELETE TABLE erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRefGuid
*/


CREATE PROCEDURE [erv].[usp_PivotFLCConfigurationsForEntityRecord]
(
	@EntityTypeId nvarchar(100), @EntityMasterRecGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @RVFieldTableReferenceGuid uniqueidentifier = NULL, @CallingReferenceGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PivotFLCConfigurationsForEntityRecord] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose :Returns the RecordVersioning fields for a given entity record, with the FCM for each Target Field captured in a separate column named using the format: "<TargetField>_RVFlag".
	-- The result of the query is stored in the relevant pivot table for the entity type (e.g. erv.tblTempEquipmentRecordVersioningFlag, erv.tblTempProductRecordVersioningFlag, etc.)
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityMasterRecGuid: master record Guid of the record
	-- 3. @SourceSiteGoup: The sitegroup from which the FLC is to be examined. This should correspond to the AssignedFromSiteGroup when the pivot SP is used in the context of a single record update
	---   or to the owner sitegroup of the record being propagated when the pivot SP is used in the context of general record version propagation.
	-- 4. @RVFieldTableReferenceGuid: 
	--		NULL: The VersionSpecific fields of the entity segment are to be retrieved in the Stored Procedure
	--		NOT NULL: The RecordVersioning fields (VersionSpecific/GlobalSpecific) of the entity segment have already been retrieved. Simply re-use the list of RV fields from erv.tblTempRecordVersioningField for the given reference guid.
	-- 5. @CallingReferenceGuid: Guid to be used to reference an entry in the relevant pivot table, where the result will be stored.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		IF (@CallingReferenceGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END

		DECLARE @tblSourceRecordVersioningFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL,
			Processed bit
		)

		DECLARE @RVFieldTableRefGuid uniqueidentifier
		SET @RVFieldTableRefGuid = @RVFieldTableReferenceGuid
		IF (@RVFieldTableReferenceGuid IS NULL)
		BEGIN
			EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @EntityMasterRecGuid, @SourceSiteGroupGuid, 'VersionSpecific', @CallingReferenceGuid
			SET @RVFieldTableRefGuid = @callingReferenceGuid
		END
		INSERT @tblSourceRecordVersioningFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @RVFieldTableRefGuid

		IF (@RVFieldTableReferenceGuid IS NULL)
		BEGIN
			DELETE erv.tblTempRecordVersioningField
			WHERE _CallingReferenceGuid = @RVFieldTableRefGuid
		END

		DECLARE @pivotTable NVARCHAR(100)
		IF (@EntityTypeId = 'Equipment')
		BEGIN
			SET @pivotTable = 'erv.tblTempEquipmentRecordVersioningFlag'
			IF ((@CallingReferenceGuid IS NULL) OR (NOT EXISTS (SELECT * FROM erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @CallingReferenceGuid)))
			BEGIN
				INSERT INTO erv.tblTempEquipmentRecordVersioningFlag
				(_CallingReferenceGuid)
				SELECT @CallingReferenceGuid
			END
		END
		ELSE IF (@EntityTypeId = 'Product')
		BEGIN
			SET @pivotTable = 'erv.tblTempProductRecordVersioningFlag'
			IF ((@CallingReferenceGuid IS NULL) OR (NOT EXISTS (SELECT * FROM erv.tblTempProductRecordVersioningFlag WHERE _CallingReferenceGuid = @CallingReferenceGuid)))
			BEGIN
				INSERT INTO erv.tblTempProductRecordVersioningFlag
				(_CallingReferenceGuid)
				SELECT @CallingReferenceGuid
			END
		END
		ELSE IF (@EntityTypeId = 'Company')
		BEGIN
			SET @pivotTable = 'erv.tblTempCompanyRecordVersioningFlag'
			IF ((@CallingReferenceGuid IS NULL) OR (NOT EXISTS (SELECT * FROM erv.tblTempCompanyRecordVersioningFlag WHERE _CallingReferenceGuid = @CallingReferenceGuid)))
			BEGIN
				INSERT INTO erv.tblTempCompanyRecordVersioningFlag
				(_CallingReferenceGuid)
				SELECT @CallingReferenceGuid
			END
		END 
		ELSE IF (@EntityTypeId = 'Transaction_Alias')
		BEGIN
			SET @pivotTable = 'erv.tblTempTransactionAliasRecordVersioningFlag'
			IF ((@CallingReferenceGuid IS NULL) OR (NOT EXISTS (SELECT * FROM erv.tblTempTransactionAliasRecordVersioningFlag WHERE _CallingReferenceGuid = @CallingReferenceGuid)))
			BEGIN
				INSERT INTO erv.tblTempTransactionAliasRecordVersioningFlag
				(_CallingReferenceGuid)
				SELECT @CallingReferenceGuid
			END
		END
		ELSE IF (@EntityTypeId = 'Personnel')
		BEGIN
			SET @pivotTable = 'erv.tblTempPersonnelRecordVersioningFlag'
			IF ((@CallingReferenceGuid IS NULL) OR (NOT EXISTS (SELECT * FROM erv.tblTempPersonnelRecordVersioningFlag WHERE _CallingReferenceGuid = @CallingReferenceGuid)))
			BEGIN
				INSERT INTO erv.tblTempPersonnelRecordVersioningFlag
				(_CallingReferenceGuid)
				SELECT @CallingReferenceGuid
			END
		END

		DECLARE @targetFieldName nvarchar(100)
		DECLARE @updateClause nvarchar(max)
		DECLARE @rvFieldName nvarchar(100)
		DECLARE @isExternalAttribute bit
		DECLARE @internalFieldName nvarchar(100)
		DECLARE @parmDefinition nvarchar(100)
		DECLARE @fieldCount int

		SET @parmDefinition = N'@callingRefGuidParam uniqueidentifier';
		SET @fieldCount = 0;
		UPDATE @tblSourceRecordVersioningFields SET Processed = 0
		WHILE ((SELECT Count(*) FROM @tblSourceRecordVersioningFields Where Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @targetFieldName = TargetField, @isExternalAttribute = IsExternalAttribute, @internalFieldName = InternalFieldName FROM @tblSourceRecordVersioningFields WHERE Processed = 0
			SET @rvFieldName = @targetFieldName
			IF ((ISNULL(@isExternalAttribute, 0) = 1) AND (@internalFieldName IS NOT NULL) AND (LEN(@internalFieldName) > 0))
			BEGIN
				SET @rvFieldName = @internalFieldName
			END
			IF ((ISNULL(@isExternalAttribute, 0) = 0) OR  ((ISNULL(@isExternalAttribute, 0) = 1) AND (@internalFieldName IS NOT NULL) AND (LEN(@internalFieldName) > 0)))
			BEGIN			
				SET @fieldCount = @fieldCount + 1
				IF (@fieldCount = 1)
				BEGIN
					SET @updateClause = N'UPDATE ' + @pivotTable + ' SET [' + @rvFieldName + '_RVFlag] = 1'				
				END
				ELSE
				BEGIN
					SET @updateClause = @updateClause + N', [' + @rvFieldName + '_RVFlag] = 1'				
				END 				
			END			
	 					 
			UPDATE @tblSourceRecordVersioningFields SET Processed = 1 WHERE TargetField = @targetFieldName
		END					
		SET @updateClause = @updateClause + N' WHERE _CallingReferenceGuid = @callingRefGuidParam'				
		EXEC sp_executesql @updateClause, @parmDefinition, @callingRefGuidParam = @callingReferenceGuid
	
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: [erv].usp_PivotFLCConfigurationsForEntityRecord' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO
