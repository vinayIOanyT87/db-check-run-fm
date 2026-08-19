CREATE TABLE [dbo].[DimTime](
	[SKey] [int] NOT NULL,
	[Time] [varchar](11) NOT NULL,
	[Time24] [varchar](8) NOT NULL,
	[HourName] [varchar](5) NOT NULL,
	[MinuteName] [varchar](8) NOT NULL,
	[HourNumber] [tinyint] NOT NULL,
	[Hour24] [int] NOT NULL,
	[MinuteNumber] [tinyint] NOT NULL,
	[SecondNumber] [tinyint] NOT NULL,
	[AMPM] [char](2) NOT NULL,
	[ElapsedMinutes] [int] NOT NULL,
	[ElapsedSeconds] [int] NOT NULL,
 CONSTRAINT [PK_DimTime_SKey] PRIMARY KEY CLUSTERED 
(
	[SKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]