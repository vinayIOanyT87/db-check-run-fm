// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProfilePrinterSettingPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the ProfilePrinterSettings type.
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
	/// This class handles the functionality for the Profile Printer Configuration tab page.
	/// </summary>
	public partial class ProfilePrinterSettings : FMUserControlBase
	{
		#region Private data members
		/// <summary>
		/// A constant string that contains all the Printer COM Port values that will be in the dropdown.
		/// </summary>
		private const string ComListStr = "None,COM1,COM2,COM3,COM4,COM5,COM6,COM7,COM8,COM9";

		/// <summary>
		/// A constant string that contains all the Printer Parity values that will be in the dropdown.
		/// </summary>
		private const string ParityListStr = "None,Even,Odd";

		/// <summary>
		/// Printer COM Port values string array.
		/// </summary>
		private string[] comList;

		/// <summary>
		/// Printer Parity values string array.
		/// </summary>
		private string[] parityList;

		/// <summary>
		/// The mobile device profile.
		/// </summary>
		private MobileDeviceProfile mobileDeviceProfile;
		#endregion

		/// <summary>
		/// This method handles the page load event.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void Page_Load(object sender, EventArgs e)
		{
			this.comList	= ComListStr.Split(',');
			this.parityList = ParityListStr.Split(',');

			if ( this.Page.IsPostBack == false )
			{
				this.UpdateView( );
			}

			this.DisableFields();
		}

		#region Public methods
		/// <summary>
		/// This method will reset all the fields when the new button is
		/// selected.
		/// </summary>
		public void ResetFieldsForNewEvent( )
		{
			this.UpdateView( );
		}

		/// <summary>
		/// This method will update the profile configuration table from the printer configuration page.
		/// </summary>
		public void UpdateChanges( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( (this.mobileDeviceProfile == null) || (this.mobileDeviceProfile.PrinterCollection.Count == 0) )
			{
				return;
			}

			MobileDeviceProfilePrinterCollection printerCollection = this.mobileDeviceProfile.PrinterCollection;
			int collectionIndex = 0;

			foreach ( DataGridItem gridItem in this.PrinterDataGrid.Items )
			{
				var printerIdTxb			= gridItem.FindControl("PrinterIdTB") as FMTextBox;
				var printerBaudRateTxb		= gridItem.FindControl("BaudRateTB") as FMTextBox;
				var printerComPortDd		= gridItem.FindControl("ComPortDD") as FMDropDownList;
				var printerDataBitsTxb		= gridItem.FindControl("DataBitsTB") as FMTextBox;
				var printerStopBitsTxb		= gridItem.FindControl("StopBitsTB") as FMTextBox;
				var printerUseXonXoffTxb	= gridItem.FindControl("UseXonXoffTB") as FMTextBox;
				var printerXonCharTxb		= gridItem.FindControl("XonCharTB") as FMTextBox;
				var printerXoffCharTxb		= gridItem.FindControl("XoffCharTB") as FMTextBox;
				var printerBufferSizeTxb	= gridItem.FindControl("BufferSizeTB") as FMTextBox;
				var printerParityDd			= gridItem.FindControl("ParityDD") as FMDropDownList;

				if ( printerIdTxb != null )
				{
					string originalValue = printerCollection[collectionIndex].PrinterId;
					printerCollection[collectionIndex].PrinterId = printerIdTxb.Text;

					if ( string.IsNullOrEmpty(printerIdTxb.Text) == false )
					{
						List<MobileDeviceProfilePrinter> compareCountList = printerCollection.FindAll(x => x.PrinterId == printerIdTxb.Text);

						// Cannot have a duplicate Printer ID for the a given profile.
						if (compareCountList.Count > 1)
						{
							printerCollection[collectionIndex].PrinterId = originalValue;
							string errMsg = "Printer ID '" + printerIdTxb.Text + "' already exists for this profile.";
							throw new Exception(errMsg);
						}
					}
				}

				if ( printerBaudRateTxb != null )
				{
					printerCollection[collectionIndex].PrinterBaudRate = this.CheckForNumericAndLength(printerBaudRateTxb.Text, "Baud Rate");
				}

				if ( printerComPortDd != null )
				{
					if ( printerComPortDd.SelectedIndex == 0 )
					{
						printerCollection[collectionIndex].PrinterComPort = string.Empty;
					}
					else
					{
						printerCollection[collectionIndex].PrinterComPort = this.comList[printerComPortDd.SelectedIndex];					
					}
				}

				if ( printerDataBitsTxb != null )
				{
					printerCollection[collectionIndex].PrinterDataBits = this.CheckForNumericAndLength(printerDataBitsTxb.Text, "Data Bits");
				}

				if ( printerStopBitsTxb != null )
				{
					printerCollection[collectionIndex].PrinterStopBits = this.CheckForNumericAndLength(printerStopBitsTxb.Text, "Stop Bits");
				}

				if ( printerUseXonXoffTxb != null )
				{
					printerCollection[collectionIndex].PrinterUseXonXoff = this.CheckForNumericAndLength(printerUseXonXoffTxb.Text, "Use Xon Xoff");
				}

				if ( printerXonCharTxb != null )
				{
					printerCollection[collectionIndex].PrinterXonChar = this.CheckForNumericAndLength(printerXonCharTxb.Text, "Xon Char");
				}

				if ( printerXoffCharTxb != null )
				{
					printerCollection[collectionIndex].PrinterXoffChar = this.CheckForNumericAndLength(printerXoffCharTxb.Text, "Xoff Char");
				}

				if ( printerBufferSizeTxb != null )
				{
					printerCollection[collectionIndex].PrinterBufferSize = this.CheckForNumericAndLength(printerBufferSizeTxb.Text, "Buffer Size");
				}

				if ( printerParityDd != null )
				{
					if ( printerParityDd.SelectedIndex == 0 )
					{
						printerCollection[collectionIndex].PrinterParity = string.Empty;
					}
					else
					{
						printerCollection[collectionIndex].PrinterParity = this.parityList[printerParityDd.SelectedIndex];
					}
				}

				collectionIndex++;
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method returns true if the user has the MODIFY_MOBILE_DEVICE_PROFILES right and the
		/// entity has not been assigned down.
		/// </summary>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		private bool HasPermission( )
		{
			this.mobileDeviceProfile = Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return false;
			}

			if ( this.mobileDeviceProfile.SiteGuid == Guid.Empty && this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) )
			{
				return true;
			}

			return this.Security.HasRight(RIGHT.MODIFY_MOBILE_DEVICE_PROFILES) && (this.Security.SiteGuid == this.mobileDeviceProfile.SiteGuid);
		}

		/// <summary>
		/// This method will disable all fields if the user does not have the
		/// "modify mobile device profile" right.
		/// </summary>
		private void DisableFields( )
		{
			this.AddBtnBottom.Enabled	= this.HasPermission();
			this.AddBtnTop.Enabled		= this.HasPermission();
		}

		/// <summary>
		/// This method will check for the value being numeric and the length being
		/// 8 digits.
		/// </summary>
		/// <param name="strValue">
		/// The str value.
		/// </param>
		/// <param name="fieldName">
		/// The field name.
		/// </param>
		/// <returns>
		/// The System.String.
		/// </returns>
		/// <exception cref="Exception">Must be numeric and 8 digits.
		/// </exception>
		private string CheckForNumericAndLength(string strValue, string fieldName)
		{
			if ( string.IsNullOrEmpty(strValue) == false )
			{
				string errMsg;

				try
				{
					Convert.ToInt32(strValue);
				}
				catch (Exception)
				{
					errMsg = "Field (" + fieldName + ") is not numeric.";
					throw new Exception(errMsg);
				}

				if ( strValue.Length < 8 )
				{
					errMsg = "Field (" + fieldName + ") must be eight digits in length.";
					throw new Exception(errMsg);
				}
			}

			return strValue;
		}

		/// <summary>
		/// This method will load the profile printer page with the data from the database.
		/// </summary>
		private void UpdateView( )
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				this.PrinterDataGrid.DataSource = new DataView();
				this.PrinterDataGrid.DataBind();
				return;
			}

			MobileDeviceProfilePrinterCollection printerCollection = this.mobileDeviceProfile.PrinterCollection;

			if ( (printerCollection != null) && (printerCollection.Count > 0) )
			{
				DataTable gridTable = new DataTable("Printers");

				DataColumn column = new DataColumn("MobileDevicePrinterGuid", Type.GetType("System.String"));
				gridTable.Columns.Add(column);

				foreach ( MobileDeviceProfilePrinter printer in printerCollection )
				{
					DataRow row = gridTable.NewRow( );

					row["MobileDevicePrinterGuid"] = printer.MobileDeviceProfilePrinterGuid.ToString( );

					gridTable.Rows.Add(row);
				}

				this.PrinterDataGrid.DataSource = new DataView(gridTable);
				this.PrinterDataGrid.DataBind( );
			}
			else
			{
				this.PrinterDataGrid.DataSource = new DataView();
				this.PrinterDataGrid.DataBind();
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// The printer item data bound.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void PrinterItemDataBound(object sender, DataGridItemEventArgs e)
		{
			MobileDeviceProfilePrinterCollection printerCollection = this.mobileDeviceProfile.PrinterCollection;
			int rowIndex = e.Item.ItemIndex;

			var printerIdTxb			= e.Item.FindControl("PrinterIdTB") as FMTextBox;
			var printerBaudRateTxb		= e.Item.FindControl("BaudRateTB") as FMTextBox;
			var printerComPortDd		= e.Item.FindControl("ComPortDD") as FMDropDownList;
			var printerDataBitsTxb		= e.Item.FindControl("DataBitsTB") as FMTextBox;
			var printerStopBitsTxb		= e.Item.FindControl("StopBitsTB") as FMTextBox;
			var printerUseXonXoffTxb	= e.Item.FindControl("UseXonXoffTB") as FMTextBox;
			var printerXonCharTxb		= e.Item.FindControl("XonCharTB") as FMTextBox;
			var printerXoffCharTxb		= e.Item.FindControl("XoffCharTB") as FMTextBox;
			var printerBufferSizeTxb	= e.Item.FindControl("BufferSizeTB") as FMTextBox;
			var printerParityDd			= e.Item.FindControl("ParityDD") as FMDropDownList;
			var deleteLinkBtn			= e.Item.FindControl("DeleteLinkButton") as FMDeleteLinkButton;

			if ( deleteLinkBtn != null )
			{
				deleteLinkBtn.Enabled = this.HasPermission();
			}

			if ( printerIdTxb != null )
			{
				printerIdTxb.Text = printerCollection[rowIndex].PrinterId;
				printerIdTxb.Enabled = this.HasPermission();
			}

			if ( printerBaudRateTxb != null )
			{
				printerBaudRateTxb.Text = printerCollection[rowIndex].PrinterBaudRate;
				printerBaudRateTxb.Enabled = this.HasPermission();
			}

			if ( printerComPortDd != null )
			{
				var compareList = new Dictionary<string, int>();
				var items = new List<ListItem>( );
				int itemValue = 0;

				foreach ( string comItem in this.comList )
				{
					var item = new ListItem { Text = comItem, Value = itemValue.ToString(CultureInfo.InvariantCulture) };
					items.Add(item);

					compareList.Add(comItem, itemValue);
					itemValue++;
				}

				printerComPortDd.DataSource		= items;
				printerComPortDd.DataTextField	= "Text";
				printerComPortDd.DataValueField = "Value";
				printerComPortDd.Sort			= false;
				printerComPortDd.DataBind( );

				string comStr = printerCollection[rowIndex].PrinterComPort;
				printerComPortDd.SelectedIndex = 0;

				if ( string.IsNullOrEmpty(comStr) == false )
				{
					int itemIndex;

					if ( compareList.TryGetValue(comStr, out itemIndex) )
					{
						printerComPortDd.SelectedIndex = itemIndex;
					}
				}

				printerComPortDd.Enabled = this.HasPermission();
			}

			if ( printerDataBitsTxb != null )
			{
				printerDataBitsTxb.Text = printerCollection[rowIndex].PrinterDataBits;
				printerDataBitsTxb.Enabled = this.HasPermission();
			}

			if ( printerStopBitsTxb != null )
			{
				printerStopBitsTxb.Text = printerCollection[rowIndex].PrinterStopBits;
				printerStopBitsTxb.Enabled = this.HasPermission();
			}

			if ( printerUseXonXoffTxb != null )
			{
				printerUseXonXoffTxb.Text = printerCollection[rowIndex].PrinterUseXonXoff;
				printerUseXonXoffTxb.Enabled = this.HasPermission();
			}

			if ( printerXonCharTxb != null )
			{
				printerXonCharTxb.Text = printerCollection[rowIndex].PrinterXonChar;
				printerXonCharTxb.Enabled = this.HasPermission();
			}

			if ( printerXoffCharTxb != null )
			{
				printerXoffCharTxb.Text = printerCollection[rowIndex].PrinterXoffChar;
				printerXoffCharTxb.Enabled = this.HasPermission();
			}

			if ( printerBufferSizeTxb != null )
			{
				printerBufferSizeTxb.Text = printerCollection[rowIndex].PrinterBufferSize;
				printerBufferSizeTxb.Enabled = this.HasPermission();
			}

			if ( printerParityDd != null )
			{
				var compareList = new Dictionary<string, int>( );
				var items = new List<ListItem>( );
				int itemValue = 0;

				foreach ( string parityItem in this.parityList )
				{
					var item = new ListItem { Text = parityItem, Value = itemValue.ToString(CultureInfo.InvariantCulture) };
					items.Add(item);

					compareList.Add(parityItem, itemValue);
					itemValue++;
				}

				printerParityDd.DataSource     = items;
				printerParityDd.DataTextField  = "Text";
				printerParityDd.DataValueField = "Value";
				printerParityDd.Sort           = false;
				printerParityDd.DataBind( );

				string parityStr = printerCollection[rowIndex].PrinterParity;
				printerParityDd.SelectedIndex = 0;

				if ( string.IsNullOrEmpty(parityStr) == false )
				{
					int itemIndex;

					if ( compareList.TryGetValue(parityStr, out itemIndex) )
					{
						printerParityDd.SelectedIndex = itemIndex;
					}
				}

				printerParityDd.Enabled = this.HasPermission();
			}
		}

		/// <summary>
		/// The add btn on click event handler.
		/// </summary>
		/// <param name="sender">
		/// The sender.
		/// </param>
		/// <param name="e">
		/// The e.
		/// </param>
		protected void AddBtnOnClick(object sender, EventArgs e)
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile != null )
			{
				MobileDeviceProfilePrinterCollection printerCollection = this.mobileDeviceProfile.PrinterCollection;
				var printer = new MobileDeviceProfilePrinter( );

				printerCollection.Add(printer);
				this.UpdateView( );
			}
		}

		/// <summary>
		/// This method will handle the delete event.  It will remove only one mobile device profile
		/// printer entry from the collection list.
		/// </summary>
		/// <param name="source">Object source of the event.</param>
		/// <param name="e">Event arguments object.</param>
		protected void PrinterDataGridDeleteCommand(object source, DataGridCommandEventArgs e)
		{
			this.mobileDeviceProfile = this.Session[PageSessionKeyConstants.ProfileConfigurationProfileObject] as MobileDeviceProfile;

			if ( this.mobileDeviceProfile == null )
			{
				return;
			}

			if ( this.mobileDeviceProfile.PrinterCollection == null || this.mobileDeviceProfile.PrinterCollection.Count <= 0 )
			{
				return;
			}

			TableCell printerGuidCell = e.Item.Cells[0];//bds

			if ( string.IsNullOrEmpty(printerGuidCell.Text) == false )
			{
				try
				{
					Guid printerGuid = Guid.Parse(printerGuidCell.Text);
					MobileDeviceProfilePrinter deletedPrinterItem = this.mobileDeviceProfile.PrinterCollection.Find(x => x.MobileDeviceProfilePrinterGuid == printerGuid);

					if ( deletedPrinterItem == null )
					{
						string errMsg = "Invalid Printer object. Cannot delete.";
						throw new Exception(errMsg);
					}

					this.mobileDeviceProfile.DeletedPrinterCollection.Add(deletedPrinterItem);
					this.mobileDeviceProfile.PrinterCollection.Remove(deletedPrinterItem);

					this.UpdateView( );
				}
				catch (Exception)
				{
					string errMsg = "Invalid GUID: " + printerGuidCell.Text;
					throw new Exception(errMsg);
				}
			}
		}
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent( );
			base.OnInit(e);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent( )
		{
			this.PrinterDataGrid.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.PrinterItemDataBound);
			this.PrinterDataGrid.DeleteCommand += new DataGridCommandEventHandler(this.PrinterDataGridDeleteCommand);
		}
		#endregion
	}
}