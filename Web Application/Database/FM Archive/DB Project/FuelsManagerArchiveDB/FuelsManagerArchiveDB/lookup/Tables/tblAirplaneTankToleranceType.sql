/*

	DROP TABLE [lookup].[tblAirplaneTankToleranceType]

*/
CREATE TABLE [lookup].[tblAirplaneTankToleranceType] (
    [TankToleranceTypeIndex] SMALLINT           NOT NULL,
    [TankToleranceTypeCode]  NVARCHAR (100)     NOT NULL,
    [TankToleranceTypeName]  NVARCHAR (100)     NULL,
    [TankToleranceTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAirplaneTankToleranceType] PRIMARY KEY NONCLUSTERED ([TankToleranceTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAirplaneTankToleranceType_ClusterIdx]
    ON [lookup].[tblAirplaneTankToleranceType]([_ClusterIdx] ASC);