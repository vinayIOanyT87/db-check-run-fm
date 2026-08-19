/*************************************************** 
[dbo].[DimEquipment]  
*****************************************************/

CREATE TABLE [dbo].[DimEquipment]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[MasterRecordKey] [nvarchar](50) NULL,
[SiteSKey] int NULL DEFAULT(0),
[EquipmentId] [nvarchar](30) NULL,
[EquipmentTypeSKey] [int] NULL DEFAULT(0),
[Description] [nvarchar](50) NULL,
[Make] [nvarchar](20) NULL,
[Model] [nvarchar](50) NULL,
[InUse] [bit] NULL,
[SerialNumber] [nvarchar](30) NULL,
[StartDate] [datetimeoffset](7) NOT NULL,
[EndDate] [datetimeoffset](7) NULL,
[_IsRecordAddedByETL] [bit] NOT NULL DEFAULT(0)
 CONSTRAINT [PK_DimEquipment] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))