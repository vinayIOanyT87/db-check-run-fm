// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfileAnalogSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfileAnalogSettingPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.Constants;
	using FMBusinessObjects.DataObjects;

	using FMControls;

	using global::FMWebApp;

	/// <summary>
	///    This class is the code behind to handle the control of the Profile Analog Input
	///    page that is part of a multi-tab page.
	/// </summary>
	public partial class ProfileAnalogSettingPage : FMUserControlBase
	{
		#region Constants and Fields

		/// <summary>
		///    The formula 01.
		/// </summary>
		private const string Formula01 = "X = Y * A + B";

		/// <summary>
		///    The formula 02.
		/// </summary>
		private const string Formula02 = "X = (Y / A) * (B - C)";

		/// <summary>
		///    The formula 03.
		/// </summary>
		private const string Formula03 = "X = (Y - A) / (B - C)";

		/// <summary>
		///    The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///    This method will reset all the fields when the new button is
		///    selected.
		/// </summary>
		public void ResetFieldsForNewEvent()
		{
			this.UpdateView();
		}

		/// <summary>
		///    This method will update the profile configuration table from the general page.
		/// </summary>
		public void UpdateChanges()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ((this.mobileDeviceProfile == null) || (this.mobileDeviceProfile.AnalogInputCollection.Count == 0))
			{
				return;
			}

			MobileDeviceProfileAnalogInputCollection analogInputCollection = this.mobileDeviceProfile.AnalogInputCollection;
			int collectionIndex = 0;

			foreach (DataGridItem gridItem in this.AnalogInputDataGrid.Items)
			{
				var lowLimitTxb = gridItem.FindControl("LowLimitTextBox") as FMTextBox;
				var highLimitTxb = gridItem.FindControl("HighLimitTextBox") as FMTextBox;
				var parameterATxb = gridItem.FindControl("ParameterATextBox") as FMTextBox;
				var parameterBTxb = gridItem.FindControl("ParameterBTextBox") as FMTextBox;
				var parameterCTxb = gridItem.FindControl("ParameterCTextBox") as FMTextBox;
				var analogInputDd = gridItem.FindControl("AnalogFormulaDropDown") as FMDropDownList;

				if (lowLimitTxb != null)
				{
					analogInputCollection[collectionIndex].LowLimit = this.ConvertToDouble(lowLimitTxb.Text, "Low Limit");
				}

				if (highLimitTxb != null)
				{
					analogInputCollection[collectionIndex].HighLimit = this.ConvertToDouble(highLimitTxb.Text, "High Limit");
				}

				if (parameterATxb != null)
				{
					analogInputCollection[collectionIndex].ParameterA = parameterATxb.Text;
				}

				if (parameterBTxb != null)
				{
					analogInputCollection[collectionIndex].ParameterB = parameterBTxb.Text;
				}

				if (parameterCTxb != null)
				{
					analogInputCollection[collectionIndex].ParameterC = parameterCTxb.Text;
				}

				if (analogInputDd != null)
				{
					switch (analogInputDd.SelectedIndex)
					{
						case 1:
							analogInputCollection[collectionIndex].AnalogFormula = Formula01;
							break;
						case 2:
							analogInputCollection[collectionIndex].AnalogFormula = Formula02;
							break;
						case 3:
							analogInputCollection[collectionIndex].AnalogFormula = Formula03;
							break;
						default:
							analogInputCollection[collectionIndex].AnalogFormula = string.Empty;
							break;
					}
				}

				collectionIndex++;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		///    The add btn on click event handler.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void AddBtnOnClick(object sender, EventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile != null)
			{
				MobileDeviceProfileAnalogInputCollection analogInputCollection = this.mobileDeviceProfile.AnalogInputCollection;
				var analogInput = new MobileDeviceProfileAnalogInput();

				analogInputCollection.Add(analogInput);
				this.UpdateView();
			}
		}

		/// <summary>
		///    This method handles the delete event from the grid.
		/// </summary>
		/// <param name="source">
		///    The source.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void AnalogInputDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return;
			}

			if (this.mobileDeviceProfile.AnalogInputCollection == null
			    || this.mobileDeviceProfile.AnalogInputCollection.Count <= 0)
			{
				return;
			}

			TableCell analogInputGuidCell = e.Item.Cells[1];//bds

			if (string.IsNullOrEmpty(analogInputGuidCell.Text) == false)
			{
				try
				{
					Guid analogInputGuid = Guid.Parse(analogInputGuidCell.Text);
					MobileDeviceProfileAnalogInput deletedAnalogInputItem =
						this.mobileDeviceProfile.AnalogInputCollection.Find(x => x.MobileDeviceProfileAnalogInputGuid == analogInputGuid);

					if (deletedAnalogInputItem == null)
					{
						string errMsg = "Invalid Analog Input object. Cannot delete.";
						throw new Exception(errMsg);
					}

					this.mobileDeviceProfile.DeletedAnalogInputCollection.Add(deletedAnalogInputItem);
					this.mobileDeviceProfile.AnalogInputCollection.Remove(deletedAnalogInputItem);

					this.UpdateView();
				}
				catch (Exception)
				{
					string errMsg = "Invalid GUID: " + analogInputGuidCell.Text;
					throw new Exception(errMsg);
				}
			}
		}

		/// <summary>
		///    The analog input item data bound.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void AnalogInputItemDataBound(object sender, DataGridItemEventArgs e)
		{
			MobileDeviceProfileAnalogInputCollection analogInputCollection = this.mobileDeviceProfile.AnalogInputCollection;
			int rowIndex = e.Item.ItemIndex;

			var lowLimitTxb = e.Item.FindControl("LowLimitTextBox") as FMTextBox;
			var highLimitTxb = e.Item.FindControl("HighLimitTextBox") as FMTextBox;
			var parameterATxb = e.Item.FindControl("ParameterATextBox") as FMTextBox;
			var parameterBTxb = e.Item.FindControl("ParameterBTextBox") as FMTextBox;
			var parameterCTxb = e.Item.FindControl("ParameterCTextBox") as FMTextBox;
			var analogInputDd = e.Item.FindControl("AnalogFormulaDropDown") as FMDropDownList;
			var deleteLinkBtn = e.Item.FindControl("DeleteLinkButton") as FMDeleteLinkButton;

			if (deleteLinkBtn != null)
			{
				deleteLinkBtn.Enabled = this.HasPermission();
			}

			if (lowLimitTxb != null)
			{
				lowLimitTxb.Text = analogInputCollection[rowIndex].LowLimit.ToString(CultureInfo.InvariantCulture);
				lowLimitTxb.Enabled = this.HasPermission();
			}

			if (highLimitTxb != null)
			{
				highLimitTxb.Text = analogInputCollection[rowIndex].HighLimit.ToString(CultureInfo.InvariantCulture);
				highLimitTxb.Enabled = this.HasPermission();
			}

			if (parameterATxb != null)
			{
				parameterATxb.Text = analogInputCollection[rowIndex].ParameterA;
				parameterATxb.Enabled = this.HasPermission();
			}

			if (parameterBTxb != null)
			{
				parameterBTxb.Text = analogInputCollection[rowIndex].ParameterB;
				parameterBTxb.Enabled = this.HasPermission();
			}

			if (parameterCTxb != null)
			{
				parameterCTxb.Text = analogInputCollection[rowIndex].ParameterC;
				parameterCTxb.Enabled = this.HasPermission();
			}

			if (analogInputDd != null)
			{
				var items = new List<ListItem>();

				// "None selected"
				var txtItem = FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
                                                    x =>
                                                    x.Get(this.Security.SiteGuid, "None") 
                                                );

				var item = new ListItem { Text = txtItem, Value = "0" };
				items.Add(item);

				// "X = Y * A + B"
				item = new ListItem { Text = Formula01, Value = "1" };
				items.Add(item);

				// "X = (Y / A) * (B - C)"
				item = new ListItem { Text = Formula02, Value = "3" };
				items.Add(item);

				// "X = (Y - A) / (B - C)"
				item = new ListItem { Text = Formula03, Value = "4" };
				items.Add(item);

				analogInputDd.DataSource = items;
				analogInputDd.DataTextField = "Text";
				analogInputDd.DataValueField = "Value";
				analogInputDd.DataBind();

				string formula = analogInputCollection[rowIndex].AnalogFormula;
				analogInputDd.SelectedIndex = 0;

				if (string.IsNullOrEmpty(formula) == false)
				{
					if (Formula01.Equals(formula))
					{
						analogInputDd.SelectedIndex = 1;
					}
					else if (Formula02.Equals(formula))
					{
						analogInputDd.SelectedIndex = 2;
					}
					else if (Formula03.Equals(formula))
					{
						analogInputDd.SelectedIndex = 3;
					}
				}

				analogInputDd.Enabled = this.HasPermission();
			}
		}

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///    This method will handle the page load event for the analog input page.
		/// </summary>
		/// <param name="sender">
		///    The sender.
		/// </param>
		/// <param name="e">
		///    The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			if (this.Page.IsPostBack == false)
			{
				this.UpdateView();
			}

			this.DisableButtons();
		}

		/// <summary>
		///    The convert to double.
		/// </summary>
		/// <param name="inStr">
		///    The in str.
		/// </param>
		/// <param name="fieldName">
		///    The field Name.
		/// </param>
		/// <returns>
		///    The System.Double.
		/// </returns>
		private double ConvertToDouble(string inStr, string fieldName)
		{
			double outValue = 0.0;

			if (string.IsNullOrEmpty(inStr))
			{
				return outValue;
			}

			try
			{
				outValue = Convert.ToDouble(inStr);
			}
			catch (Exception)
			{
				string errMsg = "Field must be a numeric value.";

				if (string.IsNullOrEmpty(fieldName) == false)
				{
					errMsg = fieldName + " field must be a numeric value.";
				}

				throw new Exception(errMsg);
			}

			return outValue;
		}

		/// <summary>
		///    This method will disable buttons based on the security rights.
		/// </summary>
		private void DisableButtons()
		{
			this.AddBtnBottom.Enabled = this.HasPermission();
			this.AddBtnTop.Enabled = this.HasPermission();
		}

		/// <summary>
		///    This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		///    entity has not been assigned down.
		/// </summary>
		/// <returns>
		///    The System.Boolean.
		/// </returns>
		private bool HasPermission()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				return false;
			}

			if (this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES))
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES)
			       && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.AnalogInputDataGrid.ItemDataBound +=
				new System.Web.UI.WebControls.DataGridItemEventHandler(this.AnalogInputItemDataBound);
			this.AnalogInputDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.AnalogInputDataGridDeleteCommand);
		}

		/// <summary>
		///    This method will load the profile generate page with the data from the database.
		/// </summary>
		private void UpdateView()
		{
			this.mobileDeviceProfile =
				this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if (this.mobileDeviceProfile == null)
			{
				this.AnalogInputDataGrid.DataSource = new DataView();
				this.AnalogInputDataGrid.DataBind();
				return;
			}

			MobileDeviceProfileAnalogInputCollection analogInputCollection = this.mobileDeviceProfile.AnalogInputCollection;

			if ((analogInputCollection != null) && (analogInputCollection.Count > 0))
			{
				var gridTable = new DataTable("AnalogInputs");

				var column = new DataColumn("InputNumber", Type.GetType("System.String"));
				gridTable.Columns.Add(column);

				column = new DataColumn("MobileDeviceAnalogInputGuid", Type.GetType("System.String"));
				gridTable.Columns.Add(column);

				int inputCount = 1;

				foreach (MobileDeviceProfileAnalogInput analogInput in analogInputCollection)
				{
					DataRow row = gridTable.NewRow();

					row["InputNumber"] = this.GetInputNumber(this.Security.SiteGuid, "Input") + " "
					                     + inputCount.ToString(CultureInfo.InvariantCulture);
					row["MobileDeviceAnalogInputGuid"] = analogInput.MobileDeviceProfileAnalogInputGuid.ToString();

					gridTable.Rows.Add(row);
					inputCount++;
				}

				this.AnalogInputDataGrid.DataSource = new DataView(gridTable);
				this.AnalogInputDataGrid.DataBind();
			}
			else
			{
				this.AnalogInputDataGrid.DataSource = new DataView();
				this.AnalogInputDataGrid.DataBind();
			}
		}

		private string GetInputNumber(Guid guid, string p)
		{
			return FMChannelHelper.MakeCall<IDataDictionariesClass, string>(
																	 x =>
																	 x.Get(this.Security.SiteGuid, "Input") 
																);
		}

		#endregion
	}
}