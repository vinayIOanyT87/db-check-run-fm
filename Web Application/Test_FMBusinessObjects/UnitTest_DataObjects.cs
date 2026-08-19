#if true
using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FMBusinessObjects;
using FMBusinessObjects.DataObjects;
using System.Data;
using System.Data.SqlClient;

namespace Test_FMBusinessObjects
{
	[TestClass]
	public class UnitTest_DataObjects
	{
		Guid _siteGuid;
		string _connectionString;
		SiteClass _site;
		MeterClass _meter1, _meter2;
		string _CreatedBy = "UnitTest_DataObjects";

		void fncUnitTest_DataObjects()
		{
			string siteGuidString = ConfigurationManager.AppSettings["SiteGuid"];
			_siteGuid = new Guid(siteGuidString);
			_connectionString =  ConfigurationManager.AppSettings["ConnectionString"];
		}

		[TestMethod]
		public void TestMeterClass()
		{
			fncUnitTest_DataObjects();
			Assert.IsTrue(true);
			TestMeterClassInsert();
			TestMeterClassSelect();
		}
		public void TestMeterClassInsert()
		{
			_meter1 = new MeterClass();
			_meter1.Reset();
			_meter1.SiteGuid = _siteGuid;
			_meter1.CreatedBy = _CreatedBy;
			_meter1.NumberOfDigits = 2;
			_meter1.ID = "uniqueId";
			_meter1.DcuID = "Truck1";
			_meter1.DcuBatteryVoltage = 12.0;
			_meter1.DcuBatteryCurrent = 1.0;
			_meter1.DcuTemperature = 60.0;
			_meter1.DcuResets = 1;
			_meter1.DcuUpdateDate = DateTimeOffset.Now.Date.AddHours(1);
			_meter1.DcuConfigurationDate = DateTimeOffset.Now.Date.AddHours(2);
			_meter1.DcuFirmwareVersion = "v30";
			_meter1.DcuBluetoothAddress = "10.10.10.10";
			// open table

			CleanUpPreviousInserts();

			// insert records
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{ 
				connection.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						_meter1.InsertSQL(cmd);					
						cmd.Connection = connection;
						cmd.ExecuteNonQuery();
						Assert.IsTrue(true, "TestMeterClassInsert");
					}
					catch( Exception e)
					{
						Assert.Fail( e.ToString());
					}
				}
			}
		}

		private void CleanUpPreviousInserts()
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						cmd.CommandText = "delete from tblMeter where MeterID='uniqueId'";
						cmd.CommandType = CommandType.Text;
						cmd.Connection = connection;
						int iRet = cmd.ExecuteNonQuery();
						Assert.IsTrue(iRet >= 0, "TestMeterClassInsert: delete");
					}
					catch (Exception e)
					{
						Assert.Fail(e.ToString());
					}
				}
			}
		}
		// read records, confirm insert
		
		public void TestMeterClassSelect()
		{
			_meter2 = new MeterClass();			
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{ 
				connection.Open();
				using (SqlCommand cmd = new SqlCommand())
				{
					try
					{
						cmd.Connection = connection;
						_meter2.SiteGuid = _siteGuid;
						_meter2.SelectByIDSQL(cmd);
						_meter2.ID = "uniqueId";
						using (SqlDataReader reader = cmd.ExecuteReader())
						{
							reader.Read();
							Assert.IsTrue(reader.HasRows, "TestMeterClassSelect");
							DataTable schemaTable = reader.GetSchemaTable();
							DataRow row = schemaTable.Rows[0];
							string createdBy = row[24].ToString();
							Assert.AreEqual(createdBy, _CreatedBy);						
						}					
					}
					catch( Exception e)
					{
						Assert.Fail(e.ToString());
					}
				}
			}
		}
		// update records, confirm update
		// delete records
		// confirm delete with a read

	}
}
#endif