// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LineItemCurrencyUnitFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Summary description for LineItemCurrencyUnitFG.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;
	using System.Web.UI;
	using System.Web.UI.HtmlControls;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	/// Summary description for LineItemCurrencyUnitFG.
	/// </summary>
	public class LineItemCurrencyUnitFG : DropDownGenerator, ILineItemField
	{
		#region Public data members
		public const string CLIENT_SIDE_KEY_LINEITEM_CURRENCY_UNIT = "CLIENT_SIDE_KEY_LINEITEM_CURRENCY_UNIT";
		public const string CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT = "CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT";
		#endregion

		private const string CLIENT_SIDE_FUNCTION_SCRIPT =
			@"	
				function CurrencyChange()																												
				{	
					var exchangeRateTextBox     = null;
					var nonDomesticPriceTextBox = null;
					var productPriceTextBox	    = null;
					var currencyUnitSelect      = null;
               var freeToAidCheckBox       = null;
					try
               {
						exchangeRateTextBox = oExchangeRateTextBox;
					}
					catch(err)
               {
						;
					}	
					try
               {
						nonDomesticPriceTextBox = oNonDomesticPriceTextBox;
					}
					catch(err)
               {
						;
					}																													
					try
               {
						productPriceTextBox = oLineItemProductPrice;
					}
					catch(err)
               {
						;
					}																													
				   try
               {
						currencyUnitSelect = oCurrencyUnitSelect;
					}
					catch(err)
               {
						;
					}		
					try
               {
						freeToAidCheckBox = oLineItemFlag05CheckBox;
					}
					catch(err)
               {
						;
					}		
																											
					if (exchangeRateTextBox != null)																								
					{																																
						exchangeRateTextBox.readOnly			 = true;																				
						exchangeRateTextBox.style.background = ""LightGrey"";																	
					}																																

					if ((productPriceTextBox == null) || (nonDomesticPriceTextBox == null) || (currencyUnitSelect == null))
               {							
						return;
               }				
					
               // Ensure that the product price text box is read only if the 
               // free to aid checkbox is checked. This script is overriding the
               // ADFCustomScript and causing the product price text box to be writable.
               if ((freeToAidCheckBox != null) && (freeToAidCheckBox.checked == true))
               {   
                  if (productPriceTextBox != null)
                  {
                     productPriceTextBox.value            = ""0.00"";
                     productPriceTextBox.readOnly         = true;
                     productPriceTextBox.style.background = ""LightGrey"";
                  }

                  return;
               }
					
               var totalForeignPriceCntrl = document.getElementById(""TransactionFields.UserDataTextFGTALUD3"");
               var totalPriceCntrl        = document.getElementById(""TransactionFields.UserDataTextFGTALUD2"");
					
               if (currencyUnitSelect.selectedIndex == 0)
               {
                  nonDomesticPriceTextBox.value            = """";
                  nonDomesticPriceTextBox.readOnly         = true;
                  nonDomesticPriceTextBox.style.background = ""LightGrey"";
						productPriceTextBox.readOnly             = false;																				
						productPriceTextBox.style.background     = ""White"";	

                  if (totalForeignPriceCntrl != null)
                  {
                     totalForeignPriceCntrl.value            = """";
                     totalForeignPriceCntrl.readOnly         = true;
                     totalForeignPriceCntrl.style.background = ""LightGrey"";
                  }

                  if (totalPriceCntrl != null)
                  {
                     totalPriceCntrl.readOnly         = false;
                     totalPriceCntrl.style.background = ""White"";
                  }

                  return;
               }	
               else
               {
						nonDomesticPriceTextBox.readOnly             = false;																				
						nonDomesticPriceTextBox.style.background     = ""White"";

                  if (totalForeignPriceCntrl != null)
                  {
                     totalForeignPriceCntrl.readOnly         = false;
                     totalForeignPriceCntrl.style.background = ""White"";
                  }	

                  if (totalPriceCntrl != null)
                  {
                     //totalPriceCntrl.value            = """";
                     totalPriceCntrl.readOnly         = true;
                     totalPriceCntrl.style.background = ""LightGrey"";
                  }
               }		
													
					if (((nonDomesticPriceTextBox.value.replace(/ /g, """") != """") 
                  && (isNaN(parseFloat(nonDomesticPriceTextBox.value)) == false))		
						|| (currencyUnitSelect.selectedIndex > 0 ))																				
					{																																
																														
						productPriceTextBox.readOnly			 = true;																				
						productPriceTextBox.style.background = ""LightGrey"";																	
					}																																
					else																															
					{																																
						productPriceTextBox.readOnly         = false;																				
						productPriceTextBox.style.background = ""White"";	
																		
						if ((productPriceTextBox.value.replace(/ /g, """") != '') 
                     && (isNaN(parseFloat(productPriceTextBox.value)) == false))				
						{																															
							nonDomesticPriceTextBox.style.background = ""LightGrey"";															
							nonDomesticPriceTextBox.value				  = """";																		
							nonDomesticPriceTextBox.readOnly			  = true;
																		
							currencyUnitSelect.style.background		  = ""LightGrey"";															
							currencyUnitSelect.selectedIndex			  = 0;
																		
                     if (totalForeignPriceCntrl != null)
                     {
                        totalForeignPriceCntrl.value            = """";
                        totalForeignPriceCntrl.readOnly         = true;
                        totalForeignPriceCntrl.style.background = ""LightGrey"";
                     }
						}																															
						else																														
						{																															
							nonDomesticPriceTextBox.readOnly         = false;																				
							nonDomesticPriceTextBox.style.background = ""White"";
																	
							currencyUnitSelect.style.background      = ""White"";	
																
                     if (totalForeignPriceCntrl != null)
                     {
                        totalForeignPriceCntrl.readOnly         = false;
                        totalForeignPriceCntrl.style.background = ""White"";
                     }
						}																															
					}																																
				}
			";

		/// <summary>
		/// Initializes a new instance of the <see cref="LineItemCurrencyUnitFG"/> class.
		/// </summary>
		public LineItemCurrencyUnitFG()
		{
		}

		/// <summary>
		/// The generate.
		/// </summary>
		/// <param name="editable">
		/// The editable.
		/// </param>
		public override void Generate(bool editable)
		{
			base.Generate(editable);
			var updatePanel = this.cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var select = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

				// If not an HTML select control, then ignore.
				if (select != null)
				{
					if (select.Items[0].Text != this.NotSetText)
					{
						select.Items.Insert(0, new ListItem(this.NotSetText, this.NotSetText));
					}
				}
			}
		}

		/// <summary>
		/// Gets the field ID.
		/// </summary>
		public override string FieldID
		{
			get
			{
				return "LineItem CurrencyUnit";
			}
		}

		/// <summary>
		/// The get entries.
		/// </summary>
		/// <returns>
		/// The <see cref="HybridDictionary"/>.
		/// </returns>
		public override HybridDictionary GetEntries()
		{
			var entries = new HybridDictionary();

			if (transContext.Currencies != null)
			{
				foreach (CurrencyDO currency in transContext.Currencies)
				{
					if (currency.DisplayFlag)
					{
						entries.Add(currency.UnitDisplayName, currency.IdentityGuid.ToString());
					}
				}
			}

			if (entries.Count == 0)
			{
				entries.Add("None", Guid.Empty.ToString());
			}

			return entries;
		}

		#region ILineItemField Members
		/// <summary>
		/// The get data value.
		/// </summary>
		/// <param name="inLineItem">
		/// The line item.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public object GetDataValue(LineItemDO inLineItem)
		{
			if (inLineItem.CurrencyGuid == Guid.Empty)
			{
				return null;
			}

			if (this.transContext.Currencies != null)
			{
				foreach (CurrencyDO currencyDO in this.transContext.Currencies)
				{
					if (inLineItem.CurrencyGuid == currencyDO.IdentityGuid)
					{
						return currencyDO.IdentityGuid.ToString();
					}
				}
			}

			return null;
		}

		/// <summary>
		/// The get data text.
		/// </summary>
		/// <param name="inLineItem">
		/// The line item.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetDataText(LineItemDO inLineItem)
		{
			if (inLineItem.CurrencyGuid == Guid.Empty)
			{
				return string.Empty;
			}

			// Iterate through the context's currency collection
			if (transContext.Currencies != null)
			{
				foreach (CurrencyDO currencyDO in transContext.Currencies)
				{
					if (currencyDO.IdentityGuid == inLineItem.CurrencyGuid)
					{
						return currencyDO.UnitDisplayName;
					}
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// The set data value.
		/// </summary>
		/// <param name="inLineItem">
		/// The line item.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		public void SetDataValue(LineItemDO inLineItem, object newValue)
		{
			if (newValue == null)
			{
				inLineItem.CurrencyGuid = Guid.Empty;
			}
			else
			{
				if (newValue.ToString() == "None")
				{
					inLineItem.CurrencyGuid = Guid.Empty;
				}
				else if (newValue is Guid)
				{
					inLineItem.CurrencyGuid = (Guid)newValue;
				}
				else if (newValue is string)
				{
					inLineItem.CurrencyGuid = Guid.Parse((string)newValue);
				}
			}

			this.OnFieldChanged();
		}
		#endregion

		/// <summary>
		/// The specialize control.
		/// </summary>
		/// <param name="control">
		/// The control.
		/// </param>
		protected override void SpecializeControl(WebControl control)
		{
			base.SpecializeControl(control);
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var htmlSelect = updatePanel.ContentTemplateContainer.Controls[0] as HtmlSelect;

				if (htmlSelect == null)
				{
					return;
				}

				if (this.transContext.Currencies != null)
				{
					var currency = new CurrencyClass(this.transContext.security);

					htmlSelect.Attributes.Add("onChange", "javascript:try{CurrencyChange();}catch(err){;}");

					string rule = currency.ClientSideScript;

					// Client-side script that initializes rates array and then enables/disables currency related fields.
					htmlSelect.Page.ClientScript.RegisterStartupScript(
						this.GetType(),
						"CURRENCY_CHANGE",
						"<script language=\"javascript\" type=\"text/javascript\"><!--\n" + rule + "\n" + CLIENT_SIDE_FUNCTION_SCRIPT + "\n//--></script>");


					// Delay client side scripting until page pre-render event in case user clicks edit button of a
					// line item while editing another line item. Such situation causes this method to be called 
					// twice, once for for each line item. Since client side script is  allowed only once to be registered,
					// later line item's client script is ignored, which is the one we actually want.
					htmlSelect.Page.Session[CLIENT_SIDE_SCRIPT_LINEITEM_CURRENCY_UNIT] =
						"<script language=\"javascript\" type=\"text/javascript\"><!--\n" +
						"var oCurrencyUnitSelect  = document.getElementById('" + this.ClientID + "');\n " +
						"\n//--></script>";

					const string Onload = @"<script language=""javascript"" type=""text/javascript"">
						var f=function(){CurrencyChange();};
						if (window.addEventListener) window.addEventListener(""load"", f, false);
						else if (window.attachEvent) window.attachEvent(""onload"", f );
						else window.onload = f;
						</script>";

					htmlSelect.Page.ClientScript.RegisterStartupScript(this.GetType(), "ONLOAD", Onload);
				}
			}
		}
	}
}
