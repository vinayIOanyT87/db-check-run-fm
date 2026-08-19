-- ===========================================================================================
-- Author:		<Author,,Peters George C>
-- Create Date:	<Create Date,,09-17-2012>
-- Description:	<Description,,This stored procedure is called by the SyncFramework prior each
-- call to the Server SyncProvider's GetChanges() method.  This particular implementation provides
-- batching support.  This stored procedure calculates the low end (last_received_anchor), 
-- upper end (new_received_anchor) and the required number of subsequent calls to GetChanges() 
-- in order to retrieve all the change records up through to max_received_anchor 
-- (most recent change version from the server)
--
-- Note: Because the commands are called by the SyncFramework, passing in 
-- specific parameters, providing an "@IsBatching" parameter requires a lower level flag which
-- we can implement at a later date to combine this sproc with the batching version.
--
-- This stored procedure functions like a "sliding window".  As each iteration of GetChanges() 
-- completes, this stored procedure will calculate:
-- 1) The new range of records to retreive (last_received_anchor -> new_received_anchor)
-- 2) Update the number of batches required to retreive all records (decrements the batch count) so
--    the SyncFramework will know how many more iterations to make.
-- 3) Identify when we're on the last batch and adjust the return values to notify the SyncFramework.
-- ===========================================================================================

/* {CheckPoint: CREATING STORED PROCEDURE: [sync].[usp_GetNewClientBatchAnchor] } */
CREATE PROCEDURE [sync].[usp_GetNewClientBatchAnchor] (
        @sync_max_client_anchor bigint
		,@sync_last_received_anchor bigint
        ,@sync_batch_size int
        ,@sync_max_received_anchor bigint output
        ,@sync_new_received_anchor bigint output
        ,@sync_batch_count int output)
AS
BEGIN
	IF @sync_batch_size <= 0
	BEGIN
		SET @sync_batch_size = 1000
	END

	IF @sync_max_client_anchor IS NOT NULL
	BEGIN
		SET @sync_max_received_anchor = @sync_max_client_anchor;
	END

	IF @sync_max_received_anchor IS NULL
	BEGIN
		-- This indicatest the LAST anchor used since min_active_rowversion() represents the NEXT one that
		-- will be given out.
		SET @sync_max_received_anchor = CONVERT(bigint, min_active_rowversion())-1
	END

	-- Basic batching model.  (First Pass = start of synchronization session)
	-- First Pass (assuming nothing has ever been previously synchronized for anything in the current SyncGroup context):
	--   We will set our sync engine to retrieving everything from the last anchor (0) up to and including the 
	--   sync_new_received_anchor value.  We'll set this equal to the batch size (ie: default of 1000).
	--   Then, we'll calculate the number of batches (segments) required to retrieve ALL the 
	--   changes up to the last known change (sync_max_received_anchor = min_active_rowversion() - 1)
	--
	--   This guides the SyncFramework engine so that it knows how many times it will call GetChanges()
	--   on the Server Provider to retreive all the records.
	--
	-- First Pass (assuming we've previously synchronized data for the current SyncGroup context):
	--   We will set our sync engine to retrieving everything from the last anchor value (determined by 
	--   querying tblSynchronizationAnchor for the current table) up to and including the 
	--   sync_new_received_anchor value.  We'll offset this value to be the batch size + last_anchor_value.
	--   (ex: last_received_anchor = 995, batch size = 1000, new_received_anchor = 1995)
	--   Then, we'll calculate the number of batches (segments) required to retrieve ALL the 
	--   changes up to the last known change (sync_max_received_anchor = min_active_rowversion() - 1)
	--
	-- Subsequent Passes:
	--   The max_received_anchor that was set when we first started the synchronization session will be passed back in
	--   by the SyncFramework.  This prevents the engine from continuously picking up a never ending list of changes
	--   during the synchronization session and throwing off the number of batches required.
	--
	--   The new_received_anchor (the top end of the next batch) is set by taking the last received anchor value
	--	 and adding the size of the batch (see example in previous section)
	--
	-- All Passes:
	--
	--   when we  
	--
	IF @sync_last_received_anchor IS NULL OR @sync_last_received_anchor = 0
	BEGIN
		SET @sync_new_received_anchor = @sync_batch_size

		IF @sync_batch_count <= 0
		BEGIN
			SET @sync_batch_count = (@sync_max_received_anchor /  @sync_batch_size) + 1
		END
	END
	ELSE
	BEGIN
		-- As we move through the synchronization session/batches, we need to adjust our "sliding window".
		-- Update the batch_count so that the SyncFramework knows how many additional calls to GetChanges()
		-- are needed to retreive all of the records.
		SET @sync_new_received_anchor = @sync_last_received_anchor + @sync_batch_size
		
		IF @sync_batch_count <= 0
		BEGIN
			SET @sync_batch_count = (@sync_max_received_anchor /  @sync_batch_size) -
									(@sync_new_received_anchor /  @sync_batch_size) + 1
		END
	END

	-- Check if this is the last batch, the above logic would have calculated the new_received_anchor based
	-- on the batch size which could put it past the "max_received_anchor" (represents our stopping point).
	-- If the new anchor >= the max anchor, set the new anchor = max anchor (so that we'll stop)
	-- If the batch count calculated out to be <= 0, set the batch count to 1 so the SyncFramework will make one 
	-- final call to GetChanges() in order to retreive the remaining records.  (Note: Keep in mind that the 
	-- previous logic in the stored procedure could have moved the new received anchor past the max received anchor 
	-- which is why we just check it here and fix it up)
	IF @sync_new_received_anchor >= @sync_max_received_anchor
	BEGIN
		SET @sync_new_received_anchor = @sync_max_received_anchor

		IF @sync_batch_count <= 0
			SET @sync_batch_count = 1
	END
END