/*
	DROP PROCEDURE [erv].[usp_PivotFLCConfigurationsForSegment]

	DECLARE @callingRefGuid uniqueidentifier
	SET @callingRefGuid = NEWID()	
	EXEC [erv].[usp_PivotFLCConfigurationsForSegment] '3A065809-A9FD-45C5-8890-4A9392172352','B85D8705-6B48-41FE-B7A5-69C4BE66992F', '00000000-0000-0000-0000-000000000001', NULL, @CallingRefGuid
	SELECT * FROM erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRefGuid

	DELETE TABLE erv.tblTempEquipmentRecordVersioningFlag WHERE _CallingReferenceGuid = @callingRefGuid
*/


CREATE PROCEDURE [erv].[usp_PivotFLCConfigurationsForSegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @FilterValueGuid uniqueidentifier, @SourceSiteGroupGuid uniqueidentifier, @VSFieldTableReferenceGuid uniqueidentifier = NULL, @CallingReferenceGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_PivotFLCConfigurationsForSegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the VersionSpecific fields for a given entity segment, with the FCM for each Target Field captured in a separate column named using the format: "<TargetField>_VSFlag".
	-- The result of the query is stored in the relevant pivot table for the entity type (e.g. erv.tblTempEquipmentRecordVersioningFlag, erv.tblTempProductRecordVersioningFlag, etc.)
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template to be examined
	-- 2. @FilterValueGuid: Specific filter value of the entity segment to be examined. The @FilterValueGuid parameter is only pertinent to entity segment templates for which a FilterFieldName has been defined.
	-- 3. @SourceSiteGoup: The sitegroup of the segment to be examined
	-- 4. @VSFieldTableReferenceGuid: 
	--		NULL: The VersionSpecific fields of the entity segment are to be retrieved in the Stored Procedure
	--		NOT NULL: The VersionSpecific fields of the entity segment has already been retrieved. Simply re-use the list of VS fields from erv.tblTempRecordVersioningField for the given reference guid.
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

		DECLARE @EntityTypeId nvarchar(100)
		SELECT @EntityTypeId = EntityTypeID FROM erv.tblEntitySegmentTemplate WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid

		IF (@EntityTypeId IS NULL)
		BEGIN
			RAISERROR('Invalid segment.',16,1); 
			RETURN;
		END

		DECLARE @tblSourceVersionSpecificFields TABLE
		(
			TargetField nvarchar(100),
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			Processed bit
		)

		DECLARE @VSFieldTableRefGuid uniqueidentifier
		SET @VSFieldTableRefGuid = @VSFieldTableReferenceGuid
		IF (@VSFieldTableReferenceGuid IS NULL)
		BEGIN
			EXEC erv.usp_GetVersionSpecificFieldsBySegment @EntitySegmentTemplateGuid, @FilterValueGuid, @SourceSiteGroupGuid, @callingReferenceGuid
			SET @VSFieldTableRefGuid = @callingReferenceGuid
		END
		INSERT @tblSourceVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName)
		SELECT TargetField, IsExternalAttribute, InternalFieldName FROM erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @VSFieldTableRefGuid

		IF (@VSFieldTableReferenceGuid IS NULL)
		BEGIN
			DELETE erv.tblTempRecordVersioningField
			WHERE _CallingReferenceGuid = @VSFieldTableRefGuid
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
		DECLARE @vsFieldName nvarchar(100)
		DECLARE @isExternalAttribute bit
		DECLARE @internalFieldName nvarchar(100)
		DECLARE @parmDefinition nvarchar(100)
		DECLARE @fieldCount int

		SET @parmDefinition = N'@callingRefGuidParam uniqueidentifier';
		SET @fieldCount = 0;
		UPDATE @tblSourceVersionSpecificFields SET Processed = 0
		WHILE ((SELECT Count(*) FROM @tblSourceVersionSpecificFields Where Processed = 0) > 0)
		BEGIN
			SELECT TOP 1 @targetFieldName = TargetField, @isExternalAttribute = IsExternalAttribute, @internalFieldName = InternalFieldName FROM @tblSourceVersionSpecificFields WHERE Processed = 0
			SET @vsFieldName = @targetFieldName
			IF ((ISNULL(@isExternalAttribute, 0) = 1) AND (@internalFieldName IS NOT NULL) AND (LEN(@internalFieldName) > 0))
			BEGIN
				SET @vsFieldName = @internalFieldName
			END
			IF ((ISNULL(@isExternalAttribute, 0) = 0) OR  ((ISNULL(@isExternalAttribute, 0) = 1) AND (@internalFieldName IS NOT NULL) AND (LEN(@internalFieldName) > 0)))
			BEGIN			
				SET @fieldCount = @fieldCount + 1
				IF (@fieldCount = 1)
				BEGIN
					SET @updateClause = N'UPDATE ' + @pivotTable + ' SET [' + @vsFieldName + '_RVFlag] = 1'				
				END
				ELSE
				BEGIN
					SET @updateClause = @updateClause + N', [' + @vsFieldName + '_RVFlag] = 1'				
				END 				
			END			
	 					 
			UPDATE @tblSourceVersionSpecificFields SET Processed = 1 WHERE TargetField = @targetFieldName
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
						+ 'Procedure Name: [erv].usp_PivotFLCConfigurationsForSegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
GO


