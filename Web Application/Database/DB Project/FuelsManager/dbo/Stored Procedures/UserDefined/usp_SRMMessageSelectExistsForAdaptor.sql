
/*
=============================================
Author: Ryan Hill
Create date: 9/24/12
Description:

Determine if any messages exist for an adaptor. 
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMMessageSelectExistsForAdaptor]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	IF EXISTS(SELECT(SRMMessageGuid) FROM tblSRMMessage WITH (NOLOCK) WHERE SRMAdaptorGuid = @SRMAdaptorGuid)
	BEGIN
		SELECT 1
	END
	ELSE
	BEGIN
		SELECT 0
	END
END