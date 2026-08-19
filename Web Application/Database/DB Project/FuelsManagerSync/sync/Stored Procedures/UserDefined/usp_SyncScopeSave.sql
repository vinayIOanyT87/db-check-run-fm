
CREATE PROCEDURE [sync].[usp_SyncScopeSave](
	@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80)
	,@ScopeTypeIndex bigint
	,@FriendlyName nvarchar(100)
	,@LongDescription nvarchar(1024)
	,@SyncProfileGuid uniqueidentifier
	,@SyncOrder int = NULL
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@SyncSinglePass bit
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;
	
	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;

	DECLARE @localSyncOrder int
	SET @localSyncOrder = @SyncOrder
	
	-- If the order wasn't provided, calculate it.	
	IF (@localSyncOrder IS NULL)
	BEGIN
		-- Get the last item in the specified Synchronization Group
		SELECT @localSyncOrder = MAX(SyncOrder)
			FROM [sync].[tblSyncScope] WITH(NOLOCK)
				WHERE SyncProfileGuid = @SyncProfileGuid

		IF (@localSyncOrder IS NULL)
			SET @localSyncOrder = 1;				
		ELSE
			SET @localSyncOrder = @localSyncOrder + 1;
	END

    ; MERGE [sync].[tblSyncScope] AS existing
    USING (SELECT @NewRowGuid
					,@ID
					,@ScopeTypeIndex
					,@FriendlyName
					,@LongDescription
					,@SyncProfileGuid
					,@SyncOrder
					,@CreatedBy
					,@UpdatedBy
					,@SyncSinglePass
            ) AS updates (SyncScopeGuid
							,ID
							,SyncScopeTypeIndex
							,FriendlyName
							,LongDescription
							,SyncProfileGuid
							,SyncOrder
							,CreatedBy
							,UpdatedBy
							,SyncSinglePass)
    ON (existing.SyncScopeGuid = updates.SyncScopeGuid)
    WHEN Matched
    THEN
        UPDATE SET ID = updates.ID
					,SyncScopeTypeIndex = updates.SyncScopeTypeIndex
					,FriendlyName = updates.FriendlyName
					,LongDescription = updates.LongDescription
					,SyncProfileGuid = updates.SyncProfileGuid
					,SyncOrder = updates.SyncOrder
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
					,SyncSinglePass = updates.SyncSinglePass
    WHEN Not Matched
    THEN
        INSERT (SyncScopeGuid
				,ID
				,SyncScopeTypeIndex
				,FriendlyName
				,LongDescription
				,SyncProfileGuid
				,SyncOrder
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy
				,SyncSinglePass)
            VALUES (@NewRowGuid
					,@ID
					,@ScopeTypeIndex
					,@FriendlyName
					,@LongDescription
					,@SyncProfileGuid
					,@localSyncOrder
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy
					,@SyncSinglePass)
    ;
	
	RETURN;
	
END