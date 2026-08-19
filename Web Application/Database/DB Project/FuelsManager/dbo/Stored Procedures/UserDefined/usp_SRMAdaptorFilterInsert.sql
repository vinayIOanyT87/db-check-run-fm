
/*
=============================================
Author: Ryan Hill
Create date: 10/2/12
Description:

Insert a Service Request Messaging Adaptor Filter record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFilterInsert]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER,
	@SiteGuid UNIQUEIDENTIFIER,
	@SRMAdaptorFilterTypeCode TINYINT,
	@FilterValue NVARCHAR(100),
	@IsEnabled BIT,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMAdaptorFilterGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMAdaptorFilterGuid = NEWID()

	INSERT INTO tblSRMAdaptorFilter
	(
		SRMAdaptorFilterGuid, 
		SRMAdaptorGuid,
		SiteGuid,
		SRMAdaptorFilterTypeCode,
		FilterValue,
		IsEnabled,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMAdaptorFilterGuid, 
		@SRMAdaptorGuid,
		@SiteGuid,
		@SRMAdaptorFilterTypeCode,
		@FilterValue,
		@IsEnabled,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END