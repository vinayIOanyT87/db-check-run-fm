/*

	DROP TABLE [lookup].[tblActivationStatus]

*/
CREATE TABLE [lookup].[tblActivationStatus] (
    [ActivationStatusIndex] INT                NOT NULL,
    [ActivationStatusCode]  NVARCHAR (100)     NULL,
    [ActivationStatusName]  NVARCHAR (100)     NULL,
    [ActivationStatusGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblActivationStatus] PRIMARY KEY NONCLUSTERED ([ActivationStatusIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblActivationStatus_ClusterIdx]
    ON [lookup].[tblActivationStatus]([_ClusterIdx] ASC);