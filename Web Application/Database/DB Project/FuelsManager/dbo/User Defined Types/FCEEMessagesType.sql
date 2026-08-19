CREATE TYPE [dbo].[FCEEMessagesType] AS TABLE
(
	[ImeiNumber] [NVarChar] (15) NOT NULL,
	[TimeStamp] [datetimeoffset] NOT NULL,
	[MsgType] [INT] NOT NULL,
	[Idx]	[INT] NOT NULL,
	[Device] [INT]	NULL,
	[BinaryData] [VARBINARY] (max) NULL,
	[EdgeData] [NVarChar] (max) NULL,
	[Validity] [BIT] NULL
)
