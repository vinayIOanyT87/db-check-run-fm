namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.ServiceRequests;


	public delegate void ClickHandler(string listName, int row, int column, string columnName);

	[Serializable]
	public class ListViewDataSet : System.Data.DataSet
	{
		public event ClickHandler ClickEvent;

		#region Attributes

		private System.Data.DataTable table;
		public ListViewDO listViewDO;
		private string listName;
		protected AccountingSite site;
		public DataGrid dataGrid;
		private BaseCollections lineItems;
		private byte volumeDecimalPlaces;
		private byte massDecimalPlaces;
		private bool loadByWeight;
		private readonly NumberFormatInfo numberFormat = new NumberFormatInfo();
		private string navigateURL = "";
		private System.Collections.Specialized.StringDictionary columnURLs;
		public string Sort = "";
		public bool SortDirection = true;
		private QuantityDisplay quantityDisplay = QuantityDisplay.GROSS_AND_NET;
		private bool showCost = false;
		private bool descKey = false;

		private readonly SecurityClass Security;

		private readonly List<ListViewColumnDO> hiddenColumnList = new List<ListViewColumnDO>();

		private Logger logger;
		private DateTimeFormatInfo dateFormat;

		private Style invErrorStyle;
		private Style invErrorLinkStyle;
		private Style closedOutStyle;
		private Style closedOutLinkStyle;
		private Style outOfToleranceStyle;
		private Style outOfToleranceLinkStyle;
		private Style physInvStyle;
		private Style physInvLinkStyle;
		private Style defaultStyle;
		private Style defaultLinkStyle;
		private Style transErrorStyle;
		private Style transErrorLinkStyle;

		#endregion

		#region Constructor
		public ListViewDataSet(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid, AccountingSite site)
		{
			this.site = site;
			this.Security = security;
			this.GetCustomListData(security, listViewType, typeGuid, site.CurrentSiteName, Guid.Empty);
			this.Init();
		}

		/// <summary>
		/// This constructor is used for working with ledger list views when a specific index is known
		/// </summary>
		/// <param name="security"></param>
		/// <param name="ledgerListViewGuid"></param>
		/// <param name="site"></param>
		public ListViewDataSet(SecurityClass security, Guid ledgerListViewGuid, AccountingSite site)
		{
			this.site = site;
			this.Security = security;
			this.GetCustomListData(security, LISTVIEW_TYPE.STANDARD, ListViewClass.GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE.LEDGER),
									site.CurrentSiteName, ledgerListViewGuid);
			this.Init();
		}

		#endregion

		#region Properties
		/// <summary>
		/// This property will get and set the show cost flag. If true
		/// the ledger will show cost instead of volume.
		/// </summary>
		public bool ShowCost
		{
			get { return this.showCost; }
			set { this.showCost = value; }
		}

		/// <summary>
		/// Sets or gets the date format information.
		/// </summary>
		public DateTimeFormatInfo DateFormatInfo
		{
			get { return this.dateFormat; }
			set { this.dateFormat = value; }
		}
		#endregion

		#region Methods
		private void Init()
		{
			this.logger = new Logger("Accounting");
			this.table = new System.Data.DataTable();
			this.columnURLs = new System.Collections.Specialized.StringDictionary();
			this.dateFormat = DateTimeFormatInfo.CurrentInfo;

			FMChannelHelper.MakeCall<IHardwareKey>(
				hardwareKeyChannel =>
				{
					hardwareKeyChannel.ReadHardwareKey();
					this.descKey = hardwareKeyChannel.IsDescKey();
				});
		}

		public void HighlightRowStatus(BaseLineItemDO.StatusFlags flag)
		{
			Logger logger = new Logger("Accounting");
			logger.Debug("ListView.highlightRowStatus() : Not implemented yet.");
		}

		public void SetSite(AccountingSite site)
		{
			this.site = site;
		}

		public void SetNavigateURL(string navigateURL)
		{
			this.navigateURL = navigateURL;
		}

		public void SetColumnURL(string columnName, string columnURL)
		{
			this.columnURLs.Add(columnName, columnURL);
		}

		public void SetDataGrid(DataGrid dataGrid)
		{
			this.dataGrid = dataGrid;

			FMChannelHelper.MakeCall<IDataDictionariesClass>(
				dict =>
					{
						ListViewColumnDO columnDO;

						for (int index = 0; (columnDO = this.listViewDO[index]) != null; ++index)
						{
							string columnName = columnDO.DataDictionaryType ? dict.Get(this.Security.SiteGuid, columnDO.ColumnName) : columnDO.ColumnName;
                     var column = new System.Data.DataColumn(columnName, columnDO.DataType);

							if (this.table.Columns.Contains(column.ColumnName))
							{
								this.logger.Debug("ListView.setDataGrid: Duplicate column '" + column.ColumnName + "'");
							}
							else
							{
								this.table.Columns.Add(column);
							}
						}
					});

			// add hidden columns
			foreach (ListViewColumnDO hiddenColumnDO in this.hiddenColumnList)
			{
				var column = new System.Data.DataColumn(hiddenColumnDO.ColumnName, hiddenColumnDO.DataType);
				this.table.Columns.Add(column);
			}

			this.Tables.Add(this.table);
			dataGrid.DataSource = this;

			dataGrid.AutoGenerateColumns = true;

			dataGrid.HeaderStyle.Wrap = true;
			dataGrid.Style.Add("text-align", "right");
			dataGrid.Width = 700;

			dataGrid.ItemCreated += this.DataGrid_ItemCreated;
			dataGrid.ItemDataBound += this.DataGrid_ItemDataBound;
		}

		public void BindData(BaseCollections lineItems,
							QuantityDisplay quantityDisplay,
							byte volumeDecimalPlaces,
							byte massDecimalPlaces,
							bool loadByWeight
							)
		{
			DateTimeOffset startTime = DateTimeOffset.Now;
			this.quantityDisplay = quantityDisplay;
			this.volumeDecimalPlaces = volumeDecimalPlaces;
			this.massDecimalPlaces = massDecimalPlaces;
			this.loadByWeight = loadByWeight;

			this.numberFormat.NumberGroupSizes = this.site.CurrentSite.GetNumberGroupSizes();
			this.numberFormat.NumberDecimalSeparator = this.site.CurrentSite.NumberDecimalSeparator;
			this.numberFormat.NumberGroupSeparator = this.site.CurrentSite.NumberGroupSeparator;


			this.lineItems = lineItems;

			try
			{
				foreach (BaseLineItemDO lineItem in lineItems)
				{
					System.Data.DataRow row = this.table.NewRow();

					ListViewColumnDO columnDO;
					int index;
					for (index = 0; (columnDO = this.listViewDO.getListViewColumn(index)) != null; ++index)
					{
						row[index] = this.GetCellData(lineItem, columnDO, quantityDisplay);
					}

					// Add hidden fields
					foreach (ListViewColumnDO hiddenColumnDO in this.hiddenColumnList)
					{
						row[index++] = this.GetCellData(lineItem, hiddenColumnDO, quantityDisplay);
					}
					this.table.Rows.Add(row);
				}
				this.dataGrid.DataSource = this.table;
				this.dataGrid.DataBind();

				DateTimeOffset stopTime = DateTimeOffset.Now;
				{
					TimeSpan elapsedTime = stopTime - startTime;
					this.logger.Perform("ListViews.BindData(" + this.listName + ") completed in " + elapsedTime.ToString() + ".");
				}
			}
			catch (Exception except)
			{
				string errorMsg = "Error: List views not configured! " + except.Message.ToString();
				throw new Exception(errorMsg);
			}
		}

		private ListViewDO GetCustomListData(SecurityClass security, LISTVIEW_TYPE listViewType, Guid typeGuid, string site, Guid listViewGuid)
		{
         ListViewSR sr = new ListViewSR
         {
            Security = security,
            Site = site,
            Type = listViewType,
            TypeGuid = typeGuid,
            ListViewGuid = listViewGuid
         };

         this.listViewDO = FMChannelHelper.MakeCall<IListViewProcessor, ListViewDO>(x => x.Process(sr));
			this.listName = this.listViewDO.ListName;

			// Special handling for hidden fields
			if( listViewType == LISTVIEW_TYPE.STANDARD)
			{
				STANDARD_FIELD_TYPE[] fieldList = ListViewClass.GetStandardViewFields(ListViewClass.GetStandardTypeFromGuid(typeGuid));
				foreach (STANDARD_FIELD_TYPE field in fieldList)
				{
					string dataPath = ListViewFieldClass.StandardFieldTypeID(field, false);
					ListViewFieldClass currentField = new ListViewFieldClass(
							LISTVIEW_FIELD_TYPE.STANDARD_FIELD, 
							ListViewFieldClass.GetGuidFromStandardFieldType(field), 
							0,
							dataPath);
					
					if (currentField.IsHidden)
					{
						currentField.DataPath = dataPath;
						this.hiddenColumnList.Add(new ListViewColumnDO(currentField));
					}
				}
			}
			return this.listViewDO;
		}


		public List<ListViewColumnDO> GetHiddenColumnList()
		{
			return hiddenColumnList;
		}


		protected string GetCellData(BaseLineItemDO lineItem, ListViewColumnDO columnDO, QuantityDisplay quantityDisplay)
		{
			string propertyName;
			int marker1;
			int marker2 = 0;
			string keyName;
			string cellValue = "";
			string DataPath = columnDO.DataPath;
			marker1 = DataPath.IndexOf('[');

			if (marker1 > 0)
			{
				propertyName = DataPath.Substring(0, marker1);
				marker2 = DataPath.IndexOf(']');
				keyName = DataPath.Substring(marker1 + 1, marker2 - marker1 - 1);
				keyName = keyName.Trim();
			}
			else
			{
				propertyName = DataPath;
				keyName = DataPath.Trim();
			}

			PropertyInfo property = lineItem.GetType().GetProperty(propertyName);

			if (property == null)
			{
				this.logger.Error("Property [" + propertyName + "] not found for " + this.listName);
				return cellValue;
			}

			object o = property.GetValue(lineItem, null);
         if ((o != null) && (marker2 > 0))
			{
				// We are expect either a Hashtable or a Dictionary<> object.
				if (o.GetType() == typeof(Hashtable))
				{
               Hashtable hashtable = (Hashtable)o;
               o = hashtable[keyName];
				}
				else
				{
					// We are going to throw an exception if the object is not a Dictionary<> type.
					// Dictionary<> inherits from IDictionary.
					IDictionary dict = o as IDictionary;

					if (dict == null)
					{
						throw new Exception("Invalid object expected, should be HashTable or IDictionary.");
					}
					else
					{
						o = dict[keyName];
					}
				}
			}

			if (o == null)
			{
				cellValue = "N/A";
			}
			else
			{
				Type type = o.GetType();

				if (type == typeof(string))
				{
					// vthompson 8/8/2008
					if (keyName.ToUpper().Equals("INVENTORYDATE") ||
						keyName.ToUpper().Equals("TRANSACTIONDATE"))
					{
						string invDate = (string)o;

						if (invDate.ToUpper().StartsWith("TOT") == true)
						{
							cellValue = invDate;
						}
						else
						{
							// vthompson 10/22/208
							// Have to get the date in a standard format or any date format that
							// does not conform to an expected format will cause an error.
							invDate = this.site.UnformatDate(invDate);

							char[] separatorList = { '-' };
							string[] stringList = invDate.Split(separatorList);

							int year = Convert.ToInt32(stringList[0]);
							int month = Convert.ToInt32(stringList[1]);
							int day = Convert.ToInt32(stringList[2]);


							if (year < 50)
							{
								year = 2000 + year;
							}
							else if (year < 1000)
							{
								year = 1900 + year;
							}

							DateTimeOffset newInvDate = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
							cellValue = this.site.FormatDate(newInvDate);
						}
					}
					else
					{
						cellValue = (string)o;
					}
				}
				else if (type == typeof(QuantityDO))
				{
					QuantityDO quantityDO = (QuantityDO)o;

					if (this.showCost == true)
					{
						if ((lineItem.CheckFlag(keyName, BaseLineItemDO.Status.NA) == false) || (quantityDO.NetPrice != 0))
						{
							BaseLineItemDO.StatusFlags flags = (BaseLineItemDO.StatusFlags)lineItem.GetCellFlags(columnDO.ColumnName);
							cellValue = this.FormatQuantity(quantityDO, quantityDisplay, flags, columnDO);
						}
						else
						{
							cellValue = "N/A";
						}
					}
					else
					{
						if ((lineItem.CheckFlag(keyName, BaseLineItemDO.Status.NA) == false) || (quantityDO.Net != 0))
						{
							BaseLineItemDO.StatusFlags flags = (BaseLineItemDO.StatusFlags)lineItem.GetCellFlags(columnDO.ColumnName);
							cellValue = this.FormatQuantity(quantityDO, quantityDisplay, flags, columnDO);
						}
						else
						{
							cellValue = "N/A";
						}
					}
				}
				else if (keyName.ToUpper() == "GROSSQUANTITY" || keyName.ToUpper() == "NETQUANTITY")
				{
					if (type == typeof(double))
					{
						double qty = (double)o;
						qty = this.site.ConvertFromSi(Math.Abs(qty), AccountingSite.ConversionUnits.VOLUME);
						cellValue = this.site.GetFormattedValue(qty, SITE_VARIABLE_TYPE.VOLUME);
					}
				}
				// vthompson 8/8/2008
				else if (type == typeof(DateTimeOffset))
				{
					DateTimeOffset date = (DateTimeOffset)o;
					cellValue = this.site.FormatDate(date);
				}
				// JS20100915 WI-17454
				else if (keyName.ToUpper().Equals("ALTERNATIVENETVOLUME"))
				{
					double? alternativeNetVolume = o as double?;
					if (alternativeNetVolume != null)
					{
						// do not need to adjust for SI, this should be in proper units
						cellValue = alternativeNetVolume.Value.ToString();
					}
					else
						cellValue = "";
				}
				else
				{
					cellValue = this.site.FormatDataObject(o); //Get rid of the if/else below.
				}
			}

			return cellValue;
		}

		/// <summary>
		/// This method formats the volume or price values for displaying in the
		/// grid. It determines if the G or N postfix should be apply or both.
		/// It formats the volume or price values with the appropriate unit type.
		/// </summary>
		/// <param name="volume"></param>
		/// <param name="grossNetFlag"></param>
		/// <returns></returns>
		private string FormatQuantity(QuantityDO quantity, QuantityDisplay quantityDisplay, BaseLineItemDO.StatusFlags flags, ListViewColumnDO column)
		{
			string stringValue = "";
			string grossLeftParen = "";
			string grossRightParen = "";
			string netLeftParen = "";
			string netRightParen = "";
			string massLeftParen = "";
			string massRightParen = "";
			string packageLeftParen = "";
			string packageRightParen = "";
			string dollarSign = "$";
			this.numberFormat.NumberDecimalDigits = this.volumeDecimalPlaces;
			string grossString = quantity.Gross.ToString("N", this.numberFormat);
			string netString = quantity.Net.ToString("N", this.numberFormat);
			this.numberFormat.NumberDecimalDigits = this.massDecimalPlaces;
			string massString = quantity.Mass.ToString("N", this.numberFormat);
			this.numberFormat.NumberDecimalDigits = (this.loadByWeight) ? this.massDecimalPlaces : this.volumeDecimalPlaces;
			string packageString = quantity.Package.ToString("N", this.numberFormat);
			this.numberFormat.NumberDecimalDigits = 2;
			string grossPriceString = quantity.GrossPrice.ToString("N", this.numberFormat);
			string netPriceString = quantity.NetPrice.ToString("N", this.numberFormat);
			string massPriceString = quantity.MassPrice.ToString("N", this.numberFormat);
			string moniker = " " + quantity.Moniker;


			if (column.IsAggregateField)
			{
				double grossQuantity = this.GetAggregateQuantity(quantity, column.AggregateType, QuantityDisplay.GROSS);
				double netQuantity = this.GetAggregateQuantity(quantity, column.AggregateType, QuantityDisplay.NET);
				double massQuantity = this.GetAggregateQuantity(quantity, column.AggregateType, QuantityDisplay.MASS);
				double packageQuantity = this.GetAggregateQuantity(quantity, column.AggregateType, QuantityDisplay.PACKAGE);
				if (column.AggregateType != LedgerAggregateColumnClass.AggregateType.CustomFunction)
				{
					this.numberFormat.NumberDecimalDigits = this.volumeDecimalPlaces;
					grossString = grossQuantity.ToString("N", this.numberFormat);
					netString = netQuantity.ToString("N", this.numberFormat);
					this.numberFormat.NumberDecimalDigits = this.massDecimalPlaces;
					massString = massQuantity.ToString("N", this.numberFormat);
					this.numberFormat.NumberDecimalDigits = (this.loadByWeight) ? this.massDecimalPlaces : this.volumeDecimalPlaces;
					packageString = packageQuantity.ToString("N", this.numberFormat);
				}
				else
				{
					grossString = grossQuantity.ToString();
					netString = netQuantity.ToString();
					massString = massQuantity.ToString();
					packageString = packageQuantity.ToString();
				}
			}

			if ((quantityDisplay == QuantityDisplay.GROSS) || (quantityDisplay == QuantityDisplay.GROSS_AND_NET))
			{
				if (quantity.GrossInventoryChange < 0 && !this.showCost)
				{
					grossLeftParen = "(";
					grossRightParen = ")";
				}

				if (quantity.GrossPriceInventoryChange < 0 && this.showCost && !this.loadByWeight)
				{
					grossLeftParen = "(";
					grossRightParen = ")";
				}

				if (quantity.MassPriceInventoryChange < 0 && this.showCost && this.loadByWeight)
				{
					grossLeftParen = "(";
					grossRightParen = ")";
				}

				string marker = (quantityDisplay == QuantityDisplay.GROSS_AND_NET) ? "&nbsp;G" : "";

				// Show the pricing value or quantity value.
				if (this.showCost == true)
				{
					stringValue = grossLeftParen + dollarSign;
					stringValue += (this.loadByWeight) ? massPriceString : grossPriceString;
					stringValue += marker + grossRightParen + moniker;
				}
				else
				{
					stringValue = grossLeftParen + grossString + marker + grossRightParen + moniker;
					if (quantity.GrossInventoryChange == 0)
					{
						stringValue = this.AddAsteriskToHyperLink(stringValue, flags);
					}
				}

				stringValue = this.AddAsteriskToHyperLinkIfReversalIsFound(stringValue, flags);

				if (quantityDisplay == QuantityDisplay.GROSS_AND_NET)
				{
					stringValue += "<br/>";
				}
			}

			if (quantityDisplay == QuantityDisplay.NET || quantityDisplay == QuantityDisplay.GROSS_AND_NET)
			{
				if (quantity.NetInventoryChange < 0 && !this.showCost)
				{
					netLeftParen = "(";
					netRightParen = ")";
				}

				if (quantity.NetPriceInventoryChange < 0 && this.showCost && !this.loadByWeight)
				{
					netLeftParen = "(";
					netRightParen = ")";
				}

				if (quantity.MassPriceInventoryChange < 0 && this.showCost && this.loadByWeight)
				{
					netLeftParen = "(";
					netRightParen = ")";
				}

				string marker = (quantityDisplay == QuantityDisplay.GROSS_AND_NET) ? "&nbsp;N" : "";

				// Show the pricing value or quantity value.
				if (this.showCost == true)
				{
					stringValue += grossLeftParen + dollarSign;
					stringValue += (this.loadByWeight) ? massPriceString : netPriceString;
					stringValue += marker + grossRightParen + moniker;
				}
				else
				{
					stringValue += netLeftParen + netString + marker + netRightParen + moniker;

					// For GROSS_AND_NET will have already been added for gross
					if (quantityDisplay == QuantityDisplay.NET && quantity.NetInventoryChange == 0)
					{
						stringValue = this.AddAsteriskToHyperLink(stringValue, flags);
					}
				}

				stringValue = this.AddAsteriskToHyperLinkIfReversalIsFound(stringValue, flags);
			}

			if (quantityDisplay == QuantityDisplay.MASS)
			{
				if (quantity.MassInventoryChange < 0 && !this.showCost)
				{
					massLeftParen = "(";
					massRightParen = ")";
				}

				if (quantity.MassPriceInventoryChange < 0 && this.showCost && this.loadByWeight)
				{
					massLeftParen = "(";
					massRightParen = ")";
				}

				if (quantity.NetPriceInventoryChange < 0 && this.showCost && !this.loadByWeight)
				{
					massLeftParen = "(";
					massRightParen = ")";
				}

				// Show the pricing value or quantity value.
				if (this.showCost == true)
				{
					stringValue += massLeftParen + dollarSign;
					stringValue += (this.loadByWeight) ? massPriceString : netPriceString;
					stringValue += massRightParen + moniker;
				}
				else
				{
					stringValue += massLeftParen + massString + massRightParen + moniker;
					if (quantity.MassInventoryChange == 0)
					{
						stringValue = this.AddAsteriskToHyperLink(stringValue, flags);
					}
				}

				stringValue = this.AddAsteriskToHyperLinkIfReversalIsFound(stringValue, flags);
			}

			if (quantityDisplay == QuantityDisplay.PACKAGE)
			{
				if (quantity.PackageInventoryChange < 0 && !this.showCost)
				{
					packageLeftParen = "(";
					packageRightParen = ")";
				}

				if (quantity.MassPriceInventoryChange < 0 && this.showCost && this.loadByWeight)
				{
					packageLeftParen = "(";
					packageRightParen = ")";
				}

				if (quantity.NetPriceInventoryChange < 0 && this.showCost && !this.loadByWeight)
				{
					packageLeftParen = "(";
					packageRightParen = ")";
				}

				// Show the pricing value or quantity value.
				if (this.showCost == true)
				{
					stringValue += packageLeftParen + dollarSign;
					stringValue += (this.loadByWeight) ? massPriceString : netPriceString;
					stringValue += packageRightParen + moniker;
				}
				else
				{
					stringValue += packageLeftParen + packageString + packageRightParen + moniker;
					if (quantity.PackageInventoryChange == 0)
					{
						stringValue = this.AddAsteriskToHyperLink(stringValue, flags);
					}
				}

				stringValue = this.AddAsteriskToHyperLinkIfReversalIsFound(stringValue, flags);
			}

			return stringValue;
		}

		private double GetAggregateQuantity(QuantityDO quantity, LedgerAggregateColumnClass.AggregateType aggregateType, QuantityDisplay quantityDisplay)
		{
			switch (aggregateType)
			{
				case LedgerAggregateColumnClass.AggregateType.NetGross:
					switch (quantityDisplay)
					{
						case QuantityDisplay.GROSS:
							return quantity.Gross;
						case QuantityDisplay.NET:
							return quantity.Net;
						case QuantityDisplay.MASS:
							return quantity.Mass;
						default:
							return quantity.Package;
					}

				case LedgerAggregateColumnClass.AggregateType.Number01:
					quantity.Gross = quantity.Number01;
					quantity.Net = quantity.Number01;
					return quantity.Number01;

				case LedgerAggregateColumnClass.AggregateType.Number02:
					quantity.Gross = quantity.Number02;
					quantity.Net = quantity.Number02;
					return quantity.Number02;

				case LedgerAggregateColumnClass.AggregateType.Number03:
					quantity.Gross = quantity.Number03;
					quantity.Net = quantity.Number03;
					return quantity.Number03;

				case LedgerAggregateColumnClass.AggregateType.Number04:
					quantity.Gross = quantity.Number04;
					quantity.Net = quantity.Number04;
					return quantity.Number04;

				case LedgerAggregateColumnClass.AggregateType.Number05:
					quantity.Gross = quantity.Number05;
					quantity.Net = quantity.Number05;
					return quantity.Number05;

				case LedgerAggregateColumnClass.AggregateType.Number06:
					quantity.Gross = quantity.Number06;
					quantity.Net = quantity.Number06;
					return quantity.Number06;

				case LedgerAggregateColumnClass.AggregateType.CustomFunction:
					quantity.Gross = quantity.Number01;
					quantity.Net = quantity.Number01;
					return quantity.Number01;

				default:
					return quantity.Gross;

			}
		}

		public string GetDataPath(string name)
		{
         return this.listViewDO != null ? FMChannelHelper.MakeCall<IDataDictionariesClass, string>(x => this.FindPath(x, name)) : string.Empty;
      }

      private string FindPath(IDataDictionariesClass dict, string name)
		{
			ListViewColumnDO columnDO;

			for ( int index = 0; ( columnDO = this.listViewDO.getListViewColumn( index ) ) != null; ++index )
			{
				string checkName = columnDO.ColumnName;

				if ( columnDO.DataDictionaryType )
				{
					checkName = dict.Get( this.Security.SiteGuid, columnDO.ColumnName );
				}

				if ( checkName == name )
				{
					return columnDO.DataPath;
				}
			}

			return string.Empty;
		}

		#endregion

		#region Private methods
		private void DataGrid_ItemCreated(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Header)
			{
				for (int Index = 1; Index < e.Item.Cells.Count; ++Index)
				{
					TableCell Cell = e.Item.Cells[Index];
					if (Index >= e.Item.Cells.Count - this.hiddenColumnList.Count)
					{
						Cell.Visible = false;
					}
					if (Cell.Controls.Count > 0)
					{
						LinkButton Button = Cell.Controls[0] as LinkButton;

						if (Button != null)
						{
							if (Button.Text == this.Sort)
							{
								Label SortedLabel = new Label();

								SortedLabel.Font.Name = "webdings";
								SortedLabel.Font.Size = FontUnit.XSmall;
								SortedLabel.Text = (this.SortDirection) ? "6" : "5";

								// A unique ID is necessary; otherwise, the sort events get muddled and do not always fire the 
								// registered sort event.
								SortedLabel.ID = "SortedLabel" + Index.ToString();

								Cell.Controls.Add(SortedLabel);

								break;
							}
						}
					}
				}
			}
			else
			{
				for (int Index = 1; Index < e.Item.Cells.Count; ++Index)
				{
					TableCell Cell = e.Item.Cells[Index];
					if (Index >= e.Item.Cells.Count - this.hiddenColumnList.Count)
					{
						Cell.Visible = false;
					}
				}
			}
		}

		/// <summary>
		/// This method will handle the data grid item data bound event. It will set the links and 
		/// style for each cell.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DataGrid_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
         if (e.Item.DataItem != null)
         {
            //Highlight Rows
            if (e.Item.DataItem.GetType() == typeof(System.Data.DataRowView))
            {
               BaseLineItemDO lineItem = (BaseLineItemDO)this.lineItems[e.Item.ItemIndex];
               Style lineItemStyle = this.GetStyle(lineItem.Flags, false, this.quantityDisplay);
               e.Item.ApplyStyle(lineItemStyle);
               for (int cellIndex = 0; cellIndex < e.Item.Cells.Count; ++cellIndex)
               {
                  TableCell cell = e.Item.Cells[cellIndex];

                  cell.Wrap = false;

                  if (cellIndex >= this.dataGrid.Columns.Count)
                  {
                     int columnIndex = cellIndex - this.dataGrid.Columns.Count;
                     ListViewColumnDO columnDO = this.listViewDO[columnIndex];
                     if (columnDO != null)
                     {
                        cell.Wrap = columnDO.IsColumnWrapped;
                        BaseLineItemDO.StatusFlags flags = (BaseLineItemDO.StatusFlags)lineItem.GetCellFlags(columnDO.ColumnName);

                        if (lineItem.CheckFlag(columnDO.ColumnName, BaseLineItemDO.Status.SUPPRESS))
                        {
                           cell.Text = "";
                        }
                        else
                        {
                           Style cellStyle;
                           if ((columnDO.IsLink == true)
                              && (lineItem.CheckFlag(columnDO.ColumnName, BaseLineItemDO.Status.SUPPRESS_LINK) == false)
                              && (lineItem.CheckFlag(BaseLineItemDO.Status.SUPPRESS_LINK) == false))
                           {
                              string url = this.navigateURL;

                              if (this.columnURLs.ContainsKey(columnDO.ColumnName) == true)
                              {
                                 url = this.columnURLs[columnDO.ColumnName];
                              }

                              string CSRFToken;
                              // Gross & Net Cell
                              if ((this.quantityDisplay == QuantityDisplay.GROSS_AND_NET) && (cell.Text.IndexOf("<br/>") != -1))
                              {
                                 cellStyle = this.GetStyle(lineItem.Flags | flags, true, QuantityDisplay.GROSS);
                                 HyperLink link = new HyperLink();
                                 link.ApplyStyle(cellStyle);
                                 link.Text = cell.Text.Substring(0, cell.Text.IndexOf("<br/>") + 4);
                                 cell.Controls.Add(link);
                                 link.ID = string.Format("{0}_{1}", e.Item.ItemIndex, link.Page.Server.UrlEncode(columnDO.ColumnName));
                                 link.NavigateUrl = url + "?Row=" + e.Item.ItemIndex.ToString() + "&Column=" + link.Page.Server.UrlEncode(columnDO.ColumnName);

                                 CSRFToken = this.Security.CSRFTokenWithParamName;

                                 if (!string.IsNullOrEmpty(CSRFToken))
                                 {
                                    link.NavigateUrl += "&" + CSRFToken;
                                 }


                                 cellStyle = this.GetStyle(lineItem.Flags | flags, true, QuantityDisplay.NET);
                                 link = new HyperLink();
                                 link.ApplyStyle(cellStyle);
                                 link.Text = cell.Text.Substring(cell.Text.IndexOf("<br/>") + 5);
                                 cell.Controls.Add(link);
                                 link.ID = string.Format("{0}_{1}", e.Item.ItemIndex, link.Page.Server.UrlEncode(columnDO.ColumnName));
                                 link.NavigateUrl = url + "?Row=" + e.Item.ItemIndex.ToString() + "&Column=" + link.Page.Server.UrlEncode(columnDO.ColumnName);
                                 CSRFToken = this.Security.CSRFTokenWithParamName;

                                 if (!string.IsNullOrEmpty(CSRFToken))
                                 {
                                    link.NavigateUrl += "&" + CSRFToken;
                                 }

                              }
                              else
                              {
                                 cellStyle = this.GetStyle(lineItem.Flags | flags, true, this.quantityDisplay);
                                 HyperLink link = new HyperLink();
                                 link.ApplyStyle(cellStyle);
                                 link.Text = cell.Text;
                                 cell.Controls.Add(link);
                                 link.ID = string.Format("{0}_{1}", e.Item.ItemIndex, link.Page.Server.UrlEncode(columnDO.ColumnName));
                                 link.NavigateUrl = url + "?Row=" + e.Item.ItemIndex.ToString() + "&Column=" + link.Page.Server.UrlEncode(columnDO.ColumnName);
                                 CSRFToken = this.Security.CSRFTokenWithParamName;
                                 if (!string.IsNullOrEmpty(CSRFToken))
                                 {
                                    link.NavigateUrl += "&" + CSRFToken;
                                 }
                              }
                           }
                           else
                           {
                              // Gross & Net Cell
                              if ((this.quantityDisplay == QuantityDisplay.GROSS_AND_NET) && (cell.Text.IndexOf("<br/>") != -1))
                              {
                                 cellStyle = this.GetStyle(lineItem.Flags | flags, false, QuantityDisplay.GROSS);
                                 Label label = new Label();
                                 label.ApplyStyle(cellStyle);
                                 label.Text = cell.Text.Substring(0, cell.Text.IndexOf("<br/>") + 4);
                                 cell.Controls.Add(label);

                                 cellStyle = this.GetStyle(lineItem.Flags | flags, false, QuantityDisplay.NET);
                                 label = new Label();
                                 label.ApplyStyle(cellStyle);
                                 label.Text = cell.Text.Substring(cell.Text.IndexOf("<br/>") + 5);
                                 cell.Controls.Add(label);
                              }

                              else if (columnDO.DataType == typeof(bool))
                              {
                                 CheckBox newCheckBox = new CheckBox
                                 {
                                    Checked = bool.Parse(cell.Text),
                                    Enabled = false
                                 };
                                 cell.Controls.Add(newCheckBox);
                              }
                              else
                              {
                                 cellStyle = this.GetStyle(lineItem.Flags | flags, false, this.quantityDisplay);
                                 cell.ApplyStyle(cellStyle);
                              }
                           }
                        }
                     }
                  }
               }
            }
         }
      }

		/// <summary>
		/// This method will add an asterisk to the hypelink based on the cell flag. If the cell
		/// flag of 0x0200 is set, then an asterisk is added to indicate that there are transaction,
		/// but the value is zero.
		/// </summary>
		/// <param name="inLink"></param>
		/// <param name="flags"></param>
		private string AddAsteriskToHyperLink(string inValue, BaseLineItemDO.StatusFlags flags)
		{
			string outValue = inValue;

			if (flags.CheckFlag(BaseLineItemDO.Status.TRANS_WITH_ZERO_VOLUME))
			{
				outValue = "*" + inValue;
			}

			return outValue;
		}

		private string AddAsteriskToHyperLinkIfReversalIsFound(string inValue, BaseLineItemDO.StatusFlags flags)
		{
			string outValue = inValue;

			if (flags.CheckFlag(BaseLineItemDO.Status.TRANS_WITH_REVERSALS))
			{
				outValue = "*" + inValue;
			}

			return outValue;
		}


		private Style GetStyle(BaseLineItemDO.StatusFlags flags, bool isLink, QuantityDisplay quantityDisplay)
		{

			if (this.defaultStyle == null)
			{
            this.invErrorStyle = new Style
            {
               CssClass = "Error"
            };
            this.invErrorLinkStyle = new Style
            {
               CssClass = "ErrorLink"
            };

            this.closedOutStyle = new Style
            {
               CssClass = "ClosedOut"
            };
            this.closedOutLinkStyle = new Style
            {
               CssClass = "ClosedOutLink"
            };

            this.outOfToleranceStyle = new Style
            {
               CssClass = "OutOfTolerance"
            };
            this.outOfToleranceLinkStyle = new Style
            {
               CssClass = "OutOfToleranceLink"
            };

            this.physInvStyle = new Style
            {
               CssClass = "PhysicalInventory"
            };
            this.physInvLinkStyle = new Style
            {
               CssClass = "PhysicalInventoryLink"
            };

            this.defaultStyle = new Style
            {
               CssClass = "tabletext"
            };
            this.defaultLinkStyle = new Style
            {
               CssClass = "DefaultLink"
            };

            this.transErrorStyle = new Style
            {
               CssClass = "TransError"
            };
            this.transErrorLinkStyle = new Style
            {
               CssClass = "TransErrorLink"
            };

         }

			if (this.descKey == false)
			{
				if (flags.Flags == BaseLineItemDO.Status.DEFAULT)
				{
               return isLink ? this.defaultLinkStyle : this.defaultStyle;
            }

            if (flags.CheckFlag(BaseLineItemDO.Status.INV_ERROR))
				{
               return isLink ? this.invErrorLinkStyle : this.invErrorStyle;
            }

            if (flags.CheckFlag(BaseLineItemDO.Status.CLOSED_OUT))
				{
               return isLink ? this.closedOutLinkStyle : this.closedOutStyle;
            }

            if ((quantityDisplay != QuantityDisplay.NET) && (flags.CheckFlag(BaseLineItemDO.Status.OUT_OF_TOLERANCE_GROSS) == true))
				{
               return isLink ? this.outOfToleranceLinkStyle : this.outOfToleranceStyle;
            }

            if ((quantityDisplay != QuantityDisplay.GROSS) && (flags.CheckFlag(BaseLineItemDO.Status.OUT_OF_TOLERANCE_NET) == true))
				{
               return isLink ? this.outOfToleranceLinkStyle : this.outOfToleranceStyle;
            }

            if (flags.CheckFlag(BaseLineItemDO.Status.PHYS_INV_EXISTS) == true)
				{
               return isLink ? this.physInvLinkStyle : this.physInvStyle;
            }
         }

         return flags.CheckFlag(BaseLineItemDO.Status.TRANS_ERROR_FLAG) == true
                ? isLink ? this.transErrorLinkStyle : this.transErrorStyle
                : isLink ? this.defaultLinkStyle : this.defaultStyle;
      }

      private void Link_Click(object sender, EventArgs e)
		{
         LinkButton button = (LinkButton)sender;
			int row = int.Parse(button.Attributes["Row"]);
			int column = int.Parse(button.Attributes["Column"]);
			string columnName = this.listViewDO[column].ColumnName;
			System.Diagnostics.Debug.WriteLine("Row: " + row + " Column : " + column + " (" + columnName + ")");
			this.ClickEvent(this.listName, row, column, columnName);
		}
		#endregion
	}
}
