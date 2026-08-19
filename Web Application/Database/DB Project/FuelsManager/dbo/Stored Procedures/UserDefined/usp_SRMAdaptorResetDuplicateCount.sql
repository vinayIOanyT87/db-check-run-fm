

/*
=============================================
Author: Ryan Hill
Create date: 8/28/12
Description:

Reset the duplicate message count for a given adaptor.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorResetDuplicateCount]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMAdaptor SET DuplicateCount = 0
	WHERE SRMAdaptorGuid = @SRMAdaptorGuid

END