

/*
	EXEC [dbo].[usp_GetEquipmentByGuid] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', NULL
	EXEC [dbo].[usp_GetEquipmentByGuid] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [dbo].[usp_GetEquipmentByGuid] '1BB8C558-5277-47A5-90AE-2461BBD1EFF7', 'B7BD440B-674F-46F6-977A-CEFC540B1A90'
	EXEC [dbo].[usp_GetEquipmentByGuid] 'C1078CE3-EC80-4CB7-81C3-0D4FA0D10215', '92E8D5FC-21FD-4560-BE57-03A8BC0CF480'
	EXEC [dbo].[usp_GetEquipmentByGuid] 'b44649ad-877a-4a41-93b1-9b0e048be377', '23a3f8fc-0d49-43bc-b20b-04ceda6a4346'
	EXEC [dbo].[usp_GetEquipmentByGuid] 'b44649ad-877a-4a41-93b1-9b0e048be377', '46426312-e408-4af8-85fd-338b622b32bf'							
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentByGuid]
(
	@EquipmentGuid uniqueidentifier, @TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve an Equipment record by Guid.
	-- Notes:
	-- 1. @EquipmentGuid: If @TargetSiteGuid is null, then @EquipmentGuid is the Guid of the Equipment to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the Equipment record to retrieve.
	-- 2. @TargetSiteGuid: If TargetSiteGuid is not null, then it is used as the target owner site of the record version that needs to be retrieved.
	-- 3. This query can be used in two modes: 
	--		(a) When the exact GUID of the target Equipment record is known, in which case the @TargetSiteGuid can be left null.
	--		(b) When trying to verify if an equipment record has a record version (child or parent) against a specific site/sitegroup, in which case the @TargetSiteGuid must be provided.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblEquipment
		WHERE EquipmentGuid = @EquipmentGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @EquipmentGuid
		END

		SELECT * FROM tblEquipment
		WHERE EquipmentGuid = @targetRecordGuid

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END