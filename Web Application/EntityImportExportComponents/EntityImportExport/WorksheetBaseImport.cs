namespace EntityImportExport
{
	using System;
	using System.Xml;
	using System.Collections;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.UtilityObjects;
	using System.Collections.Generic;
	using System.ComponentModel;

	using FMCore;
	using System.Web;

	public abstract class WorksheetBaseImport : WorksheetBase
	{
		#region Protected data members
		protected CompanyClass company;
		protected EquipmentClass equipment;
		protected PersonRoleMapClass personRoleMap;
		protected QualificationClass personQualification;
		protected QualificationClass personLicense;
		protected ProductClass product;
		protected SecurityClass security;
		protected SiteClass site;
		protected ArrayList headerColumns;
		protected ArrayList recordRows = new ArrayList();
		protected XmlNamespaceManager nsMgr;
		protected bool firstRow;
		#endregion

		public Hashtable NumberFormatList = new Hashtable(128);

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Worksheet base class.
		/// </summary>
		/// <param name="wrkshtName"></param>
		public WorksheetBaseImport(string wrkshtName)
			: base(WorksheetBase.WORKSHEET_COMPANIES)
		{
			this.security = new SecurityClass();
			base.importExportException.ClearMessages();
			this.headerColumns = new ArrayList(128);
			this.NumberFormatList = new Hashtable(128);
		}
		#endregion

		public List<EntityImportExportAttribute> RootAttributeList { get; set; }

		public XmlNamespaceManager NameSpaceManager
		{
			get { return this.nsMgr; }
			set { this.nsMgr = value; }
		}

		public XmlNode WorksheetNode
		{
			get { return base.worksheetNode; }
			set { base.worksheetNode = value; }
		}

		public SecurityClass Security
		{
			get { return this.security; }
			set { this.security = value; }
		}

		public SiteClass Site
		{
			get { return this.site; }
			set { this.site = value; }
		}

		public ArrayList RecordRows
		{
			get { return this.recordRows; }
		}



		#region Abstract methods
		public abstract void ParseWorksheet();
		#endregion

		#region Public methods

		/// <summary>
		/// This method will return the Site guid for a given Site ID.
		/// </summary>
		/// <param name="siteID"></param>
		/// <returns></returns>
		public Guid GetSiteGuid(string siteID)
		{
			Guid siteGuid = FMChannelHelper.MakeCall<ISites, Guid>(x => x.GetByID(this.security, siteID, false).IdentityGuid);
			return siteGuid;
		}

		#endregion

		#region Protected methods
		/// <summary>
		/// This method will parse the row that contains the column names and build
		/// an array list of those name. The list will be used for matching up with
		/// the data rows.
		/// </summary>
		/// <param name="headerRow"></param>
		protected void ParseWorksheetHeaderCell(XmlNode headerCell)
		{
			if (headerCell == null)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_008,
														 ImportExportException.EXCEPTION_TYPES.CRITICAL);
				throw base.importExportException;
			}

			string columnName = headerCell.InnerText;

			if ((columnName == null) || (columnName.Length <= 0))
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_008,
														  ImportExportException.EXCEPTION_TYPES.CRITICAL);
				throw base.importExportException;
			}

			if (this.headerColumns.Contains(columnName.ToUpper()) == true)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_009,
														  ImportExportException.EXCEPTION_TYPES.CRITICAL);
				throw base.importExportException;
			}

			this.headerColumns.Add(columnName.ToUpper());
		}

		/// <summary>
		/// This method will parse each Row and build an array of
		/// that contains column name / data hash tables.
		/// </summary>
		public void ParseSheet()
		{
			if (this.worksheetNode == null)
			{
				return;
			}

			XmlNodeList XMLRowList = this.worksheetNode.SelectNodes("ss:Table/ss:Row", this.nsMgr);

			if (XMLRowList == null
			|| XMLRowList.Count == 0)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_001 + this.WorksheetName,
																	  ImportExportException.EXCEPTION_TYPES.ERROR);
				throw base.importExportException;
			}

			recordRows = new ArrayList();
			firstRow = true;

			foreach (XmlNode rowNode in XMLRowList)
			{
				if (!firstRow
				|| headerColumns.Count == 0)
					ParseRowNode(rowNode);

				firstRow = false;
			}
		}

		/// <summary>
		/// This method will parse each Row and build an array of
		/// that contains column name / data hash tables.
		/// </summary>
		/// <param name="rootID">The root ID.</param>
		/// <param name="rootValue">The root value.</param>
		public void ParseSheet(string rootID, string rootValue)
		{
			if (this.worksheetNode == null)
			{
				return;
			}

			XmlNodeList XMLRowList = this.worksheetNode.SelectNodes("ss:Table/ss:Row", this.nsMgr);

			if (XMLRowList == null
			|| XMLRowList.Count == 0)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_001 + this.WorksheetName,
																	  ImportExportException.EXCEPTION_TYPES.ERROR);
				throw base.importExportException;
			}

			recordRows = new ArrayList();
			firstRow = true;

			foreach (XmlNode rowNode in XMLRowList)
			{
				if (!firstRow
				|| headerColumns.Count == 0)
					ParseRowNode(rowNode, rootID, rootValue);

				firstRow = false;
			}
		}
		/// <summary>
		/// This method will parse each Row element and build an array of
		/// that contains column name / data hash tables.
		/// </summary>
		/// <param name="rowNode"></param>
		protected void ParseRowNode(XmlNode rowNode, string rootID, string rootValue)
		{
			int columnCount = 0;

			if (rowNode == null)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_008,
														 ImportExportException.EXCEPTION_TYPES.CRITICAL);
				throw base.importExportException;
			}

			XmlNodeList cellList = rowNode.SelectNodes("ss:Cell", this.nsMgr);

			if (cellList == null
			|| cellList.Count == 0)
				throw new NullReferenceException("No Cells in Sheet Row " + this.WorksheetName);

			XmlNodeList dataList;

			Hashtable columnNameDataList = new Hashtable(512);

			foreach (XmlNode cellNode in cellList)
			{
				// Look for cell ss:Index attribute
				if (cellNode.Attributes["ss:Index"] != null)
				{
					// Get the column number for this data and skip over the unused columns
					int idx = int.Parse(cellNode.Attributes["ss:Index"].InnerText);
					while (columnCount < idx - 1)
					{
						columnNameDataList.Add(this.headerColumns[columnCount++], "");
					}
				}

				dataList = cellNode.SelectNodes("ss:Data", this.nsMgr);
				XmlNode dataNode = dataList.Item(0);

				// The first row contains the column names for all the rows. Build
				// an array list of those column name for later use.
				if (this.firstRow == true)
				{
					this.ParseWorksheetHeaderCell(dataNode);
				}
				else
				{
					if (columnCount < this.headerColumns.Count)
					{
						if (dataNode == null)
						{
							columnNameDataList.Add(this.headerColumns[columnCount], "");
						}
						else
						{
							string sData = HttpUtility.HtmlEncode(dataNode.InnerText);

							string headerName = headerColumns[columnCount].ToString();

							if (rootValue != null && headerName.Equals(rootID) && sData.NotEquals(rootValue))
							{
								return;
							}

							XmlAttribute datatype = dataNode.Attributes["ss:Type"];
							if (datatype != null && datatype.Value == "Number")
							{
								XmlAttribute styleID = cellNode.Attributes["ss:StyleID"];
								if ((styleID != null) && (NumberFormatList.ContainsKey(styleID.Value)))
								{
									string numberFormat = NumberFormatList[styleID.Value].ToString();
									if (!standardformatType.Contains(numberFormat))
									{
										string format = "{0:";
										format += numberFormat;
										format += "}";
										sData = string.Format(format, Convert.ToInt64(dataNode.InnerText));
									}
								}
							}

							columnNameDataList.Add(this.headerColumns[columnCount], sData);
						}

						columnCount++;
					}
					else
					{
						// Error: data and header count differ.
						base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_006,
																  ImportExportException.EXCEPTION_TYPES.CRITICAL);
						throw base.importExportException;
					}
				}
			}

			if (this.firstRow == false)
			{
				this.recordRows.Add(columnNameDataList);
			}
		}
		/// <summary>
		/// This method will parse each Row element and build an array of
		/// that contains column name / data hash tables.
		/// </summary>
		/// <param name="rowNode"></param>
		protected void ParseRowNode(XmlNode rowNode)
		{
			int columnCount = 0;

			if (rowNode == null)
			{
				base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_008,
														 ImportExportException.EXCEPTION_TYPES.CRITICAL);
				throw base.importExportException;
			}

			XmlNodeList cellList = rowNode.SelectNodes("ss:Cell", this.nsMgr);

			if (cellList == null
			|| cellList.Count == 0)
				throw new NullReferenceException("No Cells in Sheet Row " + this.WorksheetName);

			XmlNodeList dataList;

			Hashtable columnNameDataList = new Hashtable(512);

			foreach (XmlNode cellNode in cellList)
			{
				// Look for cell ss:Index attribute
				if (cellNode.Attributes["ss:Index"] != null)
				{
					// Get the column number for this data and skip over the unused columns
					int idx = int.Parse(cellNode.Attributes["ss:Index"].InnerText);
					while (columnCount < idx - 1)
					{
						columnNameDataList.Add(this.headerColumns[columnCount++], "");
					}
				}

				dataList = cellNode.SelectNodes("ss:Data", this.nsMgr);
				XmlNode dataNode = dataList.Item(0);

				// The first row contains the column names for all the rows. Build
				// an array list of those column name for later use.
				if (this.firstRow == true)
				{
					this.ParseWorksheetHeaderCell(dataNode);
				}
				else
				{
					if (columnCount < this.headerColumns.Count)
					{
						if (dataNode == null)
						{
							columnNameDataList.Add(this.headerColumns[columnCount], "");
						}
						else
						{
							string sData = dataNode.InnerText;

							string headerName = headerColumns[columnCount].ToString();

							foreach (var rootAttribute in this.RootAttributeList)
							{
								if (!string.IsNullOrEmpty(rootAttribute.Value) && headerName.Equals(rootAttribute.ColumnName) && sData.NotEquals(rootAttribute.Value))
								{
									if (rootAttribute.ColumnName == "SITE*")
									{
										base.importExportException.AppendMessage(
											"Root value: " + sData + " is not valid. Skipping row.", ImportExportException.EXCEPTION_TYPES.ERROR);
									}

									return;
								}
							}


							XmlAttribute datatype = dataNode.Attributes["ss:Type"];
							if (datatype != null && datatype.Value == "Number")
							{
								XmlAttribute styleID = cellNode.Attributes["ss:StyleID"];
								if ((styleID != null) && (NumberFormatList.ContainsKey(styleID.Value)))
								{
									string numberFormat = NumberFormatList[styleID.Value].ToString();
									if (!standardformatType.Contains(numberFormat))
									{
										string format = "{0:";
										format += numberFormat;
										format += "}";
										sData = string.Format(format, Convert.ToInt64(dataNode.InnerText));
									}
								}
							}

							columnNameDataList.Add(this.headerColumns[columnCount], sData);
						}

						columnCount++;
					}
					else
					{
						// Error: data and header count differ.
						base.importExportException.AppendMessage(ImportExportException.IMPORT_MSG_006,
																	ImportExportException.EXCEPTION_TYPES.CRITICAL);
						throw base.importExportException;
					}
				}
			}

			if (this.firstRow == false)
			{
				this.recordRows.Add(columnNameDataList);
			}
		}

		#endregion
	}
}
