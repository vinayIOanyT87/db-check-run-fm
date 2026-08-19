CREATE TABLE [dbo].[tblEquipmentMaintenanceLog] (
    [EquipmentID]                 NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_EquipmentID] DEFAULT ('') NOT NULL,
    [EquipmentType]               NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_EquipmentType] DEFAULT ('') NOT NULL,
    [OperatorID]                  NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_OperatorID] DEFAULT ('') NOT NULL,
    [MaintenanceReason]           NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_MaintenanceReason] DEFAULT ('Is In Service') NOT NULL,
    [InServiceFlag]               TINYINT            CONSTRAINT [DF_tblEquipmentMaintenanceLog_InServiceFlag] DEFAULT ((1)) NOT NULL,
    [ChangeDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_ChangeDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [EstReturnToServiceDate]      DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_EstReturnToServiceDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [WorkOrder]                   NVARCHAR (20)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_WorkOrder] DEFAULT ('') NOT NULL,
    [Memo]                        NVARCHAR (1000)    CONSTRAINT [DF_tblEquipmentMaintenanceLog_Memo] DEFAULT ('') NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentMaintenanceLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentMaintenanceLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [EquipmentMaintenanceLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEquipmentMaintenanceLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [SiteGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [MaintenanceReasonGuid]       UNIQUEIDENTIFIER   NULL,
    [OperatorPersonnelGuid]       UNIQUEIDENTIFIER   NULL,
    CONSTRAINT [PK_tblEquipmentMaintenanceLog_GUID] PRIMARY KEY NONCLUSTERED ([EquipmentMaintenanceLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_CreatedDate]
    ON [dbo].[tblEquipmentMaintenanceLog]([CreatedDate] ASC);

GO
