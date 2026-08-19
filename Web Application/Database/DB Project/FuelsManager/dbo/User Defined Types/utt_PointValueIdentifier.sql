CREATE TYPE [dbo].[utt_PointValueIdentifier] AS TABLE
(
	[Guid] UNIQUEIDENTIFIER NOT NULL,
	[PropertyId] NVARCHAR(50),
	[ValueType] TINYINT
)
