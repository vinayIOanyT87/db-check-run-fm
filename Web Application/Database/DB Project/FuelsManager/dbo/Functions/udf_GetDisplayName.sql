

CREATE FUNCTION [dbo].[udf_GetDisplayName](@DbElement NVARCHAR(200),@SingularForm BIT = 0)
RETURNS NVARCHAR(300)
AS

BEGIN
	SET @DbElement = REPLACE(@DbElement,'tbl','')
	IF @SingularForm = 1
	BEGIN
		-- CONVERT STRING TO SINGULAR FORM
		IF RIGHT(@DbElement,3) = 'ies'
		BEGIN
			SET @DbElement = SUBSTRING(@DbElement,1,LEN(@DbElement) - 3) + 'y'
		END

		IF RIGHT(@DbElement,1) = 's' AND RIGHT(@DbElement,6) <> 'status' AND RIGHT(@DbElement,4) <> 'ases'
			AND RIGHT(@DbElement, 7) <> 'Address' AND RIGHT(@DbElement, 7) <> 'changes' AND RIGHT(@DbElement, 5) <> 'hours'
			AND RIGHT(@DbElement, 7) <> 'returns' AND RIGHT(@DbElement, 11) <> 'identifiers' AND RIGHT(@DbElement, 10) <> 'ActiveArms'
			AND RIGHT(@DbElement, 12) <> 'BrokenBlends' AND RIGHT(@DbElement, 10) <> 'RetainLogs' AND RIGHT(@DbElement, 15) <> 'AlarmsAndEvents'
			AND RIGHT(@DbElement, 10) <> 'Operations' AND RIGHT(@DbElement, 8) <> 'Graphics' AND RIGHT(@DbElement, 9) <> 'LineItems'
		BEGIN
			SET @DbElement = SUBSTRING(@DbElement,1,LEN(@DbElement) - 1)
		END
		
		IF RIGHT(@DbElement,4) = 'ases'
		BEGIN
			SET @DbElement = SUBSTRING(@DbElement,1,LEN(@DbElement) - 2)
		END

	END
	DECLARE @Position INT
		,	@Char VARCHAR(2)
		,	@NewString NVARCHAR(200)

	SET @NewString = ''

	DECLARE @Index INT
	SET @Index = 1

	WHILE @Index <= LEN(@DbElement)
	BEGIN
		SET	@Char = SUBSTRING(@DbElement,@Index,1)
		IF @Index = 1
			SET @Char = UPPER(@Char)
			
		IF ASCII(@Char) BETWEEN 65 AND 90
		BEGIN
			IF @Index > 1
			BEGIN
				
					IF SUBSTRING(@DbElement,@Index - 1,1) <> ' '
						SET @Char =' ' + @Char
			END
		END
		
		SET @NewString = @NewString + @Char
		SET @Index = @Index + 1
	END

	SET @NewString = REPLACE(@NewString,'S O F A_ F E A','SOFA FEA')
	SET @NewString = REPLACE(@NewString,'X M L','XML')
	SET @NewString = REPLACE(@NewString,' G U I D',' GUID')
	SET @NewString = REPLACE(@NewString,'I D','ID')
	SET @NewString = REPLACE(@NewString,'C O M ','COM ')
	SET @NewString = REPLACE(@NewString,'Xon ','X on ')
	SET @NewString = REPLACE(@NewString,'Xoff ','X off ')
	SET @NewString = REPLACE(@NewString,'G S T ','GST ')
	SET @NewString = REPLACE(@NewString,'O P C','OPC')
	SET @NewString = REPLACE(@NewString,'S R M','SRM')
	SET @NewString = REPLACE(@NewString,'I A T A','IATA')
	SET @NewString = REPLACE(@NewString,'S I','SI')
	SET @NewString = REPLACE(@NewString,'S P L C','SPLC')
	SET @NewString = REPLACE(@NewString,'Address1','Address 1')
	SET @NewString = REPLACE(@NewString,'Address2','Address 2')
	SET @NewString = REPLACE(@NewString,'F A X','Fax')
	SET @NewString = REPLACE(@NewString,'B O L','BOL')
	SET @NewString = REPLACE(@NewString,'S C A D A','SCADA')
	SET @NewString = REPLACE(@NewString,'V R U','VRU')
	SET @NewString = REPLACE(@NewString,'A M ','AM ')
	SET @NewString = REPLACE(@NewString,'P M ','PM ')
	SET @NewString = REPLACE(@NewString,'Contact1 ','Contact 1 ')
	SET @NewString = REPLACE(@NewString,'Contact2 ','Contact 2 ')
	SET @NewString = REPLACE(@NewString,'Contact3 ','Contact 3 ')
	SET @NewString = REPLACE(@NewString,'U R L','URL')
	SET @NewString = REPLACE(@NewString,'Types1','Type 1')
	SET @NewString = REPLACE(@NewString,'Types2','Type 2')
	SET @NewString = REPLACE(@NewString,'Types3','Type 3')
	SET @NewString = REPLACE(@NewString,'Phone1','Phone 1')
	SET @NewString = REPLACE(@NewString,'Phone2','Phone 2')
	SET @NewString = REPLACE(@NewString,'Rate1','Rate 1')
	SET @NewString = REPLACE(@NewString,'Rate2','Rate 2')
	SET @NewString = REPLACE(@NewString,'Rate3','Rate 3')
	SET @NewString = REPLACE(@NewString,'Rate4','Rate 4')
	SET @NewString = REPLACE(@NewString,'P I N','PIN')
	SET @NewString = REPLACE(@NewString,'User Data','User Data ')
	SET @NewString = REPLACE(@NewString,'S C A C','SCAC')
	
	SET @NewString = REPLACE(@NewString,'S T D','STD')
	SET @NewString = REPLACE(@NewString,'E T D','ETD')
	SET @NewString = REPLACE(@NewString,'S T A','STA')
	SET @NewString = REPLACE(@NewString,'E T A','ETA')
	SET @NewString = REPLACE(@NewString,'S F T','SFT')
	SET @NewString = REPLACE(@NewString,'F S T','FST')
	
	SET @NewString = REPLACE(@NewString,'P O','PO')
	SET @NewString = REPLACE(@NewString,'ID1','ID 1')
	SET @NewString = REPLACE(@NewString,'ID2','ID 3')
	SET @NewString = REPLACE(@NewString,'ID3','ID 3')
	
	SET @NewString = REPLACE(@NewString,'Number1','Number 1')
	SET @NewString = REPLACE(@NewString,'Number2','Number 2')
	SET @NewString = REPLACE(@NewString,'Number3','Number 3')
	
	SET @NewString = REPLACE(@NewString,'Type1','Type 1')
	SET @NewString = REPLACE(@NewString,'Type2','Type 2')
	SET @NewString = REPLACE(@NewString,'Type3','Type 3')
	SET @NewString = REPLACE(@NewString,'Model1','Model 1')
	SET @NewString = REPLACE(@NewString,'Model2','Model 2')
	SET @NewString = REPLACE(@NewString,'Model3','Model 3')
	SET @NewString = REPLACE(@NewString,'Flag0','Flag 0')
	SET @NewString = REPLACE(@NewString,'Number0','Number 0')
	SET @NewString = REPLACE(@NewString,'Date0','Date 0')
	SET @NewString = REPLACE(@NewString,'S S A N','SSAN')
	SET @NewString = REPLACE(@NewString,'TranSID','Trans ID') 
	SET @NewString = REPLACE(@NewString,'C L I N','CLIN') 
	SET @NewString = REPLACE(@NewString,'Totalisor1','Totalisor 1') 
	SET @NewString = REPLACE(@NewString,'Totalisor2','Totalisor 2') 
	SET @NewString = REPLACE(@NewString,'_ O O S_ ',' OOS ') 
	SET @NewString = REPLACE(@NewString,'G P M','GPM') 
	SET @NewString = REPLACE(@NewString,'Q C','QC') 
	SET @NewString = REPLACE(@NewString,'C L I N','CLIN') 
	SET @NewString = REPLACE(@NewString,'C O A','COA') 
	SET @NewString = REPLACE(@NewString,'Tax1','Tax 1') 
	SET @NewString = REPLACE(@NewString,'Tax2','Tax 2') 
	SET @NewString = REPLACE(@NewString,'Tax3','Tax 3') 
	SET @NewString = REPLACE(@NewString,'Tax4','Tax 4') 
	SET @NewString = REPLACE(@NewString,'Tax5','Tax 5') 
	SET @NewString = REPLACE(@NewString,'C O A ','COA ') 
	SET @NewString = REPLACE(@NewString,'Password History','Password History ') 
	SET @NewString = REPLACE(@NewString,'E P A ','EPA ') 
	SET @NewString = REPLACE(@NewString,'PumPOff ','Pum Off ') 
	SET @NewString = REPLACE(@NewString,'P ID X ','PIDX ') 
	SET @NewString = REPLACE(@NewString,'G C A ','GCA ') 
	SET @NewString = REPLACE(@NewString,'R T D ','RTD ') 
	SET @NewString = REPLACE(@NewString,'V T O ','VTO ')
	SET @NewString = REPLACE(@NewString,'G S E ','GSE ')
	SET @NewString = REPLACE(@NewString,' G A ',' GA ')
	SET @NewString = REPLACE(@NewString,'E A ','EA ')
	SET @NewString = REPLACE(@NewString,'D C U','DCU')
	SET @NewString = REPLACE(@NewString,'UnitSIndex','Units Index')
	SET @NewString = REPLACE(@NewString,'Check1','Check 1')
	SET @NewString = REPLACE(@NewString,'Check2','Check 2')
	SET @NewString = REPLACE(@NewString,'ISIn','Is In')
	SET @NewString = REPLACE(@NewString,'SIndex','s Index')
	SET @NewString = REPLACE(@NewString,'F P E S','FPES')
	SET @NewString = REPLACE(@NewString,'I P Addr','IP Address')
	
	SET @NewString = REPLACE(@NewString,'ResultSID','Results ID')
	SET @NewString = REPLACE(@NewString,'Option2','Option 2')
	SET @NewString = REPLACE(@NewString,'G S T','GST')

	-- Translations for values on the Product Configuration Page
	SET @NewString = REPLACE(@NewString,'Tracking Product Guid','Tracking Product')
	SET @NewString = REPLACE(@NewString,'Aviation Fuel Flag','Aviation Fuel')
	SET @NewString = REPLACE(@NewString,'Additive Cycle Volume','Cycle Volume')
	SET @NewString = REPLACE(@NewString,'Octane Number','Octane')
	SET @NewString = REPLACE(@NewString,'Minor Correction Method','Correction Sub-Method')

	-- Translations for values on the site page
	SET @NewString = REPLACE(@NewString,'Email','E-mail')
	SET @NewString = REPLACE(@NewString,'Inhibit Site Ledger Rollup','Inhibit Ledger Rollup')
	SET @NewString = REPLACE(@NewString,'Inhibit Multiple Card In','Inhibit Multiple Card In''s')
	SET @NewString = REPLACE(@NewString,'Lookup Secondary Storage Fill Method Index','Fill Method')
	SET @NewString = REPLACE(@NewString,'Inventory Transaction Alias Guid','Inventory Transaction')
	SET @NewString = REPLACE(@NewString,'Adjustment Transaction Alias Guid','Adjustment Transaction')
	SET @NewString = REPLACE(@NewString,'Excess Variance','Variance')
	SET @NewString = REPLACE(@NewString,'Inhibit BOL With Broken Blends','Inhibit BOLs With Broken Blends')
	SET @NewString = REPLACE(@NewString,'Inhibit BOL With Improper Additization','Inhibit BOLs With Improper Additization')
	SET @NewString = REPLACE(@NewString,'Inhibit Overweight BOL','Inhibit Overweight BOLs')
	SET @NewString = REPLACE(@NewString,'Alarm And Event Printer','Alarm & Event Printer')
	SET @NewString = REPLACE(@NewString,'Use Tank Reconciliation','Reconcile Individual Tanks')
	SET @NewString = REPLACE(@NewString,'Import Archive Dir','Data Transmission Import Archive Directory')
	SET @NewString = REPLACE(@NewString,'Export Archive Dir','Data Transmission Export Archive Directory')
	SET @NewString = REPLACE(@NewString,'End Number','End #')
	SET @NewString = REPLACE(@NewString,'Start Number','Start #')
	SET @NewString = REPLACE(@NewString,'Next Number','Next #')
	SET @NewString = REPLACE(@NewString,'Meter Reconciliation Report Name','Meter Reconciliation Report')
	SET @NewString = REPLACE(@NewString,'Lookup Mail Connect Mode Index','Connection Mode')
	SET @NewString = REPLACE(@NewString,'Dialup Name','Dial-up Name')
	SET @NewString = REPLACE(@NewString,'Lookup Watchdog Mode Index','Watchdog Mode')
	SET @NewString = REPLACE(@NewString,'Watchdog Counter End','Counter End')
	SET @NewString = REPLACE(@NewString,'Watchdog Counter Start','Counter Start')
	SET @NewString = REPLACE(@NewString,'Inhibit BOL Summary Auto Populate','BOL Summary')
	SET @NewString = REPLACE(@NewString,'Automatically Print Alarms And Events','Automatically Print Alarms & Events')

	-- Translations for the accounting general configuration page
	SET @NewString = REPLACE(@NewString,'Forced Closeout','Force closeout after')
	SET @NewString = REPLACE(@NewString,'Show Deleted Trx Flag','Show Deleted Transaction')
	SET @NewString = REPLACE(@NewString,'Set Begin Inventory To Zero Flag','Set Begin Inventory to zero')
	SET @NewString = REPLACE(@NewString,'Reverse Trx Date Mode','Reverse Transaction Date')

	IF(@NewString = 'Method')
	BEGIN
		SET @NewString = REPLACE(@NewString,'Method','Adjustment Distribution Methods')
	END

	SET @NewString = REPLACE(@NewString,'Consortium Flag','Use consortium members only')

	--Translations for the company page
	SET @NewString = REPLACE(@NewString,'Account Number','Account #')

	--Translations for the transaction alias page
	SET @NewString = REPLACE(@NewString,'Aggregate Assoc Tran','Aggregate Associated Transactions')

	RETURN @NewString
END



