/*

	DROP TABLE [lookup].[tblDayOfWeek]

*/
CREATE TABLE [lookup].[tblDayOfWeek] (
    [DayOfWeekIndex] INT                NOT NULL,
    [DayOfWeekCode]  NVARCHAR (100)     NOT NULL,
    [DayOfWeekName]  NVARCHAR (100)     NULL,
    [DayOfWeekGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]    DATETIMEOFFSET (7) NULL,
    [CreatedBy]      [dbo].[udtUserID]  NULL,
    [UpdatedDate]    DATETIMEOFFSET (7) NULL,
    [UpdatedBy]      [dbo].[udtUserID]  NULL,
    [_RowVersion]    ROWVERSION         NOT NULL,
    [_ClusterIdx]    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblDayOfWeek] PRIMARY KEY NONCLUSTERED ([DayOfWeekIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblDayOfWeek_DayOfWeekGuid]
    ON [lookup].[tblDayOfWeek]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDayOfWeek_ClusterIdx]
    ON [lookup].[tblDayOfWeek]([_ClusterIdx] ASC);