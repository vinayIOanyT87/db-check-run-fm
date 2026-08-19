
/*
=============================================
Author: Ryan Hill
Create date: 10/2/12
Description:

Delete a Service Request Messaging Adaptor Filter
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFilterDelete]
(
	@SRMAdaptorFilterGuid UNIQUEIDENTIFIER
)
AS
BEGIN
	SET NOCOUNT ON

	DELETE FROM tblSRMAdaptorFilter
	WHERE SRMAdaptorFilterGuid = @SRMAdaptorFilterGuid

END