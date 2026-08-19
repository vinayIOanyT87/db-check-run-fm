/*
	DROP PROCEDURE [erv].[usp_GetVersionSpecificFieldsBySegment]

	EXEC [erv].[usp_GetVersionSpecificFieldsBySegment] '3A065809-A9FD-45C5-8890-4A9392172352', NULL, '00000000-0000-0000-0000-000000000001', NULL

*/



CREATE PROCEDURE [erv].[usp_GetVersionSpecificFieldsBySegment]
(
	@EntitySegmentTemplateGuid uniqueidentifier, @FilterValueGuid uniqueidentifier, @TargetSiteIndex uniqueidentifier, @CallingReferenceGuid uniqueidentifier = NULL
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [erv].[usp_GetVersionSpecificFieldsBySegment] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose :Returns the VersionSpecific fields for a given entity segment
	-- Notes:
	-- 1. @EntitySegmentTemplateGuid: Segment Template to be examined
	-- 2. @FilterValueGuid: Specific filter value of the entity segment to be examined. The @FilterValueGuid parameter is only pertinent to entity segment templates for which a FilterFieldName has been defined.
	-- 3. @TargetSiteIndex: The sitegroup of the segment to be examined
	-- 4. @CallingReferenceGuid: 
	--		NULL: Output the results of the query directly
	--		NOT NULL: Save the results of the query in table erv.tblTempRecordVersioningField, using the @CallingReferenceGuid to tag the inserted records.

	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		IF ((@EntitySegmentTemplateGuid IS NULL) OR (@TargetSiteIndex IS NULL))
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END

		DECLARE @isTargetSiteASiteGroup bit
		SET @isTargetSiteASiteGroup = (SELECT SiteGroupFlag FROM tblSites WHERE SiteGuid = @TargetSiteIndex)
		IF (@isTargetSiteASiteGroup <> 1)
		BEGIN
			RAISERROR('Sitegroup required.',16,1); 
			RETURN;
		END


		DECLARE @tblResult TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);
	
		--A TargetField for a specific entity segment is determined as being VersionSpecific if there is a tblEntityRecordVersioningFieldConfig records for the
		--TargetField and entity segment combination that has an ForwardControlMode value of VersionSpecific.
		INSERT INTO @tblResult
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		SELECT TargetField, IsExternalAttribute, InternalFieldName, ForwardControlMode FROM erv.tblEntityRecordVersioningFieldConfig
		WHERE EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid
		AND (ISNULL(FilterValueGuid, @emptyGuid) = ISNULL(@FilterValueGuid, @emptyGuid))
		AND SiteGroupGuid = @TargetSiteIndex
		AND ForwardControlMode = 'VersionSpecific'

		IF (@CallingReferenceGuid IS NULL)
		BEGIN
			SELECT * FROM @tblResult
			ORDER BY TargetField
		END
		ELSE
		BEGIN
			INSERT INTO erv.tblTempRecordVersioningField
			(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode, _CallingReferenceGuid)
			SELECT TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode, @CallingReferenceGuid
			FROM @tblResult
		END	

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
						+ 'Procedure Name: [erv].usp_GetVersionSpecificFieldsBySegment' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
