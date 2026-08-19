CREATE TYPE [dbo].[AlarmAndEventLogType] AS TABLE (
    [SiteGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [Source]         NVARCHAR (120)     NOT NULL,
    [Alarm]          BIT                NOT NULL,
    [ID]             NVARCHAR (120)     NOT NULL,
    [AssociatedData] NVARCHAR (MAX)     NOT NULL,
    [Acknowledged]   BIT                NOT NULL,
    [CreatedDate]    DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]      [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]    DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]      [dbo].[udtUserID]  NOT NULL);

