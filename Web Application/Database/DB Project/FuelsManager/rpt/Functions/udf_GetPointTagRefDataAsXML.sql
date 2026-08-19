CREATE FUNCTION [rpt].[udf_GetPointTagRefDataAsXML]
(
	@SiteGuid uniqueidentifier
)
RETURNS nvarchar(max)
AS
BEGIN
	Declare @NoData nvarchar(max)
	Set @NoData = '<DocumentElement><Tag><PointTagGuid>00000000-0000-0000-0000-000000000001</PointTagGuid><PointGuid>00000000-0000-0000-0000-000000000000</PointGuid><EngineeringUnitsIndex>0</EngineeringUnitsIndex><PointTagID>NONE</PointTagID><PointID>NONE</PointID><PointEnabled>0</PointEnabled></Tag></DocumentElement>'	
	Declare @xml nvarchar(max)
	Set @xml = ISNULL((Select 
	pt.PointTagGuid as PointTagGuid,
	pt.PointGuid as PointGuid,
	pt.EngineeringUnitsIndex as EngineeringUnitsIndex,
	pt.ID as PointTagID,
	p.ID as PointID,
	p.Enabled as PointEnabled
	from tblPointTag pt  
	INNER JOIN tblPoint p on pt.PointGuid = p.PointGuid 
	INNER JOIN tblPointTemplate q on p.PointTemplateGuid = q.PointTemplateGuid
	where p.SiteGuid = @SiteGuid AND q.PointTemplateTypeApplicationStringGuid = 'e78cd406-4c19-4978-8940-fa4e404e3e53'
	FOR XML PATH('Tag'), root('DocumentElement')),@NoData)
	return @xml
return @xml
END
