

/*
	EXEC [dbo].[usp_GetParentEquipments] 'F4761A16-AB2F-41EE-B6FA-D17658DF2602'
	EXEC [dbo].[usp_GetParentEquipments] 'df5060d4-25e4-4f56-ae46-50c25331863e'
	
*/


CREATE PROCEDURE [dbo].[usp_GetParentEquipments]
(
	@TargetSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetParentEquipments] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve Parent Equipment records for a given target site/sitegroup
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. This stored procedure replaces the EquipmentClass.EnumerateInfoSQL inline SQL.
	-- 3. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	
		DECLARE @emptyGuid uniqueidentifier
		SET @emptyGuid = CAST(CAST(0 AS binary) AS uniqueidentifier)

		SELECT b.ID, b.Xref, b.SiteGuid, b.EquipmentGuid, b._MasterRecordGuid, c.SiteGuid AssignedToSiteGuid, c.AssignedFromSiteGuid, d.Id AssignedFromSiteId   
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN map.tblEntityEquipmentToSite c WITH (NOLOCK) ON c.EquipmentGuid = b._MasterRecordGuid
        INNER JOIN tblSites d WITH (NOLOCK) ON d.SiteGuid = c.AssignedFromSiteGuid 
        WHERE c.SiteGuid = @TargetSiteGuid
        AND (b.ParentEquipmentGuid = @emptyGuid OR b.ParentEquipmentGuid IS NULL)
        ORDER BY b.ID

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
						+ 'Procedure Name: [dbo].usp_GetParentEquipments' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END