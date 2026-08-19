using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for TransactionLineItemDBI.
	/// </summary>
	public class TransactionLineItemDBI : BaseDBI
	{
		#region Attributes
		#endregion Attributes
		public TransactionLineItemDBI(System.Data.SqlClient.SqlConnection conn, string user) : base(conn, user)
		{
		}

		public void Save(SuperLineItemDO lineItem, string transID, int sequenceID)
		{
			//If line item exists, delete and insert, preserving original CreatedBy and CreatedDateTime.
			createdBy = user;
			updatedBy = user;
			createdDateTime = now;
			updatedDateTime = now;
			if(LineItemExists(transID, sequenceID) == true)
			{
				Update(lineItem, transID, sequenceID);
			}
			else
			{
				Insert(lineItem, transID, sequenceID);
			}
		}

		protected void Update(SuperLineItemDO lineItem, string transID, int sequenceID)
		{
			Delete(transID, sequenceID);
			Insert(lineItem, transID, sequenceID);
		}

		protected void Insert(SuperLineItemDO lineItem, string transID, int sequenceID)
		{
			int i = 0;
			insertCmd.Parameters[i++].Value = GetSequenceValue("LineItemID");
			insertCmd.Parameters[i++].Value = transID;
			insertCmd.Parameters[i++].Value = sequenceID;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterReading.MeterStart);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterReading.MeterStop);
			insertCmd.Parameters[i++].Value = lineItem.Volume.GrossInventoryChange;
			insertCmd.Parameters[i++].Value = lineItem.Temperature;
			insertCmd.Parameters[i++].Value = lineItem.VCF;
			insertCmd.Parameters[i++].Value = lineItem.Density;
			insertCmd.Parameters[i++].Value = lineItem.Product;
			insertCmd.Parameters[i++].Value = lineItem.ProductCode;
			insertCmd.Parameters[i++].Value = lineItem.ProductType;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.ProductPrice);
			insertCmd.Parameters[i++].Value = lineItem.CLIN;
			insertCmd.Parameters[i++].Value = lineItem.Volume.NetInventoryChange;
			insertCmd.Parameters[i++].Value = lineItem.ContractNumber;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ1.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ1.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ1.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ1.EquipmentModel;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ2.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ2.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ2.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ2.EquipmentModel;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ3.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ3.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ3.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.DestinationEQ3.EquipmentModel;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ1.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ1.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ1.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ1.EquipmentModel;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ2.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ2.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ2.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ2.EquipmentModel;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ3.RegistrationID;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ3.SerialNumber;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ3.EquipmentType;
			insertCmd.Parameters[i++].Value = lineItem.SourceEQ3.EquipmentModel;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterReading.MeterFactor);
			insertCmd.Parameters[i++].Value = lineItem.SequenceNumber;
			insertCmd.Parameters[i++].Value = lineItem.BatchNumber;
			insertCmd.Parameters[i++].Value = lineItem.DocumentNumber;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.LineFill);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.BottomVolume);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.NetCapacity);
			insertCmd.Parameters[i++].Value = lineItem.Customs;
			insertCmd.Parameters[i++].Value = lineItem.TransactionStatus;
			insertCmd.Parameters[i++].Value = lineItem.ArmNumber;
			insertCmd.Parameters[i++].Value = lineItem.LineNumber;
			insertCmd.Parameters[i++].Value = lineItem.OperatorID;
			insertCmd.Parameters[i++].Value = lineItem.TankStatus;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterReading.StartDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterReading.StopDateTime);
			insertCmd.Parameters[i++].Value = lineItem.Pit;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.RequestedDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.DispatchedDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.AcknowledgedDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.OnLocationTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.ValidationDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.CompletionDateTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.ReceiptVariance);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.DifferentialPressure);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.LoadRackVariance);
			insertCmd.Parameters[i++].Value = lineItem.RequestedBy;
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.FreezePoint);
			insertCmd.Parameters[i++].Value = lineItem.DeleteFlag;
			insertCmd.Parameters[i++].Value = createdBy;
			insertCmd.Parameters[i++].Value = createdDateTime;
			insertCmd.Parameters[i++].Value = updatedBy;
			insertCmd.Parameters[i++].Value = updatedDateTime;
			// START 2014-Apr-04 p carpenter added to support expanded FSR fields.
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.DualFuelingModeFlag);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.DualFuelingPrimaryFlag);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.EngineRunTime);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.FlowRate);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.FuelCompressionFactor);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.HydrantPressure);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MobileDeviceID);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MobileDeviceGuid);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.TemperatureQualityStatus);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterStartObtainedAutomaticallyFlag);
			insertCmd.Parameters[i++].Value = VObject.GetValue(lineItem.MeterStopObtainedAutomaticallyFlag);     

			// END   2014-Apr-04 p carpenter added to support expanded FSR fields.

			foreach(System.Data.SqlClient.SqlParameter param in insertCmd.Parameters)
			{
				if(param.Value == null)
				{
					param.Value = System.DBNull.Value;
				}
			}

			int result = insertCmd.ExecuteNonQuery();

		}

		public void Delete(string transID, int sequenceNumber)
		{
			DeleteRange(transID, sequenceNumber, sequenceNumber);
		}

		public void DeleteRange(string transID, int startSequenceID, int endSequenceID)
		{
			deleteCmd.Parameters[0].Value = transID;
			deleteCmd.Parameters[1].Value = startSequenceID;
			deleteCmd.Parameters[2].Value = endSequenceID;
			int result = deleteCmd.ExecuteNonQuery();

			System.Diagnostics.Debug.Assert(result != (endSequenceID - startSequenceID + 1),
				"TransactionLineItemDBI.Delete() deleted " + result + " line items.");
		}

		#region Overrides
		protected override void PrepareInsertStatement()
		{
			insertCmd = new System.Data.SqlClient.SqlCommand();
			insertCmd.Connection = conn;

			insertCmd.CommandText = "INSERT INTO tblTransactionLineItems VALUES (" +
				"@TransLineItemID, " +
				"@TransID, " +
				"@SequenceID, " +
				"@MeterStart, " +
				"@MeterStop, " +
				"@GrossQuantity, " +
				"@Temperature, " +
				"@Vcf, " +
				"@Density, " +
				"@Product, " +
				"@ProductCode, " +
				"@ProductType, " +
				"@ProductPrice, " +
				"@CLIN, " +
				"@NetQuantity, " +
                "@MassQuantity, " +
				"@ContractNumber, " +
				"@DestinationRegistrationID1, " +
				"@DestinationSerialNumber1, " +
				"@DestinationEquipmentType1, " +
				"@DestinationEquipmentModel1, " +
				"@DestinationRegistrationID2, " +
				"@DestinationSerialNumber2, " +
				"@DestinationEquipmentType2, " +
				"@DestinationEquipmentModel2, " +
				"@DestinationRegistrationID3, " +
				"@DestinationSerialNumber3, " +
				"@DestinationEquipmentType3, " +
				"@DestinationEquipmentModel3, " +
				"@SourceRegistrationID1, " +
				"@SourceSerialNumber1, " +
				"@SourceEquipmentType1, " +
				"@SourceEquipmentModel1, " +
				"@SourceRegistrationID2, " +
				"@SourceSerialNumber2, " +
				"@SourceEquipmentType2, " +
				"@SourceEquipmentModel2, " +
				"@SourceRegistrationID3, " +
				"@SourceSerialNumber3, " +
				"@SourceEquipmentType3, " +
				"@SourceEquipmentModel3, " +
				"@MeterFactor, " +
				"@LineItemSequenceNumber, " +
				"@BatchNumber, " +
				"@DocumentNumber, " +
				"@LineFill, " +
				"@BottomVolume, " +
				"@NetCapacity, " +
				"@Customs, " +
				"@TransactionStatus, " +
				"@ArmNumber, " +
				"@LineNumber, " +
				"@OperatorID, " +
				"@TankStatus, " +
				"@MeterStartDateTime, " +
				"@MeterStopDateTime, " +
				"@Pit, " +
				"@RequestedDateTime, " +
				"@DispatchedDateTime, " +
				"@AcknowledgedDateTime, " +
				"@OnLocationTime, " +
				"@ValidationDateTime, " +
				"@CompletionDateTime, " +
				"@ReceiptVariance, " +
				"@DifferentialPressure, " +
				"@LoadRackVariance, " +
				"@RequestedBy, " +
				"@FreezePoint, " +
				"@DeleteFlag, " +
				"@CreatedBy, " +
				"@CreatedDate, " +
				"@UpdatedBy, " +
				"@UpdatedDate," +
				// START 2014-Apr-04 p carpenter added to support expanded FSR fields.
				"@DualFuelingModeFlag, " +
				"@DualFuelingPrimaryFlag, " +
				"@EngineRunTime, " +
				"@FlowRate, " +
				"@FuelCompressionFactor, " +
				"@HydrantPressure, " +
				"@MobileDeviceID, " +
				"@MoibleDeviceGuid, " +
				"@TemperatureQualityStatus, " +
				"@MeterStartObtainedAutomaticallyFlag, " +
				"@MeterStopObtainedAutomaticallyFlag " +
				// END   2014-Apr-04 p carpenter added to support expanded FSR fields.
				")";


			insertCmd.Parameters.Add("@TransLineItemID", System.Data.SqlDbType.BigInt);
			insertCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64);
			insertCmd.Parameters.Add("@SequenceID", System.Data.SqlDbType.SmallInt);
			insertCmd.Parameters.Add("@MeterStart", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@MeterStop", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@GrossQuantity", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@Temperature", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@Vcf", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@Density", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@Product", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ProductCode", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@ProductType", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@ProductPrice", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@CLIN", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@NetQuantity", System.Data.SqlDbType.Float);
            insertCmd.Parameters.Add("@MassQuantity", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@ContractNumber", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@DestinationRegistrationID1", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationSerialNumber1", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@DestinationEquipmentType1", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationEquipmentModel1", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@DestinationRegistrationID2", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationSerialNumber2", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@DestinationEquipmentType2", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationEquipmentModel2", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@DestinationRegistrationID3", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationSerialNumber3", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@DestinationEquipmentType3", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@DestinationEquipmentModel3", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@SourceRegistrationID1", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceSerialNumber1", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@SourceEquipmentType1", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceEquipmentModel1", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@SourceRegistrationID2", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceSerialNumber2", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@SourceEquipmentType2", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceEquipmentModel2", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@SourceRegistrationID3", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceSerialNumber3", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@SourceEquipmentType3", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@SourceEquipmentModel3", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@MeterFactor", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@LineItemSequenceNumber", System.Data.SqlDbType.NVarChar, 5);
			insertCmd.Parameters.Add("@BatchNumber", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@DocumentNumber", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@LineFill", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@BottomVolume", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@NetCapacity", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@Customs", System.Data.SqlDbType.NVarChar, 20);
			insertCmd.Parameters.Add("@TransactionStatus", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@ArmNumber", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@LineNumber", System.Data.SqlDbType.Int);
			insertCmd.Parameters.Add("@OperatorID", System.Data.SqlDbType.NVarChar, 6);
			insertCmd.Parameters.Add("@TankStatus", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@MeterStartDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@MeterStopDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@Pit", System.Data.SqlDbType.NVarChar, 10);
			insertCmd.Parameters.Add("@RequestedDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@DispatchedDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@AcknowledgedDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@OnLocationTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@ValidationDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@CompletionDateTime", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@ReceiptVariance", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@DifferentialPressure", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@LoadRackVariance", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@RequestedBy", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@FreezePoint", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@DeleteFlag", System.Data.SqlDbType.Bit);
			insertCmd.Parameters.Add("@CreatedBy", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@CreatedDate", System.Data.SqlDbType.DateTime);
			insertCmd.Parameters.Add("@UpdatedBy", System.Data.SqlDbType.NVarChar, 30);
			insertCmd.Parameters.Add("@UpdatedDate", System.Data.SqlDbType.DateTime);
			// START 2014-Apr-04 p carpenter added to support expanded FSR fields.
			insertCmd.Parameters.Add("@DualFuelingModeFlag", System.Data.SqlDbType.Bit);
			insertCmd.Parameters.Add("@DualFuelingPrimaryFlag", System.Data.SqlDbType.Bit);
			insertCmd.Parameters.Add("@EngineRunTime", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@FlowRate", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@FuelCompressionFactor", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@HydrantPressure", System.Data.SqlDbType.Float);
			insertCmd.Parameters.Add("@MobileDeviceID", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@MoibleDeviceGuid", System.Data.SqlDbType.UniqueIdentifier);
			insertCmd.Parameters.Add("@TemperatureQualityStatus", System.Data.SqlDbType.NVarChar, 50);
			insertCmd.Parameters.Add("@MeterStartObtainedAutomaticallyFlag", System.Data.SqlDbType.Bit);
			insertCmd.Parameters.Add("@MeterStopObtainedAutomaticallyFlag", System.Data.SqlDbType.Bit);
			// END   2014-Apr-04 p carpenter added to support expanded FSR fields.	

			insertCmd.Prepare();
		}
		protected override void PrepareSelectStatement()
		{
			selectCmd = new System.Data.SqlClient.SqlCommand();
			selectCmd.Connection = conn;
			selectCmd.CommandText =
				"SELECT CreatedBy, CreatedDate FROM tblTransactionLineItems " + 
				"WHERE TransID = @TransID AND SequenceID = @SequenceID";
			selectCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64, "TransID");
			selectCmd.Parameters.Add("@SequenceID", System.Data.SqlDbType.Int);
			selectCmd.Prepare();

		}
		protected override void PrepareDeleteStatement()
		{
			deleteCmd = new System.Data.SqlClient.SqlCommand();
			deleteCmd.Connection = conn;
			deleteCmd.CommandText =
				"DELETE FROM tblTransactionLineItems " +
				"WHERE TransID = @TransID AND SequenceID >= @StartSequenceID AND SequenceID <= @StopSequenceID";
			deleteCmd.Parameters.Add("@TransID", System.Data.SqlDbType.NVarChar, 64, "TransID");
			deleteCmd.Parameters.Add("@StartSequenceID", System.Data.SqlDbType.BigInt);
			deleteCmd.Parameters.Add("@StopSequenceID", System.Data.SqlDbType.BigInt);
			deleteCmd.Prepare();
		}
		#endregion Overrides

		protected bool LineItemExists(string transID, int sequenceID)
		{
			bool exists = false;
			selectCmd.Parameters[0].Value = transID;
			selectCmd.Parameters[1].Value = sequenceID;
			System.Data.SqlClient.SqlDataReader reader = selectCmd.ExecuteReader();
			if(reader.HasRows)
			{
				reader.Read();
				if(! reader.IsDBNull(0))
				{
					createdBy = reader.GetString(0);
				}
				if(! reader.IsDBNull(1))
				{
					createdDateTime = reader.GetDateTime(1);
				}
				exists = true;
			}
			reader.Close();
			return exists;
		}
	}
}
