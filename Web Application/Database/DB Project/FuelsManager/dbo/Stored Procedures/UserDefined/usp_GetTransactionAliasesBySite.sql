

/*
	EXEC [dbo].[usp_GetTransactionAliasesBySite] '00000000-0000-0000-0000-000000000001'
	EXEC [dbo].[usp_GetTransactionAliasesBySite] 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'

*/



CREATE PROCEDURE [dbo].[usp_GetTransactionAliasesBySite]
(
	@TargetSiteGuid uniqueidentifier
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetTransactionAliasesBySite] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieve the Transaction Alias records that have been assigned to a given Site/SiteGroup.
	-- Notes:
	-- 1. @TargetSiteGuid: Limit results to TransactionAliases that have been assigned to this site/sitegroup only
	-- 3. This stored procedure replaces the LRTransactionAliasListDO.GetAliasAssignmentsSelectSQL inline SQL.
	-- 4. The query examines both child record versions that are owned by the Target Site/SiteGroup (RecordVersioning ON), and record versions 
	--    that are not owned by the Target Site/SiteGroup, but that have been assigned to the Target Site/SiteGroup (RecordVersioning OFF).
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT b.*, c.AliasName AssociatedAlias,
		g.SiteGuid AssignedToSiteGuid, g.AssignedFromSiteGuid, h.Id AssignedFromSiteId   
		FROM [erv].[udf_GetTransactionAliasRecordVersions](@TargetSiteGuid) a
		INNER JOIN tblTransactionAliases b
		ON b.TransactionAliasGuid = a.TransactionAliasGuid
		LEFT OUTER JOIN tblTransactionAliases c
		ON c.TransactionAliasGuid = b.AssociatedTransactionAliasGuid
		INNER JOIN map.tblEntityTransactionAliasToSite g ON g.TransactionAliasGuid = b._MasterRecordGuid
        INNER JOIN tblSites h ON h.SiteGuid = g.AssignedFromSiteGuid 
        WHERE g.SiteGuid = @TargetSiteGuid

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
						+ 'Procedure Name: [dbo].usp_GetTransactionAliasesBySite' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END