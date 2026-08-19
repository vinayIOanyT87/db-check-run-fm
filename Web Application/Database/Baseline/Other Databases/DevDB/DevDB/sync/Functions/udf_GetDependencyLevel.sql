
CREATE FUNCTION [sync].[udf_GetDependencyLevel](
    @SchemaTableName nvarchar(512)
)
RETURNS 
	int
AS
BEGIN
    DECLARE @Level int
    SELECT @Level = DependencyLevel 
        FROM [FuelsManagerDB].[sync].[tblSyncDependencyGroup] sdg
            INNER JOIN [FuelsManagerDB].[sync].[tblSyncTable] st
                ON sdg.[SyncDependencyGroupGuid] = st.[SyncDependencyGroupGuid]
        WHERE st.[TableName] = @SchemaTableName
    
    RETURN @Level;
END