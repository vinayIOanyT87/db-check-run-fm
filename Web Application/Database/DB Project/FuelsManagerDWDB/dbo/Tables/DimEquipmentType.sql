/*************************************** 
	[dbo].[DimEquipmentType] 
****************************************/

CREATE TABLE [dbo].[DimEquipmentType](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[AKey] [nvarchar](50) NULL,
	[EquipmentTypeName] [nvarchar](50) NULL,
	[EquipmentTypeDescription] [nvarchar](50) NULL,
	[EquipmentTypeIndex] [int] NULL,
    [EquipmentTypeClass] [nvarchar](100) NULL,
    [Capacity] [float](53) NULL,
    [Make] [nvarchar](20) NULL,
    [Model] [nvarchar](32) NULL,
    [Year] [smallint] NULL,
	[_RecordUpdatedDate] [datetimeoffset](7) NULL,
	[_DeletedFlag] [bit] NULL,
    [_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_DimEquipmentType] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))