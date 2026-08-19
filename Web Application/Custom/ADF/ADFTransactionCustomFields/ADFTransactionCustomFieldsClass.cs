///<summary>
///	FILE NAME:  ADFTransactionCustomFields.cs
///	PURPOSE:		ADFTransactionCustomFields Class
///					This class is a custom class that is loaded and invoked via late binding.  The logic
///					in this class is confined to the custom field settings for financial requirements of
///					the JEFM ADF project.
///
///	COMMENTS:
///		Copyright (C) Varec, Inc. (An SAIC Company) Norcross, GA, USA, 2008
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Varec, Inc.
///
///	AUTHOR(S):	Eric Simmons
///	VERSION:		1.0.0  Current version
///
///	MODIFICATION HISTORY:
///   Date:			By:			         Reason:
///   ----------	-----------------    -------------------------------------------
///   10-09-2008	E. Simmons	         Initial Revison to support CSI #6153
///   2009-07-08  Richard Panachida    WI# 4115: Added a check for payment transaction. If so, then
///                                    ignore the flag01 setting.
///</summary>
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.Interfaces;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.UtilityObjects;

namespace ADFTransactionCustomFields
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class ADFTransactionCustomFieldsClass : IFMCustomFieldStates
	{
		public ADFTransactionCustomFieldsClass ( )
		{
		}

		void IFMCustomFieldStates.SetTransactionFieldStates ( SecurityClass security, System.Web.UI.Page page )
		{
			string aliasName = "";
			if (page.Request.Params["TransAlias"] != null)
			{
				aliasName = (string) page.Request.Params["TransAlias"];

				if (string.IsNullOrEmpty ( aliasName ) == false)
				{
					aliasName = aliasName.RemoveSemicolonAndTextAfter ( );
				}
			}

			CheckBox FreeToAidCheckBox = (CheckBox) page.FindControl ( "TransactionFields.Flag01FG" );
			if (( FreeToAidCheckBox != null ) && ( aliasName.Equals ( "Payment" ) == false ))
			{
				FreeToAidCheckBox.Enabled = security.HasRight ( RIGHT.PRIVILEGED_FINANCIAL ) && security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
			}

			TextBox PriceTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemProductPrice" );
			if (PriceTextBox != null)
			{
				PriceTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (PriceTextBox.Enabled == false)
				{
					PriceTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

			TextBox exciseTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemTax1FG" );
			if (exciseTextBox != null)
			{
				exciseTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (exciseTextBox.Enabled == false)
				{
					exciseTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

			TextBox gstTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemTax2FG" );
			if (gstTextBox != null)
			{
				gstTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (gstTextBox.Enabled == false)
				{
					gstTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

			TextBox markupTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemTax3FG" );
			if (markupTextBox != null)
			{
				markupTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (markupTextBox.Enabled == false)
				{
					markupTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

			TextBox CostExclusiveTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemTax4FG" );
			if (CostExclusiveTextBox != null)
			{
				CostExclusiveTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (CostExclusiveTextBox.Enabled == false)
				{
					CostExclusiveTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

			TextBox CostInclusiveTextBox = (TextBox) page.FindControl ( "TransactionFields.LineItemTax5FG" );
			if (CostInclusiveTextBox != null)
			{
				CostInclusiveTextBox.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (CostInclusiveTextBox.Enabled == false)
				{
					CostInclusiveTextBox.BackColor = System.Drawing.Color.LightGray;
				}
			}

		}

		void IFMCustomFieldStates.SetTransactionFieldState ( SecurityClass security, System.Web.UI.WebControls.WebControl control )
		{
			if (security == null || control == null)
				return;

			if (control.ID.IndexOf ( "TransactionFields.LineItemProductPrice" ) != -1 ||
				control.ID.IndexOf ( "TransactionFields.LineItemTax1FG" ) != -1 ||
				control.ID.IndexOf ( "TransactionFields.LineItemTax2FG" ) != -1 ||
				control.ID.IndexOf ( "TransactionFields.LineItemTax3FG" ) != -1 ||
				control.ID.IndexOf ( "TransactionFields.LineItemTax4FG" ) != -1 ||
				control.ID.IndexOf ( "TransactionFields.LineItemTax5FG" ) != -1)
			{
				control.Enabled = security.HasRight ( RIGHT.MODIFY_FINANCIAL_DATA );
				if (control.Enabled == false)
				{
					control.BackColor = System.Drawing.Color.LightGray;
				}
			}
		}

	}
}
