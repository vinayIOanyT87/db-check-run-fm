// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatchEvacuatePage.aspx.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the DispatchEvacuatePage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManager.DispatchWebApp
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.Xml.Serialization;
	using System.Web;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.UtilityObjects;

	using FuelsManager.FMWebApp;

	using global::FMWebApp;

	/// <summary>
	///    Partial definition of the DispatchEvacuatePage class.  Provides functionality for the
	///    Dispatch Settings Configuration web page.
	/// </summary>
	public partial class DispatchEvacuatePage : FMFormBase
	{
		#region Constants and Fields

		/// <summary>
		///    The working directory on the server.
		/// </summary>
		private string workingDirectory;

		/// <summary>
		///    The evacuate file name on the server.
		/// </summary>
		private const string EvacuateFileName = "EvacuateXML.xml";

		/// <summary>
		///    The evacuate database name on the server.
		/// </summary>
		private const string EvacuateDatabaseName = "EvacuateDB.bak";

		/// <summary>
		///    The merge file name on the server.
		/// </summary>
		private const string MergeFileName = "MergeXML.xml";

		#endregion

		#region Methods

		/// <summary>
		///    Closes the form and redirects client to previous page or FuelsManager home page.
		///    If a close button click was used to navigate to this page then the FuelsManager
		///    home page will be displayed when this page is closed.  Otherwise the previous
		///    page will be displayed.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void CloseButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// If the menu bar was used to navigate to this page then the URL of the previous
				// page will be stored in the PreviousMenuItemUrl property.  If an open button
				// click was used to navigate to this page then the URL of the previous page
				// will be stored in the CurrentMenuItemUrl property.  The navigate action is
				// only provided on open and close button clicks.  A null or empty navigate
				// action indicates the menu bar was used to navigate to this page.
				var navigateAction = this.Session["NavigateAction"] as string;
				string redirectPageUrl;
				if (string.IsNullOrEmpty(navigateAction))
				{
					redirectPageUrl = this.ucFMMenuBar.PreviousMenuItemUrl;
				}
				else if (navigateAction == "openClick")
				{
					redirectPageUrl = this.ucFMMenuBar.CurrentMenuItemUrl;
				}
				else
				{
					redirectPageUrl = FMMenuBar.FuelsManagerHomePageUrl;
				}

				this.Redirect(redirectPageUrl + "?navigateAction=closeClick");
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Downloads the evacuated XML file.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void DownloadXmlButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				this.Response.ContentType = "text/html;charset=UTF-8";
				this.Response.AppendHeader("Content-disposition", "attachment; filename=" + EvacuateFileName);
				this.Response.TransmitFile(this.workingDirectory + EvacuateFileName);
				this.Response.End();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Downloads the evacuated database backup file.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void DownloadDbButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				Response.ContentType = "application/octet-stream";
				Response.AppendHeader("Content-disposition", "attachment; filename=" + EvacuateDatabaseName);
				Response.TransmitFile(this.workingDirectory + EvacuateDatabaseName);
				Response.End();
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Performs the dispatch evacuate function.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void EvacuateButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				DateTime startTime = DateTime.Now;
				this.statusTextBox.Text = "Start Evacuate at " + startTime.ToString(CultureInfo.InvariantCulture) + "\r\n";

				// Verify that the user has permission to create the evacuate file
				string evacuateFileName = this.workingDirectory + EvacuateFileName;
				try
				{
					var evacuateFile = new FileStream(evacuateFileName, FileMode.Create, FileAccess.Write, FileShare.None);
					evacuateFile.Close();
				}
				catch (Exception)
				{
					this.statusTextBox.Text += "\r\nUnable to create evacuate file "
						+ evacuateFileName + "\r\nTerminating operation";
					return;
				}

				// Backup the dispatch database if not running in Azure
                //if (!Azure.IsRunningInAzure())
                //{
					this.BackUpDispatchDataBase(this.workingDirectory + EvacuateDatabaseName);
				//}

				// Generate the evacuate XML file
				this.GenerateEvacuateXmlFile(evacuateFileName);

				DateTime endTime = DateTime.Now;
				this.statusTextBox.Text += "\r\nEvacuate Complete at " + endTime.ToString(CultureInfo.InvariantCulture);
				TimeSpan timespan = endTime.AddMilliseconds(500) - startTime;
				this.statusTextBox.Text += "\r\n  Total Time: " + timespan.Minutes.ToString(CultureInfo.InvariantCulture)
											+ " Minutes " + timespan.Seconds.ToString(CultureInfo.InvariantCulture) + " Seconds";
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Performs the dispatch merge function.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void MergeButtonOnClick(object sender, EventArgs e)
		{
			try
			{
				// Verify that the user has specified a merge file to upload
				if (!mergeFileUpload.HasFile)
				{
					this.statusTextBox.Text = "Please press the \"Browse...\" button to specify a merge file to upload.\r\n";
					return;
				}

				DateTime startTime = DateTime.Now;
				this.statusTextBox.Text = "Start Merge at " + startTime.ToString(CultureInfo.InvariantCulture) + "\r\n";

				// Upload the specified merge file to the server
				string mergeFileName = this.workingDirectory + MergeFileName;
				mergeFileUpload.SaveAs(mergeFileName);
				this.statusTextBox.Text += "\r\n  Uploaded merge file " + mergeFileUpload.FileName + "\r\n";

				// Merge the uploaded XML merge file
				this.MergeEvacuateXmlFile(mergeFileName);

				DateTime endTime = DateTime.Now;
				this.statusTextBox.Text += "\r\nMerge Complete at " + endTime.ToString(CultureInfo.InvariantCulture);
				TimeSpan timespan;
				timespan = endTime.AddMilliseconds(500) - startTime;
				this.statusTextBox.Text += "\r\n  Total Time: " + timespan.Minutes.ToString(CultureInfo.InvariantCulture)
											+ " Minutes " + timespan.Seconds.ToString(CultureInfo.InvariantCulture) + " Seconds";
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Executes when the page is loaded.  Disables the evacuate and merge
		///    command buttons if security requirements are not satisfied.
		/// </summary>
		/// <param name="sender">The sender object</param>
		/// <param name="e">The event arguments</param>
		protected void Page_Load(object sender, EventArgs e)
		{
			try
			{
				this.GetSecurity();
				this.workingDirectory = HttpContext.Current.Server.MapPath(@"~\");

				if (!this.Page.IsPostBack)
				{
					this.Session["NavigateAction"] = this.Request.QueryString["navigateAction"];
					if (!this.Security.HasRight(RIGHT.MODIFY_DISPATCH))
					{
						this.EnableControls(false);
					}
				}
			}
			catch (Exception except)
			{
				this.ErrorHandler(except);
			}
		}

		/// <summary>
		///    Creates a backup of the consolidated Dispatch database with the specified filename
		/// </summary>
		/// <param name="backupFileName">The specified backup filename</param>
		private void BackUpDispatchDataBase(string backupFileName)
		{
			try
			{
				this.statusTextBox.Text += "\r\n  Start Backup to server file " + backupFileName;

				FMChannelHelper.MakeCall<IDatabaseBackupProcessor>(x => x.BackupConsolidatedDatabase(this.Security, backupFileName));

				this.statusTextBox.Text += "\r\n  SQL Database Backup Complete\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "\r\n  " + exception.Message;
				this.statusTextBox.Text += "\r\n  SQL Database Backup Failed\r\n";
			}
		}

		/// <summary>
		///    Enables or disables the evacuate and merge command buttons.
		/// </summary>
		/// <param name="enable">If true controls are enables otherwise they are disabled.</param>
		private void EnableControls(bool enable)
		{
			this.evacuateButton.Enabled = enable;
			this.mergeButton.Enabled = enable;
		}

		/// <summary>
		///    Gets the list of equipment records to evacuate
		/// </summary>
		/// <returns> List of equipment records to evacuate</returns>
		private EquipmentCollectionClass GetEquipmentList()
		{
			var equipmentList = new EquipmentCollectionClass();

			try
			{
				this.statusTextBox.Text += "    Getting Equipment Records\r\n";

				EquipmentCollectionClass equipmentListBySource =
					FMChannelHelper.MakeCall<IEquipments, EquipmentCollectionClass>(x => x.EnumerateBySource(this.Security));

				SiteClass site =
					FMChannelHelper.MakeCall<ISites, SiteClass>(
						x =>
						x.Get(
							this.Security,
							this.Security.SiteGuid,
							getMemberSites: false,
							bGetAssociatedAliases: false,
							getSchedulesAndProcessVariables: false));

				FMChannelHelper.MakeCall<IEquipments>(
					equipments =>
					{
						foreach (EquipmentClass equipmentBySource in equipmentListBySource)
						{
							Guid equipmentGuid = equipmentBySource.IdentityGuid;
							EquipmentClass equipment = equipments.GetBySite(this.Security, equipmentGuid, site);
							equipmentList.Add(equipment);
						}
					});

				this.statusTextBox.Text += "      " + equipmentList.Count + " records retrieved\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Getting Equipment Records Failed\r\n";
			}

			return equipmentList;
		}

		/// <summary>
		///    Gets the list of personnel records to evacuate
		/// </summary>
		/// <returns> List of personnel records to evacuate</returns>
		private PersonCollectionClass GetPersonnelList()
		{
			var personList = new PersonCollectionClass();

			try
			{
				this.statusTextBox.Text += "    Getting Personnel Records\r\n";

				FMChannelHelper.MakeCall<IPersonnel>(
					personnel =>
					{
						PersonCollectionClass personDriverList = personnel.EnumerateByRole(this.Security, PERSON_ROLE.LOADER_ROLE);

						foreach (PersonClass personDriver in personDriverList)
						{
							Guid personGuid = personDriver.IdentityGuid;
							PersonClass person = personnel.Get(this.Security, personGuid);
							personList.Add(person);
						}
					});

				this.statusTextBox.Text += "      " + personList.Count + " records retrieved\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Getting Personnel Records Failed\r\n";
			}

			return personList;
		}

		/// <summary>
		///    Gets the list of transaction records to evacuate
		/// </summary>
		/// <returns> List of transaction records to evacuate</returns>
		private List<TransactionDO> GetTransactionList()
		{
			var transactionDOList = new List<TransactionDO>();

			try
			{
				this.statusTextBox.Text += "    Getting Transaction Records\r\n";

				DateTimeOffset fromDate;
				bool validValue = DateTimeOffset.TryParse(this.fromDateInput.Value, out fromDate);
				fromDate = validValue ? TimeConverter.ToDate(fromDate) : TimeConverter.Today();

				DateTimeOffset toDate;
				validValue = DateTimeOffset.TryParse(this.toDateInput.Value, out toDate);
				toDate = validValue ? TimeConverter.ToDate(toDate) : TimeConverter.Today();

				var sr = new DispatchTransactionsSR();

				sr.Security = this.Security;
				sr.BeginDate = fromDate.Date;
				sr.EndDate = toDate.AddDays(1).Date;

				sr.Statuses.Add("Requested");
				sr.Statuses.Add("Dispatched");
				sr.Statuses.Add("Arrived");
				sr.Statuses.Add("Started");
				sr.Statuses.Add("Stopped");
				sr.Statuses.Add("Completed");

				sr.AliasNames.Add("Refuel");
				sr.AliasNames.Add("Defuel");
				sr.AliasNames.Add("Fillstand");
				sr.AliasNames.Add("Return to Bulk");
				sr.AliasNames.Add("Recirculation");

				DispatchTransactionsDO dispatchTransactionsDO =
					FMChannelHelper.MakeCall<IDispatchTransactionsProcessor, DispatchTransactionsDO>(x => x.GetLineItems(sr));

				// create the transactionDOcollection
				DataSet ds = dispatchTransactionsDO.Transactions;
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					try
					{
						var transactionSr = new TransactionSR { Security = this.Security, TransID = row["TransID"] as string };
						TransactionDO transactionDO =
							FMChannelHelper.MakeCall<ITransactionProcessor, TransactionDO>(
								transactionProcessor => transactionProcessor.Process(transactionSr));
						if (transactionDO != null)
						{
							transactionDOList.Add(transactionDO);
						}
					}
					catch (Exception)
					{
						// Don't add to transaction list if error occurs in transaction processing
					}
				}

				this.statusTextBox.Text += "      " + transactionDOList.Count + " records retrieved\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Getting Transaction Records Failed\r\n";
			}

			return transactionDOList;
		}

		/// <summary>
		///    Generates an XML file with the evacuated Equipment, Personnel, and Transaction records.
		/// </summary>
		/// <param name="evacuateFileName">The specified evacuate filename</param>
		private void GenerateEvacuateXmlFile(string evacuateFileName)
		{
			this.statusTextBox.Text += "\r\n  Start Evacuate to server file " + evacuateFileName + "\r\n";

			var evacuateInfo = new EvacuateInfo();

			evacuateInfo.EquipmentList = this.GetEquipmentList();

			evacuateInfo.PersonnelList = this.GetPersonnelList();

			evacuateInfo.TransactionList = this.GetTransactionList();

			using (Stream evacuateFile = new FileStream(
					evacuateFileName, FileMode.Create, FileAccess.Write, FileShare.None))
			{
				var xmlSerializer = new XmlSerializer(typeof(EvacuateInfo));
				xmlSerializer.Serialize(evacuateFile, evacuateInfo);
				this.statusTextBox.Text += "  Evacuate to XML file Complete\r\n";
			}
		}

		/// <summary>
		///    Reads the Equipment, Personnel, and Transaction records from a previously
		///    evacuated XML file and imports the contained records into the database.
		/// </summary>
		/// <param name="mergeFileName">The specified evacuate filename</param>
		private void MergeEvacuateXmlFile(string mergeFileName)
		{
			this.statusTextBox.Text += "\r\n  Start Merge of server file " + mergeFileName + "\r\n";

			var evacuateInfo = new EvacuateInfo();

			using (Stream stream = new FileStream(
					mergeFileName, FileMode.Open, FileAccess.Read, FileShare.None))
			{
				var xmlSerializer = new XmlSerializer(typeof(EvacuateInfo));
				evacuateInfo = (EvacuateInfo)xmlSerializer.Deserialize(stream);
				this.statusTextBox.Text += "    Merge XML file Loaded\r\n";
			}

			this.MergeEquipment(evacuateInfo.EquipmentList);

			this.MergePersonnel(evacuateInfo.PersonnelList);

			this.MergeTransactions(evacuateInfo.TransactionList);
		}

		/// <summary>
		///    Imports the specified list of equipment records into the database.
		/// </summary>
		/// <param name="equipmentList">The specified list of equipment records to merge</param>
		private void MergeEquipment(EquipmentCollectionClass equipmentList)
		{
			try
			{
				this.statusTextBox.Text += "    Merging Equipment Records\r\n";

				FMChannelHelper.MakeCall<IEquipments>(
					equipments =>
					{
						foreach (EquipmentClass equipment in equipmentList)
						{
							equipments.Import(this.Security, equipment);
						}
					});

				this.statusTextBox.Text += "      " + equipmentList.Count + " records merged\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Merging Equipment Records Failed\r\n";
			}

		}

		/// <summary>
		///    Imports the specified list of personnel records into the database.
		/// </summary>
		/// <param name="personnelList">The specified list of personnel records to merge</param>
		private void MergePersonnel(PersonCollectionClass personnelList)
		{
			try
			{
				this.statusTextBox.Text += "    Merging Personnel Records\r\n";

				FMChannelHelper.MakeCall<IPersonnel>(
					personnel =>
					{
						foreach (PersonClass person in personnelList)
						{
							// The person class adds the schedule as part of the reset method.  During deserilization
							// seven more are added.  The new schedules need to be shifted and the old ones removed.
							if (person.AccessScheduleCollection.Count == 14)
							{
								person.AccessScheduleCollection[0] = person.AccessScheduleCollection[7];
								person.AccessScheduleCollection[1] = person.AccessScheduleCollection[8];
								person.AccessScheduleCollection[2] = person.AccessScheduleCollection[9];
								person.AccessScheduleCollection[3] = person.AccessScheduleCollection[10];
								person.AccessScheduleCollection[4] = person.AccessScheduleCollection[11];
								person.AccessScheduleCollection[5] = person.AccessScheduleCollection[12];
								person.AccessScheduleCollection[6] = person.AccessScheduleCollection[13];

								// remove the bad indexes
								person.AccessScheduleCollection.RemoveAt(13);
								person.AccessScheduleCollection.RemoveAt(12);
								person.AccessScheduleCollection.RemoveAt(11);
								person.AccessScheduleCollection.RemoveAt(10);
								person.AccessScheduleCollection.RemoveAt(9);
								person.AccessScheduleCollection.RemoveAt(8);
								person.AccessScheduleCollection.RemoveAt(7);
							}

							personnel.Import(this.Security, person);
						}
					});

				this.statusTextBox.Text += "      " + personnelList.Count + " records merged\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Merging Personnel Records Failed\r\n";
			}

		}

		/// <summary>
		///    Imports the specified list of transaction records into the database.
		/// </summary>
		/// <param name="transactionList">The specified list of transaction records to merge</param>
		private void MergeTransactions(List<TransactionDO> transactionList)
		{
			try
			{
				this.statusTextBox.Text += "    Merging Transaction Records\r\n";

				FMChannelHelper.MakeCall<ITransactionImportProcessor>(
					importProcessor =>
					{
						foreach (TransactionDO transactionDO in transactionList)
						{
							var transactionImportSr = new TransactionImportSR(this.Security, transactionDO);
							importProcessor.Process(transactionImportSr);
						}
					});

				this.statusTextBox.Text += "      " + transactionList.Count + " records merged\r\n";
			}
			catch (Exception exception)
			{
				this.statusTextBox.Text += "    " + exception.Message + "\r\n    Merging Transaction Records Failed\r\n";
			}

		}

		#endregion
	}

	/// <summary>
	/// Used to specify the object type for XML serialization
	/// </summary>
	public struct EvacuateInfo
	{
		public EquipmentCollectionClass EquipmentList;
		public PersonCollectionClass PersonnelList;
		public List<TransactionDO> TransactionList;
	}
}