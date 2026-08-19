using System;
using IBM.WMQ;
using System.Messaging;
using System.Xml;
using System.Xml.Serialization;
using FM7Accounting;
using System.IO;
using System.EnterpriseServices;
using ConsolidatedDataObjects;
using FMCommon;
using ConsolidatedDAL;
using System.Data;
using ConsolidatedBLL;

namespace DataSynchronizationProcessor
{
	/// <summary>
	/// Summary description for Class.
	/// </summary>
	public class ExportDataClass
	{
		private ExportResultClass exportResult;
		private TransactionDO transDO;
		private CloseoutDO closeoutDO;
		private TankClass tankDO;

		public ExportResultClass ExportResult
		{
			get{return exportResult;}
			set {exportResult = value;}
		}

		public TransactionDO Transaction
		{
			get{ return transDO;}
			set { transDO=value;}
		}

		public CloseoutDO Closeout
		{
			get{ return closeoutDO;}
			set { closeoutDO=value;}
		}

		public TankClass TankData
		{
			get{ return tankDO;}
			set { tankDO=value;}
		}
	}

	public class DataSynchronizationProcessorClass
	{
		public DataSynchronizationProcessorClass()
		{
			
		}

		//Microsoft MSMQ
		private MessageQueue ConnectToMSMQ(string queueName)
		{
			MessageQueue oMSMQ;
			if (MessageQueue.Exists(queueName))
				oMSMQ = new MessageQueue(queueName);
			else
				throw new Exception("MSMQ Queue " + queueName + " does not exist.");

			if (!oMSMQ.Transactional)
				throw new Exception("MSMQ Queue " + queueName + " is not transactionl queue.");
			
			return oMSMQ;
		}

		private void SendMessageToMSMQ (XmlDocument xml, MessageQueue oMSMQ, string BatchID)
		{
			MessageQueueTransaction oMQTrans = new MessageQueueTransaction();
			oMQTrans.Begin();
			try
			{
				Message oMessage = new Message();
				oMessage.Label = BatchID;
				oMessage.Body = xml;
				oMSMQ.Send(oMessage,oMQTrans);
				oMQTrans.Commit();
			}
			catch(Exception e)
			{
				oMQTrans.Abort();
				throw new Exception("Fail to send message to MSMQ "+oMSMQ.QueueName+"."+e.Message);
			}
		}

		private XmlDocument GetMessageFromMSMQ(MessageQueue oMSMQ, ref string BatchID)
		{
			TimeSpan timeSpan = new TimeSpan(100);

			MessageQueueTransaction oMQTrans = new MessageQueueTransaction();
			oMQTrans.Begin();
			Message oMessage = new Message();
			try
			{
				oMessage = oMSMQ.Receive(timeSpan,oMQTrans);
				oMQTrans.Commit();
			}
			catch(Exception e)
			{
				oMQTrans.Abort();
				throw new Exception("Fail to retrieve message from MSMQ "+oMSMQ.QueueName+"."+e.Message);
			}
			//should I check the message type ???
			BatchID = oMessage.Label; //use label to store BatchID value,which will be used by retured result
			XmlMessageFormatter xmlMF = new XmlMessageFormatter();
			XmlDocument xml =(XmlDocument)xmlMF.Read(oMessage);

			return xml;
		}


		//IBM MQSeries
		private void ConnectToMQSeries(string mqManagerName, string mqQueueName, ref MQQueueManager mqManager,ref MQQueue mqQueue)
		{
			try
			{
				mqManager = new MQQueueManager(mqManagerName);
			}
			catch (MQException mqe) 
			{
				throw new Exception( "create of MQQueueManager ended with " + mqe.Message+ mqe.Reason.ToString() );
			}

			int openOptions = MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_OUTPUT;
			
			try
			{
				// Now specify the queue that we wish to open,and the open options
				mqQueue =mqManager.AccessQueue(mqQueueName, openOptions);
			}
			catch (MQException qe) 
			{
				throw new Exception( "Fail to access Queue " + qe.Message+ qe.Reason.ToString() );
			}
		}

		private void DisconnectFromMQSeries(MQQueueManager mqManager)
		{
			try
			{
				if (mqManager !=null)
					mqManager.Disconnect();
			}
			catch(Exception e)
			{
				throw new Exception(e.Message);
			}
		}
		
