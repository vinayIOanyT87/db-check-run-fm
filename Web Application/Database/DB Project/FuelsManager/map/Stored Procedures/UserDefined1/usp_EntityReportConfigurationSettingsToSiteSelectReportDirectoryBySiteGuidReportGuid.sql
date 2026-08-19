CREATE PROCEDURE [map].[usp_EntityReportConfigurationSettingsToSiteSelectReportDirectoryBySiteGuidReportGuid] (
	@CurrentSite UNIQUEIDENTIFIER,
	@ReportGuid UNIQUEIDENTIFIER
) AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_EntityReportConfigurationSettingsToSiteSelectReportDirectoryBySiteGuidReportGuid]
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.001 / 2012-08-31 
	-- Purpose: Select  ReportDirectory for a given site and report guid for all owned and assigned reports to a Site
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
		Select TOP(1) 
		(
			SELECT tblSites.ReportDirectory 
			FROM tblSites
			WITH (NOLOCK)
			INNER JOIN dbo.tblReportDetails 
			ON dbo.tblReportDetails.SiteGuid = tblSites.SiteGuid
			INNER JOIN map.tblEntityReportConfigurationSettingsToSite 
			ON dbo.tblReportDetails.SiteGuid = map.tblEntityReportConfigurationSettingsToSite.SiteGuid
			WHERE map.tblEntityReportConfigurationSettingsToSite.MapToSiteGuid = @CurrentSite AND @ReportGuid = dbo.tblReportDetails.ReportDetailGuid
			UNION
			SELECT tblSites.ReportDirectory 
			FROM tblSites
			WITH (NOLOCK)
			INNER JOIN dbo.tblReportDetails 
			ON dbo.tblReportDetails.SiteGuid = tblSites.SiteGuid
			WHERE dbo.tblReportDetails.SiteGuid = @CurrentSite AND @ReportGuid = dbo.tblReportDetails.ReportDetailGuid
		)
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
						+ 'Procedure Name: [map].[usp_EntityReportConfigurationSettingsToSiteSelectReportDirectoryBySiteGuidReportGuid]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END