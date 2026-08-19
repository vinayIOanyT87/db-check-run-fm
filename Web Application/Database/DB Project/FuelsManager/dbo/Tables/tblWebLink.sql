CREATE TABLE [dbo].[tblWebLink]
(
	[WebLinkGuid] UNIQUEIDENTIFIER NOT NULL , 
    [LinkName] NVARCHAR(100) NOT NULL, 
    [LinkAddress] NVARCHAR(2000) NOT NULL, 
    [LinkDescription] NVARCHAR(200) NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [CreatedBy] NVARCHAR(50) NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] NVARCHAR(50) NOT NULL, 
    [_RowVersion] ROWVERSION NOT NULL,
	CONSTRAINT [PK_tblWebLink] PRIMARY KEY NONCLUSTERED ([WebLinkGuid] ASC)
)

GO
CREATE CLUSTERED INDEX [IX_tblWebLink]
    ON [dbo].[tblWebLink]([CreatedDate] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblWebLink_LinkName]
    ON [dbo].[tblWebLink]([LinkName] ASC)

GO
ALTER TABLE [dbo].[tblWebLink] ADD  CONSTRAINT [DF_tblWebLinks_CreatedDate]  DEFAULT (SYSDATETIMEOFFSET()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[tblWebLink] ADD  CONSTRAINT [DF_tblWebLinks_CreatedBy]  DEFAULT ('Varec') FOR [CreatedBy]
GO
ALTER TABLE [dbo].[tblWebLink] ADD  CONSTRAINT [DF_tblWebLinks_UpdatedDate]  DEFAULT (SYSDATETIMEOFFSET()) FOR [UpdatedDate]
GO
ALTER TABLE [dbo].[tblWebLink] ADD  CONSTRAINT [DF_tblWebLinks_UpdatedBy]  DEFAULT ('Varec') FOR [UpdatedBy]
GO
