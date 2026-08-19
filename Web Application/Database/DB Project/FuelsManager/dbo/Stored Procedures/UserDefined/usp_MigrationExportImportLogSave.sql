CREATE PROCEDURE [dbo].[usp_MigrationExportImportLogSave](
	@IdentityGuid uniqueidentifier = NULL
    ,@SiteGuid uniqueidentifier = NULL
	,@ActivityID nvarchar(30)
	,@ActivityDescription nvarchar(256)
	,@ActivityStatus nvarchar(100)
	,@PerformedBy nvarchar(100)
	,@ClientIPAddress nvarchar(50)
    ,@CreatedDate datetimeoffset(7)
	,@CreatedBy nvarchar(100)
	,@UpdatedBy nvarchar(100)
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
    MERGE [dbo].[tblMigrationExportImportLog] AS existing
    USING (SELECT @NewRowGuid
					,@SiteGuid
					,@ActivityID
					,@ActivityDescription
					,@ActivityStatus
					,@PerformedBy
					,@ClientIPAddress
					,@CreatedDate
					,@CreatedBy
					,@UpdatedBy) AS updates (MigrationExportImportLogGuid
											,SiteGuid
											,ActivityID
											,ActivityDescription
											,ActivityStatus
											,PerformedBy
											,ClientIPAddress
											,CreatedDate
        									,CreatedBy
        									,UpdatedBy)
    ON (existing.MigrationExportImportLogGuid = updates.MigrationExportImportLogGuid)
    WHEN Matched
    THEN
        UPDATE SET SiteGuid = updates.SiteGuid
					,ActivityID = updates.ActivityID
					,ActivityDescription = updates.ActivityDescription
					,ActivityStatus = updates.ActivityStatus
					,PerformedBy = updates.PerformedBy
					,ClientIPAddress = updates.ClientIPAddress
					,UpdatedDate = SYSDATETIMEOFFSET()
					,UpdatedBy = updates.UpdatedBy
    WHEN Not Matched
    THEN
        INSERT (MigrationExportImportLogGuid
                ,SiteGuid
				,ActivityID
				,ActivityDescription
				,ActivityStatus
				,PerformedBy
				,ClientIPAddress
				,CreatedDate
				,CreatedBy
				,UpdatedDate
				,UpdatedBy)
            VALUES (@NewRowGuid
					,@SiteGuid
					,@ActivityID
					,@ActivityDescription
					,@ActivityStatus
					,@PerformedBy
					,@ClientIPAddress
					,SYSDATETIMEOFFSET()
					,CreatedBy
					,SYSDATETIMEOFFSET()
					,UpdatedBy)
	;

	RETURN;
END
