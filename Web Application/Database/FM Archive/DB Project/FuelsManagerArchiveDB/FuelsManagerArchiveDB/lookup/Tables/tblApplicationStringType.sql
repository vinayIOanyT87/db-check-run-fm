/*

	DROP TABLE [lookup].[tblApplicationStringType]

*/

CREATE TABLE [lookup].[tblApplicationStringType] (
    [ApplicationStringTypeIndex] INT                NOT NULL,
    [ApplicationStringTypeCode]  NVARCHAR (100)     NOT NULL,
    [ApplicationStringTypeName]  NVARCHAR (100)     NULL,
    [ApplicationStringTypeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) NULL,
    [CreatedBy]                  [dbo].[udtUserID]  NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblApplicationStringType] PRIMARY KEY NONCLUSTERED ([ApplicationStringTypeIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblApplicationStringType_ClusterIdx]
    ON [lookup].[tblApplicationStringType]([_ClusterIdx] ASC);