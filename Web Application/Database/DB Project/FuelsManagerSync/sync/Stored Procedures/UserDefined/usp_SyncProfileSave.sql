
CREATE PROCEDURE [sync].[usp_SyncProfileSave](
	@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80)
	,@FriendlyName nvarchar(100)
	,@LongDescription nvarchar(1024)
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	SET @NewRowGuid = NULL;

	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;
	
    ;   
    MERGE [sync].[tblSyncProfile] AS existing
    USING (SELECT @NewRowGuid
					,@ID
					,@FriendlyName
					,@LongDescription
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncProfileGuid
							,ID
							,FriendlyName
							,LongDescription
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncProfileGuid = updates.SyncProfileGuid)
    WHEN Matched
    THEN
		UPDATE SET ID = updates.ID
					,FriendlyName = updates.FriendlyName
					,LongDescription = updates.LongDescription
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncProfileGuid
				,ID
				,FriendlyName
				,LongDescription				
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@ID
					,@FriendlyName
					,@LongDescription
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END