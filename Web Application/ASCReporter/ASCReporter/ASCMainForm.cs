//******************************************************************************
//	FILE NAME:		ASCMainForm.cs
//	PURPOSE:			The Main Window class for the ASC Reporting Interface
//
//	COMMENTS:
//		Copyright (C) Varec, Inc. Norcross, GA, USA, 2007
//		This file shall not be copied or reproduced in any form without
//		the express written consent of Varec, Inc.
//
//	AUTHOR(S):	Chris Knight
//	VERSION:		1.0.0.0  Current version
//
//	MODIFICATION HISTORY:
//		Date:			By:				Reason:
//		---------	-------------- -------------------------------------------
//		04-May-2007	C. Knight		1.0.0.0	- Initial Creation
//
//*******************************************************************************       
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Configuration;
using System.Diagnostics;
using DispatchPrototype;
using FMBusinessObjects.DataObjects;
using FMBusinessObjects.Constants;
using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;


namespace ASCReporter
{
	/// <summary>
	/// Main window for the ASC Interface
	/// </summary>
	/// <remarks>
	/// This is the main window for the ASC Interface.  Code here handles the main menu
	/// and the command buttons.
	/// </remarks>
	public partial class MainForm : FMBaseForm
	{


		#region Constants
		private const float fuelingTime3501 = .282F;
		#endregion

		#region Constructors
		/// <summary>
		/// Default constructor for the main window of the ASC Interface
		/// </summary>
		public MainForm ( )
		{
			AppDomain.CurrentDomain.SetData ( "APP_CONFIG_FILE", "ASCReporter.config" );
			InitializeComponent ( );
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Username from the FuelsManager Defense Dispatch security context.  This will be used
		/// for all database access, satisfying the auditability requirement of DoDI 8500.2
		/// </summary>
		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		/// <summary>
		/// User Guid from the FuelsManager Defense Dispatch security context.  Needed for future expandibility
		/// </summary>
		public Guid UserGuid
		{
			get
			{
				return this.userGuid;
			}
			set
			{
				this.userGuid = value;
			}
		}

		/// <summary>
		/// Site Guid from the FuelsManager Defense Dispatch security context.  Used now for filtering
		/// records in a multi-site environment (outside of current scope)
		/// </summary>
		public Guid SiteGuid
		{
			get
			{
				return this.siteGuid;
			}
			set
			{
				this.siteGuid = value;
			}
		}

		/// <summary>
		/// ID of the parent app
		/// </summary>
		/// <remarks>
		/// ID of the parent app (the app which launched ASCReporter.exe).  ASCReporter must close very
		/// quickly after the parent process (expected to be aviation) closes
		/// </remarks>
		public int ParentID
		{
			get
			{
				return this.parentID;
			}
			set
			{
				this.parentID = value;
			}
		}
		#endregion

		private void showReport ( string reportName, SortedDictionary<string, string> parameterList )
		{
			try
			{
				string baseAddress = ConfigurationManager.AppSettings["WebAppAddress"];
				if (String.IsNullOrEmpty ( baseAddress ))
				{

					throw new ApplicationException ( "WebAppAddress not in configuration file." );
				}

				baseAddress += "/FMReporting/ReportLandingPage.aspx?ReportType=" +
					  ( (int) ReportTypesClass.ReportTypes.VARIABLE_PARAMETERS ).ToString ( ) + "&ReportName=" + reportName.Replace ( ' ', '+' );
				foreach (KeyValuePair<string, string> parameter in parameterList)
				{
					baseAddress += "&";
					baseAddress += parameter.Key;
					baseAddress += "=";
					baseAddress += parameter.Value;
				}

				SecurityClass security = AppDomain.CurrentDomain.GetData ( "Security" ) as SecurityClass;
				string reportGroup = ConfigurationManager.AppSettings["ASCReportGroup"];
				EmbeddedBrowser embeddedBrowser = null;

				embeddedBrowser = new EmbeddedBrowser ( baseAddress );
				embeddedBrowser.ShowDialog ( this );

			}
			catch (Exception except)
			{
				ErrorHandler ( except );
			}


		}


		#region Event Handlers
		/*		private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
	  {
			
		 ASCReportViewer viewer = new ASCReportViewer();
		 SortedDictionary<string, Object> parameterList = new SortedDictionary<string, Object>();
		 showReport(viewer.ShowRefuelingCapabilityReport(this.siteName, this.UserName, parameterList));
	 
	  }
*/
		private void refuelingUnitCapabilityReportToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			SortedDictionary<string, string> parameterList = new SortedDictionary<string, string> ( );

			parameterList.Add ( "PeaceTimeDayLength", this.PeacetimeDayLength.ToString ( ) );
			parameterList.Add ( "LeavesRCC", this.LeavesRCC.ToString ( ) );
			parameterList.Add ( "PositionUnit", this.PositionsUnit.ToString ( ) );
			parameterList.Add ( "PrepareUnit", this.PreparesUnit.ToString ( ) );
			parameterList.Add ( "CompleteServicing", this.CompletesServicing.ToString ( ) );
			parameterList.Add ( "TravelNext", this.TravelsNextLocation.ToString ( ) );
			parameterList.Add ( "TravelStorage", this.TravelsStorage.ToString ( ) );
			parameterList.Add ( "WaitsRefill", this.WaitsRefill.ToString ( ) );
			parameterList.Add ( "SiteName", this.siteName );

			showReport ( "BSM-E PeacetimeWartimeRefuelingUniReport", parameterList );
		}

