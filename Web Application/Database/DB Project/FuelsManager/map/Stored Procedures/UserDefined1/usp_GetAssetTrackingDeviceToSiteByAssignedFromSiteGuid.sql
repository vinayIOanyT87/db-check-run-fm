CREATE PROCEDURE [map].[usp_GetAssetTrackingDeviceToSiteByAssignedFromSiteGuid]
(
	@AssignedFromSiteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetAssetTrackingDeviceToSiteByAssignedFromSiteGuid]
	-- Author: Richard R. Panachida
	-- Version/Date: 1.0.001 / 2016-04-26 14:21:10.4470770 -04:00
	-- Purpose: Retrieves the AssetTrackingDeviceToSite assignments from a given sitegroup. 	
	-- Notes:
	-- 1. @AssignedFromSiteGuid: Guid of the AssignedFrom sitegroup for which the mapping tree search is to start. If not provided, the owner sitegroup of the master record is used for the @AssignedToSiteGuid.
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		IF (@AssignedFromSiteGuid IS NULL)
		BEGIN
			RAISERROR('Invalid parameter value.',16,1); 
			RETURN;
		END
	
		SELECT a.AssetTrackingDeviceToSiteGuid MappingGuid, 
			   a.AssetTrackingDeviceGuid EntityRecordGuid, 
			   d.DeviceID EntityId, 
			   a.SiteGuid AssignedToSiteGuid, 
			   b.Id AssignedToSiteId, 
			   a.CreatedDate, 
			   a.CreatedBy, 
			   a.UpdatedDate, 
			   a.UpdatedBy, 
			   a._RowVersion, 
			   a.AssignedFromSiteGuid , 
			   c.ID AssignedFromSiteId 
		FROM map.tblEntityAssetTrackingDeviceToSite a
			 INNER JOIN tblSites b ON b.SiteGuid = a.SiteGuid
			 INNER JOIN tblSites c ON c.SiteGuid = a.AssignedFromSiteGuid
			 INNER JOIN tblAssetTrackingDevice d ON d.AssetTrackingDeviceGuid = a.AssetTrackingDeviceGuid
		WHERE a.AssignedFromSiteGuid = @AssignedFromSiteGuid
			  AND a.SiteGuid <> @AssignedFromSiteGuid
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
						+ 'Procedure Name: map.usp_GetAssetTrackingDeviceToSiteByAssignedFromSiteGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END
