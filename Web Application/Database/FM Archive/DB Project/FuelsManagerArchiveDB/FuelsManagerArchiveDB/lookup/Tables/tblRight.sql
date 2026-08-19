/*

	DROP TABLE [lookup].[tblRight]

*/
CREATE TABLE [lookup].[tblRight] (
    [RightIndex]  INT                NOT NULL,
    [RightCode]   NVARCHAR (100)     NOT NULL,
    [RightName]   NVARCHAR (100)     NULL,	
    [RightGuid]   UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate] DATETIMEOFFSET (7) NULL,
    [CreatedBy]   [dbo].[udtUserID]  NULL,
    [UpdatedDate] DATETIMEOFFSET (7) NULL,
    [UpdatedBy]   [dbo].[udtUserID]  NULL,
	[RightDescription] NVARCHAR (2000) NULL,
    [_RowVersion] ROWVERSION         NOT NULL,
    [_ClusterIdx] BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblRight] PRIMARY KEY NONCLUSTERED ([RightIndex] ASC)
);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblRight_ClusterIdx]
    ON [lookup].[tblRight]([_ClusterIdx] ASC);