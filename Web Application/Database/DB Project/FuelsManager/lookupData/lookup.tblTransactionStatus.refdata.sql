SET NOCOUNT ON

PRINT 'Processing Static Reference Data for table [lookup].[tblTransactionStatus]'
PRINT ''

DECLARE @TransactionStatusRefDataInserted bigint
DECLARE @TransactionStatusRefDataUpdated bigint
DECLARE @TransactionStatusRefDataDeleted bigint

SET @TransactionStatusRefDataInserted = 0
SET @TransactionStatusRefDataUpdated = 0
SET @TransactionStatusRefDataDeleted = 0

DECLARE @tblTransactionStatusRefData TABLE
(
	[ActionType] VARCHAR (50)
    ,[OldTransactionStatusIndex] INT
    ,[TransactionStatusIndex] INT
    ,[OldTransactionStatusCode] NVARCHAR (100)
    ,[TransactionStatusCode] NVARCHAR (100)
	,[OldTransactionStatusName]  NVARCHAR (100)
	,[TransactionStatusName]  NVARCHAR (100)
	,[OldTransactionStatusGuid]  NVARCHAR (100)
	,[TransactionStatusGuid]  NVARCHAR (100)
    ,[OldCreatedDate] DATETIMEOFFSET (7)
    ,[CreatedDate] DATETIMEOFFSET (7)
    ,[OldCreatedBy] NVARCHAR (255)
    ,[CreatedBy] NVARCHAR (255)
    ,[OldUpdatedDate] DATETIMEOFFSET (7)
    ,[UpdatedDate] DATETIMEOFFSET (7)
    ,[OldUpdatedBy] NVARCHAR (255)
    ,[UpdatedBy] NVARCHAR (255)
);

