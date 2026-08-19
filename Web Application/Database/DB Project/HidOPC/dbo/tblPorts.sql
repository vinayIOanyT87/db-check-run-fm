CREATE TABLE [dbo].[tblPorts] (
    [Index]    INT          IDENTITY (1, 1) NOT NULL,
    [ID]       NVARCHAR (7) NOT NULL,
    [Baud]     INT          NOT NULL,
    [DataBits] INT          NOT NULL,
    [Parity]   INT          NOT NULL,
    [StopBits] INT          NOT NULL,
    CONSTRAINT [PK_tblPorts] PRIMARY KEY CLUSTERED ([Index] ASC) WITH (FILLFACTOR = 70)
);
