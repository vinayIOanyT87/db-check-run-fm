

/*
	EXEC [dbo].[usp_GetCompanyByGuid] '00000000-0000-0000-0000-000000000001', '012D8DD3-E6FA-4B78-A81A-C84F1C360558'

*/



CREATE PROCEDURE [dbo].[usp_GetCompanyByGuid]
(
	@TargetSiteGuid uniqueidentifier, @CompanyGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetCompanyByGuid] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Company record that have a given Company Guid and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Companies that have been assigned to this site/sitegroup only
	-- 2. @CompanyGuid: If @TargetSiteGuid is null, then @CompanyGuid is the Guid of the Company to retrieve. Otherwise, it is the Guid that is used to retrieve the MasterRecordGuid of the Company record to retrieve.
	-- 3. This stored procedure replaces the CompanyClass.SelectSQL inline SQL for the case where the bInTransaction is false.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		DECLARE @masterRecordGuid uniqueidentifier

		SELECT @masterRecordGuid = _MasterRecordGuid FROM tblCompanies
		WHERE CompanyGuid = @CompanyGuid
		
		DECLARE @targetRecordGuid uniqueidentifier
		SET @targetRecordGuid = NULL
		IF (@TargetSiteGuid IS NOT NULL)
		BEGIN
			SELECT @targetRecordGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Company', @masterRecordGuid, @TargetSiteGuid)
		END
		ELSE
		BEGIN
			SET @targetRecordGuid = @CompanyGuid
		END

		SELECT a.*, b.IATAID AS IATAID, c.ID AS ShipperTypeID, d.ID AS CustomerBillToTypeID, e.ID AS CustomerShipToTypeID    
		FROM tblCompanies a
		LEFT OUTER JOIN tblIATA b
		ON b.IATAGuid = a.IATAGuid
		LEFT OUTER JOIN tblApplicationString c
		ON c.ApplicationStringGuid = a.ShipperTypeApplicationStringGuid
		LEFT OUTER JOIN tblApplicationString d
		ON d.ApplicationStringGuid = a.CustomerBillToTypeApplicationStringGuid
		LEFT OUTER JOIN tblApplicationString e
		ON e.ApplicationStringGuid = a.CustomerShipToTypeApplicationStringGuid
		WHERE a.CompanyGuid = @targetRecordGuid

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
						+ 'Procedure Name: [dbo].usp_GetCompanyByGuid' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END