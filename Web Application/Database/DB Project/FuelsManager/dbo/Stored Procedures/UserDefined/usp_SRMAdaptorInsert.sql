

/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Create a Service Request Messaging Adaptor Configuration record
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorInsert]
(
	@SRMAdaptorName NVARCHAR(100),
	@IsEnabled BIT,
	@CustomWebApplicationPage NVARCHAR(100) = NULL,
	@CreatedDate DATETIMEOFFSET(7),
	@CreatedBy dbo.udtUserID,
	@UpdatedDate DATETIMEOFFSET(7),
	@UpdatedBy dbo.udtUserID,
	@SRMAdaptorGuid UNIQUEIDENTIFIER OUTPUT
)
AS
BEGIN
	SET NOCOUNT ON

	SET @SRMAdaptorGuid = NEWID()

	INSERT INTO tblSRMAdaptor
	(
		SRMAdaptorGuid,
		SRMAdaptorName,
		IsEnabled,
		CustomWebApplicationPage,
		DuplicateCount,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	)
	VALUES
	(
		@SRMAdaptorGuid,
		@SRMAdaptorName,
		@IsEnabled,
		@CustomWebApplicationPage,
		0,
		@CreatedDate,
		@CreatedBy,
		@UpdatedDate,
		@UpdatedBy
	)
END