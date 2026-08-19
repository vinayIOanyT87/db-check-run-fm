// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationWeightScalePage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationWeightScalePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.FMWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Sockets;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;

	using Opc;

	using OpcCom;

	using Convert = System.Convert;
	using Server = Opc.Server;

	/// <summary>
	///    Summary description for StationWeightScalePage.
	/// </summary>
	public partial class StationWeightScalePage : FMUserControlBase
	{
		#region Public Methods and Operators

		/// <summary>
		///    This method will update the Station object with the information on the form.
		///    This method will be called from the StationForm since the OK and New buttons
		///    are on that form.
		/// </summary>
		public void UpdateData()
		{
			var Station = (StationClass)this.Session["Station"];

			ProcessVariableClass ProcessVariable = Station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV];
			if (null != ProcessVariable)
			{
				if (this.OPCServerDropDownList.SelectedIndex != -1)
				{
					ProcessVariable.ProgID = this.OPCServerDropDownList.SelectedItem.Text;
					ProcessVariable.URL = this.OPCServerDropDownList.SelectedItem.Value;
				}

				ProcessVariable.OPCItemID = this.OPCItemPathTextBox.Text;
			}

			Station.InhibitOperatingModePrompt = this.InhibitOperatingModePromptCheckBox.Checked;
			Station.InhibitLoadingByLoadID = this.InhibitLoadingByLoadIDCheckBox.Checked;
			Station.SetDefaultPresetToZero = this.SetPreloadToZeroCheckBox.Checked;

			if (this.BOLPrinterDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.BOLPrinter = "{None}";
			}
			else
			{
				Station.BOLPrinter = this.BOLPrinterDropDownList.SelectedItem.Text;
			}

			if (this.PreloadPrinterDropDownlist.SelectedItem.Text == this.GetTranslatedText("{None}"))
			{
				Station.PreloadPrinter = "{None}";
			}
			else
			{
				Station.PreloadPrinter = this.PreloadPrinterDropDownlist.SelectedItem.Text;
			}

			try
			{
				Station.IssueByVolumeTransactionAliasGuid = new Guid(this.IssueByVolumeTransactionDropDownList.SelectedValue);
				Station.IssueByWeightTransactionAliasGuid = new Guid(this.IssueByWeightTransactionDropDownList.SelectedValue);
				Station.ReceiptByVolumeTransactionAliasGuid = new Guid(this.ReceiptByVolumeTransactionDropDownList.SelectedValue);
				Station.ReceiptByWeightTransactionAliasGuid = new Guid(this.ReceiptByWeightTransactionDropDownList.SelectedValue);

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

				//preload number of copies
				if ((this.PreloadNumberOfCopiesTextBox.Text == null) || (this.PreloadNumberOfCopiesTextBox.Text.Length <= 0))
				{
					this.PreloadNumberOfCopiesTextBox.Text = StationClass.MinNumberOfCopies.ToString();
				}

				copies = Convert.ToInt32(this.PreloadNumberOfCopiesTextBox.Text);

				if ((copies < StationClass.MinNumberOfCopies) || (copies > StationClass.MaxNumberOfCopies))
				{
					throw new Exception(
						"Preload copies range must be between " + StationClass.MinNumberOfCopies + "and "
						+ StationClass.MaxNumberOfCopies);
				}
				else
				{
					Station.NumberOfPreloadCopies = copies;
				}
			}
			catch (Exception ex)
			{
				if (ex.Message.IndexOf("range must be") != -1)
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
				if (Station.Type != STATION_TYPE.WEIGHT_SCALE)
				{
					return;
				}

				if (this.Page.IsPostBack == false)
				{
					// Populate SelectSystemModeDropDownList
					var NewItem = new ListItem("List", "0");
					this.SelectSystemModeDropDownList.Items.Add(NewItem);
					NewItem = new ListItem("Text", "1");
					this.SelectSystemModeDropDownList.Items.Add(NewItem);
					this.SelectSystemModeDropDownList.SelectedIndex = 1;
					this.SelectSystemModeDropDownList_SelectedIndexChanged(null, null);

					// Find the default system stored in the process variable collection.
					ProcessVariableClass ProcessVariable = Station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV];
					if (null != ProcessVariable)
					{
						var Url = new URL(ProcessVariable.URL);
						this.SystemTextBox.Text = Url.HostName;
						this.EnumerateOPCServersBySystemName(this.SystemTextBox.Text);
						this.OPCItemPathTextBox.Text = ProcessVariable.OPCItemID;
					}

					this.InhibitOperatingModePromptCheckBox.Checked = Station.InhibitOperatingModePrompt;
					this.InhibitLoadingByLoadIDCheckBox.Checked = Station.InhibitLoadingByLoadID;
					this.SetPreloadToZeroCheckBox.Checked = Station.SetDefaultPresetToZero;

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

						//set preload printer
						Index = 1;
						foreach (string Printer in InstalledPrinters)
						{
							NewPrinterItem = new ListItem(Printer, Index.ToString());
							foreach (ListItem ExistingPrinterItem in this.PreloadPrinterDropDownlist.Items)
							{
								if (ExistingPrinterItem.Text.CompareTo(NewPrinterItem.Text) > 0)
								{
									int Insert = this.PreloadPrinterDropDownlist.Items.IndexOf(ExistingPrinterItem);
									this.PreloadPrinterDropDownlist.Items.Insert(Insert, NewPrinterItem);

									if (Station.PreloadPrinter == NewPrinterItem.Text)
									{
										this.PreloadPrinterDropDownlist.SelectedIndex = Insert;
									}
									NewPrinterItem = null;
									break;
								}
							}

							if (NewPrinterItem != null)
							{
								this.PreloadPrinterDropDownlist.Items.Add(NewPrinterItem);
								if (Station.PreloadPrinter == NewPrinterItem.Text)
								{
									this.PreloadPrinterDropDownlist.SelectedIndex = this.PreloadPrinterDropDownlist.Items.Count - 1;
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

						// Set the default preload printer even if it can't be enumerated. 
						// This fixes CSI #5777. (IGO 22-Apr-2008)
						if (Station.PreloadPrinter != "{None}" && Station.PreloadPrinter.Length > 0)
						{
							var PrinterItemFromDB = new ListItem(Station.PreloadPrinter, "1");
							this.PreloadPrinterDropDownlist.Items.Add(PrinterItemFromDB);
							this.PreloadPrinterDropDownlist.SelectedIndex = this.PreloadPrinterDropDownlist.Items.Count - 1;
						}
					}

					NewPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
					this.BOLPrinterDropDownList.Items.Insert(0, NewPrinterItem);
					this.PreloadPrinterDropDownlist.Items.Insert(0, NewPrinterItem);

					// Populate IssueByVolumeTransactionDropDownList
					var Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.IssueByVolumeTransactionDropDownList.Items.Add(Item);
					this.IssueByWeightTransactionDropDownList.Items.Add(Item);
					this.ReceiptByVolumeTransactionDropDownList.Items.Add(Item);
					this.ReceiptByWeightTransactionDropDownList.Items.Add(Item);

					TransactionAliasCollectionClass TransactionAliasCollection;
					TransactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.Security, TransactionTypes.T5_PrimaryDisbursement)
																);


					foreach (TransactionAliasClass TransactionAlias in TransactionAliasCollection)
					{
						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.IssueByVolumeTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.IssueByVolumeTransactionAliasGuid)
						{
							this.IssueByVolumeTransactionDropDownList.SelectedIndex = this.IssueByVolumeTransactionDropDownList.Items.Count
							                                                          - 1;
						}

						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.IssueByWeightTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.IssueByWeightTransactionAliasGuid)
						{
							this.IssueByWeightTransactionDropDownList.SelectedIndex = this.IssueByWeightTransactionDropDownList.Items.Count
							                                                          - 1;
						}
					}

					TransactionAliasCollection = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasCollectionClass>(
																	 x =>
																	 x.EnumerateByTransTypeID(this.Security, TransactionTypes.T8_Receipt)
																);

					foreach (TransactionAliasClass TransactionAlias in TransactionAliasCollection)
					{
						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.ReceiptByVolumeTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.ReceiptByVolumeTransactionAliasGuid)
						{
							this.ReceiptByVolumeTransactionDropDownList.SelectedIndex =
								this.ReceiptByVolumeTransactionDropDownList.Items.Count - 1;
						}

						Item = new ListItem(TransactionAlias.ID, TransactionAlias.MasterRecordGuid.ToString());
						this.ReceiptByWeightTransactionDropDownList.Items.Add(Item);

						if (TransactionAlias.MasterRecordGuid == Station.ReceiptByWeightTransactionAliasGuid)
						{
							this.ReceiptByWeightTransactionDropDownList.SelectedIndex =
								this.ReceiptByWeightTransactionDropDownList.Items.Count - 1;
						}
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		protected void SelectSystemModeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.SystemDropDownList.Visible && this.SystemDropDownList.SelectedIndex != -1)
			{
				this.SystemTextBox.Text = this.SystemDropDownList.SelectedItem.Text;
			}

			this.SystemDropDownList.Visible = (this.SelectSystemModeDropDownList.SelectedIndex == 1) ? false : true;
			this.SystemTextBox.Visible = !this.SystemDropDownList.Visible;

			// Only popluate the system drop down list when visible. 
			if (this.SystemDropDownList.Visible)
			{
				this.PopulateSystemDropDownList();
			}
		}

		protected void SystemDropDownList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (-1 != this.SystemDropDownList.SelectedIndex)
			{
				this.EnumerateOPCServersBySystemName(this.SystemDropDownList.SelectedItem.Text);
			}
		}

		protected void SystemTextBox_TextChanged(object sender, EventArgs e)
		{
			// Update OPC servers based on text box during post back
			if (this.SystemTextBox.Visible)
			{
				if (0 != this.SystemTextBox.Text.Length)
				{
					this.EnumerateOPCServersBySystemName(this.SystemTextBox.Text);
				}
			}
		}

		private void EnumerateOPCServersBySystemName(string SystemName)
		{
			try
			{
				// Populate the OPCServerDropDownList
				this.OPCServerDropDownList.Items.Clear();
				var Station = (StationClass)this.Session["Station"];
				foreach (ProcessVariableClass ProcessVariable in Station.ProcessVariableCollection)
				{
					if (ProcessVariable.ProcessVariableType != PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV)
					{
						continue;
					}

					var ServerEnumerator = new ServerEnumerator();
					Server[] Servers = ServerEnumerator.GetAvailableServers(
						Specification.COM_DA_20, SystemName, new ConnectData(new NetworkCredential()));
					foreach (Server Server in Servers)
					{
						Server.Name = Server.Name.Replace(SystemName + ".", "");

						if (Server.Name != "Varec.WeightScaleOPCServer" && Server.Name != "Matrikon.OPC.Simulation")
						{
							continue;
						}

						var Item = new ListItem(Server.Name, Server.Url.ToString());
						this.OPCServerDropDownList.Items.Add(Item);
						if (ProcessVariable.ProgID == Server.Name)
						{
							this.OPCServerDropDownList.SelectedIndex = this.OPCServerDropDownList.Items.Count - 1;
						}
					}
					break;
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Required method for Designer support - do not modify
		///    the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		private void PopulateSystemDropDownList()
		{
			var Station = (StationClass)this.Session["Station"];

			// Populate SystemDropDownList
			this.SystemDropDownList.Items.Clear();
			var NewItem = new ListItem("localhost", "0");
			this.SystemDropDownList.Items.Add(NewItem);
			var serverList = new List<string>();
			var domain = EnumerateLanMachines.GetDomainOrWorkgroup();
			EnumerateLanMachines.EnumerateMachines(serverList, domain);

			int Item = 1;

			ProcessVariableClass ProcessVariable = Station.ProcessVariableCollection[PROCESS_VARIABLE_TYPE.STATION_PV];
			if (null != ProcessVariable)
			{
				var Url = new URL(ProcessVariable.URL);

				foreach (string System in serverList)
				{
					NewItem = new ListItem(System, Item.ToString());
					this.SystemDropDownList.Items.Add(NewItem);
					if (System == Url.HostName)
					{
						this.SystemDropDownList.SelectedIndex = this.SystemDropDownList.Items.Count - 1;
					}
					Item++;
				}

				this.OPCItemPathTextBox.Text = ProcessVariable.OPCItemID;
			}
		}

		#endregion
	}
}