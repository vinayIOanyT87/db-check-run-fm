CREATE TABLE [dbo].[tblTankMaintenanceLog] (
    [TankID]                 NVARCHAR (50)      CONSTRAINT [DF_tblTankMaintenanceLog_TankID] DEFAULT ('') NOT NULL,
    [VesselType]             NVARCHAR (50)      CONSTRAINT [DF_tblTankMaintenanceLog_VesselType] DEFAULT ('') NOT NULL,
    [OperatorID]             NVARCHAR (50)      CONSTRAINT [DF_tblTankMaintenanceLog_OperatorID] DEFAULT ('') NOT NULL,
    [MaintenanceReason]      NVARCHAR (50)      CONSTRAINT [DF_tblTankMaintenanceLog_MaintenanceReason] DEFAULT ('Is In Service') NOT NULL,
    [InServiceFlag]          TINYINT            CONSTRAINT [DF_tblTankMaintenanceLog_InServiceFlag] DEFAULT ((1)) NOT NULL,
    [ChangeDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankMaintenanceLog_ChangeDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [EstReturnToServiceDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankMaintenanceLog_EstReturnToServiceDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [WorkOrder]              NVARCHAR (20)      CONSTRAINT [DF_tblTankMaintenanceLog_WorkOrder] DEFAULT ('') NOT NULL,
    [Memo]                   NVARCHAR (1000)    CONSTRAINT [DF_tblTankMaintenanceLog_Memo] DEFAULT ('') NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankMaintenanceLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_tblTankMaintenanceLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTankMaintenanceLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]              [dbo].[udtUserID]  CONSTRAINT [DF_tblTankMaintenanceLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [TankMaintenanceLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTankMaintenanceLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [SiteGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [LookupVesselTypeIndex]  INT                CONSTRAINT [DF_tblTankMaintenanceLog_LookupVesselTypeIndex] DEFAULT ((0)) NOT NULL,
    [MaintenanceReasonGuid]  UNIQUEIDENTIFIER   NULL,
    [OperatorPersonnelGuid]  UNIQUEIDENTIFIER   NULL,
    [TankGuid]               UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTankMaintenanceLog_GUID] PRIMARY KEY NONCLUSTERED ([TankMaintenanceLogGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTankMaintenanceLog_CreatedDate]
    ON [dbo].[tblTankMaintenanceLog]([CreatedDate] ASC);

GO
