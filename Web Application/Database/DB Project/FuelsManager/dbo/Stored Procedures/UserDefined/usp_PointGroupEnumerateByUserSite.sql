CREATE PROCEDURE [dbo].[usp_PointGroupEnumerateByUserSite]
(
	@userGuid uniqueidentifier,
	@siteGuid uniqueidentifier
)
	AS
	BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_PointGroupEnumerateByUserSite] 
	-- Author: Francisco Martin
	-- Version/Date: 1.0
	-- Purpose: Retrieve a list of Point Group by Guis for the specified user and site
	-- Notes:
	-- 1. @PointGroupGuid: @PointGroupGuid is the Guid of the Point Group to retrieve
	-- 2. @OwnerUserGuid: Owner record version that needs to be retrieved.
	-- 2. @TargetSiteGuid: Target owner site of the record version that needs to be retrieved.
	------------------------------------------------------------------------------------------------------
	BEGIN TRY	

		SELECT pg.PointGroupGuid,
		pg.ID,
		pg.[Description],
		pg.PointGroupType,
		pg.OwnerUserGuid,
		pg.SiteGuid
 		FROM tblPointGroup pg
		WHERE pg.SiteGuid = @siteGuid
		AND (( pg.OwnerUserGuid = @userGuid
		OR pg.PointGroupType = 0 -- public
		OR pg.PointGroupType = 2) -- shared
		OR
		( pg.PointGroupType = 1 AND EXISTS(SELECT TOP 1 1 FROM map.tblUserToGroup m 
					JOIN map.tblUserToGroup ug ON m.groupguid=ug.groupguid 
					JOIN map.tblGroupToRight gr ON ug.groupguid=gr.groupguid 
					JOIN lookup.tblRight r ON gr.LookupRightIndex=r.rightindex
					WHERE ug.UserGuid = @userGuid AND r.RightCode='OPERATE_ADMINISTER_POINT_GROUP'))
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
						+ 'Procedure Name: [dbo].[usp_PointGroupEnumerateByUserSite]' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
	
END