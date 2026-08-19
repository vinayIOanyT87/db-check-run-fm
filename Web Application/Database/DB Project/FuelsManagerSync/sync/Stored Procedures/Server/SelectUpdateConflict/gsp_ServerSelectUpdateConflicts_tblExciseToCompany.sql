-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblExciseToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExciseToCompany]
@ExciseToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblExciseToCompany].[ExciseToCompanyGuid],[map].[tblExciseToCompany].[ExciseGuid],[map].[tblExciseToCompany].[CompanyGuid],[map].[tblExciseToCompany].[CreatedDate],[map].[tblExciseToCompany].[CreatedBy],[map].[tblExciseToCompany].[UpdatedDate],[map].[tblExciseToCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblExciseToCompany]
            INNER JOIN [track].[tblExciseToCompany] CT
                ON CT.PK_ExciseToCompanyGuid = [map].[tblExciseToCompany].[ExciseToCompanyGuid]
        WHERE CT.PK_ExciseToCompanyGuid = @ExciseToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
