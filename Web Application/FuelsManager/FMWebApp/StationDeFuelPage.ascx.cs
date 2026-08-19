// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationDeFuelPage.ascx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the StationDeFuelPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	/// <summary>
	///	Summary description for StationDeFuelPage.
	/// </summary>
	public partial class StationDeFuelPage : FMUserControlBase
	{
		#region Properties

		private string javascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Arm Permissives and No Additive Permissives Button values according to Data Dictionary
					var LoadArmPermissivesButton=document.getElementById('LoadArmPermissivesButton');
					if(LoadArmPermissivesButton != null)
						LoadArmPermissivesButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Load Arm Permissives") + @"';
					var NoAdditivePermissivesButton=document.getElementById('NoAdditivePermissivesButton');
					if(NoAdditivePermissivesButton != null)
						NoAdditivePermissivesButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("No Additive Permissives") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators

		public void UpdateData()
		{
			var Station = (StationClass)this.Session["Station"];

			Station.SwingArmPosition = this.SwingArmPositionDropDownList.SelectedValue;
			Station.VaporRecovery = false; // VaporRecoveryCheckBox.Checked;
			Station.InhibitLoadingByLoadID = false; // InhibitLoadingByLoadIDCheckBox.Checked;
			Station.SynchronizeReferenceDensity = this.SynchronizeReferenceDensityCheckBox.Checked;
			Station.SetDefaultPresetToZero = true; // SetPreloadToZeroCheckBox.Checked;
			Station.InhibitSettingRecipeNames = true; // InhibitSettingRecipeNamesCheckBox.Checked;
			Station.MeterRecircCardNumber = this.MeterRecircCardNumber.Text;
			Station.OffLoadByOffLoadID = this.OffLoafbyOffLoadingID.Checked;
			Station.UseManualMeterData = this.UseManualMeterData.Checked;
			Station.PromptForBOLNumber = this.PromptForBOLNumber.Checked;
				Station.PromptForTemperature	= this.PromptForTemperature.Checked;
				Station.PromptForGravity		= this.PromptForGravity.Checked;

			if (this.BOLPrinterDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.BOLPrinter = "{None}";
			}
			else
			{
				Station.BOLPrinter = this.BOLPrinterDropDownList.SelectedItem.Text;
			}

			Station.ReceiptByVolumeTransactionAliasGuid = Guid.Parse(this.ReceiptTransactionDropDownList.SelectedValue);
			if (this.ReceiptTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.ReceiptByWeightTransactionAliasID = "{None}";
			}
			else
			{
				Station.ReceiptByWeightTransactionAliasID = this.ReceiptTransactionDropDownList.SelectedItem.Text;
			}

			Station.RecircTransactionAliasGuid = Guid.Parse(this.RecircTransactionDropDownList.SelectedValue);
			if (this.RecircTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.RecircTransactionAliasID = "{None}";
			}
			else
			{
				Station.RecircTransactionAliasID = this.RecircTransactionDropDownList.SelectedItem.Text;
			}

			try
			{
				if ((this.NumberOfCopiesTextBox.Text == null) || (this.NumberOfCopiesTextBox.Text.Length <= 0))
				{
					this.NumberOfCopiesTextBox.Text = StationClass.MinNumberOfCopies.ToString();
				}

				int copies = Convert.ToInt32(this.NumberOfCopiesTextBox.Text);
				if ((copies < StationClass.MinNumberOfCopies) || (copies > StationClass.MaxNumberOfCopies))
				{
					throw new Exception(
						"Copies range must be between " + StationClass.MinNumberOfCopies + "and " + StationClass.MaxNumberOfCopies);
				}
				else
				{
					Station.NumberOfCopies = copies;
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Copies range"))
				{
					throw new Exception(ex.Message);
				}
				else
				{
					throw new Exception("Value must be numeric");
				}
			}
		}

		#endregion

		#region Methods

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var Station = (StationClass)this.Session["Station"];

				if (Station.Type != STATION_TYPE.OFF_LOADING)
				{
					return;
				}

				// manual data is only available for the DET
				this.PromptForBOLNumber.Enabled = true;
				this.UseManualMeterData.Enabled = false;
				if (Station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET)
				{
					this.SwingArmPositionDropDownList.Enabled = false;
					this.UseManualMeterData.Enabled = true;
				}
				else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010)
				{
					this.MeterRecircCardNumber.Enabled = true;
					this.RecircTransactionDropDownList.Enabled = true;
					this.PromptForBOLNumber.Enabled = false;
					this.PromptForBOLNumber.Checked = false;
					this.UseManualMeterData.Checked = false;
				}
				else if (Station.InterfaceType == STATION_INTERFACE_TYPE.MICROLOAD_NET)
				{
					this.SwingArmPositionDropDownList.Enabled = false;
					this.MeterRecircCardNumber.Enabled = false;
					this.RecircTransactionDropDownList.Enabled = false;
					this.UseManualMeterData.Checked = false;
				}
				else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
				{
					this.MeterRecircCardNumber.Enabled = false;
					this.RecircTransactionDropDownList.Enabled = false;
					this.PromptForBOLNumber.Enabled = false;
					this.PromptForBOLNumber.Checked = false;
					this.UseManualMeterData.Checked = false;
					this.OffLoafbyOffLoadingID.Checked = true;
					this.OffLoafbyOffLoadingID.Enabled = false;
					this.SynchronizeReferenceDensityCheckBox.Checked = false;
					this.SynchronizeReferenceDensityCheckBox.Enabled = false;
				}
				else
				{
					this.MeterRecircCardNumber.Enabled = false;
					this.RecircTransactionDropDownList.Enabled = false;
					this.UseManualMeterData.Checked = false;
				}

				if (this.Page.IsPostBack == false)
				{
					// SwingArmPositionDropDownList
					var Item = new ListItem("A", "A");
					this.SwingArmPositionDropDownList.Items.Add(Item);

					if (Station.SwingArmPosition == "A")
					{
						this.SwingArmPositionDropDownList.SelectedIndex = 0;
					}

					Item = new ListItem("B", "B");
					this.SwingArmPositionDropDownList.Items.Add(Item);

					if (Station.SwingArmPosition != "A")
					{
						this.SwingArmPositionDropDownList.SelectedIndex = 1;
					}

					// Swing Arm Position cannot be changed after Load Arms are added
					if (Station.LoadArmCollection.Count != 0)
					{
						this.SwingArmPositionDropDownList.Enabled = false;
					}

					this.SynchronizeReferenceDensityCheckBox.Checked = Station.SynchronizeReferenceDensity;
					this.OffLoafbyOffLoadingID.Checked = Station.OffLoadByOffLoadID;

					// BOLPrinterDropDownList
					var NewPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
					this.BOLPrinterDropDownList.Items.Add(NewPrinterItem);
					try
					{
						string[] InstalledPrinters = ReportServicePrintService.EnumeratePrinters("BOL");
						int Index = 1;

						foreach (string Printer in InstalledPrinters)
						{
							NewPrinterItem = new ListItem(Printer, Index.ToString());
							foreach (ListItem ExistingPrinterItem in this.BOLPrinterDropDownList.Items)
							{
								if (ExistingPrinterItem.Text.CompareTo(NewPrinterItem.Text) > 0)
								{
									int Insert = this.BOLPrinterDropDownList.Items.IndexOf(ExistingPrinterItem);
									this.BOLPrinterDropDownList.Items.Insert(Insert, NewPrinterItem);

									if (Station.BOLPrinter == NewPrinterItem.Text)
									{
										this.BOLPrinterDropDownList.SelectedIndex = Insert;
									}

									NewPrinterItem = null;
									break;
								}
							}

							if (NewPrinterItem != null)
							{
								this.BOLPrinterDropDownList.Items.Add(NewPrinterItem);
								if (Station.BOLPrinter == NewPrinterItem.Text)
								{
									this.BOLPrinterDropDownList.SelectedIndex = this.BOLPrinterDropDownList.Items.Count - 1;
								}
							}

							Index++;
						}
					}
					catch (SocketException socketExcept)
					{
						if (socketExcept.ErrorCode != 10061)
						{
							throw socketExcept;
						}

						if (Station.BOLPrinter != "{None}" && Station.BOLPrinter.Length > 0)
						{
							var PrinterItemFromDB = new ListItem(Station.BOLPrinter, "1");
							this.BOLPrinterDropDownList.Items.Add(PrinterItemFromDB);
							this.BOLPrinterDropDownList.SelectedIndex = this.BOLPrinterDropDownList.Items.Count - 1;
						}
					}

					// Set the number of copies to be printed
					this.NumberOfCopiesTextBox.Text = Station.NumberOfCopies.ToString();

					// Set the meter recirc card number
					this.MeterRecircCardNumber.Text = Station.MeterRecircCardNumber;

					// Populate ReceiptTransactionDropDownList
					Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.ReceiptTransactionDropDownList.Items.Add(Item);
					TransactionAliasCollectionClass TransactionAliasCollection;
					TransactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	x =>
																	x.EnumerateByTransTypeID(this.Security, TransactionTypes.T8_Receipt)
																);

					foreach (TransactionAliasClass TransactionAlias in TransactionAliasCollection)
					{
						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.ReceiptTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.ReceiptByVolumeTransactionAliasGuid)
						{
							this.ReceiptTransactionDropDownList.SelectedIndex = this.ReceiptTransactionDropDownList.Items.Count - 1;
						}
					}

					// Populate RecircTransactionDropDownList
					Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.RecircTransactionDropDownList.Items.Add(Item);
					TransactionAliasCollectionClass RecircTransactionAliasCollection;

					RecircTransactionAliasCollection = 
						FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
								x =>
								x.EnumerateByTransTypeID(this.Security, TransactionTypes.T23_StorageTransfer)
						);

					foreach (TransactionAliasClass RecircTransactionAlias in RecircTransactionAliasCollection)
					{
						Item = new ListItem(RecircTransactionAlias.ID, RecircTransactionAlias.MasterRecordGuid.ToString());
						this.RecircTransactionDropDownList.Items.Add(Item);

						if (RecircTransactionAlias.MasterRecordGuid == Station.RecircTransactionAliasGuid)
						{
							this.RecircTransactionDropDownList.SelectedIndex = this.RecircTransactionDropDownList.Items.Count - 1;
						}
					}

					this.PromptForBOLNumber.Enabled = true;
					this.PromptForBOLNumber.Checked = Station.PromptForBOLNumber;
					this.PromptForTemperature.Checked = Station.PromptForTemperature;
					this.PromptForGravity.Checked = Station.PromptForGravity;
					if (Station.InterfaceType == STATION_INTERFACE_TYPE.VAREC_DET)
					{
						this.SwingArmPositionDropDownList.Enabled = false;
						this.UseManualMeterData.Enabled = true;
						this.UseManualMeterData.Checked = Station.UseManualMeterData;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010)
					{
						this.PromptForBOLNumber.Enabled = false;
						this.PromptForBOLNumber.Checked = false;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
					{
						this.PromptForBOLNumber.Enabled = false;
						this.PromptForBOLNumber.Checked = false;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.MICROLOAD_NET)
					{
						this.SwingArmPositionDropDownList.Enabled = false;
					}
				}
				this.Page.ClientScript.RegisterStartupScript(
					this.GetType(), "StationLoadArmPageScriptBlock", this.javascriptStartup);
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///	Required method for Designer support - do not modify
		///	the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		#endregion
	}
}