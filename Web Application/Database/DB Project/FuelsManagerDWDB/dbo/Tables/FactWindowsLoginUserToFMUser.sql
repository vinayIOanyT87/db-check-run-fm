CREATE TABLE [dbo].[FactWindowsLoginUserToFMUser](
	[SKey] [int] IDENTITY(1,1) NOT NULL,
	[WindowsLoginUserSKey] [int] NOT NULL,
	[FMUserSKey] [int] NOT NULL,
 CONSTRAINT [PK_FactWindowsLoginUserToFMUser] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF) 
)