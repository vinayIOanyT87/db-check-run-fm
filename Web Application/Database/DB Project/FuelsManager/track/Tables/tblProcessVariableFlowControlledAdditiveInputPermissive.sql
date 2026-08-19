/* {CheckPoint: CREATING TRACKING TABLE for tblProcessVariableFlowControlledAdditiveInputPermissive } */

/****** Object:  Table [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]   Script Date: 8/28/2012 3:24:01 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableFlowControlledAdditiveInputPermissive]') AND type in (N'U'))
--BEGIN
CREATE TABLE [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]
(
	ChangeIndex [bigint] NOT NULL IDENTITY(1,1),
	InsertedDate [datetimeoffset](7) NOT NULL,
	InsertedContext [varbinary](128) NULL,
	InsertedRowVersion [varbinary](8) NOT NULL,
	UpdatedDate [datetimeoffset](7) NULL,
	UpdatedContext [varbinary](128) NULL,
	UpdatedRowVersion [varbinary](8) NULL,
	DeletedDate [datetimeoffset](7) NULL,
	DeletedContext [varbinary](128) NULL,
	DeletedRowVersion [varbinary](8) NULL,
	CurrentSiteGuid [uniqueidentifier] NULL,
	PreviousSiteGuid [uniqueidentifier] NULL,
    PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid [UniqueIdentifier] NOT NULL,
    FK_ParentPK uniqueidentifier NULL,
	CONSTRAINT [PK_track_tblProcessVariableFlowControlledAdditiveInputPermissive_ChangeIndex] PRIMARY KEY CLUSTERED 
	(
		[ChangeIndex] ASC
	)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)
--END


GO
/****** Object:  Index [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_InsertContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableFlowControlledAdditiveInputPermissive]') AND name = N'IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_InsertContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_InsertedRowVersion] ON [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]
(
    [InsertedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid] ASC,
    [InsertedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_UpdateContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableFlowControlledAdditiveInputPermissive]') AND name = N'IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_UpdateContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_UpdatedRowVersion] ON [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]
(
    [UpdatedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid] ASC,
    [UpdatedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
/****** Object:  Index [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_DeleteContext]    Script Date: 6/5/2013 1:16:29 PM ******/
--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblProcessVariableFlowControlledAdditiveInputPermissive]') AND name = N'IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_DeleteContext')
CREATE NONCLUSTERED INDEX [IX_track_tblProcessVariableFlowControlledAdditiveInputPermissive_DeletedRowVersion] ON [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]
(
    [DeletedRowVersion] ASC,
    [PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid] ASC,
    [DeletedContext] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)


GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableFlowControlledAdditiveInputPermissive_PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid]
    ON [track].[tblProcessVariableFlowControlledAdditiveInputPermissive]([PK_ProcessVariableProductToPresetFlowControlledAdditiveGuid] ASC);


GO
