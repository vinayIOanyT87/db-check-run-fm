
/*
=============================================
Author: Ryan Hill
Create date: 7/13/12
Description:

Read a Service Request Messaging Adaptor Configuration record by either 
the primary key or the name of the Service Request Messaging Adaptor.

You can also read all configured Service Request Messaging Adaptors
by not providing any input parameters. 
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorSelect]
(
	@SRMAdaptorGuid UNIQUEIDENTIFIER = NULL,
	@SRMAdaptorName NVARCHAR(100) = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	IF(@SRMAdaptorName IS NOT NULL)
	BEGIN
		SELECT SRMAdaptorGuid,
			SRMAdaptorName,
			IsEnabled,
			CustomWebApplicationPage,
			DuplicateCount,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptor WITH (NOLOCK)	
		WHERE SRMAdaptorName = @SRMAdaptorName
	END
	ELSE IF (@SRMAdaptorGuid IS NOT NULL)
	BEGIN
		SELECT SRMAdaptorGuid,
			SRMAdaptorName,
			IsEnabled,
			CustomWebApplicationPage,
			DuplicateCount,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptor WITH (NOLOCK)	
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid
	END
	ELSE
	BEGIN
		SELECT SRMAdaptorGuid,
			SRMAdaptorName,
			IsEnabled,
			CustomWebApplicationPage,
			DuplicateCount,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptor WITH (NOLOCK)	
	END
END