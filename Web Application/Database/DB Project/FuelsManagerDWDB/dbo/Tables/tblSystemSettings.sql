/*

	DROP TABLE [dbo].[tblSystemSettings]

*/
CREATE TABLE [dbo].[tblSystemSettings](
	[SKey] [int] NOT NULL,
	[SettingKey] [nvarchar](50) NOT NULL,
	[SettingValue] [nvarchar](2000) NULL,
	[_RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_tblSystemSettings] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO