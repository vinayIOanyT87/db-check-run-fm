-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblGeneralConfiguration
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblGeneralConfiguration]
@GeneralConfigurationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblGeneralConfiguration].[Method],[dbo].[tblGeneralConfiguration].[ConsortiumFlag],[dbo].[tblGeneralConfiguration].[ShowDeletedTrxFlag],[dbo].[tblGeneralConfiguration].[AllowUndeleteFlag],[dbo].[tblGeneralConfiguration].[ReverseTrxDateMode],[dbo].[tblGeneralConfiguration].[ForcedCloseout],[dbo].[tblGeneralConfiguration].[SecurityCode],[dbo].[tblGeneralConfiguration].[AuthorizationCode],[dbo].[tblGeneralConfiguration].[MeterTolerance],[dbo].[tblGeneralConfiguration].[CreatedBy],[dbo].[tblGeneralConfiguration].[CreatedDate],[dbo].[tblGeneralConfiguration].[UpdatedBy],[dbo].[tblGeneralConfiguration].[UpdatedDate],[dbo].[tblGeneralConfiguration].[SetBeginInventoryToZeroFlag],[dbo].[tblGeneralConfiguration].[GeneralConfigurationGuid],[dbo].[tblGeneralConfiguration].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblGeneralConfiguration]
            INNER JOIN [track].[tblGeneralConfiguration] CT
                ON CT.PK_GeneralConfigurationGuid = [dbo].[tblGeneralConfiguration].[GeneralConfigurationGuid]
        WHERE CT.PK_GeneralConfigurationGuid = @GeneralConfigurationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
