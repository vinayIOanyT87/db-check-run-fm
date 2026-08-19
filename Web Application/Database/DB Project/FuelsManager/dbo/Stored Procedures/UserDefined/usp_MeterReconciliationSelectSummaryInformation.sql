
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:
This procedure returns meter start and stop information along with other data for the meter reconciliation summary screen 
for each meter in the system for a specified date.

@InventoryDate and @SiteGuid are required input parameters. The other parameters are used to refine the search criteria and are optional.

Meters are returned for a specific site only. The procedure does not return meters for child sites.

    Modification History:
    Date         Version     By          Description
    ----------   -------     ----        -------------
    04/24/2012   1.0.000     Ryan Hill   --
    07/08/2019   1.0.001     Jay R       Rewrote procedure to remove all dynamic SQL
	08/02/2022	 12.0.0		 FJM		 Fixed the In/Out tolerance clause
	08/10/2022	 12.0.1		 FJM		 Fixed the Manager and Carrier companies.  Because of FLC the company guid may be the local record version
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterReconciliationSelectSummaryInformation] 
    @InventoryDate DATE  -- Required. tells us which date to use to find meter closeout transactions and get the meter start and stop values
	,@SiteGuid UNIQUEIDENTIFIER  -- Required. Tells us which site we're doing meter reconciliation for
	,@AssetGuid UNIQUEIDENTIFIER = NULL  -- If provided, show results only for meters belonging to this particular tank, piece of equipment, or load arm
	,@MeterGuid UNIQUEIDENTIFIER = NULL  -- If provided, show results for only this particular meter. 
	,@ManagerCompanyGuid UNIQUEIDENTIFIER = NULL  -- If provided, show only meters involved in a transaction with the specified company as a manager
	,@ProductGuid UNIQUEIDENTIFIER = NULL  -- If provided, show only meters involved in a transaction using the specified product
	,@CarrierCompanyGuid UNIQUEIDENTIFIER = NULL  -- If provided, show only meters involved in a transaction with the specified company as a carrier
	,@InOutOfTolerance BIT = NULL  -- NULL = all results, 0 = in tolerance meters only, 1 = out of tolerance meters only
	,@ToleranceValue FLOAT = NULL  -- Used along with @InOutOfTolerance and @ToleranceIsPercent to limit the results
	,@ToleranceIsPercent BIT = 0  -- 0 = a quantity, 1 = a percent
