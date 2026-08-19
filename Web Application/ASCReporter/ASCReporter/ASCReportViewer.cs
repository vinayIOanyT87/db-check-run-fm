using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
//using CrystalDecisions.CrystalReports.Engine;
//using CrystalDecisions.Shared;
using Microsoft.Win32;

namespace ASCReporter
{
	/// <summary>
	/// Dialog class for displaying the actual reports.
	/// </summary>
   public partial class ASCReportViewer 
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ASCReportViewer()
		{
		//	InitializeComponent();
		}

		/// <summary>
		/// Loads refueling capability report into form and sets parameters
		/// </summary>
		/// <param name="siteName">Name of the site as it will be displayed on the report</param>
		/// <param name="userName">username to use to connect to database (not used for this report)</param>
		/// <param name="parameterList">
		/// Sorted dictionary of string-object pairs containing parameters to be passed to the report.  Required paramteters are:
		/// PeactimeDayLength : int
		/// LeavesRCC : float
		/// PositionsUnit : float
		/// PreparesUnit : float
		/// CompletesServicing : float
		/// TravelsNext : float
		/// TravelsStorage : float
		/// WaitsRefill : float
		/// </param>
		public string ShowRefuelingCapabilityReport(string siteName, string userName, SortedDictionary<string, Object> parameterList)
		{
         
		//	ReportDocument rpt = new ReportDocument();
			string rptFile;
			rptFile = reportDirectory + @"Refueling Unit Capability Report .rpt";
         return rptFile;
/*
			try
			{
				rpt.Load(rptFile);
				rpt.SetParameterValue("PeacetimeDayLength", (int)parameterList["PeacetimeDayLength"]);
				rpt.SetParameterValue("LeavesRCC", (float)parameterList["LeavesRCC"]);
				rpt.SetParameterValue("PositionsUnit", (float)parameterList["PositionsUnit"]);
				rpt.SetParameterValue("PreparesUnit", (float)parameterList["PreparesUnit"]);
				rpt.SetParameterValue("CompletesServicing", (float)parameterList["CompletesServicing"]);
				rpt.SetParameterValue("TravelsNext", (float)parameterList["TravelsNext"]);
				rpt.SetParameterValue("TravelsStorage", (float)parameterList["TravelsStorage"]);
				rpt.SetParameterValue("WaitsRefill", (float)parameterList["WaitsRefill"]);
				rpt.SetParameterValue("SiteName", siteName);

				this.crystalReportViewer1.ReportSource = rpt;
			}
			catch (Exception e)
			{
				e.ToString();
				MessageBox.Show(ErrorMessages.UnableToLoadReport);
				this.crystalReportViewer1.ReportSource = null;
			}*/
		}

		/// <summary>
		/// Loads refueling capability report into form and sets parameters
		/// </summary>
		/// <param name="siteName">Name of the site as it will be displayed on the report</param>
		/// <param name="userName">username to use to connect to database (not used for this report)</param>
		/// <param name="parameterList">
		/// Sorted dictionary of string-object pairs containing parameters to be passed to the report.  Required paramteters are:
		/// DieselGallon : float
		/// MogasLeadedGallon : float
		/// MogasUnleadedGallon : float
		/// WasteFlag : bool
		/// JFDEFFlag : bool
		/// CBOIA : int
		/// CBOIB : int
		/// CBOIC : int
		/// CBOID : int
		/// CBOIUTC : int
		/// RBOIA : int
		/// RBOIB : int
		/// RBOIC : int
		/// </param>
		public string ShowC300HSVReport(string siteName, string userName, SortedDictionary<string, Object> parameterList)
		{
         
			//ReportDocument rpt = new ReportDocument();
			string rptFile;
			rptFile = reportDirectory + @"C300 And R12 Report.rpt";
         return rptFile;
/*
			try{
			rpt.Load(rptFile);
			rpt.SetParameterValue("DieselGallon", (float)parameterList["DieselGallon"]);
			rpt.SetParameterValue("MogasLeadedGallon", (float)parameterList["MogasLeadedGallon"]);
			rpt.SetParameterValue("MogasUnleadedGallon", (float)parameterList["MogasUnleadedGallon"]);
			rpt.SetParameterValue("WasteFlag", (bool)parameterList["WasteFlag"]);
			rpt.SetParameterValue("JFDEFFlag", (bool)parameterList["JFDEFFlag"]);
			rpt.SetParameterValue("CBOIA", (int)parameterList["CBOIA"]);
			rpt.SetParameterValue("CBOIB", (int)parameterList["CBOIB"]);
			rpt.SetParameterValue("CBOIC", (int)parameterList["CBOIC"]);
			rpt.SetParameterValue("CBOID", (int)parameterList["CBOID"]);
			rpt.SetParameterValue("CBOIUTC", (int)parameterList["CBOIUTC"]);
			rpt.SetParameterValue("RBOIA", (int)parameterList["RBOIA"]);
			rpt.SetParameterValue("RBOIB", (int)parameterList["RBOIB"]);
			rpt.SetParameterValue("RBOIC", (int)parameterList["RBOIC"]);
			rpt.SetParameterValue("SiteName", siteName);

			this.crystalReportViewer1.ReportSource = rpt;
            
		}
		catch (Exception e)
		{
			e.ToString();
			MessageBox.Show(ErrorMessages.UnableToLoadReport);
			this.crystalReportViewer1.ReportSource = null;
		}*/
	}

