

/*
	EXEC [dbo].[usp_GetEquipmentsById] NULL, NULL
	EXEC [dbo].[usp_GetEquipmentsById] '00000000-0000-0000-0000-000000000001', NULL
	EXEC [dbo].[usp_GetEquipmentsById] NULL, 'HBEquipment1'
	EXEC [dbo].[usp_GetEquipmentsById] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602', NULL
	EXEC [dbo].[usp_GetEquipmentsById] 'B7BD440B-674F-46F6-977A-CEFC540B1A90', NULL
	EXEC [dbo].[usp_GetEquipmentsById] 'B7BD440B-674F-46F6-977A-CEFC540B1A90', 'HBEquipment1'
	
*/

CREATE PROCEDURE [dbo].[usp_GetEquipmentsById]
(
	@TargetSiteGuid uniqueidentifier, @EquipmentId nvarchar(30)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsById] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Equipment records that have a given Equipment Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @EquipmentId: Limit the results to Equipments that have an ID value that correspond to the @EquipmentId value
	-- 3. This stored procedure replaces the EquipmentClass.SelectByIdSql inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.Id, a.MasterRecordGuid, a.EquipmentGuid, e.EqTypeName, a.AssignedFromSiteGuid, c.Id AssignedFromSiteId, a.AssignedToSiteGuid, d.Id AssignedToSiteId 
		FROM [erv].[udf_GetEquipmentRecordVersionsById](@TargetSiteGuid, @EquipmentId) a
		INNER JOIN tblEquipment b 
		ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblSites d
		ON d.SiteGuid = a.AssignedToSiteGuid
		LEFT OUTER JOIN tblEquipmentTypes e
		ON e.EquipmentTypeGuid = b.EquipmentTypeGuid	
		WHERE ((b.ID = @EquipmentId) OR (@EquipmentId IS NULL))	

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsById' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END