/*

	DROP TABLE [lookup].[tblMailServerConnectMode]

*/
CREATE TABLE [lookup].[tblMailServerConnectMode] (
    [MailServerConnectModeIndex] TINYINT            NOT NULL,
    [MailServerConnectModeCode]  NVARCHAR (100)     NOT NULL,
    [MailServerConnectModeName]  NVARCHAR (100)     NULL,
    [MailServerConnectModeGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) NULL,
    [CreatedBy]                  [dbo].[udtUserID]  NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblMailServerConnectMode] PRIMARY KEY NONCLUSTERED ([MailServerConnectModeIndex] ASC)
);
GO
CREATE NONCLUSTERED INDEX [IXU_lookup_tblMailServerConnectMode_MailServerConnectModeGuid]
    ON [lookup].[tblMailServerConnectMode]([CreatedDate] ASC);
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMailServerConnectMode_ClusterIdx]
    ON [lookup].[tblMailServerConnectMode]([_ClusterIdx] ASC);