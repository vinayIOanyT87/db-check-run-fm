/*
	DROP TABLE [erv].[tblProcessSettings]
*/
CREATE TABLE [erv].[tblProcessSettings](
	[ProcessSettingsKey] [int] NOT NULL,
	[InhibitGlobalFieldsProcessing] [bit] NOT NULL,
	[CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessSettings_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessSettings_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessSettings_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessSettings_UpdatedBy] DEFAULT ('') NOT NULL,
	[_RowVersion] [timestamp] NOT NULL
 CONSTRAINT [PK_tblProcessSettings] PRIMARY KEY CLUSTERED 
(
	[ProcessSettingsKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [erv].[tblProcessSettings] ADD  CONSTRAINT [DF_tblProcessSettings_InhibitGlobalFieldsProcessing]  DEFAULT (0) FOR [InhibitGlobalFieldsProcessing]
GO
