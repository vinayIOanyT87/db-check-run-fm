

CREATE FUNCTION [map].[udf_CheckUniquenessMaintenanceReason]
(@MaintenanceReasonGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(30)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM dbo.tblMaintenanceReasons e WHERE e.MaintenanceReasonGuid = @MaintenanceReasonGuid)
	IF 0 < (SELECT COUNT(*) FROM dbo.tblMaintenanceReasons e
	RIGHT JOIN map.tblEntityMaintenanceReasonToSite em2 ON em2.MaintenanceReasonGuid = e.MaintenanceReasonGuid AND em2.SiteGuid =@SiteGuid
	WHERE e.MaintenanceReasonGuid <> @MaintenanceReasonGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
