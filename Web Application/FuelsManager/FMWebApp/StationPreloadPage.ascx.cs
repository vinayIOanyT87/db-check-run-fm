// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StationPreloadPage.ascx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the StationPreloadPage type.
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
	///    Summary description for StationPreloadPage.
	/// </summary>
	public partial class StationPreloadPage : FMUserControlBase
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
				Station.SetDefaultPresetToZero = this.SetPreloadToZeroCheckBox.Checked;
				Station.InhibitLoadingByLoadID = this.InhibitLoadingByLoadIDCheckBox.Checked;

				//preload number of copies
				if ((this.PreloadNumberOfCopiesTextBox.Text == null) || (this.PreloadNumberOfCopiesTextBox.Text.Length <= 0))
				{
					this.PreloadNumberOfCopiesTextBox.Text = StationClass.MinNumberOfCopies.ToString();
				}

				int copies = Convert.ToInt32(this.PreloadNumberOfCopiesTextBox.Text);

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
				if (Station.Type != STATION_TYPE.PRELOAD)
				{
					return;
				}

				if (this.Page.IsPostBack == false)
				{
					this.InhibitLoadingByLoadIDCheckBox.Checked = Station.InhibitLoadingByLoadID;

					// PreloadPrinterDropDownList
					ListItem NewPrinterItem;

					try
					{
						string[] InstalledPrinters = ReportServicePrintService.EnumeratePrinters("Preload");

						//set preload printer
						int Index = 1;
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

						if (Station.PreloadPrinter != "{None}" && Station.PreloadPrinter.Length > 0)
						{
							var PrinterItemFromDB = new ListItem(Station.PreloadPrinter, "1");
							this.PreloadPrinterDropDownlist.Items.Add(PrinterItemFromDB);
							this.PreloadPrinterDropDownlist.SelectedIndex = this.PreloadPrinterDropDownlist.Items.Count - 1;
						}
					}

					NewPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
					this.PreloadPrinterDropDownlist.Items.Insert(0, NewPrinterItem);

					// Set the number of copies to be printed
					this.PreloadNumberOfCopiesTextBox.Text = Station.NumberOfPreloadCopies.ToString();

					this.SetPreloadToZeroCheckBox.Checked = Station.SetDefaultPresetToZero;

					// Populate IssueByVolumeTransactionDropDownList
					var Item = new ListItem(this.GetTranslatedText("{None}"), Guid.Empty.ToString());
					this.IssueByVolumeTransactionDropDownList.Items.Add(Item);
					this.IssueByWeightTransactionDropDownList.Items.Add(Item);

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

		#endregion
	}
}