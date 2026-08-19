

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Update a Service Request Messaging Adaptor Configuration record.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorUpdate]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@SRMAdaptorName NVARCHAR(100),
	@IsEnabled BIT,
	@CustomWebApplicationPage NVARCHAR(100),
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMAdaptor
	SET SRMAdaptorName = @SRMAdaptorName,
		IsEnabled = @IsEnabled,
		CustomWebApplicationPage = @CustomWebApplicationPage,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE SRMAdaptorGuid = @SRMAdaptorGuid

END