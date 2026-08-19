-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGasboyStationGeneralConfiguration
-- Description:	Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGasboyStationGeneralConfiguration]
@ExternalStationGeneralConfigurationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyFleetGuid],[dbo].[tblGasboyStationGeneralConfiguration].[DefaultGasboyDepartmentGuid],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[CreatedDate],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedBy],[dbo].[tblGasboyStationGeneralConfiguration].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGasboyStationGeneralConfiguration]
            INNER JOIN [track].[tblGasboyStationGeneralConfiguration] CT
                ON CT.PK_ExternalStationGeneralConfigurationGuid = [dbo].[tblGasboyStationGeneralConfiguration].[ExternalStationGeneralConfigurationGuid]
        WHERE CT.PK_ExternalStationGeneralConfigurationGuid = @ExternalStationGeneralConfigurationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END