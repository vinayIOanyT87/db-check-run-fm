

/*
	EXEC [map].[usp_GetCandidateChildrenSites] '46426312-e408-4af8-85fd-338b622b32bf'
	EXEC [map].[usp_GetCandidateChildrenSites] 'AEBA18E3-E97B-479E-8B2D-0BCD69C1C421'

*/

CREATE PROCEDURE [map].[usp_GetCandidateChildrenSites]
(
	@SiteGroupGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [map].[usp_GetCandidateChildrenSites]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Retrieves a list of sites and sitegroups that could be added as children sites/sitegroups to a given sitegroup. 	
	-- Notes:
	-- 1. @SiteGroupGuid: Guid of the SiteGroup for which the potential children sites/sitegroups are to be retrieved.
	-- 5. This operation retrieves all the sites and sitegroups in the system, and exclude:
	--	  (i)  those sites/sitegroups that are already mapped as children to the target sitegroup
	--	  (ii) those sitegroups that are already a parent (directly or indirectly) to the target sitegroup
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY

		SELECT a.*, b.InventoryTransactionAliasGuid, b.AdjustmentTransactionAliasGuid, b.IATAGuid,b.NoteGuid, 
		d.AliasName AS InventoryTransactionAliasID, e.AliasName AS AdjustmentTransactionAliasID, f.IATAID AS IATAID  
		FROM tblSites a
		INNER JOIN tblSitesAncillaryData b ON a.SiteGuid = b.SiteGuid 
		LEFT OUTER JOIN tblTransactionAliases d ON d.TransactionAliasGuid = b.InventoryTransactionAliasGuid 
		LEFT OUTER JOIN tblTransactionAliases e ON e.TransactionAliasGuid = b.AdjustmentTransactionAliasGuid 
		LEFT OUTER JOIN tblIATA f ON f.IATAGuid = b.IATAGuid 
		WHERE @SiteGroupGuid IN (SELECT SiteGuid FROM tblSites WHERE SiteGroupFlag = 1)
		AND erv.udf_IsASiteParent(@SiteGroupGuid, a.SiteGuid) = 0

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
						+ 'Procedure Name: map.usp_GetCandidateChildrenSites' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END     
