

CREATE PROCEDURE [dbo].[usp_VersionUpdate](
	@IdentityGuid uniqueidentifier = NULL
    ,@SyncCompletedFlag bit
    ,@RowVersionSnapshot varbinary(8)
	,@UpdatedBy nvarchar(100)
)
AS
BEGIN
    UPDATE [dbo].[tblVersion]
		SET SyncCompletedFlag = @SyncCompletedFlag
			,RowVersionSnapshot = @RowVersionSnapshot
			,UpdatedDate = SYSDATETIME()
			,UpdatedBy = @UpdatedBy
		WHERE [dbo].[tblVersion].[VersionGuid] = @IdentityGuid

	RETURN;
END
