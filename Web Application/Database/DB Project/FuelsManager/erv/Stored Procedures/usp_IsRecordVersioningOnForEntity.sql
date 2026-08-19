/*
	DROP PROCEDURE [erv].[usp_IsRecordVersioningOnForEntity]

	DECLARE @result bit
	--EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Equipment', '3D923333-03F9-4805-8581-5C81CD90C14F', 'B7BD440B-674F-46F6-977A-CEFC540B1A90', @result OUTPUT
	--EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Equipment', '3D923333-03F9-4805-8581-5C81CD90C14F', 'B7BD440B-674F-46F6-977A-CEFC540B1A90', @result OUTPUT
	EXEC [erv].[usp_IsRecordVersioningOnForEntity] 'Product', '679AD57D-93B2-4C45-82D4-E5C4557EB487', 'AD74B677-F294-4BF8-8861-30D6B424ADC6', @result OUTPUT
	SELECT @result

*/

CREATE PROCEDURE [erv].[usp_IsRecordVersioningOnForEntity]
(
	@EntityTypeId nvarchar(100), @EntityMasterRecGuid uniqueidentifier, @AssignedFromSiteGroupGuid uniqueidentifier, @result bit OUTPUT
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [erv].[usp_IsRecordVersioningOnForEntity] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Determines whether Record Versioning is turned on for a given entity record, based on the Field Level Configuration settings for the parent sitegroup from where the 
	-- entity record has been assigned from.
	-- Notes: 
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityMasterRecGuid: master record Guid of the record
	-- 3. @AssignedFromSiteGroupGuid: SiteGroup from which the assignment was made.
	-- 4. This SP can be used:
	--		(i) During entity assignment to determine if RecordVersioning is turned on at the parent sitegroup for the entity record, in order to decide whether to create a new record 
	--			version or not. 
	--		(ii) When loading an entity record by its Master Record Guid, and having to decide which version of the record to load (parent or child version).
	------------------------------------------------------------------------------------------------------
/*	
	DECLARE @EntityTypeId nvarchar(100)
	DECLARE @EntityMasterRecGuid uniqueidentifier
	DECLARE @TargetSiteIndex uniqueidentifier
	SET @EntityTypeId = 'Equipment'
	SET @EntityMasterRecGuid = 'F5EA57B8-2CFB-4605-9B55-8850199671C7'
	SET @TargetSiteIndex = '0F7228B9-D8E4-41C8-A862-B71FB3F38763'
*/

	BEGIN TRY
		SET @result = 0


		DECLARE @tblSegmentInfo TABLE
		(
			FilterValueGuid uniqueidentifier NULL,
			EntitySegmentTemplateGuid uniqueidentifier NOT NULL
		);		
		--Fetch all the entity segments that apply to the record. This query will usually return a single record.
		--The only situation where the query can return more than one record is that there is more than one entity segment (i.e. more than one filter field) are defined for 
		--the entity type of the entity record.
		INSERT INTO @tblSegmentInfo
		(FilterValueGuid, EntitySegmentTemplateGuid)
		SELECT FilterValueGuid, EntitySegmentTemplateGuid
		FROM [erv].[udf_GetEntitySegmentsByEntityGuid] (@EntityTypeId, @EntityMasterRecGuid)		
		
		IF NOT EXISTS (SELECT * FROM @tblSegmentInfo)
		BEGIN
			SET @result = 0;
			RETURN;
		END

		--Fetch the basic information about the target entity record. 
		DECLARE @MasterRecordGuid uniqueidentifier
		DECLARE @OwnerSiteGuid uniqueidentifier
		EXEC [erv].[usp_GetEntityBasicInfo] @EntityTypeId, @EntityMasterRecGuid, @MasterRecordGuid OUTPUT,  @OwnerSiteGuid OUTPUT

		IF ((@MasterRecordGuid IS NULL) OR (@MasterRecordGuid <> @EntityMasterRecGuid) OR (@OwnerSiteGuid IS NULL))
		BEGIN
			SET @result = 0;
			RETURN;
		END

		--Verify if there are any VersionSpecific fields defined for the entity segment/s of the entity record, for the @AssignedFromSiteGroupGuid
		DECLARE @tblVersionSpecificFields TABLE
		(
			TargetField nvarchar(100) NOT NULL,
			IsExternalAttribute bit NULL,
			InternalFieldName nvarchar(100) NULL,
			FieldLevelControlMode nvarchar(20) NULL
		);
		INSERT @tblVersionSpecificFields
		(TargetField, IsExternalAttribute, InternalFieldName, FieldLevelControlMode)
		EXEC erv.usp_GetRecordVersioningFields @EntityTypeId, @EntityMasterRecGuid, @AssignedFromSiteGroupGuid, 'VersionSpecific'
				
		IF ((SELECT COUNT(*) FROM @tblVersionSpecificFields) > 0)
		BEGIN
			SET @result = 1
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
						+ 'Procedure Name: [erv].usp_IsRecordVersioningOnForEntity' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    

END
