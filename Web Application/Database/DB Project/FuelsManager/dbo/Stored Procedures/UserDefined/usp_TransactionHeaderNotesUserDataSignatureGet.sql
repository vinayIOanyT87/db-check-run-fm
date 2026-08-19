
CREATE PROCEDURE [dbo].[usp_TransactionHeaderNotesUserDataSignatureGet]
(
	@TransactionGuid UNIQUEIDENTIFIER = NULL,
	@TransID NVARCHAR(64) = NULL
)
AS
BEGIN
	SET NOCOUNT ON
	
	-- The use of the NOLOCK hint in this procedure is questionable at best. It should be removed and any deadlocks that result should be fixed.
	-- We allow selecting transactions via the TransID or TransactionGuid. 
	IF (@TransactionGuid IS NOT NULL)
	BEGIN

		SELECT T.*, 
			tblTransactionNotes.*, 
			tblTransactionSignature.*, 
			tblTransactionUserData.*,	
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.LevelUnitIndex, tblSites.LevelUnitIndex) AS LevelUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.TemperatureUnitIndex, tblSites.TemperatureUnitIndex) AS TemperatureUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.DensityUnitIndex, tblSites.DensityUnitIndex) AS DensityUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.PressureUnitIndex, tblSites.PressureUnitIndex) AS PressureUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.FlowUnitIndex, tblSites.FlowUnitIndex) AS FlowUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.VolumeUnitIndex, tblSites.VolumeUnitIndex) AS VolumeUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.AdditiveVolumeUnitIndex, tblSites.AdditiveVolumeUnitIndex) AS AdditiveVolumeUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.MassUnitIndex, tblSites.MassUnitIndex) AS MassUnitIndex,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.LevelDecimalPlaces, tblSites.LevelDecimalPlaces) AS LevelDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.TemperatureDecimalPlaces, tblSites.TemperatureDecimalPlaces) AS TemperatureDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.DensityDecimalPlaces, tblSites.DensityDecimalPlaces) AS DensityDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.PressureDecimalPlaces, tblSites.PressureDecimalPlaces) AS PressureDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.FlowDecimalPlaces, tblSites.FlowDecimalPlaces) AS FlowDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.VolumeDecimalPlaces, tblSites.VolumeDecimalPlaces) AS VolumeDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.AdditiveVolumeDecimalPlaces, tblSites.AdditiveVolumeDecimalPlaces) AS AdditiveVolumeDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.MassDecimalPlaces, tblSites.MassDecimalPlaces) AS MassDecimalPlaces
		FROM tblTransactions T WITH(NOLOCK)
		LEFT OUTER JOIN tblTransactionNotes WITH(NOLOCK) ON T.TransactionGuid = tblTransactionNotes.TransactionGuid 
		LEFT OUTER JOIN tblTransactionSignature WITH(NOLOCK) ON T.TransactionGuid = tblTransactionSignature.TransactionGuid 
		LEFT OUTER JOIN tblTransactionUserData WITH(NOLOCK) ON T.TransactionGuid = tblTransactionUserData.TransactionGuid 
		LEFT OUTER JOIN tblSites WITH(NOLOCK) ON T.SiteGuid = tblSites.SiteGuid
		LEFT OUTER JOIN tblTransactionAliases WITH(NOLOCK) ON tblTransactionAliases.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', T.TransactionAliasGuid, T.SiteGuid)
		WHERE T.TransactionGuid = @TransactionGuid

	END
	ELSE
	BEGIN

		SELECT T.*, 
			tblTransactionNotes.*, 
			tblTransactionSignature.*, 
			tblTransactionUserData.*,	
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.LevelUnitIndex, tblSites.LevelUnitIndex) AS LevelUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.TemperatureUnitIndex, tblSites.TemperatureUnitIndex) AS TemperatureUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.DensityUnitIndex, tblSites.DensityUnitIndex) AS DensityUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.PressureUnitIndex, tblSites.PressureUnitIndex) AS PressureUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.FlowUnitIndex, tblSites.FlowUnitIndex) AS FlowUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.VolumeUnitIndex, tblSites.VolumeUnitIndex) AS VolumeUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.AdditiveVolumeUnitIndex, tblSites.AdditiveVolumeUnitIndex) AS AdditiveVolumeUnitIndex,
			dbo.udf_GetUnitsIndex(NULL, tblTransactionAliases.MassUnitIndex, tblSites.MassUnitIndex) AS MassUnitIndex,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.LevelDecimalPlaces, tblSites.LevelDecimalPlaces) AS LevelDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.TemperatureDecimalPlaces, tblSites.TemperatureDecimalPlaces) AS TemperatureDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.DensityDecimalPlaces, tblSites.DensityDecimalPlaces) AS DensityDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.PressureDecimalPlaces, tblSites.PressureDecimalPlaces) AS PressureDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.FlowDecimalPlaces, tblSites.FlowDecimalPlaces) AS FlowDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.VolumeDecimalPlaces, tblSites.VolumeDecimalPlaces) AS VolumeDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.AdditiveVolumeDecimalPlaces, tblSites.AdditiveVolumeDecimalPlaces) AS AdditiveVolumeDecimalPlaces,
			dbo.udf_GetDecimalPlaces(NULL, tblTransactionAliases.MassDecimalPlaces, tblSites.MassDecimalPlaces) AS MassDecimalPlaces
		FROM tblTransactions T WITH(NOLOCK)
		LEFT OUTER JOIN tblTransactionNotes WITH(NOLOCK) ON T.TransactionGuid = tblTransactionNotes.TransactionGuid 
		LEFT OUTER JOIN tblTransactionSignature WITH(NOLOCK) ON T.TransactionGuid = tblTransactionSignature.TransactionGuid 
		LEFT OUTER JOIN tblTransactionUserData WITH(NOLOCK) ON T.TransactionGuid = tblTransactionUserData.TransactionGuid 
		LEFT OUTER JOIN tblSites WITH(NOLOCK) ON T.SiteGuid = tblSites.SiteGuid
		LEFT OUTER JOIN tblTransactionAliases WITH(NOLOCK) ON tblTransactionAliases.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', T.TransactionAliasGuid, T.SiteGuid)
		WHERE T.TransID = @TransID

	END 
END 

