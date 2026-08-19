
/*
=============================================
Author: Ryan Hill
Create date: 11/15/2013
Description:
	Get all the transaction aliases mapped to any of the user's groups for the site specified along with the type of right for the alias (view or modify).
=============================================
*/
CREATE PROCEDURE [dbo].[usp_TransactionAliasSelectByUserAndSite]
(
	@UserGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	BEGIN TRY

		-- Keep in mind that user to group mapping is site specific.
		-- We do not need to check if the user or group is assigned to the site specified - the mapping in map.tblUserToGroup is all we need to check
		-- Those mappings are deleted if the user or group is unassigned from the site.
		SELECT tblTransactionAliases.AliasName, 
			tblTransactionAliases.LookupTransTypeIndex, 
			map.tblGroupToTransactionAlias.LookupRightIndex --View or Modify
		FROM map.tblGroupToTransactionAlias 
		INNER JOIN tblTransactionAliases ON map.tblGroupToTransactionAlias.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid
		WHERE tblTransactionAliases.TransactionAliasGuid IN (SELECT TransactionAliasGuid FROM [erv].[udf_GetTransactionAliasRecordVersions](@SiteGuid))
		AND EXISTS (SELECT * FROM tblGroups 
			INNER JOIN map.tblUserToGroup ON map.tblUserToGroup.GroupGuid = tblGroups.GroupGuid
			WHERE map.tblUserToGroup.SiteGuid = @SiteGuid AND map.tblUserToGroup.UserGuid = @UserGuid
			AND tblGroups.GroupGuid = map.tblGroupToTransactionAlias.GroupGuid)

	END TRY
	BEGIN CATCH
		DECLARE @ErrMessage NVARCHAR(2048)
			,	@ErrNumber INT
			,	@ErrProcName NVARCHAR(126)
			,	@LineNumber INT
		
		SET @ErrMessage = ERROR_MESSAGE()
		SET	@ErrNumber = ERROR_NUMBER()
		SET @ErrProcName= ERROR_PROCEDURE()
		SET @LineNumber = ERROR_LINE()
		
		SET @ErrMessage =		'Error: ' + @ErrMessage + CHAR(13)+CHAR(10)
							+	'Number: ' + CAST(@ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10) 
							+	'Procedure Name: ' + ISNULL(@ErrProcName,OBJECT_NAME(@@PROCID)) + CHAR(13)+CHAR(10) 
							+	'Line Number: ' + ISNULL(CAST(@LineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10)
							 
		RAISERROR(@ErrMessage,16,1)
	END CATCH
END
