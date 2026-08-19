namespace FMBusinessServices.ServiceClasses
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.Exceptions;
	using FMBusinessServices.DataAccessLayer;
	public class ExStarsIrsErrorCodeLoader
	{
		#region Constants and Fields
			private readonly ConsolidatedDAClass consolidatedDa = new ConsolidatedDAClass();
			//private readonly ExStarsSiteConfigExpanded config;
			private SecurityClass Security = null;
		#endregion

		public ExStarsIrsErrorCodeLoader(SecurityClass security)
		{			
			this.Security = security;
		}

		public ExStarsIrsErrorCodeClassList GetAll()
		{
			string sql ="SELECT [CodeGroup],[Code],[Description],[ElementId] FROM [dbo].[tblExStarsIrsErrorCodes]";
			ExStarsIrsErrorCodeClassList list = new ExStarsIrsErrorCodeClassList();
			try
			{
				using (var cmd = new SqlCommand(sql))
				{
					cmd.CommandType = CommandType.Text;
					DataSet dataSet = this.consolidatedDa.GetDataSet(cmd, Security);
					if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
					{
						return null;
					}
					foreach (DataRow row in dataSet.Tables[0].Rows)
					{
						ExStarsIrsErrorCodeClass element = new ExStarsIrsErrorCodeClass();
						string codeGroup =  DataObject.getValue(row["CodeGroup"], "");
						element.CodeGroup = (ExStarsIrsErrorCodeClass.CodeGroupEnum)Enum.Parse(typeof(ExStarsIrsErrorCodeClass.CodeGroupEnum), codeGroup);
						element.Code = DataObject.getValue(row["Code"], "");
						element.Description = DataObject.getValue(row["Description"], "");
						element.ElementId = DataObject.getValue(row["ElementId"], "");
						list.Add(element, element);
					}
				}
			}
			catch (Exception e)
			{
				throw new ExStarsSqlException(e, "SQL error: {0}", sql);
			}
			return list;
		}
	}
}