/*

	DROP TABLE [lookup].[tblTimeZone]

*/
CREATE TABLE [lookup].[tblTimeZone] (
    [TimeZoneName]  NVARCHAR (100)     NOT NULL,
    [OffsetMinutes] INT                NOT NULL,
    [TimeZoneIndex] INT                IDENTITY (1, 1) NOT NULL,
    [TimeZoneGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]   DATETIMEOFFSET (7) NULL,
    [CreatedBy]     [dbo].[udtUserID]  NULL,
    [UpdatedDate]   DATETIMEOFFSET (7) NULL,
    [UpdatedBy]     [dbo].[udtUserID]  NULL,
    [_RowVersion]   ROWVERSION         NOT NULL,
    CONSTRAINT [PK_lookup_tblTimeZone] PRIMARY KEY NONCLUSTERED ([TimeZoneName] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTimeZone_TimeZoneIndex]
    ON [lookup].[tblTimeZone]([TimeZoneIndex] ASC);