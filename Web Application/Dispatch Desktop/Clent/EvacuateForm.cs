namespace Dispatch
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml.Serialization;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;

	using FMDispatchBusinessObjects.BusinessInterfaces;
	using FMDispatchBusinessObjects.ChannelFactories;

	public partial class EvacuateForm : FMBaseForm
	{
		private readonly List<DispatchTransactionsSR.DispatchTranslationPair> translations;

		public EvacuateForm(DateTime beginDatePicker, List<DispatchTransactionsSR.DispatchTranslationPair> translations)
		{
			this.translations = translations;
			this.InitializeComponent();
			this.FromDatedateTimePicker.Value = beginDatePicker;
			this.ToDatedateTimePicker.Value = DateTime.Today;

			// ensure we are not in the future
			if (this.FromDatedateTimePicker.Value >= this.ToDatedateTimePicker.Value)
			{
				this.FromDatedateTimePicker.Value = this.ToDatedateTimePicker.Value.AddDays(-1);
			}
		}

		private void ExitbuttonClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
		}

		private void EvacuateFormLoad(object sender, EventArgs e)
		{
			this.GetSecurity();

			// set the initial display
			this.StatustextBox.Text = "Idle";
			this.StatustextBox.Update();
			string evacuateDirectory = ConfigurationManager.AppSettings["EvacuateDirectory"];

			if (string.IsNullOrEmpty(evacuateDirectory))
			{
				// set the default value of drive c root
				evacuateDirectory = "C:\\";
				Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
				config.AppSettings.Settings.Add("EvacuateDirectory", evacuateDirectory);

				// Save the configuration file.
				config.Save(ConfigurationSaveMode.Modified);

				// Force a reload of a changed section.
				ConfigurationManager.RefreshSection("appSettings");
			}

			this.TargetSourcetextBox.Text = evacuateDirectory;
			this.Evacuatebutton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
			this.Mergebutton.Enabled = this.Security.HasRight(RIGHT.MODIFY_DISPATCH);
		}

		private void FromDatedateTimePickerValueChanged(object sender, EventArgs e)
		{
			// ensure we are not in the future
			if (this.FromDatedateTimePicker.Value >= this.ToDatedateTimePicker.Value)
			{
				this.FromDatedateTimePicker.Value = this.ToDatedateTimePicker.Value.AddDays(-1);
			}
		}

		private void BrowsebuttonClick(object sender, EventArgs e)
		{
			var openFileDialog1 = new FolderBrowserDialog
			                      {
				                      SelectedPath = this.TargetSourcetextBox.Text,
				                      ShowNewFolderButton = false
			                      };

			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				this.TargetSourcetextBox.Text = openFileDialog1.SelectedPath;

				// store the selection
				Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

				string evacuateDirectory = ConfigurationManager.AppSettings["EvacuateDirectory"];

				if (!string.IsNullOrEmpty(evacuateDirectory))
				{
					config.AppSettings.Settings.Remove("EvacuateDirectory");
				}

				config.AppSettings.Settings.Add("EvacuateDirectory", this.TargetSourcetextBox.Text);

				// Save the configuration file.
				config.Save(ConfigurationSaveMode.Modified);

				// Force a reload of a changed section.
				ConfigurationManager.RefreshSection("appSettings");
			}
		}

		private void EvacuatebuttonClick(object sender, EventArgs e)
		{
			// store the files to the selected directory
			if (!Directory.Exists(this.TargetSourcetextBox.Text))
			{
				MessageBox.Show("Selected Directory Does Not Exist.");
				return;
			}

			// set the wait cursor
			this.Cursor = Cursors.WaitCursor;
			DateTime startTime = DateTime.Now;
			this.StatustextBox.Clear();
			this.StatustextBox.Text = "Start Evacuate at " + startTime;
			this.StatustextBox.Update();

			// now generate the xml files
			if (!this.GenerateXMLFiles())
			{
				this.Cursor = Cursors.Default;
				return;
			}

			// the next step is to backup the sql database
			if (!this.BackUpSQLDataBase())
			{
				this.Cursor = Cursors.Default;
				return;
			}

			DateTime endTime = DateTime.Now;
			this.StatustextBox.Text += "\r\nEvacuate Complete at " + endTime;
			TimeSpan timespan = endTime - startTime;
			this.StatustextBox.Text += "\r\nTotal Time " + timespan.Minutes + " Minutes " + timespan.Seconds + " Seconds";

			this.StatustextBox.Update();
			this.Cursor = Cursors.Default;
		}

		private bool BackUpSQLDataBase()
		{
			/*
			SqlConnectionStringBuilder dependencyConnectionString = null;
			SqlConnection Connection = null;
			DateTime CurrentDateTime = System.DateTime.Now;

			try
			{
				StatustextBox.Text += "\r\nBacking Up SQL Database";
				StatustextBox.Update();

				SecurityClass security = AppDomain.CurrentDomain.GetData("Security") as SecurityClass;

				if (security == null)
				{
					throw new Exception("Security not in AppDomain");
				}

				dependencyConnectionString = new SqlConnectionStringBuilder(DispatchDataAccess.ConnectString);

				// use special security for BSME only (IGO 2010-Aug-12)
				var isDescKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsDescKey());
				if (isDescKey)
				{
					dependencyConnectionString.IntegratedSecurity = false;
					dependencyConnectionString.UserID = security.UserID;
					dependencyConnectionString.Password = DBAccess.GetDBPassword(security.Password);
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
				command.CommandTimeout = 300;
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

				SqlException ex = exception as SqlException;

				// this is to check for success of back up because if dispatch users do not have permissions to write a record to msdb
				// and in this case we still succeeded in creating the backup
				if (ex != null)
				{
					foreach(SqlError err in ex.Errors)
					{
						if (err.Number == 3009)
						{
							StatustextBox.Text += "\r\nSQL Database Backup Complete";
							StatustextBox.Update();
							//could not write to msdb, its ok
							return true;
						}
					}
				}

				MessageBox.Show(this, exception.Message, this.Text);
				StatustextBox.Text += "\r\nSQL Database Backup Failed";
				StatustextBox.Update();
				return false;
			}
			 */
			return false;
		}

		private bool GenerateXMLFiles()
		{
			var dataAccess = new DispatchDataAccess(this.Security);

			if (!this.GenerateEquipmentXML(dataAccess))
			{
				return false;
			}

			if (!this.GeneratePersonnelXML(dataAccess))
			{
				return false;
			}

			if (!this.GenerateTransactionsXML(dataAccess))
			{
				return false;
			}

			return true;
		}

		private bool GenerateEquipmentXML(DispatchDataAccess dataAccess)
		{
			try
			{
				this.StatustextBox.Text += "\r\nGenerating Equipment.xml";
				this.StatustextBox.Update();
				var xmlSerializer = new XmlSerializer(typeof(EquipmentCollectionClass));

				var equipmentCollection1 = new EquipmentCollectionClass();
				EquipmentCollectionClass equipmentCollection = dataAccess.GetEquipmentNoUpdateConnection();

				FMChannelHelper.MakeCall<IClientDispatchService>(
					x =>
					{
						foreach (var equipment in equipmentCollection)
						{
							var equipment1 = x.GetEquipment(this.Security, equipment.IdentityGuid);
							equipmentCollection1.Add(equipment1);
						}
					});

				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Equipment.xml", 
												FileMode.Create, 
												FileAccess.Write, 
												FileShare.None);

				xmlSerializer.Serialize(stream, equipmentCollection1);
				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				this.StatustextBox.Text += "\r\nGenerating Equipment.xml Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nGenerating Equipment.xml Complete";
			this.StatustextBox.Update();
			return true;
		}

		private bool GeneratePersonnelXML(DispatchDataAccess dataAccess)
		{
			try
			{
				this.StatustextBox.Text += "\r\nGenerating Personnel.xml";
				this.StatustextBox.Update();
				var xmlSerializer = new XmlSerializer(typeof(PersonCollectionClass));

				var personCollection1 = new PersonCollectionClass();
				PersonCollectionClass personCollection = dataAccess.GetPersonnelNoUpdateConnection();

				FMChannelHelper.MakeCall<IClientDispatchService>(
					personnel =>
					{
						foreach (PersonClass person in personCollection)
						{
							PersonClass person1 = personnel.GetPerson(this.Security, person.IdentityGuid);
							personCollection1.Add(person1);
						}
					});

				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Personnel.xml", 
												FileMode.Create, 
												FileAccess.Write, 
												FileShare.None);

				xmlSerializer.Serialize(stream, personCollection1);

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				this.StatustextBox.Text += "\r\nGenerating Personnel.xml Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nGenerating Personnel.xml Complete";
			this.StatustextBox.Update();
			return true;
		}

		private bool GenerateTransactionsXML(DispatchDataAccess dataAccess)
		{
			try
			{
				this.StatustextBox.Text += "\r\nGenerating Transactions.xml";
				this.StatustextBox.Update();
				DateTime startDateTime = this.FromDatedateTimePicker.Value;
				DateTime endDateTime = this.ToDatedateTimePicker.Value;

				var xmlSerializer = new XmlSerializer(typeof(TransactionDOCollectionClass));

				var transactionDOCollection = new TransactionDOCollectionClass();
				var sr = new DispatchTransactionsSR
				         {
					         Security = this.Security,
					         Translations = this.translations,
					         BeginDate = startDateTime,
					         EndDate = endDateTime.AddDays(1)
				         };

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

				DispatchTransactionsDO transactions = dataAccess.GetTransactionsNoUpdateConnection(sr);
				DataSet ds = transactions.Transactions;

				// create the transactionDOcollection
				foreach (DataRow row in ds.Tables[0].Rows)
				{
					var transID = row[0] as string;

					TransactionDO transactionDO = this.GetTransaction(transID);
					transactionDOCollection.Add(transactionDO);
				}

				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Transactions.xml", 
												FileMode.Create, 
												FileAccess.Write, 
												FileShare.None);

				xmlSerializer.Serialize(stream, transactionDOCollection);
				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				this.StatustextBox.Text += "\r\nGenerating Transactions.xml Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nGenerating Transactions.xml Complete";
			this.StatustextBox.Update();
			return true;
		}

		private void MergebuttonClick(object sender, EventArgs e)
		{
			if (!Directory.Exists(this.TargetSourcetextBox.Text))
			{
				MessageBox.Show("Selected Directory Does Not Exist.");
				return;
			}

			// read the xml files from the directory and restore them to the database
			DateTime startTime = DateTime.Now;
			this.StatustextBox.Clear();
			this.StatustextBox.Text = "Start Merge at " + startTime;
			this.StatustextBox.Update();

			// set the wait cursor
			this.Cursor = Cursors.WaitCursor;

			if (!this.MergeEquipment())
			{
				this.Cursor = Cursors.Default;
				return;
			}
			if (!this.MergePersonnel())
			{
				this.Cursor = Cursors.Default;
				return;
			}
			if (!this.MergeTransactions())
			{
				this.Cursor = Cursors.Default;
				return;
			}

			DateTime endTime = DateTime.Now;
			this.StatustextBox.Text += "\r\nMerge Complete at " + endTime;
			TimeSpan timespan = endTime - startTime;

			this.StatustextBox.Text += "\r\nTotal Time " + timespan.Minutes + " Minutes " + timespan.Seconds + " Seconds";
			this.StatustextBox.Update();
			this.Cursor = Cursors.Default;
		}

		private bool MergeEquipment()
		{
			this.StatustextBox.Text += "\r\nMerging Equipment";
			this.StatustextBox.Update();
			try
			{
				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Equipment.xml", FileMode.Open, FileAccess.Read, FileShare.None);
				this.StatustextBox.Text += "\r\nUpdateing Equipment";
				this.StatustextBox.Text += "\r\n";
				this.StatustextBox.Update();
				var xmlSerializer = new XmlSerializer(typeof(EquipmentCollectionClass));

				var equipmentCollection = (EquipmentCollectionClass) xmlSerializer.Deserialize(stream);

				FMChannelHelper.MakeCall<IClientDispatchService>(
					equipments =>
					{
						foreach (EquipmentClass equipment in equipmentCollection)
						{
							equipments.ImportEquipment(this.Security, equipment);
							this.StatustextBox.Text += ".";
							this.StatustextBox.Update();
						}
					});

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				this.StatustextBox.Text += "\r\nMerging Equipment Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nMerging Equipment Complete";
			this.StatustextBox.Update();
			return true;
		}

		private bool MergePersonnel()
		{
			this.StatustextBox.Text += "\r\nMerging Personnel";
			this.StatustextBox.Update();
			string personName = string.Empty;

			try
			{
				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Personnel.xml", FileMode.Open, FileAccess.Read, FileShare.None);
				this.StatustextBox.Text += "\r\nUpdateing Personnel";
				this.StatustextBox.Text += "\r\n";
				this.StatustextBox.Update();

				var xmlSerializer = new XmlSerializer(typeof(PersonCollectionClass));

				var personCollection = (PersonCollectionClass) xmlSerializer.Deserialize(stream);

				FMChannelHelper.MakeCall<IClientDispatchService>(
					personnel =>
					{
						foreach (PersonClass person in personCollection)
						{
							// the person class adds the schedule as part of the reset method. During deserilization seven more are added
							// we need to remap these and get rid of the old ones
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

							personnel.ImportPerson(this.Security, person);
							this.StatustextBox.Text += ".";
							this.StatustextBox.Update();
						}
					});

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message + personName, this.Text);
				this.StatustextBox.Text += "\r\nMerging Personnel Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nMerging Personnel Complete";
			this.StatustextBox.Update();
			return true;
		}

		private bool MergeTransactions()
		{
			this.StatustextBox.Text += "\r\nMerging Transactions";
			this.StatustextBox.Update();

			try
			{
				Stream stream = new FileStream(this.TargetSourcetextBox.Text + "\\Transactions.xml", 
												FileMode.Open, 
												FileAccess.Read, 
												FileShare.None);

				var xmlSerializer = new XmlSerializer(typeof(TransactionDOCollectionClass));
				var transactionDOCollection = (TransactionDOCollectionClass) xmlSerializer.Deserialize(stream);

				this.StatustextBox.Text += "\r\nUpdateing Transaction";
				this.StatustextBox.Text += "\r\n";
				this.StatustextBox.Update();

				var accountingSite =
					FMChannelHelper.MakeCall<IClientDispatchService, AccountingSite>(x => x.LoadSiteInfo(this.Security, this.Security.SiteGuid));

				FMChannelHelper.MakeCall<IClientDispatchService>(
					processor =>
					{
						foreach (TransactionDO transactionDO in transactionDOCollection)
						{
							var transactionimportSR = new TransactionImportSR(
								this.Security, transactionDO, accountingSite, false);

							processor.ProcessTransactionImportServiceRequest(transactionimportSR);

							this.StatustextBox.Text += ".";
							this.StatustextBox.Update();
						}
					});

				stream.Close();
			}
			catch (Exception exception)
			{
				MessageBox.Show(this, exception.Message, this.Text);
				this.StatustextBox.Text += "\r\nMerging Personnel Failed";
				this.StatustextBox.Update();
				return false;
			}

			this.StatustextBox.Text += "\r\nMerging Personnel Complete";
			this.StatustextBox.Update();
			return true;
		}
	}
}
