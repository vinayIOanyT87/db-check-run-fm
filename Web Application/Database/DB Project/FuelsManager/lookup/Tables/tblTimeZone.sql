CREATE TABLE [lookup].[tblTimeZone] (
    [TimeZoneName]  NVARCHAR (100)     NOT NULL,
    [OffsetMinutes] INT                NOT NULL,
    [TimeZoneIndex] INT                IDENTITY (1, 1) NOT NULL,
    [TimeZoneGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblTimeZone_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tbltblTimeZone_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tbltblTimeZone_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]   DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tbltblTimeZone_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]     [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tbltblTimeZone_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]   ROWVERSION         NOT NULL,
    CONSTRAINT [PK_lookup_tblTimeZone] PRIMARY KEY NONCLUSTERED ([TimeZoneName] ASC)
);


 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTimeZone_TimeZoneIndex]
    ON [lookup].[tblTimeZone]([TimeZoneIndex] ASC);

