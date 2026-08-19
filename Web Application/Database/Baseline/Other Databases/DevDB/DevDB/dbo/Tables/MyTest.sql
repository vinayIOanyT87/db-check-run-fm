CREATE TABLE [dbo].[MyTest] (
    [RowNum] INT          IDENTITY (1, 1) NOT NULL,
    [Col1]   VARCHAR (10) NULL,
    [Col2]   VARCHAR (10) NULL,
    PRIMARY KEY CLUSTERED ([RowNum] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_MyTest_01]
    ON [dbo].[MyTest]([Col2] ASC);

