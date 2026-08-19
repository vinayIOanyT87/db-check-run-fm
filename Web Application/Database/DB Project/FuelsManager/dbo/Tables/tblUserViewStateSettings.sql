CREATE TABLE [dbo].[tblUserViewStateSettings]
(
	[ID] [nvarchar](30) NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_ID]  DEFAULT (''),
	[ValueType] [nvarchar](max) NULL,
	[Value] [xml] NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_CreatedDate]  DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_CreatedBy]  DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_UpdatedDate]  DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_UpdatedBy]  DEFAULT (''),
	[_RowVersion] [timestamp] NOT NULL,
	[UserViewStateSettingGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_tblUserViewStateSettings_UserViewStateSettingGuid]  DEFAULT (newid()),
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[ClientIpAddress] NVARCHAR (50) NULL,
   [WindowName] NVARCHAR (50) NULL,
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
   CONSTRAINT [PK_tblUserViewStateSettings__UserViewStateSettingGuid] PRIMARY KEY NONCLUSTERED ([UserViewStateSettingGuid] ASC),
   CONSTRAINT [FK_tblUserViewStateSettings_tblSites] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites]([SiteGuid]),
	CONSTRAINT [FK_tblUserViewStateSettings_tblUsers] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers]([UserGuid]),
	CONSTRAINT [CK_tblUserViewStateSettings_ClientIpAddress_IPv4] CHECK (
		[ClientIpAddress] IS NULL
		OR (
			[ClientIpAddress] NOT LIKE N'%[^0-9.]%'
			AND (LEN([ClientIpAddress]) - LEN(REPLACE([ClientIpAddress], N'.', N''))) = 3
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 1)) IS NOT NULL
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 1)) BETWEEN 0 AND 255
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 2)) IS NOT NULL
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 2)) BETWEEN 0 AND 255
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 3)) IS NOT NULL
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 3)) BETWEEN 0 AND 255
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 4)) IS NOT NULL
			AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 4)) BETWEEN 0 AND 255
		)
	)
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblUserViewStateSettings_ID_SiteGuid_UserGuid_ClientIpAddress_WindowName]
ON [dbo].[tblUserViewStateSettings] ([ID], [SiteGuid], [UserGuid], [ClientIpAddress], [WindowName]);

GO
