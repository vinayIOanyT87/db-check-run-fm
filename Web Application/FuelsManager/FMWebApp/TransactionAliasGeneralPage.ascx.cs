// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionAliasGeneralPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionAliasGeneralPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Net;
	using System.Web;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ReportSvr2005;
	using FMBusinessObjects.ServiceRequests;

	using FMControls;
    using FMCore;

	using global::FMWebApp;

	public partial class TransactionAliasGeneralPage : FMUserControlBase
	{
		#region Constants and Fields
		protected FMLabel Fmlabel1;
		protected TextBox HiddenID;
		#endregion

		#region Properties
		private string JavascriptClient
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					function TransactionTypeDropDownListChange()
					{
						var confirmtext=" + HttpUtility.JavaScriptStringEncode(this.GetTranslatedText("Reset assigned fields?"), true)
				                + @";
						var result=confirm(confirmtext);
						var theform;
						if (window.navigator.appName.toLowerCase().indexOf('microsoft') > -1) {
							theform = document.Form1;
						}
						else {
							theform = document.forms['Form1'];
						}
						theform.__EVENTTARGET.value = 'TransactionAliasGeneralPage:TransactionTypeDropDownList';
						theform.__EVENTARGUMENT.value = result;
						theform.submit();
					}
					//-->
				</script>
				";

				return script;
			}
		}
		#endregion

		#region Methods
		protected void MultipleGaugeReadingCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Reorder the Weight Readings to the end of the body
			if (transactionAlias.MultipleWeightReadings)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);

				if (fields.Length != 0)
				{
					int displayOrder = fields[fields.Length - 1].DisplayOrder + 1;
					fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.WEIGHT_READINGS);

					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			transactionAlias.MultipleWeightReadings = this.MultipleWeightReadingCheckBox.Checked;

			// Reorder the Weight Readings and the Body
			if (transactionAlias.MultipleLineItems)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.WEIGHT_READINGS);
				
				if (fields.Length != 0)
				{
					int displayOrder = 0;
					
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}

				fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
				
				if (fields.Length != 0)
				{
					int displayOrder = 0;
					
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			var orderPage = (TransactionAliasFieldOrderPage) this.Page.FindControl("tcTransactionAliasTabs")
				    .FindControl("tpFieldOrderPage").FindControl("TransactionAliasFieldOrderPage");
			orderPage.ReloadSectionTypeDropDown();
		}

		protected void MultipleLineItemCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Reorder the Line Items to the end of the body
			if (transactionAlias.MultipleLineItems)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
				
				if (fields.Length != 0)
				{
					int displayOrder = fields[fields.Length - 1].DisplayOrder + 1;
					fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);
					
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			transactionAlias.MultipleLineItems = this.MultipleLineItemCheckBox.Checked;

			// Reorder the LineItems and the Body
			if (transactionAlias.MultipleLineItems)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.LINE_ITEMS);
				
				if (fields.Length != 0)
				{				
					int displayOrder = 0;
					
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}

				fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
				
				if (fields.Length != 0)
				{
					int displayOrder = 0;

					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			var orderPage = (TransactionAliasFieldOrderPage) 
				this.Page.FindControl("tcTransactionAliasTabs").FindControl("tpFieldOrderPage").FindControl("TransactionAliasFieldOrderPage");

			orderPage.ReloadSectionTypeDropDown();
		}

		/// <summary>
		///    This method handles the event of the multiple transport line item check box being
		///    checked or unchecked.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void MultipleTransportLineItemCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Reorder the Transport Line Items to the end of the body
			if (transactionAlias.MultipleTransportLineItems)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);

				if (fields.Length != 0)
				{
					int displayOrder = fields[fields.Length - 1].DisplayOrder + 1;
					fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.TRANPORT_INFO);

					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			transactionAlias.MultipleTransportLineItems = this.MultipleTransportLineItemCheckBox.Checked;

			// Reorder the Transport Line Items and the Body
			if (transactionAlias.MultipleTransportLineItems)
			{
				FieldClass[] fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.TRANPORT_INFO);
				if (fields.Length != 0)
				{
					int displayOrder = 0;
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}

				fields = transactionAlias.DisplayOrder(TRANSACTION_SECTION_TYPE.BODY);
				if (fields.Length != 0)
				{
					int displayOrder = 0;
					foreach (FieldClass field in fields)
					{
						field.DisplayOrder = displayOrder++;
					}
				}
			}

			var orderPage = (TransactionAliasFieldOrderPage)
				this.Page.FindControl("tcTransactionAliasTabs").FindControl("tpFieldOrderPage").FindControl("TransactionAliasFieldOrderPage");
			
			orderPage.ReloadSectionTypeDropDown();
		}

		/// <summary>
		/// The on init.
		/// </summary>
		/// <param name="e">
		/// The e.
		/// </param>
		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			this.AliasDropDown.SelectedIndexChanged += this.AliasDropDownSelectedIndexChanged;
			this.ReportDropDown.SelectedIndexChanged += this.ReportDropDownSelectedIndexChanged;
			this.PreLoadReportDropDown.SelectedIndexChanged += this.PreLoadReportDropDownSelectIndexChanged;
			base.OnInit(e);
		}

		/// <summary>
		/// The page_ load.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				if (!this.Page.IsPostBack)
				{
					this.Identifier.Text = transactionAlias.ID;

					// Hide the report drop down list on page load (IGO 22-Aug-2007)
					this.ReportDropDown.Visible = false;
					this.ReportTextBox.Visible = true;
					this.ReportSetButton.Visible = true;

					// PreLoadReport is only enabled if the transTypeID = 5 or 6
					if (transactionAlias.TransTypeID == TransactionTypes.T5_PrimaryDisbursement
					    || transactionAlias.TransTypeID == TransactionTypes.T6_SecondaryDisbursement)
					{
						this.PreLoadReportDropDown.Visible = false;
						this.PreLoadReportTextbox.Visible = true;
						this.PreLoadReportSetButton.Visible = true;
					}
					else
					{
						this.PreLoadReportDropDown.Enabled = false;
						this.PreLoadReportLabel.Enabled = false;
						this.PreLoadReportTextbox.Enabled = false;
						this.PreLoadReportSetButton.Enabled = false;
					}

					bool orderEntryHardwareKey = this.CheckHardwareKey();

					// Populate TransactionTypeDropDownList
					for (var type = TransactionTypes.T1_PrimaryAdjustment; type < TransactionTypes.T_Maximum; type++)
					{
						// TODO: Temporary commented out so that QA does not test Invoices features.  Remove IF statement when ready for testing.
						if (type == TransactionTypes.T21_AccountPayableInvoice || type == TransactionTypes.T22_AccountReceivableInvoice)
						{
							continue;
						}

						if (type == TransactionTypes.T19_EndOfDay || type == TransactionTypes.T20_EndOfMonth
						    || type == TransactionTypes.T24_Aggregate)
						{
							continue;
						}

						// If the hardware key is not set, do not let Order alias be an option
						if ((type == TransactionTypes.T17_Order || type == TransactionTypes.T18_SupplyOrder)
						    && orderEntryHardwareKey == false)
						{
							continue;
						}

						var item = new ListItem(TransactionAliasClass.TransactionTypeID(type), ((int)type).ToString());
						this.TransactionTypeDropDownList.Items.Add(item);
						if (transactionAlias.TransTypeID == type)
						{
							this.TransactionTypeDropDownList.SelectedIndex = this.TransactionTypeDropDownList.Items.Count - 1;
						}
					}

					for (var type = TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY;
					     type <= TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID;
					     type++)
					{
						var item = new ListItem(TransactionAliasClass.GetShowCompanyDisplayName(type), ((short)type).ToString());
						this.ShowCompanyNameDropDownList.Items.Add(item);
						if (transactionAlias.ShowCompanyName == type)
						{
							this.ShowCompanyNameDropDownList.SelectedIndex = this.ShowCompanyNameDropDownList.Items.Count - 1;
						}
					}

					this.MeterCloseoutCheckBox.Checked = transactionAlias.MeterCloseout;

					// Only allow one Type 12 as Meter Closout
					if (transactionAlias.TransTypeID == TransactionTypes.T12_InventoryNotAffected)
					{
						var transactionAliasCollection =
							FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
								x => x.EnumerateByTransTypeID(this.Security, TransactionTypes.T12_InventoryNotAffected));

						bool meterCloseoutExists = false;
						foreach (TransactionAliasClass existingTransactionAlias in transactionAliasCollection)
						{
							if (existingTransactionAlias.MeterCloseout
							    && transactionAlias.IdentityGuid != existingTransactionAlias.IdentityGuid)
							{
								meterCloseoutExists = true;
								break;
							}
						}

						if (meterCloseoutExists)
						{
							this.MeterCloseoutCheckBox.Checked = false;
							this.MeterCloseoutCheckBox.Enabled = false;
						}
					}
					else
					{
						this.MeterCloseoutCheckBox.Checked = false;
						this.MeterCloseoutCheckBox.Enabled = false;
					}

					this.BulkShipmentCheckBox.Checked = transactionAlias.BulkShipment;
					this.DistributedImpactCheckBox.Checked = transactionAlias.DistributedImpact;
					this.MultipleLineItemCheckBox.Checked = transactionAlias.MultipleLineItems;
					this.MultipleWeightReadingCheckBox.Checked = transactionAlias.MultipleWeightReadings;
					this.LimitSelectionsBasedOnHierarchyCheckBox.Checked = transactionAlias.LimitSelectionsBasedOnHierarchy;
					this.MultipleTransportLineItemCheckBox.Checked = transactionAlias.MultipleTransportLineItems;
					this.IncludeInDispatchCheckBox.Checked = transactionAlias.IncludeInDispatch;

					this.UseComboBoxControlsCheckBox.Checked = transactionAlias.UseComboxControls;
					this.EnableAutoCompleteCheckBox.Checked = transactionAlias.EnableAutoCompleteControls;
					this.PermitNonReferenceDataCheckBox.Enabled = transactionAlias.UseComboxControls;
					this.PermitNonReferenceDataCheckBox.Checked = transactionAlias.PermitNonReferenceData;

					// assign the associatied report is one exists. (IGO 22-Aug-2007)
					if (0 != transactionAlias.AssociatedReport.Length)
					{
						this.ReportTextBox.Text = transactionAlias.AssociatedReport;
					}

					if (0 != transactionAlias.AssociatedPreloadReport.Length)
					{
						this.PreLoadReportTextbox.Text = transactionAlias.AssociatedPreloadReport;
					}

					if ((transactionAlias.TransTypeID == TransactionTypes.T17_Order
					|| transactionAlias.TransTypeID == TransactionTypes.T18_SupplyOrder) && this.CheckHardwareKey())
					{
						this.LoadAliasAssociationDropDown();
					}
					else
					{
						// Hide the Order only configuration fields
						this.AliasLabel.Visible = false;
						this.AliasDropDown.Visible = false;
					}

					this.LimitSelectionsBasedOnHierarchyCheckBoxCheckedChanged(null, null);
					this.UseComboBoxControlsCheckBoxCheckedChanged(null, null);
					this.PermitNonReferenceDataCheckBoxCheckedChanged(null, null);
					this.EnableAutoCompleteCheckBoxCheckedChanged(null, null);

					this.LoadUserGroupAssignmentListBoxes();
                    this.SetFieldAccessibilityForChildRecordVersion();
                }
				else
				{
					transactionAlias.ID									= this.Identifier.Text;
					transactionAlias.MeterCloseout						= this.MeterCloseoutCheckBox.Checked;
					transactionAlias.BulkShipment						= this.BulkShipmentCheckBox.Checked;
					transactionAlias.DistributedImpact					= this.DistributedImpactCheckBox.Checked;
					transactionAlias.LimitSelectionsBasedOnHierarchy	= this.LimitSelectionsBasedOnHierarchyCheckBox.Checked;
					transactionAlias.UseComboxControls					= this.UseComboBoxControlsCheckBox.Checked;
					transactionAlias.IncludeInDispatch					= this.IncludeInDispatchCheckBox.Checked;
					transactionAlias.EnableAutoCompleteControls			= this.EnableAutoCompleteCheckBox.Checked;
					transactionAlias.PermitNonReferenceData				= this.PermitNonReferenceDataCheckBox.Checked;					

					transactionAlias.ShowCompanyName = (TRANSACTION_SHOW_COMPANY_NAME)
						Enum.Parse(typeof(TRANSACTION_SHOW_COMPANY_NAME), this.ShowCompanyNameDropDownList.SelectedItem.Value);
				}

				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "TransactionAliasGeneralPageClientScriptBlock", this.JavascriptClient);
            }
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void PreLoadReportDropDownSelectIndexChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			transactionAlias.AssociatedPreloadReport = this.PreLoadReportDropDown.SelectedItem.Text;
			this.PreLoadReportTextbox.Text = transactionAlias.AssociatedPreloadReport;

			// Hide the report drop down list after new item is selected 
			this.PreLoadReportDropDown.Visible = false;
			this.PreLoadReportTextbox.Visible = true;
			this.PreLoadReportSetButton.Visible = true;
		}

		protected void PreLoadReportSetButtonClick(object sender, EventArgs e)
		{
			// Hide the report text box and set button (IGO 22-Aug-2007)
			this.PreLoadReportDropDown.Visible = true;
			this.PreLoadReportTextbox.Visible = false;
			this.PreLoadReportSetButton.Visible = false;

			// Load the report drop down list
			this.LoadReportAssociationDropDown(this.PreLoadReportDropDown);
		}

		protected void ReportDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			transactionAlias.AssociatedReport = this.ReportDropDown.SelectedItem.Text;
			this.ReportTextBox.Text = transactionAlias.AssociatedReport;

			// Hide the report drop down list after new item is selected (IGO 22-Aug-2007)
			this.ReportDropDown.Visible = false;
			this.ReportTextBox.Visible = true;
			this.ReportSetButton.Visible = true;
		}

		protected void ReportSetButtonClick(object sender, EventArgs e)
		{
			// Hide the report text box and set button (IGO 22-Aug-2007)
			this.ReportDropDown.Visible = true;
			this.ReportTextBox.Visible = false;
			this.ReportSetButton.Visible = false;

			// Load the report drop down list
			this.LoadReportAssociationDropDown(this.ReportDropDown);
		}

		protected void TransactionTypeDropDownListSelectedIndexChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			if (this.Request.GetQueryOrFormValue("__EVENTARGUMENT") == "true")
			{
				transactionAlias.TransTypeID = (TransactionTypes)Convert.ToByte(this.TransactionTypeDropDownList.SelectedValue);
			}
			else
			{
				transactionAlias._TransTypeID = (TransactionTypes)Convert.ToByte(this.TransactionTypeDropDownList.SelectedValue);
			}

			if ((transactionAlias.TransTypeID == TransactionTypes.T17_Order)
			    || (transactionAlias.TransTypeID == TransactionTypes.T18_SupplyOrder))
			{
				transactionAlias.MultipleLineItems = true;
				this.MultipleLineItemCheckBox.Checked = true;
			}

			this.LimitSelectionsBasedOnHierarchyCheckBoxCheckedChanged(null, null);
			this.UseComboBoxControlsCheckBoxCheckedChanged(null, null);
			this.PermitNonReferenceDataCheckBoxCheckedChanged(null, null);
			this.EnableAutoCompleteCheckBoxCheckedChanged(null, null);

			this.Redirect("TransactionAliasForm.aspx");
		}

		protected void UseComboBoxControlsCheckBoxCheckedChanged(object sender, EventArgs e)
		{
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (!this.UseComboBoxControlsCheckBox.Checked)
			{
                if ((transactionAlias == null) || (transactionAlias.IdentityGuid.Equals(Guid.Empty))
                    || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid)))
                {
					this.PermitNonReferenceDataCheckBox.Checked = false;
					this.PermitNonReferenceDataCheckBox.Enabled = false;
					this.ShowCompanyNameDropDownList.Enabled = true;
                }
                else 
                {
					this.PermitNonReferenceDataCheckBox.Checked = false;
					this.PermitNonReferenceDataCheckBox.Enabled = false;
                    if (versionSpecificFields != null)
                        this.ShowCompanyNameDropDownList.Enabled = versionSpecificFields.Contains("ShowCompanyName");
                }
			}
			else
			{
				if (!this.LimitSelectionsBasedOnHierarchyCheckBox.Checked)
				{
					this.PermitNonReferenceDataCheckBox.Enabled = true;
				}

				this.ShowCompanyNameDropDownList.SelectedValue = ((short)TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY).ToString();
				this.ShowCompanyNameDropDownList.Enabled = false;
			}
		}

		protected void PermitNonReferenceDataCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			if (this.PermitNonReferenceDataCheckBox.Checked)
			{
				this.LimitSelectionsBasedOnHierarchyCheckBox.Enabled = false;
				this.LimitSelectionsBasedOnHierarchyCheckBox.Checked = false;
			}
			else
				this.LimitSelectionsBasedOnHierarchyCheckBox.Enabled = true;
		}

		protected void EnableAutoCompleteCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			if ( !this.UseComboBoxControlsCheckBox.Checked && !this.EnableAutoCompleteCheckBox.Checked )
			{
				this.PermitNonReferenceDataCheckBox.Checked = false;
				this.PermitNonReferenceDataCheckBox.Enabled = false;
				this.ShowCompanyNameDropDownList.Enabled = true;
			}
			else
			{
				if ( !this.LimitSelectionsBasedOnHierarchyCheckBox.Checked )
				{
					this.PermitNonReferenceDataCheckBox.Enabled = true;
				}

				this.ShowCompanyNameDropDownList.SelectedValue = ( (short)TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY ).ToString(CultureInfo.InvariantCulture);
				this.ShowCompanyNameDropDownList.Enabled = false;
			}
		}

		/// <summary>
		///    Adds a list item to a list box in alphabetical order
		/// </summary>
		/// <param name="listBox">ListBox to add to</param>
		/// <param name="newListItem">ListItem to insert</param>
		private void AddListItemToListBox(ListBox listBox, ListItem newListItem)
		{
			int idx = 0;

			foreach (ListItem existingListItem in listBox.Items)
			{
				if (String.Compare(existingListItem.Text, newListItem.Text, StringComparison.Ordinal) > 0)
				{
					// The existing is after the new alphabetically, so add
					// right before the existing
					listBox.Items.Insert(idx, newListItem);
					return;
				}

				idx++;
			}

			// New one belongs at the end
			listBox.Items.Add(newListItem);
		}

		private void AliasDropDownSelectedIndexChanged(object sender, EventArgs e)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			transactionAlias.AssociatedTransactionAliasGuid = Guid.Parse(this.AliasDropDown.SelectedValue);
			
			if (transactionAlias.AssociatedTransactionAliasGuid == Guid.Empty)
			{
				transactionAlias.AssociatedAlias = string.Empty;
			}
			else
			{
				transactionAlias.AssociatedAlias = this.AliasDropDown.SelectedItem.Text;
			}
		}

		private bool CheckHardwareKey()
		{
			// Check the hardware key
			return FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsAnOrderEntryKey());
		}

		private bool CheckOrderSecurity()
		{
			if (this.CheckHardwareKey() == false)
			{
				return false;
			}

			// Check security for this page
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			return this.Security.HasViewTransactionRightByAliasName(transactionAlias.ID);
		}

		private void HandleReportErrorCondition(ErrorObject dataObj)
		{
			var logger = new Logger("OrderEntry");
			logger.Error("TransactionAliasGeneralPage.ascx.cs - " + (dataObj).ErrorMessage);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.ModifyUnassignGroupsButton.Command += this.ModifyUnassignGroupsButtonCommand;
			this.ModifyAssignGroupsButton.Command += this.ModifyAssignGroupsButtonCommand;
			this.ViewUnassignGroupsButton.Command += this.ViewUnassignGroupsButtonCommand;
			this.ViewAssignGroupsButton.Command += this.ViewAssignGroupsButtonCommand;
		}

		private void LoadAliasAssociationDropDown()
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			// Get the aliases assigned to the current context & exclude Orders

			// Create a new service request
			var sr = new TransactionAliasListSR();

			// Initialize service reqeust
			sr.Security = this.Security;

			// Get the aliases
			TransactionAliasListDO aliasListDO =
				FMChannelHelper.MakeCall<ITransactionAliasListProcessor, TransactionAliasListDO>(x => x.Process(sr));

			if (aliasListDO.aliasList.Keys.Count > 0)
			{
				this.AliasDropDown.Items.Add(new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString()));

				foreach (TransactionAliasClass alias in aliasListDO.aliasList.Values)
				{
					if (transactionAlias.TransTypeID == TransactionTypes.T17_Order
					    && (alias.TransTypeID != TransactionTypes.T5_PrimaryDisbursement
					        && alias.TransTypeID != TransactionTypes.T6_SecondaryDisbursement
					        && alias.TransTypeID != TransactionTypes.T3_PrimaryDefuel
					        && alias.TransTypeID != TransactionTypes.T4_SecondaryDefuel
					        && alias.TransTypeID != TransactionTypes.T25_Shipment))
					{
						continue;
					}

					if (transactionAlias.TransTypeID == TransactionTypes.T18_SupplyOrder
					    && alias.TransTypeID != TransactionTypes.T8_Receipt)
					{
						continue;
					}

					this.AliasDropDown.Items.Add(new ListItem(alias.ID, alias.MasterRecordGuid.ToString()));
				}

				// Now select the one saved for this alias
				for (int nLoop = 0; nLoop < this.AliasDropDown.Items.Count; ++nLoop)
				{
					if (transactionAlias.AssociatedTransactionAliasGuid.ToString() == this.AliasDropDown.Items[nLoop].Value)
					{
						this.AliasDropDown.SelectedIndex = nLoop;
						return;
					}
				}

				// If got here, the last known associated alias is no longer valid so just pick one.
				this.AliasDropDown.SelectedIndex = 0;
				transactionAlias.AssociatedTransactionAliasGuid = Guid.Empty;
				transactionAlias.AssociatedAlias = string.Empty;
			}
		}

		private void LoadReportAssociationDropDown(DropDownList reportDropDownList)
		{
			// clear previous contents (IGO 22-Aug-2007)
			reportDropDownList.Items.Clear();

			// Load the Report selection drop down
			int idx = 0;
			var listItem = new ListItem(this.GetTranslatedText("{None}"), idx.ToString(CultureInfo.InvariantCulture));
			reportDropDownList.Items.Add(listItem);
			idx++;

			try
			{
				SystemSettingClass systemSetting =
					FMChannelHelper.MakeCall<ISystemSettings, SystemSettingClass>(x => x.Get(this.Security));

				SiteClass siteClass = FMChannelHelper.MakeCall<ISites, SiteClass>(
					sites => sites.GetBasic(this.Security, this.Security.SiteGuid));

				////**** Use ReportServerCredentials when running in azure. Use dbAccessClient when not Azure *******
				var reportingService = new ReportingService2005
				                       {
					                       Url = systemSetting.ReportServerUrl + "/ReportService2005.asmx",
					                       CookieContainer = new CookieContainer()
				                       };

				if (!string.IsNullOrEmpty(systemSetting.ReportServerUserName))
				{
					string[] userName = systemSetting.ReportServerUserName.Split('\\');
					if (userName.Length > 1)
					{
						reportingService.Credentials = new NetworkCredential(userName[1], systemSetting.ReportServerPassword, userName[0]);
					}
					else
					{
						reportingService.Credentials = new NetworkCredential(userName[0], systemSetting.ReportServerPassword, ".");
					}
				}
				else
				{
					reportingService.Credentials = CredentialCache.DefaultCredentials;
				}

				//replace // with / if necessary.  ReportPath in db may or may not have preceeding /
				string tempPath = ("/" + siteClass.ReportDirectory).Replace("//", "/");
				
				//remove trailing / if necessary
				if (tempPath.Substring(tempPath.Length - 1) == "/")
				{
					tempPath = tempPath.Substring(0, tempPath.Length - 1);
				}

				CatalogItem[] items = reportingService.ListChildren(tempPath, false);

				foreach (CatalogItem item in items)
				{
					if (item.Type != ItemTypeEnum.Report && item.Type != ItemTypeEnum.LinkedReport)
					{
						continue;
					}

					// Create a new item
					listItem = new ListItem(item.Name, idx.ToString());

					// Add the item to the drop down control
					reportDropDownList.Items.Add(listItem);

					++idx;
				}

				var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

				for (int nLoop = 0; nLoop < reportDropDownList.Items.Count; ++nLoop)
				{
					if (reportDropDownList == this.ReportDropDown
					    && transactionAlias.AssociatedReport == reportDropDownList.Items[nLoop].Text)
					{
						reportDropDownList.SelectedIndex = nLoop;
						return;
					}

					if (reportDropDownList == this.PreLoadReportDropDown
					    && transactionAlias.AssociatedPreloadReport == reportDropDownList.Items[nLoop].Text)
					{
						reportDropDownList.SelectedIndex = nLoop;
						return;
					}
				}

				// If got here, the saved report value is not valid so just select one.
				if (reportDropDownList == this.ReportDropDown && !string.IsNullOrEmpty(transactionAlias.AssociatedReport))
				{
					reportDropDownList.SelectedIndex = 0;
					transactionAlias.AssociatedReport = reportDropDownList.SelectedValue;
				}

				if (reportDropDownList == this.PreLoadReportDropDown && !string.IsNullOrEmpty(transactionAlias.AssociatedPreloadReport))
				{
					reportDropDownList.SelectedIndex = 0;
					transactionAlias.AssociatedPreloadReport = reportDropDownList.SelectedValue;
				}
			}
			catch (Exception e)
			{
				var errorObj = new ErrorObject { ErrorLevel = ErrorObject.ErrorLevels.ERROR, ErrorMessage = e.Message };
				this.HandleReportErrorCondition(errorObj);
			}
		}

		private void LoadUserGroupAssignmentListBoxes()
		{
			FMChannelHelper.MakeCall<IGroups>(
				groups => FMChannelHelper.MakeCall<IRights>(rights => this.LoadUserGroupAssignmentListBoxesActual(groups, rights)));
		}

		private void LoadUserGroupAssignmentListBoxesActual(IGroups groups, IRights rights)
		{
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

			foreach (
				GroupTransactionAliasMapClass groupTransactionAliasMap in transactionAlias.GroupTransactionAliasMapCollection)
			{
				//GroupClass newgroup = groups.Get(this.Security, groupTransactionAliasMap.GroupGuid);

				if (groupTransactionAliasMap.Right == GroupTransactionAliasMapClass.RIGHT.MODIFY)
				{
					// Add to both Modify and View assigned groups list boxes
					var newUserGroupListItem = new ListItem(groupTransactionAliasMap.ID, groupTransactionAliasMap.GroupGuid.ToString());
					this.AddListItemToListBox(this.ModifyAssignedUserGroupsListBox, newUserGroupListItem);
					this.AddListItemToListBox(this.ViewAssignedUserGroupsListBox, newUserGroupListItem);
				}
				else
				{
					// Add to View assigned groups list box
					var newUserGroupListItem = new ListItem(groupTransactionAliasMap.ID, groupTransactionAliasMap.GroupGuid.ToString());
					this.AddListItemToListBox(this.ViewAssignedUserGroupsListBox, newUserGroupListItem);
				}
			}

			// Load the UnassignedUserGroups
			GroupCollectionClass groupCollection = groups.Enumerate(this.Security);
			foreach (GroupClass group in groupCollection)
			{
				group.RightCollection = rights.EnumerateByGroup( this.Security, group.IdentityGuid );

				if (group.RightCollection.RightInCollection(RIGHT.MODIFY_TRANSACTION_DATA)
				    && this.ModifyAssignedUserGroupsListBox.Items.FindByText(group.ID) == null)
				{
					// Modify unassigned groups
					var newUserGroupListItem = new ListItem(group.ID, group.IdentityGuid.ToString());
					this.AddListItemToListBox(this.ModifyUnassignedUserGroupsListBox, newUserGroupListItem);
				}

				if ((group.RightCollection.RightInCollection(RIGHT.MODIFY_TRANSACTION_DATA)
				     || group.RightCollection.RightInCollection(RIGHT.VIEW_TRANSACTION_DATA))
				    && this.ViewAssignedUserGroupsListBox.Items.FindByText(group.ID) == null)
				{
					// View unassigned groups
					var newUserGroupListItem = new ListItem(group.ID, group.IdentityGuid.ToString());
					this.AddListItemToListBox(this.ViewUnassignedUserGroupsListBox, newUserGroupListItem);
				}
			}
		}

		private void ModifyAssignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			FMChannelHelper.MakeCall<IFMEventLog>(
				fmLog =>
					{
						// The user moved an item from Modify Unassigned to Assigned. Update the list boxes and the collection.
						// Also, move to View Assigned list box if not already there.
						var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

						ListItem unassignedUserGroupItem;
						while ((unassignedUserGroupItem = this.ModifyUnassignedUserGroupsListBox.SelectedItem) != null)
						{
							unassignedUserGroupItem.Selected = false;

							// Remove from Modify Unassigned list box
							this.ModifyUnassignedUserGroupsListBox.Items.Remove(unassignedUserGroupItem);

							// Add to Modify Assigned list box
							this.AddListItemToListBox(this.ModifyAssignedUserGroupsListBox, unassignedUserGroupItem);

							// If it's in View Unassigned, move it to View Assigned
							if (this.ViewUnassignedUserGroupsListBox.Items.FindByText(unassignedUserGroupItem.Text) != null)
							{
								this.ViewUnassignedUserGroupsListBox.Items.Remove(unassignedUserGroupItem);
								this.AddListItemToListBox(this.ViewAssignedUserGroupsListBox, unassignedUserGroupItem);
							}

							// Create map object
							var groupTransactionAliasMap = new GroupTransactionAliasMapClass
								{
									TransactionAliasGuid = transactionAlias.IdentityGuid,
									ID = unassignedUserGroupItem.Text,
									GroupGuid = Guid.Parse(unassignedUserGroupItem.Value),
									Right = GroupTransactionAliasMapClass.RIGHT.MODIFY
								};

						
							// Remove from map collection if Guid combination is already there
							transactionAlias.GroupTransactionAliasMapCollection.RemoveAll(
								x =>
								(x.GroupGuid == groupTransactionAliasMap.GroupGuid
								 && x.TransactionAliasGuid == groupTransactionAliasMap.TransactionAliasGuid));

						
							// Add to map collection
							transactionAlias.GroupTransactionAliasMapCollection.Add(groupTransactionAliasMap);
						}
						
					});
		}

		private void ModifyUnassignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			// The user moved an item from Modify Assigned to Unassigned. Update the list boxes and change the
			// right from Modify to View.
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			ListItem assignedUserGroupItem;
			while ((assignedUserGroupItem = this.ModifyAssignedUserGroupsListBox.SelectedItem) != null)
			{
				assignedUserGroupItem.Selected = false;

				// Remove from Modify Assigned list box
				this.ModifyAssignedUserGroupsListBox.Items.Remove(assignedUserGroupItem);

				// Add to Modify Unassigned list box
				this.AddListItemToListBox(this.ModifyUnassignedUserGroupsListBox, assignedUserGroupItem);

				// Create map object
				var groupTransactionAliasMap = new GroupTransactionAliasMapClass
				                               {
					                               TransactionAliasGuid = transactionAlias.IdentityGuid,
					                               ID = assignedUserGroupItem.Text,
					                               GroupGuid = Guid.Parse(assignedUserGroupItem.Value),
					                               Right = GroupTransactionAliasMapClass.RIGHT.VIEW
				                               };

				// Remove from map collection if Guid combination is already there
				transactionAlias.GroupTransactionAliasMapCollection.RemoveAll(
					x =>
					(x.GroupGuid == groupTransactionAliasMap.GroupGuid
					 && x.TransactionAliasGuid == groupTransactionAliasMap.TransactionAliasGuid));

				// Add to map collection
				transactionAlias.GroupTransactionAliasMapCollection.Add(groupTransactionAliasMap);
			}
		}

		private void ViewAssignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			FMChannelHelper.MakeCall<IFMEventLog>(
				fmLog =>
					{

						// The user moved an item from View Unassigned to Assigned. Update the list boxes and the collection.
						var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];

						ListItem unassignedUserGroupItem;
						while ((unassignedUserGroupItem = this.ViewUnassignedUserGroupsListBox.SelectedItem) != null)
						{
							unassignedUserGroupItem.Selected = false;

							// Remove from View Unassigned list box
							this.ViewUnassignedUserGroupsListBox.Items.Remove(unassignedUserGroupItem);

							// Add to View Assigned list box
							this.AddListItemToListBox(this.ViewAssignedUserGroupsListBox, unassignedUserGroupItem);

							// Create map object
							var groupTransactionAliasMap = new GroupTransactionAliasMapClass
							                               {
								                               TransactionAliasGuid = transactionAlias.IdentityGuid,
								                               ID = unassignedUserGroupItem.Text,
								                               GroupGuid = Guid.Parse(unassignedUserGroupItem.Value),
								                               Right = GroupTransactionAliasMapClass.RIGHT.VIEW
							                               };

							// Add to map collection
							transactionAlias.GroupTransactionAliasMapCollection.Add(groupTransactionAliasMap);
						}
						
					});
		}

		private void ViewUnassignGroupsButtonCommand(object sender, CommandEventArgs e)
		{
			// The user moved an item from View Assigned to Unassigned. Update the list boxes and collection.
			// Also, move from Modify Assigned to Unassigned list box if necessary.
			var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
			ListItem assignedUserGroupItem;
			while ((assignedUserGroupItem = this.ViewAssignedUserGroupsListBox.SelectedItem) != null)
			{
				assignedUserGroupItem.Selected = false;

				// Remove from View Assigned list box
				this.ViewAssignedUserGroupsListBox.Items.Remove(assignedUserGroupItem);

				// Add to View Unassigned list box
				this.AddListItemToListBox(this.ViewUnassignedUserGroupsListBox, assignedUserGroupItem);

				// If it's in Modify Assigned, move it to Modify Unassigned
				if (this.ModifyAssignedUserGroupsListBox.Items.FindByText(assignedUserGroupItem.Text) != null)
				{
					this.ModifyAssignedUserGroupsListBox.Items.Remove(assignedUserGroupItem);
					this.AddListItemToListBox(this.ModifyUnassignedUserGroupsListBox, assignedUserGroupItem);
				}

				// Remove from map collection
				transactionAlias.GroupTransactionAliasMapCollection.RemoveAll(
					x =>
					(x.GroupGuid == Guid.Parse(assignedUserGroupItem.Value) && x.TransactionAliasGuid == transactionAlias.IdentityGuid));
			}
		}

		protected void LimitSelectionsBasedOnHierarchyCheckBoxCheckedChanged(object sender, EventArgs e)
		{
			if (this.LimitSelectionsBasedOnHierarchyCheckBox.Checked)
			{
				this.PermitNonReferenceDataCheckBox.Checked = false;
				this.PermitNonReferenceDataCheckBox.Enabled = false;
			}
			else if (this.UseComboBoxControlsCheckBox.Checked || this.EnableAutoCompleteCheckBox.Checked)
			{
				this.PermitNonReferenceDataCheckBox.Enabled = true;
			}
		}


        private void SetFieldAccessibilityForChildRecordVersion()
        {
            var transactionAlias = (TransactionAliasClass)this.Session["TransactionAlias"];
            var versionSpecificFields = this.Session[PageSessionKeyConstants.TRANS_ALIAS_VERSION_SPECIFIC_FIELDS] as List<string>;
            bool currentSiteOwnsRecordVersion = (transactionAlias.SiteGuid == this.Security.SiteGuid);

            if (versionSpecificFields != null && (transactionAlias.IdentityGuid.Equals(Guid.Empty)
                                              || (currentSiteOwnsRecordVersion && transactionAlias.IdentityGuid.Equals(transactionAlias.MasterRecordGuid))))
            {
                return;
            }

            if (versionSpecificFields != null)
            {
                this.Identifier.Enabled = (this.Identifier.Enabled && versionSpecificFields.Contains("AliasName"));
                this.TransactionTypeDropDownList.Enabled = (this.TransactionTypeDropDownList.Enabled && versionSpecificFields.Contains("LookupTransTypeIndex"));
                this.AliasDropDown.Enabled = (this.AliasDropDown.Enabled && versionSpecificFields.Contains("AssociatedTransactionAliasGuid"));
                this.ShowCompanyNameDropDownList.Enabled = (this.ShowCompanyNameDropDownList.Enabled && versionSpecificFields.Contains("ShowCompanyName"));
                this.ReportTextBox.Enabled = (this.ReportTextBox.Enabled && versionSpecificFields.Contains("AssociatedReport"));
                this.ReportSetButton.Enabled = (this.ReportSetButton.Enabled && versionSpecificFields.Contains("AssociatedReport"));
                this.ReportDropDown.Enabled = (this.ReportDropDown.Enabled && versionSpecificFields.Contains("AssociatedReport"));
                this.PreLoadReportTextbox.Enabled = (this.PreLoadReportTextbox.Enabled && versionSpecificFields.Contains("AssociatedPreloadReport"));
                this.PreLoadReportSetButton.Enabled = (this.PreLoadReportSetButton.Enabled && versionSpecificFields.Contains("AssociatedPreloadReport"));
                this.MeterCloseoutCheckBox.Enabled = (this.MeterCloseoutCheckBox.Enabled && versionSpecificFields.Contains("MeterCloseout"));
                this.LimitSelectionsBasedOnHierarchyCheckBox.Enabled = (this.LimitSelectionsBasedOnHierarchyCheckBox.Enabled && versionSpecificFields.Contains("LimitSelectionsBasedOnHierarchy"));
                this.MultipleTransportLineItemCheckBox.Enabled = (this.MultipleTransportLineItemCheckBox.Enabled && versionSpecificFields.Contains("MultipleTransportLineItems"));
                this.DistributedImpactCheckBox.Enabled = (this.DistributedImpactCheckBox.Enabled && versionSpecificFields.Contains("DistributedImpact"));
                this.BulkShipmentCheckBox.Enabled = (this.BulkShipmentCheckBox.Enabled && versionSpecificFields.Contains("BulkShipment"));
                this.UseComboBoxControlsCheckBox.Enabled = (this.UseComboBoxControlsCheckBox.Enabled && versionSpecificFields.Contains("UseComboBoxControls"));
                this.EnableAutoCompleteCheckBox.Enabled = ( this.EnableAutoCompleteCheckBox.Enabled && versionSpecificFields.Contains( "EnableAutoCompleteControls" ) );
                this.MultipleWeightReadingCheckBox.Enabled = (this.MultipleWeightReadingCheckBox.Enabled && versionSpecificFields.Contains( "MultipleWeightReadings" ) );
                this.MultipleLineItemCheckBox.Enabled = (this.MultipleLineItemCheckBox.Enabled && versionSpecificFields.Contains("MultipleLineItems"));
                this.IncludeInDispatchCheckBox.Enabled = (this.IncludeInDispatchCheckBox.Enabled && versionSpecificFields.Contains("IncludeInDispatch"));
                this.ModifyAssignGroupsButton.Enabled = (this.ModifyAssignGroupsButton.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ModifyUnassignGroupsButton.Enabled = (this.ModifyUnassignGroupsButton.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ViewAssignGroupsButton.Enabled = (this.ViewAssignGroupsButton.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ViewUnassignGroupsButton.Enabled = (this.ViewUnassignGroupsButton.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ModifyUnassignedUserGroupsListBox.Enabled = (this.ModifyUnassignedUserGroupsListBox.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ModifyAssignedUserGroupsListBox.Enabled = (this.ModifyAssignedUserGroupsListBox.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ViewUnassignedUserGroupsListBox.Enabled = (this.ViewUnassignedUserGroupsListBox.Enabled && versionSpecificFields.Contains("UserGroups"));
                this.ViewAssignedUserGroupsListBox.Enabled = (this.ViewAssignedUserGroupsListBox.Enabled && versionSpecificFields.Contains("UserGroups"));
            }
        }
		#endregion
	}
}