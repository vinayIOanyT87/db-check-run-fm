namespace DataObjects.Handlers
{
	using System.Collections.Generic;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml.Serialization;

	using global::DataObjects.DataObjects;

	public class FileHandler
	{
		/// <summary>
		/// This method saves the mobile originated message to file.
		/// </summary>
		/// <param name="saveFileDialog">The save file dialog object.</param>
		/// <param name="moMessage">The mobile originated message data object.</param>
		public void SaveMoMessageToFile(SaveFileDialog saveFileDialog, MobileOriginatedMessageDO moMessage)
		{
			using (Stream fileStream = saveFileDialog.OpenFile())
			{
				var xmlSerializer = new XmlSerializer(moMessage.GetType());
				xmlSerializer.Serialize(fileStream, moMessage);

				fileStream.Flush();
				fileStream.Close();
				fileStream.Dispose();
			}
		}

		/// <summary>
		/// This method will read the mobile originated message info from the
		/// saved file.
		/// </summary>
		/// <param name="openFileDialog">The open file dialog object.</param>
		/// <returns>Returns the mobile originated message data object.</returns>
		public MobileOriginatedMessageDO ReadMoMessageFile(OpenFileDialog openFileDialog)
		{
			MobileOriginatedMessageDO moMessage;

			using (Stream fileStream = openFileDialog.OpenFile())
			{
				var xmlSerializer = new XmlSerializer(typeof(MobileOriginatedMessageDO));
				moMessage = xmlSerializer.Deserialize(fileStream) as MobileOriginatedMessageDO;
				fileStream.Close();
				fileStream.Dispose();
			}

			return moMessage;
		}

		/// <summary>
		/// This method will read the raw data from a file and return
		/// a collection of raw data objects.
		/// </summary>
		/// <param name="openFileDialog">The file dialog containing the file name.</param>
		/// <returns>Returns a collection of raw data objects.</returns>
		public List<RawDataDO> ReadRawData(OpenFileDialog openFileDialog)
		{
			var rawDataCollection = new List<RawDataDO>();

			string[] rawDataRecords = File.ReadAllLines(openFileDialog.FileName);

			foreach (string record in rawDataRecords)
			{
				var rawDataDo = new RawDataDO();
				rawDataDo.Load(record);
				rawDataCollection.Add(rawDataDo);
			}

			return rawDataCollection;
		}

		/// <summary>
		/// This method will save the Iridium configuration items.
		/// </summary>
		/// <param name="configDo">Contains the configuration items.</param>
		public void SaveIridiumSimulatorConfiguration(ConfigurationDO configDo)
		{
			string path = Path.GetDirectoryName(Application.ExecutablePath);
			path = path + "\\IridiumSimulatorConfiguration.xml";

			if (File.Exists(path))
			{
				using (Stream fileStream = new FileStream(path, FileMode.Truncate))
				{
					fileStream.Flush();
					fileStream.Close();
					fileStream.Dispose();
				}
			}

			using (Stream fileStream = new FileStream(path, FileMode.OpenOrCreate))
			{
				var xmlSerializer = new XmlSerializer(configDo.GetType());
				xmlSerializer.Serialize(fileStream, configDo);

				fileStream.Close();
				fileStream.Dispose();
			}
		}

		/// <summary>
		/// This method will read the Iridium Simulator configuration data from a file.
		/// </summary>
		/// <returns>Returns the Iridium simulator configuration data.</returns>
		public ConfigurationDO ReadIridiumSimulatorConfigurationDataFromFile()
		{
			ConfigurationDO configDo = null;
			string path = Path.GetDirectoryName(Application.ExecutablePath);
			path = path + "\\IridiumSimulatorConfiguration.xml";

			if (File.Exists(path))
			{
				using (Stream fileStream = new FileStream(path, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(typeof(ConfigurationDO));
					configDo = xmlSerializer.Deserialize(fileStream) as ConfigurationDO;
					fileStream.Close();
					fileStream.Dispose();
				}
			}

			return configDo;
		}
	}
}
