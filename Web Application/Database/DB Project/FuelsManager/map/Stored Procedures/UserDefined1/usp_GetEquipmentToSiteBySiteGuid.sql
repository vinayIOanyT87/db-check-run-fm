/*
	DROP PROCEDURE [map].[usp_GetEquipmentToSiteBySiteGuid]

	EXEC [map].[usp_GetEquipmentToSiteBySiteGuid] '00000000-0000-0000-0000-000000000001'

	EXEC [map].[usp_GetEquipmentToSiteBySiteGuid] '8E1934C4-63DF-4E79-992F-8A5764B3ADDB'

	EXEC [map].[usp_GetEquipmentToSiteBySiteGuid] '8E1934C4-63DF-4E79-992F-8A5764B3ADDB', 0

	EXEC [map].[usp_GetEquipmentToSiteBySiteGuid] '8E1934C4-63DF-4E79-992F-8A5764B3ADDB', 1

*/
CREATE PROCEDURE [map].[usp_GetEquipmentToSiteBySiteGuid]
(
	@AssignedToSiteGuid uniqueidentifier,
	@ExcludeCompartments bit = 0
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetEquipmentToSiteBySiteGuid]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the EquipmentToSite assignments to a given site/sitegroup. 	
	-- Notes:
	-- 1. @AssignedToSiteGuid: Guid of the AssignedTo site/sitegroup from which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	-- 2. @ExcludeCompartments: 0: Cover Equipments of all Equipment Types, including Compartments. 1: Exclude Compartments from the query. 
	-- 3. This operation assumes that an entity record can only have one assignment mapping entry to a given site/sitegroup.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedToSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
	
		SELECT a.EquipmentToSiteGuid MappingGuid, a.EquipmentGuid EntityRecordGuid, d.ID EntityId, a.SiteGuid AssignedToSiteGuid, b.Id AssignedToSiteId, a.CreatedDate, a.CreatedBy, a.UpdatedDate, a.UpdatedBy, a._RowVersion, a.AssignedFromSiteGuid , c.ID AssignedFromSiteId 
		FROM map.tblEntityEquipmentToSite a
		INNER JOIN tblSites b
		ON b.SiteGuid = a.SiteGuid
		INNER JOIN tblSites c
		ON c.SiteGuid = a.AssignedFromSiteGuid
		INNER JOIN tblEquipment d
		ON d._MasterRecordGuid = a.EquipmentGuid
		AND d.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', a.EquipmentGuid, a.SiteGuid)
		WHERE a.SiteGuid = @AssignedToSiteGuid
		AND 
		(
			(@ExcludeCompartments = 0)
			OR
			((@ExcludeCompartments = 1) AND (d.ParentEquipmentGuid IS NULL))
		)
		ORDER BY EntityId

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
						+ 'Procedure Name: map.usp_GetEquipmentToSiteBySiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
