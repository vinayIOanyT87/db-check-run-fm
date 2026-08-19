CREATE PROCEDURE [dbo].[usp_MovementSummaryEnumerateByUserSite]
(
	@userGuid uniqueidentifier,
	@siteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_MovementSummaryEnumerateByUserSite] 
	-- Author: Francisco Martin
	-- Version/Date: 1.1
	-- Purpose: Retrieve a list of Movement Summary for the specified user and site
	-- Notes:
	-- 1. @UserGuid: Owner record version that needs to be retrieved.
	-- 2. @SiteGuid: Target owner site of the record version that needs to be retrieved.
	-- Last Updated: 09-30-2025
	-- By: Srini
	-- Updated to include _RowVersion in the result set
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT ms.MovementSummaryGuid,
		ms.ID,
		ms.[Description],
		ms.MovementSummaryType,
		'' as ColumnsDefinition,
		ms.FontSize,
		'' as RowsDefinition,
		ms.OwnerUserGuid,
		ms.SiteGuid,
		ms._RowVersion
 		FROM tblMovementSummary ms
		WHERE ms.SiteGuid = @siteGuid
		AND (( ms.OwnerUserGuid = @userGuid
		OR ms.MovementSummaryType = 0 -- public
		OR ms.MovementSummaryType = 2) -- shared
		OR
		(ms.MovementSummaryType = 1 AND EXISTS(SELECT TOP 1 1 FROM map.tblUserToGroup m 
					JOIN map.tblUserToGroup ug ON m.groupguid=ug.groupguid 
					JOIN map.tblGroupToRight gr ON ug.groupguid=gr.groupguid 
					JOIN lookup.tblRight r ON gr.LookupRightIndex=r.rightindex
					WHERE ug.UserGuid = @userGuid AND r.RightCode='OPERATE_ADMINISTER_MOVEMENT_SUMMARY') )-- private
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
						+ 'Procedure Name: [dbo].[usp_MovementSummaryEnumerateByUserSite]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END