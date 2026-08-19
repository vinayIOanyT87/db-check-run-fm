

/*
=============================================
Author: Ryan Hill
Create date: 8/28/12
Description:

Increment the duplicate message count for a given adaptor.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorIncrementDuplicateCount]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMAdaptor SET DuplicateCount = DuplicateCount + 1
	WHERE SRMAdaptorGuid = @SRMAdaptorGuid

END