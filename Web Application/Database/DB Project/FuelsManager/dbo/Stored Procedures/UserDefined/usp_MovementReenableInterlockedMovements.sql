CREATE PROCEDURE [dbo].[usp_MovementReenableInterlockedMovements]
	@DeactivatedMovementGuid uniqueidentifier
AS
BEGIN
	BEGIN TRY
		declare @DeactivatedNodes table (TransferDirection nvarchar(20), NodeGuid uniqueidentifier)

		insert into @DeactivatedNodes (TransferDirection, NodeGuid)
		select MD.TD.value('TransferDirection[1]','nvarchar(20)') TransferDirection
			, MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') MovementNodeGuid
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
		where 1=1
		and pp.ID = 'Movement Settings'
		and pp.PointGuid = @DeactivatedMovementGuid

		declare @MovementsToReenable table (MovementGuid uniqueidentifier, MovementId nvarchar(30))

		insert into @MovementsToReenable (MovementGuid, MovementId)
		select distinct pp.PointGuid, p.ID
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
			inner join @DeactivatedNodes an on MD.TD.value('TransferDirection[1]','nvarchar(20)') = an.TransferDirection and MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') = an.NodeGuid
			inner join tblPointTag pt on pp.PointGuid = pt.PointGuid
			inner join tblPoint p on pp.PointGuid = p.PointGuid
		where pp.PointGuid <> @DeactivatedMovementGuid
			and pt.ID = 'Status' and cast(pt.Value as nvarchar(max)) = '<MovementStatus>Disabled</MovementStatus>'

		-- Unlike the Disable procedure, the Reenable procedure has to ensure that the movements to reenable
		-- do not include movement nodes that are currently active on movements other than the one
		-- we are deactivating.  First get those active nodes.
		declare @NodesActiveOnOtherNodes table (MovementNodeGuid uniqueidentifier, TransferDirection nvarchar(20))
		insert into @NodesActiveOnOtherNodes (MovementNodeGuid, TransferDirection)
		select MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') MovementNodeGuid,
			MD.TD.value('TransferDirection[1]','nvarchar(20)') TransferDirection
		from tblPointProperty pp
			cross apply pp.[Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
			inner join tblPointTag pt on pt.PointGuid = pp.PointGuid
		where 1=1
		and pp.ID = 'Movement Settings'
		and pt.ID = 'Status'
		and pt.Value.value('MovementStatus[1]','nvarchar(20)') = 'Active'
		and pp.PointGuid <> @DeactivatedMovementGuid

		-- now prune the list of movements to reenable based on nodes on
		-- other active movements
		delete from @MovementsToReenable
		from @MovementsToReenable mr
			inner join tblPointProperty pp on mr.MovementGuid = pp.PointGuid
			cross apply pp.[Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
			inner join @NodesActiveOnOtherNodes nn on MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') = nn.MovementNodeGuid
				and MD.TD.value('TransferDirection[1]','nvarchar(20)') = nn.TransferDirection

		update tblPointTag
		set Value = '<MovementStatus>Inactive</MovementStatus>', ServerTimeStamp = SYSUTCDATETIME(), SourceTimeStamp = SYSUTCDATETIME()
		where ID = 'Status' and PointGuid in (select MovementGuid from @MovementsToReenable)

		update tblPointTag
		set Value = '<MovementCommand>Stop</MovementCommand>', ServerTimeStamp = SYSUTCDATETIME(), SourceTimeStamp = SYSUTCDATETIME()
		where ID = 'Command' and PointGuid in (select MovementGuid from @MovementsToReenable)

		select * from @MovementsToReenable
	END TRY
	BEGIN CATCH
		DECLARE	@_ErrMessage NVARCHAR(2048)
				, @_ErrNumber INT
				, @_ErrProcName NVARCHAR(126)
				, @_ErrLineNumber INT;
		SET @_ErrMessage = ERROR_MESSAGE();
		SET @_ErrNumber = ERROR_NUMBER();
		SET @_ErrProcName= ERROR_PROCEDURE();
		SET @_ErrLineNumber = ERROR_LINE();
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)
						+ 'Procedure Name: [dbo].usp_MovementDisableInterlockedMovements' + CHAR(13)+CHAR(10)
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);
		RAISERROR(@_ErrMessage,16,1);
	END CATCH
END