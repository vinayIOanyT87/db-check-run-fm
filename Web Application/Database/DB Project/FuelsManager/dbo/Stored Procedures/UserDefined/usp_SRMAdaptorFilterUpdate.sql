
/*
=============================================
Author: Ryan Hill
Create date: 10/2/12
Description:

Update a Service Request Messaging Adaptor Filter
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFilterUpdate]
(
	@SRMAdaptorFilterGuid UNIQUEIDENTIFIER, 
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@SRMAdaptorFilterTypeCode TINYINT,
	@FilterValue NVARCHAR(100),
	@IsEnabled BIT,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID
)
AS
BEGIN
	SET NOCOUNT ON

	UPDATE tblSRMAdaptorFilter
	SET SRMAdaptorGuid = @SRMAdaptorGuid,
		SiteGuid = @SiteGuid,
		SRMAdaptorFilterTypeCode = @SRMAdaptorFilterTypeCode,
		FilterValue = @FilterValue,
		IsEnabled = @IsEnabled,
		UpdatedDate = @UpdatedDate,
		UpdatedBy = @UpdatedBy
	WHERE SRMAdaptorFilterGuid = @SRMAdaptorFilterGuid
END