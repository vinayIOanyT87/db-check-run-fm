CREATE PROCEDURE [dbo].[usp_VruThresholdInsert]
	@SiteGuid uniqueidentifier,
	@ID nvarchar(60),
	@Interval int,
	@IntervalType int,
	@Limit float,
	@Tolerance decimal(10,2),
	@Enabled bit,
	@ResetDate datetimeoffset,
	@CreatedDate datetimeoffset,
	@CreatedBy dbo.udtUserID,
	@UpdatedDate datetimeoffset,
	@UpdatedBy dbo.udtUserID
AS
	DECLARE @NextGuidTable AS Table (NextGuid uniqueidentifier)

	INSERT INTO tblVRUThresholds 
	(SiteGuid,
	ID,
	Interval,
	IntervalType,
	Limit,
	Tolerance,
	Enabled,
	ResetDate,
	CreatedDate,
	CreatedBy,
	UpdatedDate,
	UpdatedBy
	)
	OUTPUT inserted.[VRUThresholdGuid] into @NextGuidTable (NextGuid)
	SELECT @SiteGuid, 
	@ID, 
	@Interval, 
	@IntervalType,
	@Limit,
	@Tolerance,
	@Enabled,
	@ResetDate,
	@CreatedDate,
	@CreatedBy,
	@UpdatedDate,
	@UpdatedBy

	SELECT NextGuid FROM @NextGuidTable

RETURN 0
