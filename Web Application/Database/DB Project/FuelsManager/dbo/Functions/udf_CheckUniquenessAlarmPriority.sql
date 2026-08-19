

CREATE FUNCTION [dbo].[udf_CheckUniquenessAlarmPriority]
(@AlarmPriorityGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(32))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblAlarmPriority
	IF 0 < (SELECT COUNT(*) FROM tblAlarmPriorities e
	LEFT JOIN map.tblEntityAlarmPriorityToSite em1 ON em1.AlarmPriorityGuid = e.AlarmPriorityGuid
	RIGHT JOIN map.tblEntityAlarmPriorityToSite em2 ON em2.AlarmPriorityGuid = @AlarmPriorityGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.AlarmPriorityGuid <> @AlarmPriorityGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
