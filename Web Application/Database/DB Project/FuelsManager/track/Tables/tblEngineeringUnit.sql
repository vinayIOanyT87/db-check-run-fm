/* {CheckPoint: CREATING TRACKING TABLE for tblEngineeringUnit } */

/****** Object:  Table [track].[tblEngineeringUnit]   Script Date: 8/28/2012 3:24:01 PM ******/
CREATE TABLE [track].[tblEngineeringUnit]
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
	[PK_EngineeringUnitIndex] [int] NOT NULL,
	[FK_ParentPK] [uniqueidentifier] NULL,
	[_RowVersion] [ROWVERSION] NOT NULL,
	CONSTRAINT [PK_track_tblEngineeringUnit_ChangeIndex] PRIMARY KEY CLUSTERED
	(
		[ChangeIndex] ASC
	)
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEngineeringUnit_PK_EngineeringUnitIndex] ON [track].[tblEngineeringUnit]
(
    [PK_EngineeringUnitIndex],
        [ChangeIndex] ASC
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEngineeringUnit_InsertedRowVersion] ON [track].[tblEngineeringUnit]
(
    [InsertedRowVersion] ASC,
    [PK_EngineeringUnitIndex],
    [InsertedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEngineeringUnit_UpdatedRowVersion] ON [track].[tblEngineeringUnit]
(
    [UpdatedRowVersion] ASC,
    [PK_EngineeringUnitIndex],
    [UpdatedContext] 
)
GO

CREATE NONCLUSTERED INDEX [IX_track_tblEngineeringUnit_DeletedRowVersion] ON [track].[tblEngineeringUnit]
(
    [DeletedRowVersion] ASC,
    [PK_EngineeringUnitIndex],
    [DeletedContext] 
)
GO
CREATE NONCLUSTERED INDEX [IX_track_tblEngineeringUnit_PK_EngineeringUnitIndex_Sync] ON [track].[tblEngineeringUnit]
(
	[PK_EngineeringUnitIndex] ASC
)INCLUDE([ChangeIndex],[UpdatedContext],[UpdatedRowVersion],[CurrentSiteGuid],[PreviousSiteGuid])
GO
CREATE TRIGGER track.trg_insupd_tblEngineeringUnit_DeletedRowVersionUpdate_ForSync
   ON track.tblEngineeringUnit
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
        FROM track.tblEngineeringUnit t
            INNER JOIN inserted i on i.[ChangeIndex] = t.[ChangeIndex]
            INNER JOIN deleted d on d.[ChangeIndex] = i.[ChangeIndex]
        WHERE i.DeletedDate IS NOT NULL and d.DeletedDate IS NULL
    END
END