; MERGE INTO [lookup].[tblTransactionStatus] AS Target
USING (VALUES
(-1, N'None', N'None', N'e9390759-d21b-437e-b25e-ecd273465964', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(0, N'Completed', N'Completed', N'1ba5e0bb-b7d9-4e79-a57b-c44c32c25309', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(1, N'InProgress', N'InProgress', N'9dcaddf8-0cbc-4ad4-b3b2-500e8b36f0f3', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(2, N'Dispatched', N'Dispatched', N'0fc717ca-6f16-48f6-a4e6-c7a4238e786c', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(3, N'Requested', N'Requested', N'39a108d4-4186-4ced-a41d-20c1956dd9b9', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(4, N'Closed', N'Closed', N'3cda8c9a-8985-4366-bbc0-d122a378ebaf', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(5, N'OnHold', N'OnHold', N'0313b432-51b2-428d-a547-a048827a8c47', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(6, N'Scheduled', N'Scheduled', N'9946af7a-01c2-4f9f-a6b7-b9344f0c84f2', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(7, N'Cancelled', N'Cancelled', N'89629a98-9dc6-4e73-9eaf-e91a0d88c83e', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(8, N'Acknowledged', N'Acknowledged', N'7e3a205a-4deb-4120-8159-050df8751758', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(9, N'LoadPending', N'LoadPending', N'254502d6-423e-4f80-9253-9f2e2452fd8f', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(10, N'WeighOutPending', N'WeighOutPending', N'5d5b53e4-2451-4989-a6d8-2ec617115991', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(11, N'Posted', N'Posted', N'34f3eb86-a97e-4d99-8404-7bf71bef2c3e', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(12, N'Arrived', N'Arrived', N'ef205a64-892e-4444-95a9-453359818a7f', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(13, N'Started', N'Started', N'7f213543-92b7-4b6f-aa15-46bc8fe6e445', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(14, N'Stopped', N'Stopped', N'6b07a4c6-68f1-4bc7-aca0-bfad7804e18d', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'6/18/2012 1:03:42 PM +00:00', N'Administrator')
,(15, N'Suspended', N'Suspended', N'd6ec7b86-c625-4913-a6c5-9d0f26fd3c51', N'6/18/2012 1:03:42 PM +00:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(16, N'Pending', N'Pending', N'0326416e-a4d1-451f-81e0-7a5c50e728c0', N'11/28/2012 12:37:27 PM -05:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(17, N'Updated', N'Updated', N'9469d22b-bdce-4e43-a7e1-8c55f3efdc09', N'11/28/2012 12:37:27 PM -05:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(18, N'Superseded', N'Superseded', N'3f4f252d-d2d2-4099-85bb-0792a0ef8491', N'3/12/2013 11:04:54 AM -04:00', N'Administrator', N'3/12/2013 11:04:54 AM -04:00', N'Administrator')
,(19, N'Enterprise', N'Enterprise', N'96219547-2C0D-4CE5-B5FD-EB68A231B936', N'11/26/2018 11:04:54 AM -04:00', N'Administrator', N'11/26/2018 11:04:54 AM -04:00', N'Administrator')
,(20, N'Pushed', N'Pushed', N'3145346A-7DB1-4740-9A6C-B84DAFBE1D92', N'2/16/2023 11:04:54 AM -04:00', N'Administrator', N'2/16/2023 11:04:54 AM -04:00', N'Administrator')
,(21, N'Pulled', N'Pulled', N'8CB0AA9C-A3B5-4C3C-B3EB-D2326256F10A', N'2/16/2023 11:04:54 AM -04:00', N'Administrator', N'2/16/2023 11:04:54 AM -04:00', N'Administrator')
) AS Source ([TransactionStatusIndex], [TransactionStatusCode], [TransactionStatusName], [TransactionStatusGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
ON (Target.[TransactionStatusIndex] = Source.[TransactionStatusIndex])
WHEN MATCHED AND (Target.[TransactionStatusCode] <> Source.[TransactionStatusCode] 
					OR Target.[TransactionStatusName] <> Source.[TransactionStatusName]
					OR Target.[TransactionStatusGuid] <> Source.[TransactionStatusGuid]) THEN
	UPDATE SET 
				[TransactionStatusCode] = Source.[TransactionStatusCode]
				, [TransactionStatusName] = Source.[TransactionStatusName]
				, [TransactionStatusGuid] = Source.[TransactionStatusGuid]
				, [CreatedDate] = Source.[CreatedDate]
				, [CreatedBy] =	Source.[CreatedBy]
				, [UpdatedDate] = Source.[UpdatedDate]
				, [UpdatedBy] =	Source.[UpdatedBy]
WHEN NOT MATCHED THEN
	INSERT ([TransactionStatusIndex], [TransactionStatusCode], [TransactionStatusName], [TransactionStatusGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
		VALUES (Source.[TransactionStatusIndex],Source.[TransactionStatusCode],Source.[TransactionStatusName],Source.[TransactionStatusGuid],Source.[CreatedDate],Source.[CreatedBy],Source.[UpdatedDate],Source.[UpdatedBy])
OUTPUT
   $action AS ActionType,
   deleted.[TransactionStatusIndex],
   inserted.[TransactionStatusIndex],
   deleted.[TransactionStatusCode],
   inserted.[TransactionStatusCode],
   deleted.[TransactionStatusName],
   inserted.[TransactionStatusName],
   deleted.[TransactionStatusGuid],
   inserted.[TransactionStatusGuid],
   deleted.[CreatedDate],
   inserted.[CreatedDate],
   deleted.[CreatedBy],
   inserted.[CreatedBy],
   deleted.[UpdatedDate],
   inserted.[UpdatedDate],
   deleted.[UpdatedBy],
   inserted.[UpdatedBy]
INTO @tblTransactionStatusRefData;

SELECT @TransactionStatusRefDataInserted = COUNT(*) FROM @tblTransactionStatusRefData WHERE ActionType IN ( 'INSERT' );
SELECT @TransactionStatusRefDataUpdated = COUNT(*) FROM @tblTransactionStatusRefData WHERE ActionType IN ( 'UPDATE' )
SELECT @TransactionStatusRefDataDeleted = COUNT(*) FROM @tblTransactionStatusRefData WHERE ActionType IN ( 'DELETE' )

IF (@TransactionStatusRefDataInserted > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @TransactionStatusRefDataInserted) + ' NEW RECORDS INSERTED INTO [lookup].[tblTransactionStatus] **'
	PRINT ''
END

IF (@TransactionStatusRefDataUpdated > 0)
BEGIN
	PRINT '** ' + CONVERT(NVARCHAR(25), @TransactionStatusRefDataUpdated) + ' EXISTING RECORDS UPDATED IN [lookup].[tblTransactionStatus] **'
	PRINT ''
	SELECT * FROM @tblTransactionStatusRefData WHERE ActionType IN ( 'UPDATE' );
END

SET NOCOUNT OFF
