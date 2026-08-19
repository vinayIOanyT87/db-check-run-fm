/******************************************************************************
	FILE NAME:		StationBillOfLadingPage.ascx.cs
	PURPOSE:		Implementation of StationBillOfLadingPage

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-12-18	Richard Panachida	Added a new field to handle the number of BOL copies
												to be printed (CSI 3867).

		2008-01-07	W.Gray				7.3.2.0 - Added Signature Device

		2008-04-22	I.Orndorff			7.4.0.0 - Modified "Page_Load()" to set the default BOL printer 
												  even if it can't be enumerated. This fixes CSI #5777.

		2008-04-22	W.Gray				7.4.0.1 - Modified to index starting at 1 for enumerated printers
												CSI 5590

*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	///		Summary description for StationBillOfLadingPage.
	/// </summary>
	public partial class StationBillOfLadingPage : FMUserControlBase
	{

		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				StationClass Station = (StationClass) this.Session["Station"];
				if (Station.Type != STATION_TYPE.BOL)
					return;

				if (this.Page.IsPostBack == false) 
				{
					// BOLPrinterDropDownList
					ListItem NewPrinterItem;

					try
					{
						string[] InstalledPrinters = FMBusinessObjects.UtilityObjects.ReportServicePrintService.EnumeratePrinters("BOL");
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
										this.BOLPrinterDropDownList.SelectedIndex = Insert;

									NewPrinterItem = null;
									break;
								}
							}

							if (NewPrinterItem != null)
							{
								this.BOLPrinterDropDownList.Items.Add(NewPrinterItem);

								if (Station.BOLPrinter == NewPrinterItem.Text)
									this.BOLPrinterDropDownList.SelectedIndex = this.BOLPrinterDropDownList.Items.Count - 1;
							}

							Index++;
						}
					}
					catch (System.Net.Sockets.SocketException socketExcept)
					{
						if(socketExcept.ErrorCode != 10061)
							throw socketExcept;

						if (Station.BOLPrinter != "{None}" &&
						Station.BOLPrinter.Length > 0)
						{
							ListItem PrinterItemFromDB = new ListItem(Station.BOLPrinter, "1");
							this.BOLPrinterDropDownList.Items.Add(PrinterItemFromDB);
							this.BOLPrinterDropDownList.SelectedIndex = this.BOLPrinterDropDownList.Items.Count - 1;
						}
					}

					NewPrinterItem = new ListItem(this.GetTranslatedText("{None}"), "0");
					this.BOLPrinterDropDownList.Items.Insert(0, NewPrinterItem);

					this.SignatureCapturePort.Text = Station.SignatureDevicePort.ToString();
					this.SignatureCaptureBaudRate.Text = Station.SignatureDeviceBaudRate.ToString();
					this.BOLAgeInMinutesTextBox.Text = Station.BOLAgeInMinutes.ToString();
					this.NumberOfCopiesTextBox.Text = Station.NumberOfCopies.ToString();
					this.SignatureDeviceTextBox.Text = Station.SignatureDevice;
				}
			}

			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will update the Station object with the information on the form.
		/// This method will be called from the StationForm since the OK and New buttons
		/// are on that form.
		/// </summary>
		public void UpdateData()
		{
			StationClass station = (StationClass) this.Session["Station"];
		    if (this.BOLPrinterDropDownList.SelectedItem.Text == this.GetTranslatedText("{None}"))
		    {
		        station.BOLPrinter = "{None}";
		    }
		    else
		    {
		        station.BOLPrinter   = this.BOLPrinterDropDownList.SelectedItem.Text;
		    }

			try
			{
				station.BOLAgeInMinutes = Convert.ToInt32(this.BOLAgeInMinutesTextBox.Text);

				if (string.IsNullOrEmpty(this.NumberOfCopiesTextBox.Text))
				{
					this.NumberOfCopiesTextBox.Text = StationClass.MinNumberOfCopies.ToString();
				}

				int copies = Convert.ToInt32(this.NumberOfCopiesTextBox.Text);
				if ((copies < StationClass.MinNumberOfCopies) || (copies > StationClass.MaxNumberOfCopies))
				{
					throw new Exception("Copies range must be between " + StationClass.MinNumberOfCopies +
						                "and " + StationClass.MaxNumberOfCopies);
				}
				else
				{
					station.NumberOfCopies = copies;
				}
				station.SignatureDevice=this.SignatureDeviceTextBox.Text;
				station.SignatureDevicePort = Convert.ToInt32(this.SignatureCapturePort.Text);
				station.SignatureDeviceBaudRate = Convert.ToInt32(this.SignatureCaptureBaudRate.Text);
			}
			catch (Exception ex)
			{
				if (ex.Message.StartsWith("Copies range") == true)
				{
					throw new Exception(ex.Message.ToString());
				}
				else
				{
					throw new Exception("Value must be numeric");
				}
			}		
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			this.InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion
	}
}
