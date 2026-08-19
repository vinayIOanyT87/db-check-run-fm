
/*
=============================================
Author: Ryan Hill
Create date: 10/2/12
Description:

Select Service Request Messaging Adaptor Filter records that match:
	The primary key, 
	The AdaptorGuid, SiteGuid, Filter Type, and Filter Value,
	The AdaptorGuid and SiteGuid,
	Or select all records
=============================================
*/
CREATE PROCEDURE [dbo].[usp_SRMAdaptorFilterSelect]
(
	@SRMAdaptorFilterGuid UNIQUEIDENTIFIER = NULL,
	@SRMAdaptorGuid UNIQUEIDENTIFIER = NULL,
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@SRMAdaptorFilterTypeCode TINYINT = NULL,
	@FilterValue NVARCHAR(100) = NULL
)
AS
BEGIN
	SET NOCOUNT ON

	IF (@SRMAdaptorFilterGuid IS NOT NULL)
	BEGIN
		SELECT SRMAdaptorFilterGuid, 
			SRMAdaptorGuid,
			SiteGuid,
			SRMAdaptorFilterTypeCode,
			FilterValue,
			IsEnabled,
			SiteGuid,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptorFilter WITH(NOLOCK)
		WHERE SRMAdaptorFilterGuid = @SRMAdaptorFilterGuid 
	END
	ELSE IF (@SRMAdaptorGuid IS NOT NULL 
		AND @SiteGuid IS NOT NULL
		AND @SRMAdaptorFilterTypeCode IS NOT NULL
		AND @FilterValue IS NOT NULL)
	BEGIN
		SELECT SRMAdaptorFilterGuid, 
			SRMAdaptorGuid,
			SiteGuid,
			SRMAdaptorFilterTypeCode,
			FilterValue,
			IsEnabled,
			SiteGuid,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptorFilter WITH(NOLOCK)
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid
			AND SiteGuid = @SiteGuid
			AND SRMAdaptorFilterTypeCode = @SRMAdaptorFilterTypeCode
			AND FilterValue = @FilterValue
	END
	ELSE IF (@SRMAdaptorGuid IS NOT NULL AND @SiteGuid IS NOT NULL)
	BEGIN
		SELECT SRMAdaptorFilterGuid, 
			SRMAdaptorGuid,
			SiteGuid,
			SRMAdaptorFilterTypeCode,
			FilterValue,
			IsEnabled,
			SiteGuid,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptorFilter WITH(NOLOCK)
		WHERE SRMAdaptorGuid = @SRMAdaptorGuid 
			AND SiteGuid = @SiteGuid
	END
	ELSE 
	BEGIN
		SELECT SRMAdaptorFilterGuid, 
			SRMAdaptorGuid,
			SiteGuid,
			SRMAdaptorFilterTypeCode,
			FilterValue,
			IsEnabled,
			SiteGuid,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		FROM tblSRMAdaptorFilter WITH(NOLOCK)
	END
END