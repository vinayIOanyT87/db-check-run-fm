function MeterStartStopChange(meterStartID, meterStopID, grossID, meterTotalID, decimalSymbol, decimalPlaces)
{
	var oMeterStartTextBox = document.getElementById(meterStartID);
	var oMeterStopTextBox  = document.getElementById(meterStopID);
	var oGrossTextBox      = document.getElementById(grossID);
	var oMeterTotalTextBox = document.getElementById(meterTotalID);

	if (oMeterStartTextBox == null
	|| oMeterStopTextBox == null)
	{
		return; 
	}

	var meterStartString = oMeterStartTextBox.value;
	meterStartString = meterStartString.replace(decimalSymbol == "." ? "," : ".", "");
	meterStartString = meterStartString.replace(decimalSymbol == "," ? "," : ".", ".");
	var meterStart = parseFloat(meterStartString);
	
	if (isNaN(meterStart)) 
	{
		return; 
	}

	var meterStopString = oMeterStopTextBox.value;
	meterStopString = meterStopString.replace(decimalSymbol == "." ? "," : ".", "");
	meterStopString = meterStopString.replace(decimalSymbol == "," ? "," : ".", ".");
	var meterStop = parseFloat(meterStopString);

	if (isNaN(meterStop)) 
	{
		return;
	}
	
	//Handle meter rollover
	if (meterStop >= meterStart)
	{
		if(oGrossTextBox != null)
			oGrossTextBox.value = (meterStop - meterStart).toFixed(decimalPlaces);
		if (oMeterTotalTextBox != null)
			oMeterTotalTextBox.value = (meterStop - meterStart).toFixed(decimalPlaces);
	}
	else
	{
		var meterRollover   = meterStart.toString();
		meterRollover       = meterRollover.replace(/([012345678])/g, "9");
		meterRollover = Number(meterRollover) + 1;
		if(oGrossTextBox != null)
			oGrossTextBox.value = ((meterRollover - meterStart) + meterStop).toFixed(decimalPlaces);
		if (oMeterTotalTextBox != null)
			oMeterTotalTextBox.value = ((meterRollover - meterStart) + meterStop).toFixed(decimalPlaces);
	}

	if (decimalSymbol == ",")
	{
		if (oMeterTotalTextBox != null)
			oMeterTotalTextBox.value = oMeterTotalTextBox.value.replace(".", ",");
		if (oGrossTextBox != null)
			oGrossTextBox.value = oGrossTextBox.value.replace(".", ",");
	}
}




function TempDensityVCFChange(temperatureID, densityID, vcfID, netQuantityID)
{
	var oTemperatureTextBox = document.getElementById(temperatureID);
	var oDensityTextBox     = document.getElementById(densityID);
	var oVCFTextBox         = document.getElementById(vcfID);
	var oNetQuantityTextBox	= document.getElementById(netQuantityID);


	if (oVCFTextBox != null
	&& oVCFTextBox.value.replace(/ /g, "") != "")
	{
		if(oTemperatureTextBox != null)
		{
			oTemperatureTextBox.readOnly = true;
			oTemperatureTextBox.style.background = "LightGrey";
			oTemperatureTextBox.value="";
		}
		
		if(oDensityTextBox != null)
		{
			oDensityTextBox.readOnly     = true;
			oDensityTextBox.style.background     = "LightGrey";
			oDensityTextBox.value="";
		}
		
//		if(oNetQuantityTextBox != null)
//		{
//			oNetQuantityTextBox.readOnly     = true;
//			oNetQuantityTextBox.style.background     = "LightGrey";
//			oNetQuantityTextBox.value="";
//		}
	}
	else
	{
		if(oTemperatureTextBox != null)
		{
			oTemperatureTextBox.readOnly = false;
			oTemperatureTextBox.style.background = "White";
		}
		
		if(oDensityTextBox != null)
		{
			oDensityTextBox.readOnly     = false;
			oDensityTextBox.style.background     = "White";
		}

	}
	
	if ( (oTemperatureTextBox != null
	&& oTemperatureTextBox.value.replace(/ /g, "") != "")
	|| (oDensityTextBox != null
	&& oDensityTextBox.value.replace(/ /g, "") != "") )
	{
		if(oVCFTextBox != null)
		{
			oVCFTextBox.readOnly				= true;
			oVCFTextBox.style.background	= "LightGrey";
			oVCFTextBox.value					= "";
		}
	}
	else
	{
		if(oVCFTextBox != null)
		{
			oVCFTextBox.readOnly         = false;
			oVCFTextBox.style.background = "White";
		}
	}
}

function DataGridKeyPress(gridName)
{
	if(event.keyCode == 13)
	{
		var source = document.getElementById("EnterKeySource");
		source.value = gridName;
	}
}

function CallGlossary(glossaryName,fieldID)
{
    var field = document.getElementById(fieldID);
    if (field != null) {

        var fieldText = removeSpecialCharacters(field.innerText);
        fieldText = removeTrailingColon(fieldText);
        var sLoc = glossaryName + "#" + fieldText;
        var sWndName = "Glossary";
        var sOptions = "toolbar=0,location=0,scrollbars=1,width=400,height=300";

        var wndGlossary = window.open(sLoc, sWndName, sOptions)
        wndGlossary.focus();
    }
}

function removeSpecialCharacters(mystr)
{
    var workStr = mystr;
    var n = workStr.match(/[\/@&()]/g);
    if (n != null) {
        workStr = workStr.replace(/\s+/g, '');
        workStr = workStr.replace(/[\/@&()]/g, '');
    }
    return workStr;
}

function removeTrailingColon(mystr)
{
    var outstr = ((mystr.charAt(mystr.length - 1, 1) == ":") ? mystr.substring(0, mystr.length - 1) : mystr);
    return outstr.replace(/ /g, "_");
}

//////////////////////////////////////////////////////////////////////////////
// The purpose of this function is to handle the base functionality of the 
// MasterOnChange if the custom script is not present.
//////////////////////////////////////////////////////////////////////////////
function BaseMasterOnChange(fieldName)
{
   if (fieldName != null)
   {
      if (fieldName == "LineItem ProductPrice")
      {
         GenericHandleProductPrice();
      }
   }
}

/////////////////////////////////////////////////////////////////////////////
// The purpose of this function to handle generic functionality of unit
// price. The ADF custom script if not registered, the unit price is not
// save because the Hidden field (dirty flag) is not set. Therefore, this
// function is required.
/////////////////////////////////////////////////////////////////////////////
function GenericHandleProductPrice()
{
   var lineItemProductPrice       = null;
   var hiddenLineItemProductPrice = null;

   try
   {
      lineItemProductPrice       = oLineItemProductPrice;
      hiddenLineItemProductPrice = oHiddenLineItemProductPrice;
   }
   catch (Error)
   {
   }
   
   if (lineItemProductPrice != null && hiddenLineItemProductPrice != null)
   {
      hiddenLineItemProductPrice.value = "true";
   }
}