		private void jetFuelRefuelingUnitAuthorizationReportToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			SortedDictionary<string, string> parameterList = new SortedDictionary<string, string> ( );

			if (this.PeacetimeDayLength < ( this.LeavesRCC +
														this.PositionsUnit +
														this.PreparesUnit +
														MainForm.fuelingTime3501 +
														this.CompletesServicing +
														this.TravelsNextLocation ))
			{
				MessageBox.Show ( ErrorMessages.PATsTooBig );
				return;
			}

			parameterList.Add ( "PeaceTimeDayLength", this.PeacetimeDayLength.ToString ( ) );
			parameterList.Add ( "LeavesRCC", this.LeavesRCC.ToString ( ) );
			parameterList.Add ( "PositionUnit", this.PositionsUnit.ToString ( ) );
			parameterList.Add ( "PrepareUnit", this.PreparesUnit.ToString ( ) );
			parameterList.Add ( "CompleteServicing", this.CompletesServicing.ToString ( ) );
			parameterList.Add ( "TravelNext", this.TravelsNextLocation.ToString ( ) );
			parameterList.Add ( "TravelStorage", this.TravelsStorage.ToString ( ) );
			parameterList.Add ( "WaitsRefill", this.WaitsRefill.ToString ( ) );
			parameterList.Add ( "SiteName", this.siteName );

			showReport ( "BSM-E JetFuelRefuelingUnit", parameterList );
		}

		private void c300andr12reportToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			ASCReportViewer viewer = new ASCReportViewer ( );
			SortedDictionary<string, string> parameterList = new SortedDictionary<string, string> ( );

			try
			{
				parameterList.Add ( "DieselGallon", this.maxDieselQuantity.ToString ( ) );
				parameterList.Add ( "MogasLeadedGallon", this.maxLeadedQuantity.ToString ( ) );
				parameterList.Add ( "MogasUnleadedGallon", this.maxUnleadedQuantity.ToString ( ) );
				parameterList.Add ( "WasteFlag", this.GroundVBBD.ToString ( ) );
				parameterList.Add ( "JFDEFFlag", this.GroundVQWS.ToString ( ) );
				parameterList.Add ( "CBOIA", this.AuthorizedDieselUnits.ToString ( ) );
				parameterList.Add ( "CBOIB", this.AuthorizedLeadedUnits.ToString ( ) );
				parameterList.Add ( "CBOIC", this.AuthorizedUnleadedUnits.ToString ( ) );
				parameterList.Add ( "CBOID", this.AuthorizedWasteUnits.ToString ( ) );
				parameterList.Add ( "CBOIUTC", this.AuthorizedUTCJFDEFUnits.ToString ( ) );
				parameterList.Add ( "RBOIA", this.HsvBoiA.ToString ( ) );
				parameterList.Add ( "RBOIB", this.HsvBoiB.ToString ( ) );
				parameterList.Add ( "RBOIC", this.HsvBoiC.ToString ( ) );
				parameterList.Add ( "SiteName", this.siteName );
			}
			catch (InvalidCastException x)
			{
				x.ToString ( );
				MessageBox.Show ( ErrorMessages.ErrorWrappingReportParameters );
				return;
			}

