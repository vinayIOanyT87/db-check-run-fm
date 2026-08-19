CREATE PROCEDURE [dbo].[usp_GetBlockedCardListAirCards]
@BaseDate datetimeoffset,
@BaseCount int
AS
declare @MaxDate datetimeoffset
declare @CountReference int
Set @MaxDate = (Select ISNULL(Max(UpdatedDate),'1980-01-01') from vw_BlockedCardListAirCards)
Set @CountReference = (Select Count(CardNumber) from vw_BlockedCardListAirCards);
if(@MaxDate > @BaseDate OR @BaseCount != @CountReference)
--If change detected then send entire black list of Aircards
Select CardNumber from vw_BlockedCardListAirCards
ELSE
--If no change detected then send empty list
Select CardNumber from vw_BlockedCardListAirCards where CardNumber is null
