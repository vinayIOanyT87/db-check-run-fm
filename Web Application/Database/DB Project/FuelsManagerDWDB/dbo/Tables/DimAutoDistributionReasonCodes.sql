/******************************************* 
	[dbo].[DimAutoDistributionReasonCodes] 
*****************************************/

CREATE TABLE [dbo].[DimAutoDistributionReasonCodes] 
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[AKey] [nvarchar](50) NULL,
[SiteSKey] int NULL DEFAULT (0),
[ReasonCode] [nvarchar](50) NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_DeletedFlag] [bit] NULL
 CONSTRAINT [PK_tblAutoDistributionReasonCodes] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))