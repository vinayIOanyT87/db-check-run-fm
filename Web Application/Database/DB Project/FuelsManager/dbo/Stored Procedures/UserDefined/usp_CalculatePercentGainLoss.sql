
CREATE PROCEDURE [dbo].[usp_CalculatePercentGainLoss]
@XMLstring NVARCHAR (4000)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @Aliases xml
	SET @Aliases = @XMLstring
	
	DECLARE @RecCntrNumber01 FLOAT -- Shipment quantity
	DECLARE @RecTrfrNumber01 FLOAT -- Shipment quantity
	DECLARE @RecCntrNumber02 FLOAT -- Gain/loss quantity
	DECLARE @RecTrfrNumber02 FLOAT -- Gain/loss quantity
	DECLARE @TotalNumber01   FLOAT
	DECLARE @TotalNumber02   FLOAT
	DECLARE @PercentGainLoss FLOAT

	SELECT @RecCntrNumber01 = ParamValues.ID.value('.', 'FLOAT')
	FROM @Aliases.nodes('/Receive-Contract/n1') AS ParamValues(ID)
	
	SELECT @RecCntrNumber02 = ParamValues.ID.value('.', 'FLOAT')
	FROM @Aliases.nodes('/Receive-Contract/n2') AS ParamValues(ID)
	
	SELECT @RecTrfrNumber01 = ParamValues.ID.value('.', 'FLOAT')
	FROM @Aliases.nodes('/Receive-Transfer/n1') AS ParamValues(ID)
	
	SELECT @RecTrfrNumber02 = ParamValues.ID.value('.', 'FLOAT')
	FROM @Aliases.nodes('/Receive-Transfer/n2') AS ParamValues(ID)
	
	-- Total the Shipment quantity
	SELECT @TotalNumber01 = @RecCntrNumber01 + @RecTrfrNumber01
	
	-- Total the Gain/Loss quantity
	SELECT @TotalNumber02 = @RecCntrNumber02 + @RecTrfrNumber02
	
	SELECT @PercentGainLoss = 0.0
	
	-- Calculate the percentage gain/loss
	IF (@TotalNumber01 <> 0)
	BEGIN
		SELECT @PercentGainLoss = (@TotalNumber02 / @TotalNumber01) * 100.0
	END
	
	-- For custom functions the calculated value is always returned in field Number01.
	-- All the listed fields have to be returned, that is the convention.		
	SELECT CAST(0.0 AS FLOAT) AS Gross,
	       CAST(0.0 AS FLOAT) AS Net,
	       CAST(0.0 AS FLOAT) AS Mass,
	       CAST(0.0 AS FLOAT) AS GrossPrice,
	       CAST(0.0 AS FLOAT) AS NetPrice,
	       CAST(0.0 AS FLOAT) AS MassPrice,
	       CAST(round(@PercentGainLoss,2) AS FLOAT)   AS Number01,
	       CAST(0.0 AS FLOAT) AS Number02,
	       CAST(0.0 AS FLOAT) AS Number03,
	       CAST(0.0 AS FLOAT) AS Number04,
	       CAST(0.0 AS FLOAT) AS Number05,
	       CAST(0.0 AS FLOAT) AS Number06
END
