CREATE PROCEDURE dbo.nspa_GetVolumeUnitName
(
	@SiteID	NVARCHAR(60)
)
AS
BEGIN
SET NOCOUNT ON

SELECT	u.EngineeringUnitName
FROM	lookup.tblEngineeringUnit u INNER JOIN tblSites s ON s.VolumeUnitIndex = u.EngineeringUnitIndex
WHERE	s.ID = @SiteID

END