AS
BEGIN
	SET NOCOUNT ON

	-- if we are passing the local company record in a FLC environment we need to get the masterrecordguid
	SELECT @ManagerCompanyGuid = _MasterRecordGuid  
	FROM tblCompanies
	WHERE CompanyGuid = @ManagerCompanyGuid

	SELECT @CarrierCompanyGuid = _MasterRecordGuid  
	FROM tblCompanies
	WHERE CompanyGuid = @CarrierCompanyGuid

	--determine which transaction alias is the meter closeout transaction
	DECLARE @CloseoutTransactionAliasGuid UNIQUEIDENTIFIER

	SELECT @CloseoutTransactionAliasGuid = a.MasterRecordGuid
	FROM [erv].[udf_GetTransactionAliasRecordVersions](@SiteGuid) a
	INNER JOIN tblTransactionAliases b WITH (NOLOCK) ON b.TransactionAliasGuid = a.TransactionAliasGuid
	WHERE b.MeterCloseout = 1
		AND b.LookupTransTypeIndex = 12

	SELECT *
		,
		--meter variance is the difference between transaction meter totals and meter total
		MeterVariance = ABS(TransactionMeterTotal - MeterTotal)
		,
		--Volume variance is the difference between transaction gross volume totals and meter total
		VolumeVariance = ABS(TransactionVolumeTotal - MeterTotal)
	FROM (
		SELECT tblMeter.MeterGuid
			,tblMeter.MeterID
			,
			--get the AssetGuid, which is identity guid of a piece of equipment, a tank, or a load arm associated with the meter
			AssetGuid = CASE 
				WHEN EXISTS (
						SELECT 1
						FROM [erv].[udf_GetEquipmentRecordVersions](@SiteGuid) a
						INNER JOIN map.tblMeterToEquipment b WITH (NOLOCK) ON b.EquipmentGuid = a.EquipmentGuid
						WHERE b.MeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT c._MasterRecordGuid
							FROM [erv].[udf_GetEquipmentRecordVersions](@SiteGuid) a
							INNER JOIN map.tblMeterToEquipment b WITH (NOLOCK) ON b.EquipmentGuid = a.EquipmentGuid
							INNER JOIN tblEquipment c WITH (NOLOCK) ON c.EquipmentGuid = b.EquipmentGuid
							WHERE b.MeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblMeterToTank WITH (NOLOCK)
						WHERE map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT map.tblMeterToTank.TankGuid
							FROM map.tblMeterToTank WITH (NOLOCK)
							INNER JOIN tblTanks WITH (NOLOCK) ON map.tblMeterToTank.TankGuid = tblTanks.TankGuid
							WHERE map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblProductToPresetInjector WITH (NOLOCK)
						WHERE map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT TOP (1) AssignedToLoadArmGuid
							FROM map.tblProductToPresetInjector WITH (NOLOCK)
							WHERE map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblProductToPresetComponentTankOrTankGroup WITH (NOLOCK)
						WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT TOP (1) AssignedToLoadArmGuid
							FROM map.tblProductToPresetComponentTankOrTankGroup WITH (NOLOCK)
							WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid
							)
				END
			,
			--get the AssetID, which is ID of a piece of equipment, a tank, or a load arm associated with the meter
			AssetID = CASE 
				WHEN EXISTS (
						SELECT 1
						FROM [erv].[udf_GetEquipmentRecordVersions](@SiteGuid) a
						INNER JOIN map.tblMeterToEquipment b WITH (NOLOCK) ON b.EquipmentGuid = a.EquipmentGuid
						WHERE b.MeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT c.ID
							FROM [erv].[udf_GetEquipmentRecordVersions](@SiteGuid) a
							INNER JOIN map.tblMeterToEquipment b WITH (NOLOCK) ON b.EquipmentGuid = a.EquipmentGuid
							INNER JOIN tblEquipment c WITH (NOLOCK) ON c.EquipmentGuid = b.EquipmentGuid
							WHERE b.MeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblMeterToTank WITH (NOLOCK)
						WHERE map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT TankID
							FROM map.tblMeterToTank WITH (NOLOCK)
							INNER JOIN tblTanks WITH (NOLOCK) ON map.tblMeterToTank.TankGuid = tblTanks.TankGuid
							WHERE map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblProductToPresetInjector WITH (NOLOCK)
						WHERE map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT TOP (1) LoadRackText
							FROM map.tblProductToPresetInjector WITH (NOLOCK)
							INNER JOIN tblLoadArms WITH (NOLOCK) ON tblLoadArms.LoadArmGuid = AssignedToLoadArmGuid
							WHERE map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid
							)
				WHEN EXISTS (
						SELECT 1
						FROM map.tblProductToPresetComponentTankOrTankGroup WITH (NOLOCK)
						WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid
						)
					THEN (
							SELECT TOP (1) LoadRackText
							FROM map.tblProductToPresetComponentTankOrTankGroup WITH (NOLOCK)
							INNER JOIN tblLoadArms WITH (NOLOCK) ON tblLoadArms.LoadArmGuid = AssignedToLoadArmGuid
							WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid
							)
				END
			,tblMeter.RotatesBackwardsFlag
			,MeterCloseouts.MeterStart
			,MeterCloseouts.MeterStop
			,
			--get the meter total, which is based off the closeout values we determined
			MeterTotal = dbo.udf_MeterReconciliationCalculateMeterTotal(tblMeter.RotatesBackwardsFlag, tblMeter.NumberOfDigits, MeterCloseouts.MeterStart, MeterCloseouts.MeterStop)
			,
			--get the transaction meter total, which is the sum of meter movements from transactions for a particular day.
			--if the meter has an error, do not bother calculating the transaction meter total
			TransactionMeterTotal = CASE 
				WHEN (
						MoreThanOneCloseoutFlag = 1
						OR NoCurrentCloseoutFlag = 1
						OR NoPreviousCloseoutFlag = 1
						)
					THEN NULL
				ELSE dbo.udf_MeterReconciliationCalculateTransactionTotal(@InventoryDate, tblMeter.MeterGuid, @SiteGuid, @CloseoutTransactionAliasGuid)
				END
			,
			--get the transaction volume total, which is the sum of gross volumes from transactions for a particular day.
			--if the meter has an error, do not bother calculating the transaction volume total
			TransactionVolumeTotal = CASE 
				WHEN (
						MoreThanOneCloseoutFlag = 1
						OR NoCurrentCloseoutFlag = 1
						OR NoPreviousCloseoutFlag = 1
						)
					THEN NULL
				ELSE dbo.udf_MeterCalculateTxVolumeTotal(@InventoryDate, tblMeter.MeterGuid, @SiteGuid, @CloseoutTransactionAliasGuid)
				END
			,
			--meters may be involved with multiple products. If they are, return an asterisk after the product ID to indicate that there is more than one
			Product = CASE 
				WHEN (
						(
							SELECT COUNT(*)
							FROM (
								SELECT Product
								FROM tblTransactionLineItems WITH (NOLOCK)
								INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
								WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionLineItems.DeleteFlag = 0
										OR tblTransactionLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								
								UNION
								
								SELECT Product
								FROM tblTransactionSubLineItems WITH (NOLOCK)
								INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
								WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionSubLineItems.DeleteFlag = 0
										OR tblTransactionSubLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								) AS Products
							) > 1
						)
					THEN (
							SELECT TOP (1) Product + '*'
							FROM (
								SELECT Product
								FROM tblTransactionLineItems WITH (NOLOCK)
								INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
								WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionLineItems.DeleteFlag = 0
										OR tblTransactionLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								
								UNION
								
								SELECT Product
								FROM tblTransactionSubLineItems WITH (NOLOCK)
								INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
								WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionSubLineItems.DeleteFlag = 0
										OR tblTransactionSubLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								) AS Products
							)
				ELSE (
						SELECT TOP (1) Product
						FROM (
							SELECT Product
							FROM tblTransactionLineItems WITH (NOLOCK)
							INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
							WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
								AND (
									tblTransactionLineItems.DeleteFlag = 0
									OR tblTransactionLineItems.DeleteFlag IS NULL
									)
								AND tblTransactions.InventoryDate = @InventoryDate
								AND tblTransactions.SiteGuid = @SiteGuid
							
							UNION
							
							SELECT Product
							FROM tblTransactionSubLineItems WITH (NOLOCK)
							INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
							WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
								AND (
									tblTransactionSubLineItems.DeleteFlag = 0
									OR tblTransactionSubLineItems.DeleteFlag IS NULL
									)
								AND tblTransactions.InventoryDate = @InventoryDate
								AND tblTransactions.SiteGuid = @SiteGuid
							) AS Products
						)
				END
			,
			--meters may be involved with multiple carriers. If they are, return an asterisk after the carrier ID to indicate that there is more than one
			Carrier = CASE 
				WHEN (
						(
							SELECT COUNT(*)
							FROM (
								SELECT CarrierID
								FROM tblTransactions WITH (NOLOCK)
								INNER JOIN tblTransactionLineItems WITH (NOLOCK) ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
								WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionLineItems.DeleteFlag = 0
										OR tblTransactionLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								
								UNION
								
								SELECT CarrierID
								FROM tblTransactions WITH (NOLOCK)
								INNER JOIN tblTransactionSubLineItems WITH (NOLOCK) ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
								WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionSubLineItems.DeleteFlag = 0
										OR tblTransactionSubLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								) AS Carriers
							WHERE CarrierID IS NOT NULL
							) > 1
						)
					THEN (
							SELECT TOP (1) CarrierID + '*'
							FROM (
								SELECT CarrierID
								FROM tblTransactions WITH (NOLOCK)
								INNER JOIN tblTransactionLineItems WITH (NOLOCK) ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
								WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionLineItems.DeleteFlag = 0
										OR tblTransactionLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								
								UNION
								
								SELECT CarrierID
								FROM tblTransactions WITH (NOLOCK)
								INNER JOIN tblTransactionSubLineItems WITH (NOLOCK) ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
								WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
									AND (
										tblTransactionSubLineItems.DeleteFlag = 0
										OR tblTransactionSubLineItems.DeleteFlag IS NULL
										)
									AND tblTransactions.InventoryDate = @InventoryDate
									AND tblTransactions.SiteGuid = @SiteGuid
								) AS Carriers
							WHERE CarrierID IS NOT NULL
							)
				ELSE (
						SELECT TOP (1) CarrierID
						FROM (
							SELECT CarrierID
							FROM tblTransactions WITH (NOLOCK)
							INNER JOIN tblTransactionLineItems WITH (NOLOCK) ON tblTransactionLineItems.TransactionGuid = tblTransactions.TransactionGuid
							WHERE tblTransactionLineItems.MeterGuid = tblMeter.MeterGuid
								AND (
									tblTransactionLineItems.DeleteFlag = 0
									OR tblTransactionLineItems.DeleteFlag IS NULL
									)
								AND tblTransactions.InventoryDate = @InventoryDate
								AND tblTransactions.SiteGuid = @SiteGuid
							
							UNION
							
							SELECT CarrierID
							FROM tblTransactions WITH (NOLOCK)
							INNER JOIN tblTransactionSubLineItems WITH (NOLOCK) ON tblTransactionSubLineItems.TransactionGuid = tblTransactions.TransactionGuid
							WHERE tblTransactionSubLineItems.MeterGuid = tblMeter.MeterGuid
								AND (
									tblTransactionSubLineItems.DeleteFlag = 0
									OR tblTransactionSubLineItems.DeleteFlag IS NULL
									)
								AND tblTransactions.InventoryDate = @InventoryDate
								AND tblTransactions.SiteGuid = @SiteGuid
							) AS Carriers
						WHERE CarrierID IS NOT NULL
						)
				END
			,MeterCloseouts.CurrentCloseoutTransactionID
			,MeterCloseouts.MoreThanOneCloseoutFlag
			,MeterCloseouts.NoCurrentCloseoutFlag
			,MeterCloseouts.NoPreviousCloseoutFlag
			,MeterCloseouts.CurrentCloseoutTransactionGuid
		FROM tblMeter WITH (NOLOCK)
		INNER JOIN dbo.udf_MeterReconciliationSelectCloseoutInformation(@InventoryDate, @SiteGuid, @CloseoutTransactionAliasGuid) MeterCloseouts ON tblMeter.MeterGuid = MeterCloseouts.MeterGuid
		) AS Results
	WHERE (
			@AssetGuid IS NULL
			OR Results.AssetGuid = @AssetGuid
			)
		AND (
			@MeterGuid IS NULL
			OR Results.MeterGuid = @MeterGuid
			)
		AND (
			@ManagerCompanyGuid IS NULL
			OR (
				EXISTS (
					SELECT 1
					FROM tblTransactionLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
					WHERE (
							tblTransactionLineItems.DeleteFlag = 0
							OR tblTransactionLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionLineItems.MeterGuid = Results.MeterGuid
						AND ManagerCompanyGuid = @ManagerCompanyGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				OR EXISTS (
					SELECT 1
					FROM tblTransactionSubLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
					WHERE (
							tblTransactionSubLineItems.DeleteFlag = 0
							OR tblTransactionSubLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionSubLineItems.MeterGuid = Results.MeterGuid
						AND ManagerCompanyGuid = @ManagerCompanyGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				)
			)
		AND (
			@CarrierCompanyGuid IS NULL
			OR (
				EXISTS (
					SELECT 1
					FROM tblTransactionLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
					WHERE (
							tblTransactionLineItems.DeleteFlag = 0
							OR tblTransactionLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionLineItems.MeterGuid = Results.MeterGuid
						AND CarrierCompanyGuid = @CarrierCompanyGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				OR EXISTS (
					SELECT 1
					FROM tblTransactionSubLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
					WHERE (
							tblTransactionSubLineItems.DeleteFlag = 0
							OR tblTransactionSubLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionSubLineItems.MeterGuid = Results.MeterGuid
						AND CarrierCompanyGuid = @CarrierCompanyGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				)
			)
		AND (
			@ProductGuid IS NULL
			OR (
				EXISTS (
					SELECT 1
					FROM tblTransactionLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionLineItems.TransactionGuid
					WHERE (
							tblTransactionLineItems.DeleteFlag = 0
							OR tblTransactionLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionLineItems.MeterGuid = Results.MeterGuid
						AND ProductGuid = @ProductGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				OR EXISTS (
					SELECT 1
					FROM tblTransactionSubLineItems WITH (NOLOCK)
					INNER JOIN tblTransactions WITH (NOLOCK) ON tblTransactions.TransactionGuid = tblTransactionSubLineItems.TransactionGuid
					WHERE (
							tblTransactionSubLineItems.DeleteFlag = 0
							OR tblTransactionSubLineItems.DeleteFlag IS NULL
							)
						AND tblTransactionSubLineItems.MeterGuid = Results.MeterGuid
						AND ProductGuid = @ProductGuid
						AND tblTransactions.InventoryDate = @InventoryDate
						AND tblTransactions.SiteGuid = @SiteGuid
					)
				)
			)
		AND (
			@InOutOfTolerance IS NULL
			OR ((
				@InOutOfTolerance = 0
				AND @ToleranceIsPercent = 0
				AND (ISNULL( ABS(TransactionMeterTotal - MeterTotal), 0 ) < @ToleranceValue
					AND ISNULL(ABS(TransactionVolumeTotal - MeterTotal), 0 ) < @ToleranceValue))
				)
			OR (
				@InOutOfTolerance = 0
				AND @ToleranceIsPercent = 1
				AND (ISNULL( ((ABS(TransactionMeterTotal - MeterTotal) / MeterTotal) * 100), 0 ) < @ToleranceValue
					AND ISNULL( ((ABS(TransactionVolumeTotal - MeterTotal) / MeterTotal) * 100), 0 ) < @ToleranceValue)
				)
			OR (
				@InOutOfTolerance = 1
				AND @ToleranceIsPercent = 0
				AND (ISNULL( ABS(TransactionMeterTotal - MeterTotal), 0 ) >= @ToleranceValue
					OR ISNULL( ABS(TransactionVolumeTotal - MeterTotal), 0 ) >= @ToleranceValue)
				)
			OR (
				@InOutOfTolerance = 1
				AND @ToleranceIsPercent = 1
				AND (ISNULL( ((ABS(TransactionMeterTotal - MeterTotal) / MeterTotal) * 100), 0 ) >= @ToleranceValue
					OR ISNULL( ((ABS(TransactionVolumeTotal - MeterTotal) / MeterTotal) * 100), 0 ) >= @ToleranceValue)
				)
			)
END
