CREATE PROCEDURE [dbo].[usp_MovementDisableInterlockedMovements]
	@ActivatedMovementGuid uniqueidentifier
AS
BEGIN
	BEGIN TRY
		declare @ActivatedNodes table (TransferDirection nvarchar(20), NodeGuid uniqueidentifier)

		insert into @ActivatedNodes (TransferDirection, NodeGuid)
		select MD.TD.value('TransferDirection[1]','nvarchar(20)') TransferDirection
			, MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') MovementNodeGuid
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
		where 1=1
		 and pp.ID = 'Movement Settings'
		 and pp.PointGuid = @ActivatedMovementGuid

		declare @MovementsToDisable table (MovementGuid uniqueidentifier)

		insert into @MovementsToDisable (MovementGuid)
		select distinct pp.PointGuid
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
			inner join @ActivatedNodes an on MD.TD.value('TransferDirection[1]','nvarchar(20)') = an.TransferDirection and MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') = an.NodeGuid
			inner join tblPointTag pt on pp.PointGuid = pt.PointGuid
		where pp.PointGuid <> @ActivatedMovementGuid
			and pt.ID = 'Status' and cast(pt.Value as nvarchar(max)) = '<MovementStatus>Inactive</MovementStatus>'

		update tblPointTag
		set Value = '<MovementStatus>Disabled</MovementStatus>', ServerTimeStamp = SYSUTCDATETIME(), SourceTimeStamp = SYSUTCDATETIME()
		where ID = 'Status' and PointGuid in (select MovementGuid from @MovementsToDisable)

		update tblPointTag
		set Value = '<MovementCommand>Disable</MovementCommand>', ServerTimeStamp = SYSUTCDATETIME(), SourceTimeStamp = SYSUTCDATETIME()
		where ID = 'Command' and PointGuid in (select MovementGuid from @MovementsToDisable)

		select * from @MovementsToDisable
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