CREATE TABLE [dbo].[StoredProcedureTemplate] (
    [TemplateNumber] INT           IDENTITY (1, 1) NOT NULL,
    [TemplateCode]   VARCHAR (50)  NOT NULL,
    [Template]       VARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_StoredProcedureTemplate] PRIMARY KEY CLUSTERED ([TemplateNumber] ASC) WITH (FILLFACTOR = 70)
);

