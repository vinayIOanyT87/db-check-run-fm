/******************************************************************************
	FILE NAME:		StationSignatureStationPage.ascx.cs
	PURPOSE:		Implementation of StationSignatureStationPage

	COMMENTS:
		Copyright (C) Varec, Inc. Norcross, GA, USA
		This file shall not be copied or reproduced in any form without
		the express written consent of Varec, Inc.

	AUTHOR(S):	C. Knight
	VERSION:	7.4.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2008-04-16	C. Knight			7.4.0.0 - Initial Creation

*******************************************************************************/
namespace FuelsManager.FMWebApp
{
	using System;

	using FMBusinessObjects.DataObjects;


	/// <summary>
	///		Summary description for StationSignatureStationPage.
	/// </summary>
	public partial class StationSignatureStationPage : FMUserControlBase
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				StationClass Station = (StationClass) Session["Station"];
				if (Station.Type != STATION_TYPE.SIGNATURE)
					return;

				if (Page.IsPostBack == false) 
				{
					SignatureDeviceTextBox.Text = Station.SignatureDevice;
					SignatureCapturePort.Text = Station.SignatureDevicePort.ToString();
					SignatureCaptureBaudRate.Text = Station.SignatureDeviceBaudRate.ToString();
				}
			}

			catch (Exception except)
			{
				ErrorHandler(except);
			}
		}

		/// <summary>
		/// This method will update the Station object with the information on the form.
		/// This method will be called from the StationForm since the OK and New buttons
		/// are on that form.
		/// </summary>
		public void UpdateData()
		{
			StationClass Station = (StationClass) Session["Station"];

			Station.SignatureDevice=SignatureDeviceTextBox.Text;
			Station.SignatureDevicePort = System.Convert.ToInt32(SignatureCapturePort.Text);
			Station.SignatureDeviceBaudRate = System.Convert.ToInt32(SignatureCaptureBaudRate.Text);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
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
