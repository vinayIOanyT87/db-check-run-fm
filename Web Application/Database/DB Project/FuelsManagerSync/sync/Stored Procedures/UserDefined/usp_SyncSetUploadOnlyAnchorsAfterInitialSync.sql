CREATE PROCEDURE [sync].[usp_SyncSetUploadOnlyAnchorsAfterInitialSync]
(@IdentityGuid uniqueidentifier = NULL, @ID nvarchar(80) =  NULL)
AS
BEGIN
	DECLARE @NewSyncAnchor binary(8)
	SET @NewSyncAnchor = CONVERT(binary(8), CONVERT(bigint, MIN_ACTIVE_ROWVERSION() - 1))

	UPDATE [sync].[tblSyncAnchor] 
		SET LastSentAnchor1 = @NewSyncAnchor, LastSentAnchor2 = @NewSyncAnchor
	WHERE SyncAnchorGuid IN (SELECT SyncAnchorGuid
								FROM [sync].[tblSyncAnchor] sa
									INNER JOIN [sync].[tblSyncTable] st
										ON sa.[TableName] = st.[TableName]
									INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
										ON st.[SyncTableGuid] = sttsm.[SyncTableGuid]
								WHERE sttsm.[SyncDirection] = 1 -- Microsoft SyncFramework value for Upload Only.
							)
END
