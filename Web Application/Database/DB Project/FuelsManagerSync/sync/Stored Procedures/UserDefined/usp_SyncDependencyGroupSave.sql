
CREATE PROCEDURE [sync].[usp_SyncDependencyGroupSave](
	@IdentityGuid uniqueidentifier = NULL
	,@ID nvarchar(80)
	,@FriendlyName nvarchar(100)
	,@LongDescription nvarchar(1024)
	,@DependencyLevel int
	,@CreatedBy udtUserID
	,@UpdatedBy udtUserID
	,@NewRowGuid uniqueidentifier out
)
AS
BEGIN
	IF (@IdentityGuid IS NULL)
		SET @NewRowGuid = newid();
	ELSE
		SET @NewRowGuid = @IdentityGuid;

    ;   
    MERGE [sync].[tblSyncDependencyGroup] AS existing
    USING (SELECT @NewRowGuid
					,@ID
					,@FriendlyName
					,@LongDescription
					,@DependencyLevel
					,@CreatedBy
					,@UpdatedBy
            ) AS updates (SyncDependencyGroupGuid
							,ID
							,FriendlyName
							,LongDescription
							,DependencyLevel
							,CreatedBy
							,UpdatedBy)
    ON (existing.SyncDependencyGroupGuid = updates.SyncDependencyGroupGuid)
    WHEN Matched
    THEN
        UPDATE SET ID = updates.ID
					,FriendlyName = updates.FriendlyName
					,LongDescription = updates.LongDescription
					,DependencyLevel = updates.DependencyLevel
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (SyncDependencyGroupGuid
				,ID
				,FriendlyName
				,LongDescription
				,DependencyLevel
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@ID
					,@FriendlyName
					,@LongDescription
					,@DependencyLevel
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
    ;
	
	RETURN;
END