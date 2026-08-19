using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Data.SqlClient;

using FMBusinessObjects.DataObjects;
using FMBusinessObjects.ServiceRequests;
using FMBusinessObjects.UtilityObjects;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.BusinessInterfaces;

namespace DispatchPrototype
{
	public partial class EvacuateForm : FMBaseForm
	{
		private List<DispatchTransactionsSR.DispatchTranslationPair> Translations = null;
		public EvacuateForm(DateTime BeginDatePicker, List<DispatchTransactionsSR.DispatchTranslationPair> translations)
		{
			Translations = translations;
			InitializeComponent();
			FromDatedateTimePicker.Value = BeginDatePicker;
			ToDatedateTimePicker.Value = System.DateTime.Today;

			// ensure we are not in the future
			if (FromDatedateTimePicker.Value >= ToDatedateTimePicker.Value)
			{
				FromDatedateTimePicker.Value = ToDatedateTimePicker.Value.AddDays(-1);
			}
		}

		private void Exitbutton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void EvacuateForm_Load(object sender, EventArgs e)
		{
			GetSecurity();
			// set the initial display
			StatustextBox.Text = "Idle";
			StatustextBox.Update();
			string EvacuateDirectory = ConfigurationManager.AppSettings["EvacuateDirectory"];
			if (string.IsNullOrEmpty(EvacuateDirectory))
			{
				// set the default value of drive c root
				EvacuateDirectory = "C:\\";
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				config.AppSettings.Settings.Add("EvacuateDirectory", EvacuateDirectory);
				// Save the configuration file.
				config.Save(ConfigurationSaveMode.Modified);
				// Force a reload of a changed section.
				ConfigurationManager.RefreshSection("appSettings");
			}
			TargetSourcetextBox.Text = EvacuateDirectory;

			Evacuatebutton.Enabled = Security.HasRight(RIGHT.MODIFY_DISPATCH);
			Mergebutton.Enabled = Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}

		private void FromDatedateTimePickerValueChanged(object sender, EventArgs e)
		{
			// ensure we are not in the future
			if (FromDatedateTimePicker.Value >= ToDatedateTimePicker.Value)
				FromDatedateTimePicker.Value = ToDatedateTimePicker.Value.AddDays(-1);
		}

		private void Browsebutton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog openFileDialog1 = new FolderBrowserDialog();

			openFileDialog1.SelectedPath = TargetSourcetextBox.Text;
			openFileDialog1.ShowNewFolderButton = false;	// do not allow creation of a new folder

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				TargetSourcetextBox.Text = openFileDialog1.SelectedPath;
				// store the selection
				System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

				string EvacuateDirectory = ConfigurationManager.AppSettings["EvacuateDirectory"];
				if (!string.IsNullOrEmpty(EvacuateDirectory))
					config.AppSettings.Settings.Remove("EvacuateDirectory");

