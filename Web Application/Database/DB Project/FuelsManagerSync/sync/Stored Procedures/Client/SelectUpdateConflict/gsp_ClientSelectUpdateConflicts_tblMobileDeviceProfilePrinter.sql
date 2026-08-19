-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblMobileDeviceProfilePrinter
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMobileDeviceProfilePrinter]
@MobileDeviceProfilePrinterGUID uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblMobileDeviceProfilePrinter].[MobileDeviceProfilePrinterGUID],[dbo].[tblMobileDeviceProfilePrinter].[MobileDeviceProfileGUID],[dbo].[tblMobileDeviceProfilePrinter].[PrinterID],[dbo].[tblMobileDeviceProfilePrinter].[BaudRate],[dbo].[tblMobileDeviceProfilePrinter].[COMPort],[dbo].[tblMobileDeviceProfilePrinter].[DataBits],[dbo].[tblMobileDeviceProfilePrinter].[StopBits],[dbo].[tblMobileDeviceProfilePrinter].[UseXonXoff],[dbo].[tblMobileDeviceProfilePrinter].[XonChar],[dbo].[tblMobileDeviceProfilePrinter].[XoffChar],[dbo].[tblMobileDeviceProfilePrinter].[BufferSize],[dbo].[tblMobileDeviceProfilePrinter].[Parity],[dbo].[tblMobileDeviceProfilePrinter].[CreatedBy],[dbo].[tblMobileDeviceProfilePrinter].[UpdatedBy],[dbo].[tblMobileDeviceProfilePrinter].[CreatedDate],[dbo].[tblMobileDeviceProfilePrinter].[UpdatedDate], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblMobileDeviceProfilePrinter]
            INNER JOIN [track].[tblMobileDeviceProfilePrinter] CT
                ON CT.PK_MobileDeviceProfilePrinterGUID = [dbo].[tblMobileDeviceProfilePrinter].[MobileDeviceProfilePrinterGUID]
        WHERE CT.PK_MobileDeviceProfilePrinterGUID = @MobileDeviceProfilePrinterGUID
    ORDER BY CT.UpdatedRowVersion ASC
END
