-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyDepartment
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyDepartment]
@GasboyDepartmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid],[dbo].[tblGasboyDepartment].[SiteGuid],[dbo].[tblGasboyDepartment].[DepartmentID],[dbo].[tblGasboyDepartment].[DepartmentCode],[dbo].[tblGasboyDepartment].[DepartmentName],[dbo].[tblGasboyDepartment].[GroupRuleName],[dbo].[tblGasboyDepartment].[PriceListName],[dbo].[tblGasboyDepartment].[LookupGasboyRecordStatusIndex],[dbo].[tblGasboyDepartment].[UsePINCodeFlag],[dbo].[tblGasboyDepartment].[PINCode],[dbo].[tblGasboyDepartment].[AuthPINFrom],[dbo].[tblGasboyDepartment].[PromptForVehiclePlateFlag],[dbo].[tblGasboyDepartment].[LookupGasboyVehiclePlateCheckTypeIndex],[dbo].[tblGasboyDepartment].[AlwaysPromptForAdditionalValidationFlag],[dbo].[tblGasboyDepartment].[CreatedBy],[dbo].[tblGasboyDepartment].[CreatedDate],[dbo].[tblGasboyDepartment].[UpdatedBy],[dbo].[tblGasboyDepartment].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGasboyDepartment]
            INNER JOIN [track].[tblGasboyDepartment] CT
                ON CT.PK_GasboyDepartmentGuid = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
        WHERE CT.PK_GasboyDepartmentGuid = @GasboyDepartmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END