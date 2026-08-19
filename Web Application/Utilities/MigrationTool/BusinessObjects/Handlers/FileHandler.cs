namespace MigrationToolBusinessObjects.Handlers
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    public class FileHandler
	{
		public const string DbConfigurationFileName = "MigrationToolDbConfiguration.xml";
		public enum FileTypes { None, Station, ApplicationString, EquipmentType, Equipment, Personnel, Footnote, Qualification, PersonnelUserData, EquipmentUserData }

		/// <summary>
		/// This method saves the database configuration to file.
		/// </summary>
		/// <param name="saveFileDialog">The save file dialog object.</param>
		/// <param name="dbConfigurationDo">The database configuration data object.</param>
		public void SaveDbConfigToFile(SaveFileDialog saveFileDialog, DbConfigurationDO dbConfigurationDo)
		{
			using (Stream fileStream = saveFileDialog.OpenFile())
			{
				var xmlSerializer = new XmlSerializer(dbConfigurationDo.GetType());
				xmlSerializer.Serialize(fileStream, dbConfigurationDo);

				fileStream.Flush();
				fileStream.Close();
				fileStream.Dispose();
			}
		}

		/// <summary>
		/// This method will read the database connection configuration data from a file.
		/// </summary>
		/// <returns>Returns the database configuration data.</returns>
		public DbConfigurationDO ReadDbConnectionConfigurationDataFromFile(OpenFileDialog openFileDialog)
		{
			DbConfigurationDO dbConfigDo = null;

			using (Stream fileStream = openFileDialog.OpenFile())
			{
				var xmlSerializer = new XmlSerializer(typeof(DbConfigurationDO));
				dbConfigDo = xmlSerializer.Deserialize(fileStream) as DbConfigurationDO;
				fileStream.Close();
				fileStream.Dispose();
			}

			return dbConfigDo;
		}

		/// <summary>
		/// This method will open a migration file based on the migration type.
		/// </summary>
		/// <param name="fileType">The migration file type</param>
		/// <returns>Returns the file handle</returns>
		public FileStream OpenMigrationFile(FileTypes fileType)
        {
			string fileName = this.GetNewFileName(fileType);
			string path = Path.Combine(Environment.CurrentDirectory, fileName);
			FileStream fileHandle = File.OpenWrite(path);

			return fileHandle;
        }

		/// <summary>
		/// This method will write the migrated item to the file.
		/// </summary>
		/// <param name="fileHandle">The file handle</param>
		/// <param name="migratedItem">The migrated item info</param>
		public void WriteMigrationData(FileStream fileHandle, MigratedItem migratedItem)
        {
			var xmlSerializer = new XmlSerializer(typeof(MigratedItem));
			xmlSerializer.Serialize(fileHandle, migratedItem);
		}

		/// <summary>
		/// This method will generate a file name based on the migration tool type.
		/// </summary>
		/// <param name="fileType"></param>
		/// <returns></returns>
		private string GetNewFileName(FileTypes fileType)
        {
			string fileName = string.Empty;
			var dateTime = DateTime.Now;
			string fileNamePostfix = "_" + dateTime.Year;

			var temp = dateTime.Month < 10 ? "0" + dateTime.Month : dateTime.Month.ToString();
			fileNamePostfix = fileNamePostfix + temp;

			temp = dateTime.Day < 10 ? "0" + dateTime.Day : dateTime.Day.ToString();
			fileNamePostfix = fileNamePostfix + temp;

			temp = dateTime.Hour < 10 ? "0" + dateTime.Hour : dateTime.Hour.ToString();
			fileNamePostfix = fileNamePostfix + "_" + temp;

			temp = dateTime.Minute < 10 ? "0" + dateTime.Minute : dateTime.Minute.ToString();
			fileNamePostfix = fileNamePostfix + temp;

			temp = dateTime.Second < 10 ? "0" + dateTime.Second : dateTime.Second.ToString();
			fileNamePostfix = fileNamePostfix + temp;

			fileNamePostfix = fileNamePostfix + "_" + dateTime.Millisecond;

			switch (fileType)
            {
				case FileTypes.ApplicationString:
					fileName = "ApplicationStringMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.Equipment:
					fileName = "EquipmentMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.EquipmentType:
					fileName = "EquipmentTypeMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.EquipmentUserData:
					fileName = "EquipmentUserDataMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.Footnote:
					fileName = "FootnoteMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.Personnel:
					fileName = "PersonnelMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.PersonnelUserData:
					fileName = "PersonnelUserDataMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.Qualification:
					fileName = "QualificationMigratedItems" + fileNamePostfix;
					break;
				case FileTypes.Station:
					fileName = "StationMigratedItems" + fileNamePostfix;
					break;
            }

			return fileName;
        }
	}

	public class MigratedItem
    {
		public MigratedItem()
        {
			this.Init();
        }

		public string ID { get; set; }
		public Guid ItemGuid { get; set; }
		public Guid SiteGuid { get; set; }

		private void Init()
        {
			this.ID = string.Empty;
			this.ItemGuid = Guid.Empty;
			this.SiteGuid = Guid.Empty;
        }
    }
}
