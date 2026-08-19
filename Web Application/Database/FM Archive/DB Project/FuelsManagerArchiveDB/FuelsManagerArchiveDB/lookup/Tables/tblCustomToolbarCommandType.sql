/*

	DROP TABLE [lookup].[tblCustomToolbarCommandType]

*/
CREATE TABLE [lookup].[tblCustomToolbarCommandType] (
    [CustomToolbarCommandTypeIndex] INT					NOT NULL,
    [CustomToolbarCommandTypeCode]  NVARCHAR (100)		NOT NULL,
    [CustomToolbarCommandTypeName]  NVARCHAR (100)		NULL,
    [LookupCustomToolbarTypeIndex]  INT					NOT NULL,
    [CustomToolbarCommandTypeGuid]  UNIQUEIDENTIFIER	NULL,
    [CreatedDate]					DATETIMEOFFSET (7)	NULL,
    [CreatedBy]						[dbo].[udtUserID]	NULL,
    [UpdatedDate]					DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]						[dbo].[udtUserID]	NULL,
    [_RowVersion]					ROWVERSION			NOT NULL,
    [Default]						BIT					NULL, 
    [DefaultOrder]					INT					NULL, 
    [ImageSource]					NVARCHAR(100)		NULL, 
    [_ClusterIdx]					BIGINT				IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblCustomToolbarCommandType] PRIMARY KEY NONCLUSTERED ([CustomToolbarCommandTypeIndex] ASC)
);
GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblCustomToolbarCommandType_CustomToolbarCommandTypeGuid]
    ON [lookup].[tblCustomToolbarCommandType]([CustomToolbarCommandTypeGuid] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCustomToolbarCommandType_ClusterIdx]
    ON [lookup].[tblCustomToolbarCommandType]([_ClusterIdx] ASC);