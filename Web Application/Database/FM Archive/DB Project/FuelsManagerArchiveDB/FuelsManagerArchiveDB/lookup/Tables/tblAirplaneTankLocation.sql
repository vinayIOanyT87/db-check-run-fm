/*

	DROP TABLE [lookup].[tblAirplaneTankLocation]

*/

CREATE TABLE [lookup].[tblAirplaneTankLocation] (
    [TankLocationIndex] INT                NOT NULL,
    [TankLocationCode]  NVARCHAR (100)     NOT NULL,
    [TankLocationName]  NVARCHAR (100)     NULL,
    [TankLocationGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]       DATETIMEOFFSET (7) NULL,
    [CreatedBy]         [dbo].[udtUserID]  NULL,
    [UpdatedDate]       DATETIMEOFFSET (7) NULL,
    [UpdatedBy]         [dbo].[udtUserID]  NULL,
    [_RowVersion]       ROWVERSION         NOT NULL,
    [_ClusterIdx]       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAirplaneTankLocation] PRIMARY KEY NONCLUSTERED ([TankLocationIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAirplaneTankLocation_ClusterIdx]
    ON [lookup].[tblAirplaneTankLocation]([_ClusterIdx] ASC);