CREATE PROCEDURE [dbo].[xsp_LedgerCalculator](@BeginDate		[smalldatetime], 
										     @EndDate			[smalldatetime],
								             @ProductGuid		uniqueidentifier ,
								             @ManagerGuid		uniqueidentifier ,
								             @OwnerGuid			uniqueidentifier ,
								             @SelectedSiteGuid	uniqueidentifier ,
								             @UserGuid			uniqueidentifier ,
								             @LedgerRequest		[int],
								             @ReportLedger		[int]--,
								             --@TankGuid			uniqueidentifier ,
								             --@SystemEdition		[int])
											 )
AS EXTERNAL NAME [LedgerCore].[LRStoredProcedureClass].[xsp_LedgerCalculator]
GO