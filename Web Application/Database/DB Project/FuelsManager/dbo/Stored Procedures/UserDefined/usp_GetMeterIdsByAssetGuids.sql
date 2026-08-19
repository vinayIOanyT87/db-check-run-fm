CREATE PROCEDURE [dbo].[usp_GetMeterIdsByAssetGuids]
(
@AssetGuids GuidListType READONLY
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [dbo].[usp_GetMeterIdsByAssetGuids]
	-- Author: Gregory Lybanon
	-- Version/Date: 1.0.002 / 2019-06-05 
	-- Purpose: Retrieve a list of meter IDs of the meters that are attached to the list of equiment 
	--			Guids passed in.  Eager loading the meters attached to a collection of equipment is 
	--			very slow, so this method is meant to return data much faster.
	------------------------------------------------------------------------------------------------------

	select M.MeterID 
	from dbo.tblMeter M 
	join map.tblMeterToEquipment MTE ON MTE.MeterGuid = M.MeterGuid 
	join dbo.tblEquipment E on E.EquipmentGuid = MTE.EquipmentGuid 
	Where E._MasterRecordGuid in (Select Guid from @AssetGuids)

END