		private XmlDocument GetMessageFromMQSeries(MQQueueManager mqManager,MQQueue mqQueue, ref string BatchID)
		{
			if (null == mqQueue)
				return  null;

			MQMessage           mqMsg;           // MQMessage instance
			MQGetMessageOptions mqGetMsgOpts;    // MQGetMessageOptions instance

			mqMsg = new MQMessage();
			mqGetMsgOpts = new MQGetMessageOptions();
			mqGetMsgOpts.WaitInterval = 15000;  // 15 second limit for waiting
			mqGetMsgOpts.Options = (int)MQC.MQGMO_SYNCPOINT + (int)MQC.MQGMO_NO_WAIT;

			XmlDocument xml = new XmlDocument();
			mqManager.Begin();
			
			try 
			{
				mqQueue.Get( mqMsg, mqGetMsgOpts );
				if (mqMsg.Format.CompareTo(MQC.MQFMT_STRING) == 0) 
				{
					BatchID = mqMsg.ApplicationIdData;
					xml.Load(mqMsg.ReadString(mqMsg.MessageLength));
					mqManager.Commit();

					return xml;
				}
				else 
				{
					throw new Exception("Message format is invalid.");
				}
			}
			catch (MQException mqe) 
			{
				// report reason, if any
				if ( mqe.Reason == MQC.MQRC_NO_MSG_AVAILABLE ) 
				{
					return null;
				} 
				
				// treat truncated message as a failure for this sample
				if ( mqe.Reason == MQC.MQRC_TRUNCATED_MSG_FAILED ) 
				{
					throw new Exception("Message is truncated.");
				}
				else
					throw new Exception(mqe.Message);
			}
			finally
			{
				mqManager.Backout();
			}
		}
		

		private void SendMessageToMQSeries(MQQueueManager mqManager,MQQueue mqQueue,XmlDocument xml,string BatchID)
		{
			if (xml != null) 
			{
				// put the next message to the queue
				MQMessage mqMsg = new MQMessage();
				//use applicationIDDate to store BatchID, which will be used by returned results
				mqMsg.ApplicationIdData = BatchID;
				mqMsg.WriteString(xml.ToString());
				mqMsg.Format = MQC.MQFMT_STRING;
				MQPutMessageOptions mqPutMsgOpts = new MQPutMessageOptions();
				
				mqManager.Begin();
				try
				{
					mqQueue.Put( mqMsg, mqPutMsgOpts );
					mqManager.Commit();
				}
				catch (MQException mqe)
				{
					mqManager.Backout();
					throw new Exception("Fail to send message. "+mqe.Message );
				}
			}
			else
				throw new Exception("Xml file is null.");
		}


/// <summary>
/// Serialize objects to XML document
/// </summary>
/// <param name="transDO"></param>
/// <returns></returns>
		private XmlDocument ConvertTransDOToXml(TransactionDO transDO, SecurityClass Security)
		{
			try
			{
				//check if siteID is blank
				if (transDO.Site=="")
				{
					SitesClass sites = new SitesClass();
					transDO.Site = sites.Get(Security,(int)transDO.SiteIndex.Value).SiteID;
				}

				XmlDocument xml= new XmlDocument();
				XmlSerializer ser = new XmlSerializer(typeof(TransactionDO));
				Stream str = new MemoryStream();
				ser.Serialize(str,transDO);
				xml.Load(str);
				return xml;
			}
			catch (Exception e)
			{
				throw new Exception("Fail to serialize transaction "+transDO.TransID+"."+e.Message);
			}
		}

		private XmlDocument ConvertCloseoutDOToXml(CloseoutDO closeoutDO, SecurityClass Security)
		{
			try
			{
				//check if siteID is blank
				if (closeoutDO.SiteID=="")
				{
					SitesClass sites = new SitesClass();
					closeoutDO.SiteID = sites.Get(Security,(int)closeoutDO.SiteIndex.Value).SiteID;
				}

				XmlDocument xml= new XmlDocument();
				XmlSerializer ser = new XmlSerializer(typeof(CloseoutDO));
				Stream str = new MemoryStream();
				ser.Serialize(str,closeoutDO);
				xml.Load(str);

				return xml;
			}
			catch (Exception e)
			{
				throw new Exception("Fail to serialize closeout."+e.Message);
			}
		}