			showReport ( "BSM-E GroundProductHydrantServicingVehReport", parameterList );
		}

		private void fuelingPeakDaysReportToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			ASCReportViewer viewer = new ASCReportViewer ( );
			SortedDictionary<string, string> parameterList = new SortedDictionary<string, string> ( );

			parameterList.Add ( "SiteName", this.siteName );

			showReport ( "BSM-E SixPeakDaysReport", parameterList );
		}

		private void aboutToolStripMenuItem_Click ( object sender, EventArgs e )
		{
			AboutBox aboutBox = new AboutBox ( );

			aboutBox.ShowDialog ( );
		}
		private void InitDefaultConfiguration ( )
		{
			string x = ConfigurationManager.AppSettings["Site"];
			string configFile = "Dispatch.config";

			// Map the roaming configuration file. This enables the application to access 
			// the configuration file using the System.Configuration.Configuration class
			ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap ( );
			configFileMap.ExeConfigFilename = configFile;

			// Get the mapped configuration file.
			Configuration config = ConfigurationManager.OpenMappedExeConfiguration ( configFileMap, ConfigurationUserLevel.None );
			if (config == null || !config.HasFile)
			{
				throw new ApplicationException ( "Dispatch.config missing." );
			}
			foreach (KeyValueConfigurationElement configElement in config.AppSettings.Settings)
			{
				ConfigurationManager.AppSettings[configElement.Key] = config.AppSettings.Settings[configElement.Key].Value;
			}

		}

		private void MainForm_Load ( object sender, EventArgs e )
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				SecurityClass security = null;
				string securityStr = Environment.GetEnvironmentVariable ( "Security" );
				string token = Environment.GetEnvironmentVariable ( "Token" );
				InitDefaultConfiguration ( );
				string webAppAddress = ConfigurationManager.AppSettings["WebAppAddress"];//Environment.GetEnvironmentVariable("WebAppAddress");
				if (securityStr != null)
				{
					XmlSerializer xmlFormat = new XmlSerializer ( typeof ( SecurityClass ) );
					System.IO.StringReader sReader = new System.IO.StringReader ( securityStr );
					security = (SecurityClass) xmlFormat.Deserialize ( sReader );
					this.UserName = security.UserID;
					this.UserGuid = security.UserGuid;
					this.SiteGuid = security.SiteGuid;

					sReader.Close ( );
					AppDomain.CurrentDomain.SetData ( "Security", security );
					//   MessageBox.Show("security user = " + security.UserID + ", site = " + security.SiteID);
				}
				else
				{
					//  MessageBox.Show("NULL security");
#if DEBUG
					//security = new SecurityClass();
					//security.SiteID = "R68971";
					//security.UserID = "Administrator";
					//security.UserIndex = 1;
					//security.SiteIndex = 1;
					//security.LoginSiteIndex = 1;
					//security.Password = "marietta";
					//this.UserName = security.UserID;
					//this.UserIndex = security.UserIndex;
					//this.SiteIndex = security.SiteIndex;
#else
				throw new Exception("Security missing.");
#endif
				}
				if (token == null)
				{
					//  MessageBox.Show("NULL token");
#if DEBUG
					//token = "e39d252d-9fe8-4fa4-b144-4fff269814d6";
#else
			   throw new Exception("Token missing.");
#endif
				}
				if (webAppAddress == null)
				{
					//     MessageBox.Show("NULL webAppAddress");
#if DEBUG
					//webAppAddress = "http://localhost/FuelsManager";
#else
			   throw new Exception("WebAppAddress missing.");
#endif
				}
				AppDomain.CurrentDomain.SetData ( "Security", security );
				AppDomain.CurrentDomain.SetData ( "Token", token );
				//  MessageBox.Show("Token = " + token + ", webAppAddress = " + webAppAddress);


				connectionBuilder = new SqlConnectionStringBuilder ( ConsolidatedDA.ConnectionString );
				connectionBuilder.UserID = security.UserID;

				FMChannelFactory<IDBAccess> dbAccessClient = new FMChannelFactory<IDBAccess> ( );
				IDBAccess DBAccess = dbAccessClient.CreateProxy ( );

				connectionBuilder.Password = DBAccess.GetDBPassword ( security.Password );
				connectionBuilder.IntegratedSecurity = false;
				this.connection = new SqlConnection ( connectionBuilder.ConnectionString );
				this.connection.Open ( );

				SqlCommand createTempTables = this.connection.CreateCommand ( );
				createTempTables.CommandType = CommandType.Text;
				createTempTables.CommandText = Strings.CreateRefuelActivityTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateDefuelActivityTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateGroundActivityTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateAviationProductClassTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateGroundProductClassTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateAviationAuthorizationsTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateFuelingPeakDaysTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateRefuelExcludedDaysTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateDefuelExcludedDaysTable;
				createTempTables.ExecuteNonQuery ( );
				createTempTables.CommandText = Strings.CreateGroundExcludedDaysTable;
				createTempTables.ExecuteNonQuery ( );

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites> ( );
				ISites sites = sitesClient.CreateProxy ( );
				SiteClass site = sites.Get(security, security.SiteGuid, false, false, false);
				this.siteName = site.Number;

				if (String.IsNullOrEmpty ( this.siteName ))
				{
					this.siteName = site.ID;
				}

				this.AviationProductClassification_Init ( );
				this.GroundProductClassification_Init ( );

				this.peacetimeDayLength.SelectedIndex = 1;
			}
			catch (Exception xcpt)
			{

				if (this.connection != null)
				{
					if (this.connection.State != ConnectionState.Closed)
					{
						this.connection.Close ( );
					}

					xcpt.ToString ( );
					MessageBox.Show ( ErrorMessages.DatabaseConnectionFailed );
				}
				else
				{
					MessageBox.Show ( xcpt.Message );
				}
				this.Close ( );
			}

			this.timerContainerCheck.Enabled = true;

			this.gpaTab_Init ( );

			Cursor.Current = Cursors.Default;
		}

		private void timerContainerCheck_Tick ( object sender, EventArgs e )
		{
#if !DEBUG
			Process[] processes = Process.GetProcessesByName("dispatch");

			if (processes.Length < 1)
			{
				// no more Aviation.exe
				//this.Close();
				Application.Exit();
			}
#endif
		}

		private void MainForm_FormClosed ( object sender, FormClosedEventArgs e )
		{
			//		this.connection.Close();
		}

		private void productConfiguration_Enter ( object sender, EventArgs e )
		{
			this.AviationProductClassification_SetActive ( );

			this.GroundProductClassification_SetActive ( );
		}

		private void productConfiguration_Leave ( object sender, EventArgs e )
		{
			this.AviationProductClassification_KillActive ( );

			this.GroundProductClassification_KillActive ( );
		}

		private void buttonSpecialFuels_Click ( object sender, EventArgs e )
		{
			RefuelerActivity refuelerActivity = new RefuelerActivity ( RefuelerActivity.RefuelerActivityType.SpecialFuels,
																						this.yearEndDate.Value.Date,
																						this.connection,
																						this.siteGuid,
																						this.userName );

			refuelerActivity.ShowDialog ( );
		}

		private void buttonStandardFuels_Click ( object sender, EventArgs e )
		{
			RefuelerActivity refuelerActivity = new RefuelerActivity ( RefuelerActivity.RefuelerActivityType.StandardFuels,
																						this.yearEndDate.Value.Date,
																						this.connection,
																						this.siteGuid,
																						this.userName );

			refuelerActivity.ShowDialog ( );
		}

		private void ProductConfig_CellClick ( object sender, DataGridViewCellEventArgs e )
		{
			if (sender is DataGridView && ( (DataGridView) sender ).Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
			{
				( (DataGridView) sender ).BeginEdit ( true );
			}
		}

		private void MaskField_Enter ( object sender, EventArgs e )
		{
			if (sender is MaskedTextBox)
			{
				MaskedTextBox sendingBox = (MaskedTextBox) sender;
				this.BeginInvoke ( (MethodInvoker) delegate ( ) { sendingBox.Select ( 0, sendingBox.Text.Length ); } );
			}
		}
		#endregion

		#region Private Members
		string userName;
		Guid userGuid;
		Guid siteGuid;
		string siteName;
		int parentID;
		private SqlConnectionStringBuilder connectionBuilder;
		SqlConnection connection;
		SqlCommand gpahsvCommand;
		SqlDataAdapter gpahsvAdapter;
		SqlCommandBuilder gpahsvCommandBuilder;
		DataSet gpahsvDataSet;
		#endregion

		#region Private member functions
		private void combinedGroundHSV_Init ( )
		{
			if (this.gpahsvCommand == null)
			{
				// Set up the command for populating the worksheet
				this.gpahsvCommand = this.connection.CreateCommand ( );
				this.gpahsvCommand.CommandText = "SELECT ndx, DieselMax, LeadedMax, UnleadedMax, WasteFlag, JFDEFFlag,  "
														+ "DieselTrucks, LeadedTrucks, UnleadedTrucks, WasteTrucks, JFDEFTrucks, "
														+ "HSV750, HSV600, HSVDefuel, SiteIndex "
														+ "FROM ASC_C300_HSV_ALLOCATION "
														+ "WHERE SiteGuid = @SiteGuid AND DeleteFlag = 0;";
				this.gpahsvCommand.Parameters.AddWithValue ( "@SiteGuid", this.siteGuid );
			}

			//Adapter setup
			if (this.gpahsvAdapter == null)
			{
				this.gpahsvAdapter = new SqlDataAdapter ( this.gpahsvCommand );
			}
			if (this.gpahsvCommandBuilder == null)
			{
				this.gpahsvCommandBuilder = new SqlCommandBuilder ( this.gpahsvAdapter );
			}

			// Create and fill dataset
			if (this.gpahsvDataSet == null)
			{
				this.gpahsvDataSet = new DataSet ( );
				this.gpahsvAdapter.TableMappings.Add ( "Table", "ASC_C300_HSV_ALLOCATION" );
			}

		}
		#endregion

		private void close_MouseClick ( object sender, MouseEventArgs e )
		{
			this.Close ( );
		}

	}
}


