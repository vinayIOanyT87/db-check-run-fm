/*

	DROP TABLE [lookup].[tblEngineeringUnit]

*/

CREATE TABLE [lookup].[tblEngineeringUnit] (
    [EngineeringUnitIndex]        INT                NOT NULL,
    [EngineeringUnitCode]         NVARCHAR (100)     NOT NULL,
    [EngineeringUnitName]         NVARCHAR (100)     NULL,
    [EngineeringUnitAbbreviation] NVARCHAR (100)     NULL,
    [EngineeringUnitGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) NULL,
    [CreatedBy]                   [dbo].[udtUserID]  NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblEngineeringUnit] PRIMARY KEY NONCLUSTERED ([EngineeringUnitIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblEngineeringUnit_EngineeringUnitGuid]
    ON [lookup].[tblEngineeringUnit]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEngineeringUnit_ClusterIdx]
    ON [lookup].[tblEngineeringUnit]([_ClusterIdx] ASC);