		private XmlDocument ConvertTankDOToXml(TankClass tank,SecurityClass Security)
		{
			try
			{
				//check if siteID is blank
				if (tank.SiteID=="")
				{
					SitesClass sites = new SitesClass();
					tank.SiteID = sites.Get(Security,tank.SiteIndex).SiteID;
				}

				XmlDocument xml= new XmlDocument();
				XmlSerializer ser = new XmlSerializer(typeof(TankClass));
				Stream str = new MemoryStream();
				ser.Serialize(str,tank);
				xml.Load(str);

				return xml;
			}
			catch (Exception e)
			{
				throw new Exception("Fail to serialize tank data."+e.Message);
			}
		}

		// Deserilize XMLDocument to TAS objects
		private TransactionDO ConvertXmlToTransactionDO(XmlDocument xml, SecurityClass Security)
		{
			TransactionDO transDO = new TransactionDO();
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TransactionDO));
				
				Stream reader = new MemoryStream();
				xml.Save(reader);
				transDO = (TransactionDO)serializer.Deserialize(reader);
				reader.Close();
				return transDO;
			}
			catch (Exception err)
			{
				throw new Exception("Fail to read XML file"+err.Message);
			}
		}

		private string ImportTranaction(TransactionDO transDO,SecurityClass Security)
		{
			//get site index first because every other indexes need siteindex
			SitesClass sites = new SitesClass();
			int siteIndex = sites.GetIndex(Security, transDO.Site);
			if (siteIndex == 0)
				throw new Exception("Site " + transDO.Site + " is not configured in the database.");
			else
			{
				transDO.SiteIndex = new VInteger(siteIndex);
			}

			ConsolidatedDAClass consolidatedDA = new ConsolidatedDAClass();
			string sql = "Exec dbo.fm_GetIndexFields " 
				+ "0," //0 for header indexes
				+ siteIndex.ToString() + ",'"
				+ transDO.Alias + "','" 
				+ transDO.ShipToID + "','"
				+ transDO.SupplierID + "','" 
				+ transDO.ShipperID + "','"
				+ transDO.OwnerID + "','"
				+ transDO.ManagerID+"','"
				+ transDO.CarrierID +"','"
				+ transDO.BillToID+"','"
				+ transDO.DestinationEQ1.RegistrationID+"','"
				+ transDO.DestinationEQ2.RegistrationID+"','"
				+ transDO.DestinationEQ3.RegistrationID+"','"
				+ transDO.SourceEQ1.RegistrationID +"','"
				+ transDO.SourceEQ2.RegistrationID + "','"
				+ transDO.SourceEQ3.RegistrationID + "','"
				+ transDO.OperatorID +"',NULL,NULL,NULL,NULL,NULL,NULL";

			DataSet dataset = consolidatedDA.GetDataSet(sql, Security);

			//return two rows, column names is fieldName, first row is ID value, second row is index value
			//build this table so I can create the error message with fieldname and ID value
			string valiadator = "";
			for(int i =0; i<dataset.Tables[0].Columns.Count;i++)
			{
				if (dataset.Tables[0].Rows[0].ItemArray.GetValue(i) != null && dataset.Tables[0].Rows[1].ItemArray.GetValue(i) == null)
					valiadator += dataset.Tables[0].Columns[i].ColumnName + " " + dataset.Tables[0].Rows[1].ItemArray.GetValue(i).ToString()+ " is not configured.";
			}

			if (valiadator == "") //everything has an index
			{
				int j = 0;
				transDO.AliasIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.ShipToIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.SupplierIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.ShipperIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.OwnerIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.ManagerIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.CarrierIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.BillToIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.DestinationEQ1.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.DestinationEQ2.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.DestinationEQ3.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.SourceEQ1.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.SourceEQ2.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.SourceEQ3.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
				transDO.OperatorIndex =GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
			}
           
			transDO.TransID = transDO.TransID + "-Test2";  //for testing purpose

			//set lineitem index
			if (transDO.LineItems.Count > 0) 
			{
				foreach (LineItemDO lineItem in transDO.LineItems)
				{
					//set sublineItem index
					if (lineItem.SubLineItems.Count > 0) 
					{
						foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
						{

							sql = "Exec dbo.fm_GetIndexFields "
								+ "2," //0 for sublineItem section
								+ siteIndex.ToString() + ","
								+ "Null,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'"
								+subLineItem.Product + "',NULL,NULL,'" 
								+subLineItem.StorageLocationID + "',NULL";
                            
							dataset = consolidatedDA.GetDataSet(sql,Security);

							for (int i = 0; i < dataset.Tables[0].Columns.Count; i++)
							{
								if (dataset.Tables[0].Rows[0].ItemArray.GetValue(i) != null && dataset.Tables[0].Rows[1].ItemArray.GetValue(i) == null)
									valiadator += dataset.Tables[0].Columns[i].ColumnName + " " + dataset.Tables[0].Rows[1].ItemArray.GetValue(i).ToString() + " is not configured.";
							}

							if (valiadator == "") //everything has an index
							{
								int j = 0;
								subLineItem.ProductIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
								subLineItem.StorageLocationIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
							}
						}

					}

					sql = "Exec dbo.fm_GetIndexFields "
						+ "1," //1 for lineitem indexes
						+ siteIndex.ToString() + ","
						+ "Null,NULL,NULL,NULL,NULL,NULL,NULL,NULL,'"
						+ lineItem.DestinationEQ.CompanyEquipmentID + "',"
						+ "NULL,NULL,'"
						+ lineItem.SourceEQ.CompanyEquipmentID + "',"
						+ "NULL,NULL,'"
						+ lineItem.OperatorID+ "','"
						+ lineItem.AdditiveProfileID+"','"
						+ lineItem.Product + "','"
						+ lineItem.DestinationCompartmentID+"','"
						+ lineItem.SourceCompartmentID+"','"
						+ lineItem.StorageLocationID+ "','"
						+ lineItem.LoadingLocationID+"'";

					dataset = consolidatedDA.GetDataSet(sql, Security);

					for (int i = 0; i < dataset.Tables[0].Columns.Count; i++)
					{
						if (dataset.Tables[0].Rows[0].ItemArray.GetValue(i) != null && dataset.Tables[0].Rows[1].ItemArray.GetValue(i) == null)
							valiadator += dataset.Tables[0].Columns[i].ColumnName + " " + dataset.Tables[0].Rows[1].ItemArray.GetValue(i).ToString() + " is not configured.";
					}

					if (valiadator == "") //everything has an index
					{
						int j = 0;
						lineItem.AdditiveProfileIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.ProductIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j)); //productIndex
						lineItem.DestinationEQ.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.DestinationCompartmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.SourceEQ.EquipmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.SourceCompartmentIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.OperatorIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.StorageLocationIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
						lineItem.LoadingLocationIndex = GetIndex(dataset.Tables[0].Rows[1].ItemArray.GetValue(++j));
					}
				}
			}
            
			if (valiadator == "") //everything has an index
			{
				//save the transactions through Accounting Service
				SaveTransactionsSR saveTransactionsSR = new SaveTransactionsSR();
				saveTransactionsSR.Security = Security;
				saveTransactionsSR.ConvertUnits = false;
				saveTransactionsSR.Transactions.Add(transDO);
				AccountingServiceImpl AccountingService = new AccountingServiceImpl();
				AccountingService.processRequest(saveTransactionsSR);

				//return results in XML
			}
			else
				throw new Exception("Configuration data is not found." + valiadator);

			return valiadator;
		}
    
		private VInteger GetIndex(Object value)
		{
			if (value==DBNull.Value)
				return null;
			else
				return new VInteger(Convert.ToInt32(value));
		}

		//Get site index from site ID
		private int GetSiteIndex(string siteID, SecurityClass Security)
		{
			//get site index first because every other indexes need siteindex
			SitesClass sites = new SitesClass();
			int siteIndex = sites.GetIndex(Security, siteID);
			if (siteIndex == 0)
				throw new Exception("Site " + siteID + " is not configured in the database.");
			else
				return siteIndex;
		}

		//verify manager and product are configured and assigned to this site
		private int GetManagerIndex(string managerID, SecurityClass Security)
		{
			CompaniesClass managers = new CompaniesClass();
			int managerIndex = managers.GetIndex(Security,managerID);
			if (managerIndex ==0)
				throw new Exception("Manager " + managerID + " is not configured in the database.");

			//check if this company has manager role
			CompanyClass manager = managers.Get(Security, managerIndex);
			foreach (CompanyRoleMapClass roleMap in manager.RoleCollection)
			{
				if (roleMap.Role == COMPANY_ROLE.MANAGER)
				{
					return managerIndex;
				}
			}

			//no manager role
			throw new Exception("Manager " + managerID + " is not assigned as Manager.");

		}

		private int GetProductIndex(string productID,SecurityClass Security)
		{
			//verify product
			ProductsClass products = new ProductsClass();
			int productIndex = products.GetIndex(Security, productID);

			if (productIndex == 0)
				throw new Exception("Product " + productID + "  is not configured in the database.");
			else
			{
				return productIndex;
			}
		}

		private CloseoutDO ConvertXmlToCloseoutDO(XmlDocument xml, SecurityClass Security)
		{
			CloseoutDO closeoutDO = new CloseoutDO();
           
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(CloseoutDO));
				Stream reader = new MemoryStream();
				xml.Save(reader);
				closeoutDO = (CloseoutDO)serializer.Deserialize(reader);
				reader.Close();

				return closeoutDO;
			}
			catch (Exception err)
			{
				throw new Exception("Fail to read XML file" + err.Message);
			}
		}

		private void ImportCloseoutDO(CloseoutDO closeoutDO, SecurityClass Security)
		{
		
			CloseoutSR closeoutSR = new CloseoutSR();
			closeoutSR.Security = Security;
			closeoutSR.Site = closeoutDO.SiteID;

			//get site index first because every other indexes need siteindex
			closeoutSR.CurrentSiteIndex = GetSiteIndex(closeoutDO.SiteID,Security);

			//verify manager and product are configured and assigned to this site
			closeoutSR.ManagerIndex = GetManagerIndex(closeoutDO.ManagerName,Security);
			closeoutSR.ManagerName = closeoutDO.ManagerName;

			//verify product
			closeoutSR.ProductIndex = GetProductIndex(closeoutDO.ProductName,Security);
			closeoutSR.ProductName = closeoutDO.ProductName;
			
			closeoutSR.InventoryDate = closeoutDO.CloseoutDate;
			closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CALCULATE; //calculate first to verify if numbers match

			AccountingServiceImpl AccountingService = new AccountingServiceImpl();
			CloseoutDO closeoutRecord = (CloseoutDO)AccountingService.processRequest(closeoutSR);

			//verify numbers match the closeoutDO from local site
			if (closeoutRecord != null
				&& closeoutRecord.TotalPhysicalInventory == closeoutDO.TotalPhysicalInventory
				&& closeoutRecord.TotalVariance == closeoutDO.TotalVariance
				&& closeoutRecord.Variance == closeoutDO.Variance
				&& closeoutRecord.BookInventory == closeoutDO.BookInventory)
			{
				closeoutSR.CloseoutCommand = CloseoutSR.CloseoutType.CREATE;
				closeoutSR.Closeout = closeoutRecord;
				AccountingService.processRequest(closeoutSR);
			}
			else
				throw new Exception("Fail to closeout becuase closeout record calculation does not match.");
                
		}

		private TankClass ConvertXmlToTankClass(SecurityClass Security,XmlDocument xml)
		{
			TankClass tank = new TankClass();
			
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(TankClass));
				Stream reader = new MemoryStream();
				xml.Save(reader);
				tank = (TankClass)serializer.Deserialize(reader);
				reader.Close();

				return tank;
			}
			catch (Exception err)
			{
				throw new Exception("Fail to read XML file" + err.Message);
			}
		}

		private void ImportTankData(SecurityClass Security,TankClass tank)
		{
			
			//get site index
			tank.SiteIndex = GetSiteIndex(tank.SiteID,Security);

			Security.SiteIndex = tank.SiteIndex; //ensure it accesses as local site

			//get manager index
			tank.ManagerIndex = GetManagerIndex(tank.ManagerID,Security);
			
			//get product index
			tank.ProductIndex = GetProductIndex(tank.ProductID,Security);
		
			//check if tank exists
			TanksClass tanks = new TanksClass();
			int tankIndex = tanks.GetIndex(Security,tank.ID);
			if (tankIndex ==0) //tank is not configured 
				throw new Exception("Fail to import tank inventory data because tank "+tank.ID+ " is not configured for site "+tank.SiteID+".");
			else
				tank.Index = tankIndex;

			//tank exists, modify the tank
			tanks.Modify(Security,tank);
		}

		//package data with ExportResult before sending out
		private XmlDocument CreateExportData(object obj,string interfaceName,string batchID,SecurityClass Security)
		{
			ExportDataClass exportData = new ExportDataClass();

			ExportResultClass exportResult = new ExportResultClass();

			exportResult.BatchID = batchID;
			exportResult.InterfaceName = interfaceName;
			exportResult.TransDateTime = DateTime.Now;

			if (obj.GetType() == typeof(TransactionDO))
			{
				TransactionDO transDO = (TransactionDO)obj;
				//check if siteID is blank
				if (transDO.Site=="")
				{
					SitesClass sites = new SitesClass();
					transDO.Site = sites.Get(Security,(int)transDO.SiteIndex.Value).SiteID;
				}

				exportResult.Type = EXPORT_RESULT_TYPE.TRANSACTION;
				exportResult.SiteID = transDO.Site;
				exportResult.TransVersion = transDO.TransVersion;
				exportResult.TransDateTime = DateTime.Now;
				
				ExportResultDetailClass resultDetail = new ExportResultDetailClass();
				resultDetail.TransVersion = transDO.TransVersion;
				resultDetail.RecordID = transDO.TransID;
				
				exportResult.ExportResultDetailCollection.Add(resultDetail);

				exportData.ExportResult = exportResult;
				exportData.Transaction = transDO;
			}
			
			if (obj.GetType() == typeof(TankClass))
			{
				TankClass tank = (TankClass)obj;
				//check if siteID is blank
				if (tank.SiteID=="")
				{
					SitesClass sites = new SitesClass();
					tank.SiteID = sites.Get(Security,tank.SiteIndex).SiteID;
				}

				exportResult.Type = EXPORT_RESULT_TYPE.TANK;
				exportResult.SiteID = tank.SiteID;
				//exportResult.TransVersion = transDO.TransVersion;
				exportResult.TransDateTime = DateTime.Now;
				
				ExportResultDetailClass resultDetail = new ExportResultDetailClass();
				//resultDetail.TransVersion = transDO.TransVersion;
				resultDetail.RecordID = tank.ID;
				
				exportResult.ExportResultDetailCollection.Add(resultDetail);

				exportData.ExportResult = exportResult;
				exportData.TankData = tank;
			}

			if (obj.GetType() == typeof(CloseoutDO))
			{
				CloseoutDO closeoutDO = (CloseoutDO)obj;
				//check if siteID is blank
				if (closeoutDO.SiteID=="")
				{
					SitesClass sites = new SitesClass();
					closeoutDO.SiteID = sites.Get(Security,(int)closeoutDO.SiteIndex.Value).SiteID;
				}
				
				exportResult.Type = EXPORT_RESULT_TYPE.CLOSEOUT;
				exportResult.SiteID = closeoutDO.SiteID;
			//	exportResult.TransVersion = closeoutDO.TransVersion;
				exportResult.TransDateTime = DateTime.Now;
				
				ExportResultDetailClass resultDetail = new ExportResultDetailClass();
				//resultDetail.TransVersion = transDO.TransVersion;
				//resultDetail.RecordID = closeoutDO.TransID;
				
				exportResult.ExportResultDetailCollection.Add(resultDetail);

				exportData.ExportResult = exportResult;
				exportData.Closeout = closeoutDO;

			}

			XmlDocument xml= new XmlDocument();
			XmlSerializer ser = new XmlSerializer(typeof(ExportDataClass));
			Stream str = new MemoryStream();
			ser.Serialize(str,exportData);
			xml.Load(str);

			return xml;

		}

		private ExportDataClass ConvertXmlToExportData(XmlDocument xml, SecurityClass Security)
		{
			ExportDataClass exportData = new ExportDataClass();
           
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(ExportDataClass));
				Stream reader = new MemoryStream();
				xml.Save(reader);
				exportData = (ExportDataClass)serializer.Deserialize(reader);
				reader.Close();

				return exportData;
			}
			catch (Exception err)
			{
				throw new Exception("Fail to read XML file" + err.Message);
			}
		}
	}
}