		/// <summary>
		/// Loads refueling capability report into form and sets parameters
		/// </summary>
		/// <param name="siteName">Name of the site as it will be displayed on the report</param>
		/// <param name="userName">username to use to connect to database</param>
		/// <param name="parameterList">
		/// Sorted dictionary of string-object pairs containing parameters to be passed to the report.  Required paramteters are:
		/// PeactimeDayLength : int
		/// LeavesRCC : float
		/// PositionsUnit : float
		/// PreparesUnit : float
		/// CompletesServicing : float
		/// TravelsNext : float
		/// TravelsStorage : float
		/// WaitsRefill : float
		/// </param>
		public string ShowRefuelingAuthorizationReport(string siteName, string userName, SortedDictionary<string, Object> parameterList)
		{
			//ReportDocument rpt = new ReportDocument();
			string rptFile;
			rptFile = reportDirectory + @"Jet Fuel Refueling Unit Authorization Report.rpt";
         return rptFile;
         /*
                  try{
                  rpt.Load(rptFile);
                  rpt.SetDatabaseLogon(userName, DBPasswordGenerator.getDBPassword(userName));
                  rpt.SetParameterValue("PeacetimeDayLength", (int)parameterList["PeacetimeDayLength"]);
                  rpt.SetParameterValue("LeavesRCC", (float)parameterList["LeavesRCC"]);
                  rpt.SetParameterValue("PositionsUnit", (float)parameterList["PositionsUnit"]);
                  rpt.SetParameterValue("PreparesUnit", (float)parameterList["PreparesUnit"]);
                  rpt.SetParameterValue("CompletesServicing", (float)parameterList["CompletesServicing"]);
                  rpt.SetParameterValue("TravelsNext", (float)parameterList["TravelsNext"]);
                  rpt.SetParameterValue("TravelsStorage", (float)parameterList["TravelsStorage"]);
                  rpt.SetParameterValue("WaitsRefill", (float)parameterList["WaitsRefill"]);
                  rpt.SetParameterValue("SiteName", siteName);

                  this.crystalReportViewer1.ReportSource = rpt;
               }
               catch (Exception e)
               {
                  e.ToString();
                  MessageBox.Show(ErrorMessages.UnableToLoadReport);
                  this.crystalReportViewer1.ReportSource = null;
               }*/
	}

		/// <summary>
		/// Loads refueling capability report into form and sets parameters
		/// </summary>
		/// <param name="siteName">Name of the site as it will be displayed on the report</param>
		/// <param name="userName">username to use to connect to database (not used for this report)</param>
		/// <param name="parameterList">
		/// Sorted dictionary of string-object pairs containing parameters to be passed to the report.  Required paramteters are:
		/// &lt;none&gt;
		/// </param>
		public string ShowPeakDaysReport(string siteName, string userName, SortedDictionary<string, Object> parameterList)
		{
			//ReportDocument rpt = new ReportDocument();
			string rptFile;
			rptFile = reportDirectory + @"Fueling Peak Days Report.rpt";
         return rptFile;

		/*	try{
			rpt.Load(rptFile);
			rpt.SetDatabaseLogon(userName, DBPasswordGenerator.getDBPassword(userName));

			rpt.SetParameterValue("SiteName", siteName);

			this.crystalReportViewer1.ReportSource = rpt;
		}
		catch (Exception e)
		{
			e.ToString();
			MessageBox.Show(ErrorMessages.UnableToLoadReport);
			this.crystalReportViewer1.ReportSource = null;
		}*/
	}

		private string reportDirectory
		{
			get
			{
            object regValue = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Endress + Hauser\Control Center",
														@"Reports Directory",
														@"C:\Program Files\FuelsManager\Reports");

				string rptFile = @"C:\Program Files\FuelsManager\Reports";
            if (regValue != null)
            {
               rptFile = regValue.ToString();
            }

				if (!rptFile.EndsWith(@"\"))
				{
					rptFile += @"\";
				}
				return rptFile;
			}
		}



	}
}