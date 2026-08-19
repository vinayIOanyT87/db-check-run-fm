
CREATE FUNCTION [sync].[udf_GetMappedScopeLevel](
    @SchemaTableName nvarchar(512)
    ,@SyncProfileID nvarchar(512)
)
RETURNS 
	nvarchar(512)
AS
BEGIN
    DECLARE @SyncProfile nvarchar(255)
    DECLARE @SyncScopeID nvarchar(255)

    DECLARE @SchemaName nvarchar(255)
    DECLARE @TableName nvarchar(255)

    IF (@SyncProfileID IS NULL)
    BEGIN
        SET @SyncProfile = '{Complete}'
    END

    SELECT @SchemaName = SchemaName, @TableName = TableName FROM [DevDB].[dbo].[udf_SplitTableName](@SchemaTableName);

    IF (@SchemaName IS NULL OR @SchemaName = '')
    BEGIN
        SET @SchemaName = 'dbo';
    END

    SELECT @SyncScopeID = ss.[ID] 
        FROM [FuelsManagerDB].[sync].[tblSyncProfile] sp
                INNER JOIN [FuelsManagerDB].[sync].[tblSyncScope] ss
                    ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
                INNER JOIN [FuelsManagerDB].[sync].[tblSyncTableToScopeMap] sttsm
                    ON sttsm.[SyncScopeGuid] = ss.[SyncScopeGuid]
                INNER JOIN [FuelsManagerDB].[sync].[tblSyncTable] st
                    ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
        WHERE sp.[ID] = @SyncProfile
                AND st.[TableName] = @SchemaTableName

    RETURN @SyncScopeID;
END
