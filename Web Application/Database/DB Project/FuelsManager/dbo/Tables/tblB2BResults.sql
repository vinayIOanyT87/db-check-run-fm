CREATE TABLE [dbo].[tblB2BResults] (
    [ResultsID]        INT                IDENTITY (1, 1) NOT NULL,
    [TransID]          NVARCHAR (64)      CONSTRAINT [DF_tblB2BResults_TransID] DEFAULT ('') NOT NULL,
    [Type]             CHAR (10)          CONSTRAINT [DF_tblB2BResults_Type] DEFAULT ('') NOT NULL,
    [Message]          NVARCHAR (100)     NULL,
    [DataError]        NVARCHAR (15)      CONSTRAINT [DF_tblB2BResults_DataError] DEFAULT ('') NOT NULL,
    [ErrorStatus]      INT                CONSTRAINT [DF_tblB2BResults_ErrorStatus] DEFAULT ((0)) NOT NULL,
    [Disputed]         INT                NULL,
    [Corrected]        INT                NULL,
    [ReceivedSentDate] DATETIMEOFFSET (7) NULL,
    [B2BResultGuid]    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblB2BResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]      ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblB2BResults_GUID] PRIMARY KEY NONCLUSTERED ([B2BResultGuid] ASC)
);






GO
CREATE NONCLUSTERED INDEX [IX_tblB2BResults]
    ON [dbo].[tblB2BResults]([ReceivedSentDate] ASC);

GO





GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblB2BResults_ResultsID]
    ON [dbo].[tblB2BResults]([ResultsID] ASC);

