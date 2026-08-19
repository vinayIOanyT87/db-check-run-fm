CREATE PROCEDURE [dbo].[usp_MovementCheckForActiveInterlockedMovements]
	@ActivatingMovementGuid uniqueidentifier
AS
BEGIN
	BEGIN TRY
		set nocount on
		declare @ActivatingNodes table (TransferDirection nvarchar(20), NodeGuid uniqueidentifier)

		insert into @ActivatingNodes (TransferDirection, NodeGuid)
		select MD.TD.value('TransferDirection[1]','nvarchar(20)') TransferDirection
			, MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') MovementNodeGuid
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
		where 1=1
		 and pp.ID = 'Movement Settings'
		 and pp.PointGuid = @ActivatingMovementGuid

		declare @ActiveInterlockedMovements table (MovementGuid uniqueidentifier, MovementID nvarchar(30))

		insert into @ActiveInterlockedMovements (MovementGuid, MovementID)
		select distinct pp.PointGuid, p.ID
		from tblPointProperty pp
			cross apply [Value].nodes('/MovementModuleSettings/MovementNodeDataList/MovementNodeData') as MD(TD)
			inner join @ActivatingNodes an on MD.TD.value('TransferDirection[1]','nvarchar(20)') = an.TransferDirection and MD.TD.value('MovementNodeGuid[1]','uniqueidentifier') = an.NodeGuid
			inner join tblPointTag pt on pp.PointGuid = pt.PointGuid
			inner join tblPoint p on pt.PointGuid = p.PointGuid
		where pp.PointGuid <> @ActivatingMovementGuid
			and pt.ID = 'Status' and pt.[Value].value('MovementStatus[1]','nvarchar(20)') = 'Active'

		select * from @ActiveInterlockedMovements ORDER BY MovementID
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