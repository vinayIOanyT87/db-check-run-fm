/*
	DROP PROCEDURE [erv].[usp_AddGlobalSpecificQueueRecord]

	[erv].[usp_AddGlobalSpecificQueueRecord] 'Product', '8A970C48-1B04-4DFB-83FD-01D734C84199', 'HB'
*/

CREATE PROCEDURE [erv].[usp_AddGlobalSpecificQueueRecord]
(
	@EntityTypeId nvarchar(100), @EntityGuid uniqueidentifier, @UserId nvarchar(100)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [erv].[usp_AddGlobalSpecificQueueRecord] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Add a record to the erv.tblGlobalSpecificChangesQueue to queue a request for the Record Versioning propagation of the 
	--          GlobalSpecific field values of a child record version.
	-- Notes:
	-- 1. @EntityTypeId: Entity Type Id as captured in the Entity Segment Template (erv.tblEntitySegmentTemplate)
	-- 2. @EntityGuid: Record Guid of the child record version
	-- 3. @UserId: Id of the user that needs to be tied to the changes
	-- 4. Only changes made to a child record version are queued for GlobalSpecific change propagation.
	-- 5. Only changes made to an entity record that has GlobalSpecific fields are queued for GlobalSpecific change propagation.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		DECLARE @assignedFromSiteGuid uniqueidentifier
		DECLARE @isChildRecord int
		DECLARE @callingRef1Guid uniqueidentifier
		DECLARE @createdDate datetimeoffset(7)
		DECLARE @entityMasterRecGuid uniqueidentifier
		DECLARE @ownerSiteGuid uniqueidentifier
		DECLARE @ervEntityTypeId nvarchar(100)

		SET @createdDate = SYSDATETIMEOFFSET()
		SET @UserId = ISNULL(@UserId,SUSER_SNAME())

		--Try convert the EntityTypeId to the one listed in the EntitySegmentTemplate. The FuelsManager App uses the descriptive name of the entity types for their ids, and is likely to use those names as the entity type ids when calling this procedure.
		SELECT @ervEntityTypeId = EntityTypeId FROM erv.tblEntitySegmentTemplate WHERE EntityTypeDisplayName = @EntityTypeId
		SET @ervEntityTypeId =  ISNULL(@ervEntityTypeId, @EntityTypeId)

		SET @isChildRecord = 0				
		IF (@ervEntityTypeId = 'Equipment')
		BEGIN
			SELECT @entityMasterRecGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid 
			FROM dbo.tblEquipment WHERE EquipmentGuid = @EntityGuid AND EquipmentGuid <> _MasterRecordGuid

			SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityEquipmentToSite 
			WHERE EquipmentGuid = @entityMasterRecGuid
			AND SiteGuid = @ownerSiteGuid
		END
		ELSE IF (@ervEntityTypeId = 'Product')
		BEGIN
			SELECT @entityMasterRecGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid 
			FROM dbo.tblProducts WHERE ProductGuid = @EntityGuid AND ProductGuid <> _MasterRecordGuid

			SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityProductToSite 
			WHERE ProductGuid = @entityMasterRecGuid
			AND SiteGuid = @ownerSiteGuid
		END
		ELSE IF (@ervEntityTypeId = 'Company')
		BEGIN
			SELECT @entityMasterRecGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid 
			FROM dbo.tblCompanies WHERE CompanyGuid = @EntityGuid AND CompanyGuid <> _MasterRecordGuid

			SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityCompanyToSite 
			WHERE CompanyGuid = @entityMasterRecGuid
			AND SiteGuid = @ownerSiteGuid
		END
		ELSE IF (@ervEntityTypeId = 'Transaction_Alias')
		BEGIN
			SELECT @entityMasterRecGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid 
			FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @EntityGuid AND TransactionAliasGuid <> _MasterRecordGuid

			SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityTransactionAliasToSite 
			WHERE TransactionAliasGuid = @entityMasterRecGuid
			AND SiteGuid = @ownerSiteGuid
		END
		ELSE IF (@ervEntityTypeId = 'Personnel')
		BEGIN
			SELECT @entityMasterRecGuid = _MasterRecordGuid, @ownerSiteGuid = SiteGuid 
			FROM dbo.tblPersonnel WHERE PersonnelGuid = @EntityGuid AND PersonnelGuid <> _MasterRecordGuid

			SELECT @assignedFromSiteGuid = AssignedFromSiteGuid FROM map.tblEntityPersonnelToSite 
			WHERE PersonnelGuid = @entityMasterRecGuid
			AND SiteGuid = @ownerSiteGuid
		END

		IF (@entityMasterRecGuid IS NULL)
		BEGIN
			RETURN;		-- Record is not a child record version or cannot be located
		END
	
		SET @callingRef1Guid = NEWID()
		EXEC erv.usp_GetRecordVersioningFields @ervEntityTypeId, @entityMasterRecGuid, @assignedFromSiteGuid, 'GlobalSpecific', @callingRef1Guid 
		IF (NOT EXISTS (SELECT * FROM erv.tblTempRecordVersioningField WHERE _CallingReferenceGuid = @callingRef1Guid))
		BEGIN							
			RETURN; 	-- No GlobalSpecific fields to propagate.
		END
		DELETE erv.tblTempRecordVersioningField
		WHERE _CallingReferenceGuid = @callingRef1Guid

		--Use the supplied EntityTypeId (instead of the erv EntityTypeId) to identify the entity type, since the queue is to be consumed by the FuelsManager App and not directly by a database object.
		INSERT INTO erv.tblGlobalSpecificChangesQueue
		(EntityTypeId, EntityGuid, MasterRecordGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
		VALUES
		(@EntityTypeId, @EntityGuid, @entityMasterRecGuid, @ownerSiteGuid, @createdDate, @UserId, @createdDate, @UserId)

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
						+ 'Procedure Name: [erv].usp_AddGlobalSpecificQueueRecord' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
