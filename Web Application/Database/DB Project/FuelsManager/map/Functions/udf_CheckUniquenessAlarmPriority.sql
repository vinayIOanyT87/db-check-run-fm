

CREATE FUNCTION [map].[udf_CheckUniquenessAlarmPriority]
(@AlarmPriorityGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(32)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblAlarmPriorities e WHERE e.AlarmPriorityGuid = @AlarmPriorityGuid)
	IF 0 < (SELECT COUNT(*) FROM tblAlarmPriorities e 
	RIGHT JOIN map.tblEntityAlarmPriorityToSite em ON em.SiteGuid = @SiteGuid AND em.AlarmPriorityGuid = e.AlarmPriorityGuid 
	WHERE e.AlarmPriorityGuid <> @AlarmPriorityGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

