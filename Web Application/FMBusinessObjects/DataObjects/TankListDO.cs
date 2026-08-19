/// <summary>
/// File name:	TankListDO.cs
/// Purpose:	The purpose of the tanklist data object is to retrieve the 
///				tank list to populate the
///				tank dropdown on the inventory Reconcilation page.
///	Comments:	Copyright (C) Varec, Inc. Norcross, GA, USA.  
///	            This file shall not be copied or reproduced in any form 
///				without the express written consent of Varec, Inc.
///	Author(s):	Chris Knight
///	Version:	8.0.0.0  Current version
///	
///	Modification History:
///		Date:			By:						Reason:
///		----------		--------------------	----------------------------------
///		02-Spt-2010     C. Knight               Initial Creation
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.Serialization;
using System.Data;
using System.Data.SqlClient;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class TankListDO : DataObject
	{
		#region Private Methods
		private ArrayList tankList;
		#endregion

		#region Contructor
		/// <summary>
		/// This is the default constructor for the tank list data object class.
		/// </summary>
		public TankListDO ( )
		{
			this.tankList = new ArrayList ( );
		}
		#endregion

		#region SQL Public Methods
		/// <summary>
		/// This method will return a SQL that will retrieve the tanks which
		/// have transactions referencing them.
		/// </summary>
		/// <returns></returns>
		public void retrieveTankSelectSql ( SqlCommand cmd, string managerId )
		{
			cmd.CommandText = "SELECT DISTINCT StorageLocationID " +
						 "FROM tblTransactionLineItems INNER JOIN " +
						 "tblTransactions ON tblTransactionLineItems.TransIndex = tblTransactions.TransIndex " +
						 "WHERE tblTransactions.ManagerID = @ManagerID";

			cmd.Parameters.AddWithValue("@ManagerID", managerId);
		}

		/// <summary>
		/// This method will return a SQL that will retrieve the tanks which
		/// have transactions for a particular product referencing them.
		/// </summary>
		/// <returns></returns>
		public void retrieveTankSelectSqlForProduct(SqlCommand cmd, string productId, string managerId)
		{
			cmd.CommandText = "SELECT DISTINCT StorageLocationID " +
						 "FROM tblTransactionLineItems INNER JOIN " +
						 "tblTransactions ON tblTransactionLineItems.TransIndex = tblTransactions.TransIndex " +
						 "WHERE tblTransactionLineItems.Product = @ProductID AND tblTransactions.ManagerID = @ManagerID";

			cmd.Parameters.AddWithValue("@ProductID", productId);
			cmd.Parameters.AddWithValue("@ManagerID", managerId);
		}

		/// <summary>
		/// This mehtod will retrieve the max and min inventory dates from the 
		/// transaction table. It will call a private method to create the month
		/// year list.
		/// </summary>
		/// <param name="dataSet"></param>
		public void loadTankListData ( DataSet dataSet )
		{
			tankList.Add ( "{All}" );

			if (dataSet != null)
			{
				System.Data.DataTable table = dataSet.Tables[0];

				foreach (DataRow row in table.Rows)
				{
					tankList.Add ( DataObject.getValue<string>(row["StorageLocationID"], "{No Storage Location ID}") );
				}
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the month data list.
		/// </summary>
		[DataMember]
		public ArrayList TankList
		{
			get { return this.tankList; }
		}
		#endregion

		#region Override Methods
		override public string getUpdateCommand ( )
		{
			return null;
		}

		override public string getDeleteCommand ( )
		{
			return null;
		}

		override public string getInsertCommand ( )
		{
			return null;
		}

		override public string getSelectCommand ( )
		{
			return "SELECT * from tblTransactions";
		}

		public override void GetSelectCommand(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT * from tblTransactions";
		}

		public override void GetInsertCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetDeleteCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}

		public override void GetUpdateCommand(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
