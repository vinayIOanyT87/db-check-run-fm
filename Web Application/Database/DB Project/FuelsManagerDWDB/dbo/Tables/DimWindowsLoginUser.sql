CREATE TABLE [dbo].[DimWindowsLoginUser](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[WindowsLoginUserId] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_DimWindowsLoginUser] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF) 
)