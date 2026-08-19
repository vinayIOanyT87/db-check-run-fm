/* {CheckPoint: CREATING TRACKING TABLE for tblPIDXProfileToCompany } */

/****** Object:  Table [track].[tblPIDXProfileToCompany]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblPIDXProfileToCompany]
( 
	[ChangeIndex] [bigint] IDENTITY(1,1) NOT NULL,
	[InsertedDate] [datetimeoffset](7) NOT NULL,
	[InsertedContext] [varbinary](128) NULL,
	[InsertedRowVersion] [varbinary](8) NOT NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedContext] [varbinary](128) NULL,
	[UpdatedRowVersion] [varbinary](8) NULL,
	[DeletedDate] [datetimeoffset](7) NULL,
	[DeletedContext] [varbinary](128) NULL,
	[DeletedRowVersion] [varbinary](8) NULL,
	[CurrentSiteGuid] [uniqueidentifier] NULL,
	[PreviousSiteGuid] [uniqueidentifier] NULL,
	[PK_PIDXProfileToCompanyGuid] [uniqueidentifier] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblPIDXProfileToCompany_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfileToCompany_PK_PIDXProfileToCompanyGuid] ON [track].[tblPIDXProfileToCompany]
(
    [PK_PIDXProfileToCompanyGuid],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfileToCompany_InsertedRowVersion] ON [track].[tblPIDXProfileToCompany]
(
    [InsertedRowVersion] ASC,
    [PK_PIDXProfileToCompanyGuid],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfileToCompany_UpdatedRowVersion] ON [track].[tblPIDXProfileToCompany]
(
    [UpdatedRowVersion] ASC,
    [PK_PIDXProfileToCompanyGuid],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfileToCompany_DeletedRowVersion] ON [track].[tblPIDXProfileToCompany]
(
    [DeletedRowVersion] ASC,
    [PK_PIDXProfileToCompanyGuid],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblPIDXProfileToCompany_PK_PIDXProfileToCompanyGuid_Sync] ON [track].[tblPIDXProfileToCompany]
(
	[PK_PIDXProfileToCompanyGuid] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblPIDXProfileToCompany_DeletedRowVersionUpdate_ForSync
   ON track.tblPIDXProfileToCompany
   AFTER UPDATE
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
 
    IF ( UPDATE( DeletedDate ) )
    BEGIN
        UPDATE t
            SET DeletedRowVersion = convert(varbinary(8), i._RowVersion)
        FROM track.tblPIDXProfileToCompany t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END