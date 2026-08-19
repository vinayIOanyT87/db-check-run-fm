CREATE TABLE [dbo].[tblVersion] (
    [VersionIndex]       INT                IDENTITY (1, 1) NOT NULL,
    [Version]            NVARCHAR (32)      CONSTRAINT [DF_tblVersion_Version] DEFAULT ('') NOT NULL,
    [PackageName]        NVARCHAR (32)      CONSTRAINT [DF_tblVersion_PackageName] DEFAULT ('StandardDatabase') NOT NULL,
    [DateApplied]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblVersion_DateApplied] DEFAULT (sysdatetimeoffset()) NULL,
    [Comments]           NVARCHAR (4000)    CONSTRAINT [DF_tblVersion_Comments] DEFAULT ('') NOT NULL,
    [Check1]             BIGINT             CONSTRAINT [DF_tblVersion_Check1] DEFAULT ((-1)) NOT NULL,
    [Check2]             BIGINT             CONSTRAINT [DF_tblVersion_Check2] DEFAULT ((-2)) NOT NULL,
    [VersionGuid]        UNIQUEIDENTIFIER   CONSTRAINT [DF_tblVersion_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [CreatedDate]        DATETIME           CONSTRAINT [DF_tblVersion_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]          [dbo].[udtUserID]  CONSTRAINT [DF_tblVersion_CreatedBy] DEFAULT ('') NULL,
    [UpdatedDate]        DATETIME           CONSTRAINT [DF_tblVersion_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  CONSTRAINT [DF_tblVersion_UpdatedBy] DEFAULT ('') NULL,
    [SyncCompletedFlag]  BIT                CONSTRAINT [DF_tblVersion_SyncCompletedFlag] DEFAULT ((0)) NULL,
    [RowVersionSnapshot] VARBINARY (8)      NULL,
    CONSTRAINT [PK_tblVersion_GUID] PRIMARY KEY NONCLUSTERED ([VersionGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblVersion]
    ON [dbo].[tblVersion]([CreatedDate] ASC);

GO

CREATE TRIGGER [dbo].[TR_Version_U]
    ON [dbo].[tblVersion]
    FOR UPDATE
    AS BEGIN
		IF (UPDATE(VersionIndex)
			OR UPDATE(Version) 
			OR UPDATE(PackageName)
			OR UPDATE(DateApplied)
			OR UPDATE(Comments)
			OR UPDATE(Check1)
			OR UPDATE(Check2))
		BEGIN
			RAISERROR(N'     *** UPDATEs are not allowed on the tblVersion table.  Use:  INSERT tblVersion (Version) VALUES (''1.2.3.4.5'') instead.', 20, 1) WITH NOWAIT
			ROLLBACK
		END
END

GO
CREATE TRIGGER [dbo].[TR_Version_I]
    ON [dbo].[tblVersion]
    FOR INSERT
    AS BEGIN
 	RAISERROR('   ### INSERT into tblVersion detected.  This change will not  ', 10, 1) WITH NOWAIT
	RAISERROR('   ### take effect until IIS is re-started, LogService is      ', 10, 1) WITH NOWAIT
	RAISERROR('   ### stopped, and DllHost end-tasked.',								  10, 1) WITH NOWAIT
END
GO
CREATE TRIGGER [dbo].[TR_Version_D]
    ON [dbo].[tblVersion]
    AFTER DELETE
    AS BEGIN
	SET NOCOUNT ON
	RAISERROR(N'     *** DELETEs are not allowed on the tblVersion table.', 20, 1) WITH NOWAIT
	ROLLBACK
END
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblVersion_VersionIndex]
    ON [dbo].[tblVersion]([VersionIndex] ASC);

