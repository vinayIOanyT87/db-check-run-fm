

/*
	EXEC [dbo].[usp_GetEquipmentsByCompany] NULL, NULL, NULL
	EXEC [dbo].[usp_GetEquipmentsByCompany] NULL, '70A85585-4EEB-4C5B-AA40-B5240214F9D3', NULL
	EXEC [dbo].[usp_GetEquipmentsByCompany] NULL, NULL, ''	
	EXEC [dbo].[usp_GetEquipmentsByCompany] 'f4761a16-ab2f-41ee-b6fa-d17658df2602', '70a85585-4eeb-4c5b-aa40-b5240214f9d3', 'ZZZ02'	
*/


CREATE PROCEDURE [dbo].[usp_GetEquipmentsByCompany]
(
	@TargetSiteGuid uniqueidentifier, @CompanyGuid uniqueidentifier, @CompanyEquipmentId nvarchar(30)
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [map].[usp_GetEquipmentsByCompany] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Equipment records that have a given set of Company parameters and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Equipments that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: Limit the results to Equipments that have a CompanyGuid value that correspond to the @CompanyGuid value
	-- 3. @CompanyId: Limit the results to Equipments that have a CompanyEquipmentID value that correspond to the @CompanyId value
	-- 4. This stored procedure replaces the EquipmentClass.SelectByCompanyGuidAndEquipmentIdSQL inline SQL and the EquipmentClass.EnumerateByCompanyGetIDTypeOnlySQL inline SQL.
	-- 5. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioing ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.Id, a.MasterRecordGuid, a.EquipmentGuid, b.EquipmentTypeGuid, e.LookupEquipmentTypeIndex, e.EqTypeName, a.AssignedFromSiteGuid, c.Id AssignedFromSiteId, a.AssignedToSiteGuid, d.Id AssignedToSiteId 
		FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblEquipment b 
		ON b.EquipmentGuid = a.EquipmentGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblSites d
		ON d.SiteGuid = a.AssignedToSiteGuid
		LEFT OUTER JOIN tblEquipmentTypes e
		ON e.EquipmentTypeGuid = b.EquipmentTypeGuid	
		WHERE ((b.CompanyGuid = @CompanyGuid) OR (@CompanyGuid IS NULL))
		AND ((b.CompanyEquipmentID = @CompanyEquipmentId) OR (@CompanyEquipmentId IS NULL))	

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
						+ 'Procedure Name: [dbo].usp_GetEquipmentsByCompany' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END