CREATE TABLE [dbo].[tblMaintenanceReasons] (
    [ID]                    NVARCHAR (30)      CONSTRAINT [DF_tblMaintenanceReasons_ID] DEFAULT ('') NOT NULL,
    [Description]           NVARCHAR (50)      CONSTRAINT [DF_tblMaintenanceReasons_Description] DEFAULT ('') NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblMaintenanceReasons_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblMaintenanceReasons_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblMaintenanceReasons_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblMaintenanceReasons_UpdatedBy] DEFAULT ('') NOT NULL,
    [DeletedFlag]           BIT                NULL,
    [MaintenanceReasonGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblMaintenanceReasons_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblMaintenanceReasons_GUID] PRIMARY KEY NONCLUSTERED ([MaintenanceReasonGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblMaintenanceReasons_CreatedDate]
    ON [dbo].[tblMaintenanceReasons]([CreatedDate] ASC);

GO
