SET NOCOUNT ON

PRINT 'Checking to see if any existing Tank point(s) Vessel settings need to be refreshed with CST Tank Information ...'
PRINT ''

DECLARE @PointsThatNeedsToBeUpdated int = 0

DECLARE @TempTable TABLE 
(
	ID NVARCHAR(100), 
	PointTemplateGuid UniqueIdentifier, 
	PointID Nvarchar(255), 
	PointTemplatePropertyGuid UniqueIdentifier, 
	PointGuid UniqueIdentifier, 
	PointPropertyGuid UniqueIdentifier, 
	Value Xml
)

INSERT INTO @TempTable select appl.id, templ.PointTemplateGuid, templ.ID, templProp.PointTemplatePropertyGuid, point.PointGuid,
pointProp.PointPropertyGuid, pointProp.Value FROM tblPointTemplate templ
LEFT JOIN dbo.tblApplicationString appl
ON appl.ApplicationStringGuid = templ.PointTemplateTypeApplicationStringGuid
INNER JOIN tblPointTemplateProperty templProp
ON templProp.PointTemplateGuid = templ.PointTemplateGuid AND templProp.ID = 'Vessel'
INNER JOIN tblPoint point ON point.PointTemplateGuid = templ.PointTemplateGuid
INNER JOIN tblPointProperty pointProp ON pointProp.PointTemplatePropertyGuid = templProp.PointTemplatePropertyGuid AND pointProp.PointGuid = point.PointGuid
WHERE appl.ID = 'Tank' AND pointProp.[Value].exist('/Vessel/CSTCapacity/EngineeringUnitsType') = 0

SELECT @PointsThatNeedsToBeUpdated = COUNT(*) FROM @TempTable

IF (@PointsThatNeedsToBeUpdated > 0)
BEGIN

	DECLARE @CST_Info as NVARCHAR(MAX) = '<CSTManufacturerName />' +
	'<CSTManufactureDate>2023-10-02T00:00:00-04:00</CSTManufactureDate>' +
	'<CSTCapacity>' +
	'<EngineeringUnitsType>FmuVolume</EngineeringUnitsType>' +
	'<Value>0</Value>' +
	'</CSTCapacity>' +
	'<CSTSerialNumber />' +
	'<CSTLocationName />' +
	'<CSTLatitude xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />' +
	'<CSTLongitude xmlns:p2="http://www.w3.org/2001/XMLSchema-instance" p2:nil="true" />' +
	'<CSTCommissionDate>2023-10-02T00:00:00-04:00</CSTCommissionDate> </Vessel>' 
		
	UPDATE pProp SET Value = CONVERT(XML, REPLACE(CONVERT(NVARCHAR(MAX), pProp.Value), '</Vessel>', @CST_Info)) FROM tblPointProperty pProp JOIN
	@TempTable x ON pProp.PointPropertyGuid = x.PointPropertyGuid

	PRINT '** ' + CONVERT(NVARCHAR(25), @PointsThatNeedsToBeUpdated) + ' EXISTING RECORDS ARE UPDATED IN [dbo].[tblPointProperty] FOR CST Tank Information **'
END

ELSE

BEGIN
	PRINT '** No changes are required in [dbo].[tblPointProperty] for CST Tank Information **'
END

PRINT ''

SET NOCOUNT OFF
