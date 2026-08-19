CREATE FUNCTION [dbo].[udf_UpdateXmlForDataContractSerializer]
(
	@ValueType NVARCHAR(100),
	@Value XML
)
RETURNS XML
AS
BEGIN
	IF @ValueType = 'System.Double' RETURN CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<double>','<double xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.Single' RETURN CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<float>','<float xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.Int16' RETURN CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<short>','<short xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) 
	ELSE IF @ValueType = 'System.UInt16' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<unsignedShort>','<unsignedShort xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.Int32' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<int>','<int xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.UInt32' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<unsignedInt>','<unsignedInt xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.Boolean' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<boolean>','<boolean xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.DateTime' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<dateTime>','<dateTime xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.DateTimeOffset' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<DateTimeOffset>','<DateTimeOffset xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.TimeSpan' RETURN  CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<TimeSpan>','<TimeSpan xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'))
	ELSE IF @ValueType = 'System.String' RETURN  CONVERT(XML,REPLACE(REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<string>','<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">'),'<string />','<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/" />'))
	ELSE IF @ValueType = 'FMBusinessObjects.DataObjects.VcfModuleSettings' RETURN CONVERT(XML,REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR(MAX),@Value),'<K>','<K xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">'),'<double','<a:double'),'</double','</a:double'))
	RETURN @Value
END
