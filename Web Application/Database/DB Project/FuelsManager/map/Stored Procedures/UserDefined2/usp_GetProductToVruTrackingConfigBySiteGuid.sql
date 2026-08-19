/*
	DROP PROCEDURE [map].[usp_GetProductToVruTrackingConfigBySiteGuid]

	EXEC [map].[usp_GetProductToVruTrackingConfigBySiteGuid] 'd76222ca-2b16-4289-a987-554f895169c0'
	EXEC [map].[usp_GetProductToVruTrackingConfigBySiteGuid] '73D449F8-F3EE-4D4D-9933-5616C9287420'
	
*/
CREATE PROCEDURE [map].[usp_GetProductToVruTrackingConfigBySiteGuid]
(
	@TargetSiteGuid uniqueidentifier
)
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [map].[usp_GetProductToVruTrackingConfigBySiteGuid]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Get map.tblProductToVruTrackingConfig mappings for a given Site.
  -- Notes:
  -- 1. @TargetSiteGuid: Site/Sitegroup for which to retrieve the mappings.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    SELECT a.* , c.ProductID AS AssignedID, c.ProductCode AS AssignedCode, 
	c.Description AS AssignedDescription, c.LookupProductTypeIndex AS AssignedProductType, 
	c.LoadRackDisplayText AS AssignedLoadRackDisplayText, c.LockedOut AS LockedOut, 
	c.HazardousMaterial AS HazardousMaterial, c.LoadByWeight AS LoadByWeight, c.PIDXCode AS PIDXCode, 
	c.PIDXFamilyCode AS PIDXFamilyCode,  c.IsEthanol AS IsEthanol, c.ContaminationPromptLoadRackText AS ContaminationPromptLoadRackText, 
	d.ID AS AdditiveProfileID,  e.TankID AS TankID  
	FROM map.tblProductToVruTrackingConfig  a
	INNER JOIN [erv].[udf_GetProductRecordVersions](@TargetSiteGuid) b 
	ON b.MasterRecordGuid = a.ProductGuid  
	INNER JOIN tblProducts c
	ON c.ProductGuid = b.ProductGuid  
	LEFT JOIN tblAdditiveProfiles d
	ON d.AdditiveProfileGuid = a.AdditiveProfileGuid  
	LEFT JOIN dbo.tblTanks e
	ON e.TankGuid = a.TankGuid  
	WHERE a.AssignedToSiteGuid = @TargetSiteGuid

  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [map].[usp_GetProductToVruTrackingConfigBySiteGuid]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
