

/*
	EXEC [dbo].[usp_GetCompaniesById] '00000000-0000-0000-0000-000000000001', 'HBCompany02'
	EXEC [dbo].[usp_GetCompaniesById] '00000000-0000-0000-0000-000000000001', NULL

*/



CREATE PROCEDURE [dbo].[usp_GetCompaniesById]
(
	@TargetSiteGuid uniqueidentifier, @Id nvarchar(100)
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetCompaniesById] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Company records that have a given Company Id and that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to Companies that have been assigned to this site/sitegroup only
	-- 2. @ID: Limit the results to Companies that have an ID value that correspond to the @Id value
	-- 3. This stored procedure replaces the CompanyClass.SelectByIDSQL inline SQL for the case where the bInTransaction is false.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.IATAID AS IATAID, d.ID AS ShipperTypeID, e.ID AS CustomerBillToTypeID, f.ID AS CustomerShipToTypeID,
		g.SiteGuid AssignedToSiteGuid, g.AssignedFromSiteGuid, h.Id AssignedFromSiteId     
		FROM [erv].[udf_GetCompanyRecordVersionsById](@TargetSiteGuid, @Id) a
		INNER JOIN tblCompanies b
		ON b.CompanyGuid = a.CompanyGuid
		LEFT OUTER JOIN tblIATA c
		ON c.IATAGuid = b.IATAGuid
		LEFT OUTER JOIN tblApplicationString d
		ON d.ApplicationStringGuid = b.ShipperTypeApplicationStringGuid
		LEFT OUTER JOIN tblApplicationString e
		ON e.ApplicationStringGuid = b.CustomerBillToTypeApplicationStringGuid
		LEFT OUTER JOIN tblApplicationString f
		ON f.ApplicationStringGuid = b.CustomerShipToTypeApplicationStringGuid
		INNER JOIN map.tblEntityCompanyToSite g ON g.CompanyGuid = b._MasterRecordGuid
        INNER JOIN tblSites h ON h.SiteGuid = g.AssignedFromSiteGuid 
        WHERE g.SiteGuid = @TargetSiteGuid
		AND ((b.ID = @Id) OR (@Id IS NULL))

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
						+ 'Procedure Name: [dbo].usp_GetCompaniesById' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END