				config.AppSettings.Settings.Add("EvacuateDirectory", TargetSourcetextBox.Text);
				// Save the configuration file.
				config.Save(ConfigurationSaveMode.Modified);
				// Force a reload of a changed section.
				ConfigurationManager.RefreshSection("appSettings");
			}
		}

		private void Evacuatebutton_Click(object sender, EventArgs e)
		{
			// store the files to the selected directory
			if (!Directory.Exists(TargetSourcetextBox.Text))
			{
				MessageBox.Show("Selected Directory Does Not Exist.");
				return;
			}
			// set the wait cursor
			Cursor = System.Windows.Forms.Cursors.WaitCursor;
			DateTime StartTime = System.DateTime.Now;
			StatustextBox.Clear();
			StatustextBox.Text = "Start Evacuate at " + StartTime.ToString();
			StatustextBox.Update();
			// the first step is to backup the sql database
			if (!BackUpSQLDataBase())
			{
				Cursor = System.Windows.Forms.Cursors.Default;
				return;
			}
			// now generate the xml files
			if (!GenerateXMLFiles())
			{
				Cursor = System.Windows.Forms.Cursors.Default;
				return;
			}

			DateTime EndTime = System.DateTime.Now;
			StatustextBox.Text += "\r\nEvacuate Complete at " + EndTime.ToString();
			TimeSpan timespan = new TimeSpan();
			timespan = EndTime - StartTime;
			StatustextBox.Text += "\r\nTotal Time " + timespan.Minutes.ToString() + " Minutes " + timespan.Seconds.ToString() + " Seconds";
			StatustextBox.Update();
			Cursor = System.Windows.Forms.Cursors.Default;
		}

		private bool BackUpSQLDataBase()
		{
			SqlConnectionStringBuilder dependencyConnectionString = null;
			SqlConnection Connection = null;
			DateTime CurrentDateTime = System.DateTime.Now;

			try
			{
				StatustextBox.Text += "\r\nBacking Up SQL Database";
				StatustextBox.Update();
				// connect to the sql server instance

				SecurityClass security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

				if (security == null)
					throw new Exception("Security not in AppDomain");

				// Get connection string
				//dependencyConnectionString = new SqlConnectionStringBuilder ( DispatchDataAccess.ConnectString );

				// use special security for BSME only (IGO 2010-Aug-12)
				FMChannelFactory<IHardwareKey> hardwareKeyClient = new FMChannelFactory<IHardwareKey>();
				IHardwareKey HardwareKey = hardwareKeyClient.CreateProxy();

				if (true == HardwareKey.IsDescKey())
				{
					dependencyConnectionString.IntegratedSecurity = false;
					dependencyConnectionString.UserID = security.UserID;

					FMChannelFactory<IDBAccess> dbAccessClient = new FMChannelFactory<IDBAccess>();
					IDBAccess dbAccess = dbAccessClient.CreateProxy();

					dependencyConnectionString.Password = dbAccess.GetDBPassword(security.Password);
					dependencyConnectionString.ApplicationName = "Dispatch";
				}

				Connection = new SqlConnection(dependencyConnectionString.ConnectionString);

				Connection.Open();

				Connection.ChangeDatabase("ConsolidatedDB");

				string BackupFileName = "Evacuate_" + CurrentDateTime.Month.ToString() + "_" + CurrentDateTime.Day.ToString() + "_" + CurrentDateTime.Year.ToString();
				BackupFileName += "_" + CurrentDateTime.Hour.ToString() + "+" + CurrentDateTime.Minute.ToString() + "+" + CurrentDateTime.Second.ToString() + ".bak";

				StatustextBox.Text += "\r\nCreating File " + BackupFileName;
				StatustextBox.Update();

				SqlCommand command;

				command = new SqlCommand(@"backup database ConsolidatedDB to disk ='" + TargetSourcetextBox.Text + "\\" + BackupFileName + "' with init,stats=10", Connection);

				command.ExecuteNonQuery();

				Connection.Close();

				StatustextBox.Text += "\r\nSQL Database Backup Complete";
				StatustextBox.Update();
				return true;
			}
			catch (Exception exception)
			{
				if (Connection.State == ConnectionState.Open)
					Connection.Close();
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nSQL Database Backup Failed";
				StatustextBox.Update();
				return false;
			}
		}

		private bool GenerateXMLFiles()
		{
			DispatchDataAccess DataAccess = new DispatchDataAccess(Security);

			if (!GenerateEquipmentXML(DataAccess))
				return false;
			if (!GeneratePersonnelXML(DataAccess))
				return false;
			if (!GenerateTransactionsXML(DataAccess))
				return false;

			return true;
		}

		private bool GenerateEquipmentXML(DispatchDataAccess DataAccess)
		{
			try
			{
				StatustextBox.Text += "\r\nGenerating Equipment.xml";
				StatustextBox.Update();
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<EquipmentClass>));

				List<EquipmentClass> EquipmentCollection = new List<EquipmentClass>();
				List<EquipmentClass> EquipmentCollection1 = new List<EquipmentClass>();
				EquipmentCollection = DataAccess.GetEquipmentNoUpdateConnection(Security);

				FMChannelFactory<IEquipments> equipmentsClient = new FMChannelFactory<IEquipments>();
				IEquipments Equipments = equipmentsClient.CreateProxy();

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
				ISites sites = sitesClient.CreateProxy();

				SiteClass site = sites.Get(Security, Security.SiteGuid, getMemberSites: false, getSchedulesAndProcessVariables: false, bGetAssociatedAliases: false);

				foreach (EquipmentClass Equipment in EquipmentCollection)
				{
					EquipmentClass Equipment1 = Equipments.GetBySite(Security, Equipment.IdentityGuid, site);
					EquipmentCollection1.Add(Equipment1);
				}

				XmlDocument xmlDocument = new XmlDocument();

				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Equipment.xml", FileMode.Create, FileAccess.Write, FileShare.None);

				xmlSerializer.Serialize(stream, EquipmentCollection1);

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nGenerating Equipment.xml Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nGenerating Equipment.xml Complete";
			StatustextBox.Update();
			return true;
		}

		private bool GeneratePersonnelXML(DispatchDataAccess DataAccess)
		{
			try
			{
				StatustextBox.Text += "\r\nGenerating Personnel.xml";
				StatustextBox.Update();
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PersonCollectionClass));

				PersonCollectionClass PersonCollection = new PersonCollectionClass();
				PersonCollectionClass PersonCollection1 = new PersonCollectionClass();
				PersonCollection = DataAccess.GetPersonnelNoUpdateConnection(Security);

				FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel>();
				IPersonnel Personnel = personnelClient.CreateProxy();

				foreach (PersonClass Person in PersonCollection)
				{
					PersonClass Person1 = Personnel.Get(Security, Person.IdentityGuid);
					PersonCollection1.Add(Person1);
				}

				XmlDocument xmlDocument = new XmlDocument();

				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Personnel.xml", FileMode.Create, FileAccess.Write, FileShare.None);

				xmlSerializer.Serialize(stream, PersonCollection1);

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nGenerating Personnel.xml Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nGenerating Personnel.xml Complete";
			StatustextBox.Update();
			return true;
		}

		private bool GenerateTransactionsXML(DispatchDataAccess DataAccess)
		{
			try
			{
				StatustextBox.Text += "\r\nGenerating Transactions.xml";
				StatustextBox.Update();
				DateTime StartDateTime = FromDatedateTimePicker.Value;
				DateTime EndDateTime = ToDatedateTimePicker.Value;

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransactionDOCollectionClass));

				TransactionDOCollectionClass TransactionDOCollection = new TransactionDOCollectionClass();
				DispatchTransactionsDO Transactions = new DispatchTransactionsDO();
				DispatchTransactionsSR sr = new DispatchTransactionsSR();

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
				ISites sites = sitesClient.CreateProxy();

				SiteClass site = sites.Get(Security, Security.SiteGuid, false, false, false);
				SiteTimeConverter timeConverter = new SiteTimeConverter(site);
				sr.Security = Security;
				sr.Translations = Translations;
				sr.BeginDate = StartDateTime;
				sr.EndDate = EndDateTime.AddDays(1);

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

				Transactions = DataAccess.GetTransactionsNoUpdateConnection(sr);
				DataSet ds = (DataSet)Transactions.Transactions;

				// create the transactionDOcollection
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					string TransID = row["TransID"] as string;

					TransactionDO transactionDO = GetTransaction(TransID);

					TransactionDOCollection.Add(transactionDO);
				}

				XmlDocument xmlDocument = new XmlDocument();

				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Transactions.xml", FileMode.Create, FileAccess.Write, FileShare.None);

				xmlSerializer.Serialize(stream, TransactionDOCollection);

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nGenerating Transactions.xml Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nGenerating Transactions.xml Complete";
			StatustextBox.Update();
			return true;
		}

		private void RemoveAttributes(XmlNode node)
		{
			if (node.Attributes != null)
				node.Attributes.RemoveAll();

			if (node.HasChildNodes)
			{
				XmlElement element = node as XmlElement;
				foreach (XmlNode childNode in node.ChildNodes)
					RemoveAttributes(childNode);
			}
		}

		private void Mergebutton_Click(object sender, EventArgs e)
		{
			if (!Directory.Exists(TargetSourcetextBox.Text))
			{
				MessageBox.Show("Selected Directory Does Not Exist.");
				return;
			}
			// read the xml files from the directory and restore them to the database
			DateTime StartTime = System.DateTime.Now;
			StatustextBox.Clear();
			StatustextBox.Text = "Start Merge at " + StartTime.ToString();
			StatustextBox.Update();
			// set the wait cursor
			Cursor = System.Windows.Forms.Cursors.WaitCursor;

			if (!MergeEquipment())
			{
				Cursor = System.Windows.Forms.Cursors.Default;
				return;
			}
			if (!MergePersonnel())
			{
				Cursor = System.Windows.Forms.Cursors.Default;
				return;
			}
			if (!MergeTransactions())
			{
				Cursor = System.Windows.Forms.Cursors.Default;
				return;
			}

			DateTime EndTime = System.DateTime.Now;
			StatustextBox.Text += "\r\nMerge Complete at " + EndTime.ToString();
			TimeSpan timespan = new TimeSpan();
			timespan = EndTime - StartTime;
			StatustextBox.Text += "\r\nTotal Time " + timespan.Minutes.ToString() + " Minutes " + timespan.Seconds.ToString() + " Seconds";
			StatustextBox.Update();
			Cursor = System.Windows.Forms.Cursors.Default;
		}

		private bool MergeEquipment()
		{
			StatustextBox.Text += "\r\nMerging Equipment";
			StatustextBox.Update();
			try
			{
				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Equipment.xml", FileMode.Open, FileAccess.Read, FileShare.None);
				StatustextBox.Text += "\r\nUpdateing Equipment";
				StatustextBox.Text += "\r\n";
				StatustextBox.Update();
				List<EquipmentClass> EquipmentCollection = new List<EquipmentClass>();
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<EquipmentClass>));

				EquipmentCollection = (List<EquipmentClass>)xmlSerializer.Deserialize(stream);

				FMChannelFactory<IEquipments> equipmentsClient = new FMChannelFactory<IEquipments>();
				IEquipments Equipments = equipmentsClient.CreateProxy();

				foreach (EquipmentClass Equipment in EquipmentCollection)
				{
					Equipments.Import(Security, Equipment);
					StatustextBox.Text += ".";
					StatustextBox.Update();
				}
				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nMerging Equipment Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nMerging Equipment Complete";
			StatustextBox.Update();
			return true;
		}

		private bool MergePersonnel()
		{
			StatustextBox.Text += "\r\nMerging Personnel";
			StatustextBox.Update();
			string PersonName = string.Empty;

			try
			{
				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Personnel.xml", FileMode.Open, FileAccess.Read, FileShare.None);
				StatustextBox.Text += "\r\nUpdateing Personnel";
				StatustextBox.Text += "\r\n";
				StatustextBox.Update();
				PersonCollectionClass PersonCollection = new PersonCollectionClass();

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(PersonCollectionClass));

				PersonCollection = (PersonCollectionClass)xmlSerializer.Deserialize(stream);
				FMChannelFactory<IPersonnel> personnelClient = new FMChannelFactory<IPersonnel>();
				IPersonnel Personnel = personnelClient.CreateProxy();

				foreach (PersonClass Person in PersonCollection)
				{
					// the person class adds the schedule as part of the reset method. During deserilization seven more are added
					// we need to remap these and get rid of the old ones
					if (Person.AccessScheduleCollection.Count == 14)
					{
						Person.AccessScheduleCollection[0] = Person.AccessScheduleCollection[7];
						Person.AccessScheduleCollection[1] = Person.AccessScheduleCollection[8];
						Person.AccessScheduleCollection[2] = Person.AccessScheduleCollection[9];
						Person.AccessScheduleCollection[3] = Person.AccessScheduleCollection[10];
						Person.AccessScheduleCollection[4] = Person.AccessScheduleCollection[11];
						Person.AccessScheduleCollection[5] = Person.AccessScheduleCollection[12];
						Person.AccessScheduleCollection[6] = Person.AccessScheduleCollection[13];
						// remove the bad indexes
						Person.AccessScheduleCollection.RemoveAt(13);
						Person.AccessScheduleCollection.RemoveAt(12);
						Person.AccessScheduleCollection.RemoveAt(11);
						Person.AccessScheduleCollection.RemoveAt(10);
						Person.AccessScheduleCollection.RemoveAt(9);
						Person.AccessScheduleCollection.RemoveAt(8);
						Person.AccessScheduleCollection.RemoveAt(7);
					}
					Personnel.Import(Security, Person);
					StatustextBox.Text += ".";
					StatustextBox.Update();
				}
				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message + PersonName, this.Text);
				StatustextBox.Text += "\r\nMerging Personnel Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nMerging Personnel Complete";
			StatustextBox.Update();
			return true;
		}

		private bool MergeTransactions()
		{
			StatustextBox.Text += "\r\nMerging Transactions";
			StatustextBox.Update();
			try
			{
				Stream stream = new FileStream(TargetSourcetextBox.Text + "\\Transactions.xml", FileMode.Open, FileAccess.Read, FileShare.None);
				TransactionDOCollectionClass TransactionDOCollection = new TransactionDOCollectionClass();

				XmlSerializer xmlSerializer = new XmlSerializer(typeof(TransactionDOCollectionClass));

				TransactionDOCollection = (TransactionDOCollectionClass)xmlSerializer.Deserialize(stream);

				FMChannelFactory<ISites> sitesClient = new FMChannelFactory<ISites>();
				ISites sites = sitesClient.CreateProxy();

				StatustextBox.Text += "\r\nUpdateing Transaction";
				StatustextBox.Text += "\r\n";
				StatustextBox.Update();
				foreach (TransactionDO transactionDO in TransactionDOCollection)
				{
					TransactionImportSR transactionimportSR = new TransactionImportSR(Security, transactionDO);

					FMChannelFactory<ITransactionImportProcessor> transactionImportProcessorClient = new FMChannelFactory<ITransactionImportProcessor>();
					ITransactionImportProcessor importProc = transactionImportProcessorClient.CreateProxy();

					importProc.Process(transactionimportSR);

					StatustextBox.Text += ".";
					StatustextBox.Update();
				}
				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nMerging Personnel Failed";
				StatustextBox.Update();
				return false;
			}
			StatustextBox.Text += "\r\nMerging Personnel Complete";
			StatustextBox.Update();
			return true;
		}
	}
}
