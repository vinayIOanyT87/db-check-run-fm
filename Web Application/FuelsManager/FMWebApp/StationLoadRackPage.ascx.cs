// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationLoadRackPage.ascx.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the StationLoadRackPage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using OpcCom;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;
	using Server = Opc.Server;

	public partial class StationLoadRackPage : FMUserControlBase
	{
		#region Properties

		private string javascriptStartup
		{
			get
			{
				string script = @"
				<script type='text/javascript'>
				<!--
					// Set Station Permissives according to Data Dictionary
					var StationPermissivesButton=document.getElementById('StationPermissivesButton');
					if(StationPermissivesButton != null)
						StationPermissivesButton.value='" + ((FMFormBase)this.Page).GetTranslatedText("Station Permissives") + @"';

					// Set EnableDynamicRecipesCheckBox according to Data Dictionary
					var EnableDynamicRecipesCheckBox=document.getElementById('EnableDynamicRecipesCheckBox');
					if(EnableDynamicRecipesCheckBox != null)
						EnableDynamicRecipesCheckBox.Text='" + ((FMFormBase)this.Page).GetTranslatedText("Enable Dynamic Recipes") + @"';

					// Set EthanolExcessCheckBox according to Data Dictionary
					var EthanolExcessCheckBox=document.getElementById('EthanolExcessCheckBox');
					if(EthanolExcessCheckBox != null)
						EthanolExcessCheckBox.Text='" + ((FMFormBase)this.Page).GetTranslatedText("Ethanol Excess") + @"';
				//-->
				</script>
				";
				return script;
			}
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		///	This method will update the Station object with the information on the form.
		///	This method will be called from the StationForm since the OK and New buttons
		///	are on that form.
		/// </summary>
		public void UpdateData()
		{
			var Station = (StationClass)this.Session["Station"];

			Station.SwingArmPosition = this.SwingArmPositionDropDownList.SelectedValue;
			Station.VaporRecovery = this.VaporRecoveryCheckBox.Checked;
			Station.InhibitLoadingByLoadID = this.InhibitLoadingByLoadIDCheckBox.Checked;
			Station.SynchronizeReferenceDensity = this.SynchronizeReferenceDensityCheckBox.Checked;
			Station.SetDefaultPresetToZero = this.SetPreloadToZeroCheckBox.Checked;
			Station.InhibitSettingRecipeNames = this.InhibitSettingRecipeNamesCheckBox.Checked;
			Station.MeterRecircCardNumber = this.MeterRecircCardNumber.Text;
			Station.LastTransactionNumber = Convert.ToInt32(this.NumberOfLastTransaction.Text);
			Station.EnableDynamicRecipes = this.EnableDynamicRecipesCheckBox.Checked;
         Station.EthanolExcess = this.EthanolExcessCheckBox.Checked;

         if (this.BOLPrinterDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.BOLPrinter = "{None}";
			}
			else
			{
				Station.BOLPrinter = this.BOLPrinterDropDownList.SelectedItem.Text;
			}

			Station.IssueByVolumeTransactionAliasGuid = new Guid(this.IssueTransactionDropDownList.SelectedValue);
			if (this.IssueTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.IssueByVolumeTransactionAliasID = "{None}";
			}
			else
			{
				Station.IssueByVolumeTransactionAliasID = this.IssueTransactionDropDownList.SelectedItem.Text;
			}

			Station.RecircTransactionAliasGuid = new Guid(this.RecircTransactionDropDownList.SelectedValue);
			if (this.RecircTransactionDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.RecircTransactionAliasID = "{None}";
			}
			else
			{
				Station.RecircTransactionAliasID = this.RecircTransactionDropDownList.SelectedItem.Text;
			}

			if (Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q || Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA)
			{
				Station.EnableEquipmentValidate = this.EnableEquipmentValidateCheckBox.Checked;
				Station.EnableScully = this.EnableScullyCheckBox.Checked;
			}
			else
			{
				Station.EnableEquipmentValidate = false;
				Station.EnableScully = false;
			}
			// Only ACCULOADIII_SA AND ACCULOADIII_Q should have a SCULLY_PV
			if (Station.EnableScully)
			{
				UpdateProcessVariable(PROCESS_VARIABLE_TYPE.SCULLY_PV);
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
			InitializeComponent();
			base.OnInit(e);
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				var Station = (StationClass)this.Session["Station"];

				if (Station.Type != STATION_TYPE.LOAD_RACK)
				{
					return;
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

					this.VaporRecoveryCheckBox.Checked = Station.VaporRecovery;
					this.InhibitLoadingByLoadIDCheckBox.Checked = Station.InhibitLoadingByLoadID;
					this.SynchronizeReferenceDensityCheckBox.Checked = Station.SynchronizeReferenceDensity;
					this.SetPreloadToZeroCheckBox.Checked = Station.SetDefaultPresetToZero;
					this.InhibitSettingRecipeNamesCheckBox.Checked = Station.InhibitSettingRecipeNames;
					this.NumberOfLastTransaction.Text = Station.LastTransactionNumber.ToString();
					this.EnableDynamicRecipesCheckBox.Checked = Station.EnableDynamicRecipes;
               this.EthanolExcessCheckBox.Checked = Station.EthanolExcess;

               // disable the recirc since this currently applies to the contrec
               this.MeterRecircCardNumber.Enabled = false;
					this.RecircTransactionDropDownList.Enabled = false;
					this.StationPermissivesButton.Visible = true;
					this.NumberOfLastTransaction.Enabled = false;

					if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010)
					{
						this.MeterRecircCardNumber.Enabled = true;
						this.RecircTransactionDropDownList.Enabled = true;
						this.InhibitSettingRecipeNamesCheckBox.Checked = false;
						this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						this.SynchronizeReferenceDensityCheckBox.Checked = false;
						this.SynchronizeReferenceDensityCheckBox.Enabled = false;
						this.StationPermissivesButton.Visible = false;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II)
					{
						this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						this.InhibitSettingRecipeNamesCheckBox.Checked = false;
						this.SynchronizeReferenceDensityCheckBox.Enabled = true;
						this.SetPreloadToZeroCheckBox.Checked = false;
						this.SetPreloadToZeroCheckBox.Enabled = false;
						this.StationPermissivesButton.Visible = false;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
					{
						this.InhibitSettingRecipeNamesCheckBox.Checked = false;
						this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						this.SynchronizeReferenceDensityCheckBox.Checked = false;
						this.SynchronizeReferenceDensityCheckBox.Enabled = false;
						this.StationPermissivesButton.Visible = true;
						this.InhibitLoadingByLoadIDCheckBox.Checked = false;
						this.InhibitLoadingByLoadIDCheckBox.Enabled = false;
						this.SetPreloadToZeroCheckBox.Checked = false;
						this.SetPreloadToZeroCheckBox.Enabled = false;
						this.VaporRecoveryCheckBox.Checked = false;
						this.VaporRecoveryCheckBox.Enabled = false;
						this.NumberOfLastTransaction.Enabled = true;
					}
					else
					{
						this.InhibitSettingRecipeNamesCheckBox.Enabled = true;
						this.SynchronizeReferenceDensityCheckBox.Enabled = true;
						if (Station.InterfaceType == STATION_INTERFACE_TYPE.MULTILOAD_II_SMP) // the smp only supports one product per arm
						{
							this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						}
					}

					// BOLPrinterDropDownList
					ListItem NewPrinterItem;
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

					NewPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
					this.BOLPrinterDropDownList.Items.Insert(0, NewPrinterItem);

					// Set the number of copies to be printed
					this.NumberOfCopiesTextBox.Text = Station.NumberOfCopies.ToString();

					// Populate IssueTransactionDropDownList
					Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.IssueTransactionDropDownList.Items.Add(Item);
					TransactionAliasCollectionClass TransactionAliasCollection;

					TransactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	x =>
																	x.EnumerateByTransTypeID(this.Security, TransactionTypes.T5_PrimaryDisbursement)
																);

					foreach (TransactionAliasClass TransactionAlias in TransactionAliasCollection)
					{
						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.IssueTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.IssueByVolumeTransactionAliasGuid)
						{
							this.IssueTransactionDropDownList.SelectedIndex = this.IssueTransactionDropDownList.Items.Count - 1;
						}
					}

					// Populate RecircTransactionDropDownList
					Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.RecircTransactionDropDownList.Items.Add(Item);
					TransactionAliasCollectionClass RecircTransactionAliasCollection;

					RecircTransactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
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
						
					// set the recirc card number
					this.MeterRecircCardNumber.Text = Station.MeterRecircCardNumber;

					if (Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q || Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA)
					{
						this.EnableScullyCheckBox.Checked = Station.EnableScully;
						this.EnableScullyCheckBox_CheckedChanged(this.EnableScullyCheckBox, null);
						this.EnableEquipmentValidateCheckBox.Checked = Station.EnableEquipmentValidate;

						//if (this.EnableScullyCheckBox.Checked)
						{
							this.EnumerateOpcServersBySystemName(PROCESS_VARIABLE_TYPE.SCULLY_PV);
						}

						ProcessVariableClass processVariable = Station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.SCULLY_PV];
						if (processVariable != null)
						{
							this.OPCItemPathTextBox.Text = processVariable.OPCItemID;
						}

						this.ResetScully(Station.EnableScully);
						this.EnableScullyCheckBox.Enabled = true;
					}
					else
							ResetScully(false);
				}
				else
				{
					if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010)
					{
						this.InhibitSettingRecipeNamesCheckBox.Checked = true;
						this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						this.SynchronizeReferenceDensityCheckBox.Checked = false;
						this.SynchronizeReferenceDensityCheckBox.Enabled = false;
					}
					else if (Station.InterfaceType == STATION_INTERFACE_TYPE.CONTREC1010_RA)
					{
						this.InhibitSettingRecipeNamesCheckBox.Checked = true;
						this.InhibitSettingRecipeNamesCheckBox.Enabled = false;
						this.SynchronizeReferenceDensityCheckBox.Checked = false;
						this.SynchronizeReferenceDensityCheckBox.Enabled = false;
					}
					else
					{
						this.InhibitSettingRecipeNamesCheckBox.Enabled = true;
						this.SynchronizeReferenceDensityCheckBox.Enabled = true;
					}
						if (Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_Q || Station.InterfaceType == STATION_INTERFACE_TYPE.ACCULOADIII_SA)
								ResetScully(true);
						else
								ResetScully(false);
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

		private void UpdateProcessVariable(PROCESS_VARIABLE_TYPE type)
		{
			var station = (StationClass)Session["Station"];

			ProcessVariableClass processVariable = station.ProcessVariableCollection[type];
			if (processVariable == null)
			{
				processVariable = new ProcessVariableClass
				{
					UnitType = UNIT_TYPE.STATION_UNIT,
					ProcessVariableType = type,
					Input = false
				};
				station.ProcessVariableCollection.Add(processVariable);
			}

			DropDownList dropDownList;
			TextBox textBox;
			if (type == PROCESS_VARIABLE_TYPE.SCULLY_PV)
			{
				dropDownList = this.OPCServerDropDownList;
				textBox = this.OPCItemPathTextBox;
			}
			else
				return;

			if (dropDownList.SelectedIndex != -1)
			{
				processVariable.ProgID = dropDownList.SelectedItem.Text;
				processVariable.URL = dropDownList.SelectedItem.Value;
			}

			processVariable.OPCItemID = textBox.Text;
		}

		private void EnumerateOpcServersBySystemName(PROCESS_VARIABLE_TYPE type)
		{
			try
			{
				// Populate the OPCServerDropDownList					
				var station = (StationClass)Session["Station"];

				ProcessVariableClass processVariable = station.ProcessVariableCollection[type];
				if (processVariable == null)
				{
					processVariable = new ProcessVariableClass
					{
						UnitType = UNIT_TYPE.STATION_UNIT,
						ProcessVariableType = type,
						Input = false
					};
					station.ProcessVariableCollection.Add(processVariable);
				}
				string name;
				DropDownList dropDownList;
				if (type == PROCESS_VARIABLE_TYPE.SCULLY_PV)
				{
					name = "Varec.ScullyOPCServer";
					dropDownList = this.OPCServerDropDownList;
				}
				else
					return;

				//dropDownList.Items.Clear();
				var serverEnumerator = new ServerEnumerator();
				Server[] servers = serverEnumerator.GetAvailableServers(Opc.Specification.COM_DA_20);
				foreach (Server server in servers)
				{
					server.Name = server.Name.Replace("localhost" + ".", string.Empty);

					if (server.Name != name && server.Name != "Matrikon.OPC.Simulation")
					{
							continue;
					}

					var item = new ListItem(server.Name, server.Url.ToString());
					if (dropDownList.Items.FindByText(server.Name) == null)
					{
						dropDownList.Items.Add(item);
						if(processVariable.ProgID == server.Name)
						{
							dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
						}
					}
				}

				if (dropDownList.SelectedIndex == -1)
					dropDownList.SelectedIndex = dropDownList.Items.Count - 1;
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		private void ResetScully(bool enable)
		{
			this.OPCItemPathTextBox.Enabled = enable;
			this.OPCServerDropDownList.Enabled = enable;
			this.OPCItemPathLbl.Enabled = enable;
			this.OPCServerLbl.Enabled = enable;
		}

		protected void EnableScullyCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			ResetScully(this.EnableScullyCheckBox.Checked);
		}
		private void EnableStationFormControls(bool enable)
		{
			var stationForm = (StationForm)this.Page;
			stationForm.EnableControls(enable);
		}

		protected void EnableDynamicRecipesCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			var station = (StationClass)Session["Station"];
			station.EnableDynamicRecipes = this.EnableDynamicRecipesCheckBox.Checked;

			List<LoadArmClass> swingArms = station.LoadArmCollection.Where(loadArm => loadArm.SwingArm).ToList();

			List<Guid> swingArmPartnerStationGuids = new List<Guid>();

			foreach (var arm in swingArms)
			{
				swingArmPartnerStationGuids.Add((station?.SwingArmPosition == "A") ? arm.BayBStationGuid : arm.BayAStationGuid);
			}

			bool doTheyMatch = true;

			if (swingArms.Count > 0)
			{
				List<bool> isDynamicRecipesEnabledOnPartnerStations =
					 FMChannelHelper.MakeCall<IStations, List<bool>>(
						  x => x.IsDynamicRecipesEnabledOnPartnerStations(this.Security, station.IdentityGuid, swingArmPartnerStationGuids, station.Type));

				doTheyMatch = isDynamicRecipesEnabledOnPartnerStations.All(x => x == station.EnableDynamicRecipes);
			}

			bool DisplayConfirmationPrompt = !doTheyMatch; 

			Button OKButton = (Button)this.Page.FindControl("OK");

			if (OKButton != null)
			{
				OKButton.OnClientClick = DisplayConfirmationPrompt ? "return OKButton_Click();" : "";
			}
		
			//Re-enable station form New/OK/Cancel buttons that were disabled on the client side
			EnableStationFormControls(true);
		}
	}
}