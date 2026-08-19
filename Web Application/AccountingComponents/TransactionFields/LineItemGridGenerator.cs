/*****************************************************************************************************************
  FILE NAME:		LineItemGridGenerator.cs
	PURPOSE:		This class generates line item fields, binds the data to the fields and
				  sets the bindings.

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	Thomas Beckum
	VERSION:		1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-02	Richard Panachida	Corrected the defect that places the same product in the TO and FROM
												fields (CSI 3277 & 3574).
		2006-12-14	Richard Panachida	Added one line of code to the column add in order to turn wrap off (CSI 3811).
		2007-04-02	W.Gray				Change to always have CellStartIndex begin at 4.  Columns are never removed
												they are set invisible. (CSI 4402)
		05-28-2008	V. Thompson			Made changes to allow the configuration of a field's required attribute
		2008-05-29	I.Orndorff			- Removed "LoadByNet" check for LineItemGrossQuantityFG and LineItemNetQuantityFG in
												"AddColumn()". This fixes CSI #5911.

		06-27-2008	V. Thompson			Updated to allow for line item user data
		
		09-30-2008	V. Thompson			Added code to set the line item gross qty field to be read only
												when the alias is set to aggregate associated transactions and
												the gross quantity's line item has associated transactions.
		10-10-2008	E. Simmons			Added SecurityClass parameter to constructor and a new SecurityClass attribute
												to support CSI #6153

		2009-02-11  A. Coker          Defect 1380. Do not display field if user does not 
									  have view finance data security right. 
 
	  2009-03-15  A. Coker          Moved quantity field gray out and readonly setting to TransactionDetails.

	  2009-03-23  R. Panachida      Defect 2140: Added code fix the issue of clicking on the New button causing
									an error.
 
		2010-03-09	W.Gray				Revised to support Conjoined Fields as part of Transaction Alias
 
*****************************************************************************************************************/

namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	using FMBusinessObjects.LogClient;
	using FMBusinessObjects.DataObjects;
	using FMControls;
   using System.Web;

   /// <summary>
   /// Summary description for LineItemGridGenerator.
   /// </summary>
   public class LineItemGridGenerator
	{
		#region Protected data members
		protected DataGrid grid;
		protected ArrayList columnList;
		protected TransactionContext transContext;
		protected TransactionDO trans;
		protected TransactionFieldGenerator fieldGenerator;
		protected Logger logger;

		protected int generatedLineItemIndex;
		protected int generatedSubLineItemIndex;

		protected bool bProductColumnExists;

		protected SecurityClass security;
		protected FieldConfiguration fieldConfiguration;
		#endregion

		public TransactionDO Trans
		{
			set
			{
				this.trans = value;
			}
		}

		#region Constructors
		public LineItemGridGenerator ( DataGrid grid,
										TransactionContext transContext,
										TransactionDO trans,
										TransactionFieldGenerator fieldGenerator,
										SecurityClass securityObj )
		{
			this.grid = grid;
			this.transContext = transContext;
			this.trans = trans;
			this.fieldGenerator = fieldGenerator;
			this.logger = new Logger ( "Accounting" );

			this.security = securityObj;
			this.fieldConfiguration = new FieldConfiguration ( fieldGenerator.Page );
			this.fieldConfiguration.LoadConfigurationData();
		}
		#endregion

		public void Generate ( ) { Generate ( true ); }
		public void Generate ( bool bind )
		{
			this.generatedLineItemIndex = -1;
			this.generatedSubLineItemIndex = -1;

			bProductColumnExists = false;
			columnList = new ArrayList ( );

			foreach (FieldClass field in transContext.aliasClass.DisplayOrder ( TRANSACTION_SECTION_TYPE.LINE_ITEMS ))
			{
				if (trans.TransTypeID == TransactionTypes.T23_StorageTransfer ||
					trans.TransTypeID == TransactionTypes.T13_OwnerTransfer)
				{
					AddColumn(field);
				}
				else
				{
					AddColumn(field);
				}
			}

			grid.ItemCreated += this.GridItemCreated;
			grid.ItemDataBound += this.GridItemDataBound;

			if (bind)
			{
				Bind ( );
			}
		}

		/// <summary>
		/// This method will bind the transaction line item to the transaction detail
		/// grid.
		/// </summary>
		public void Bind ( )
		{
			this.generatedLineItemIndex = -1;
			this.generatedSubLineItemIndex = -1;

			var gridItemList = new ArrayList ( );

			foreach (LineItemDO lineItem in trans.LineItems)
			{
				gridItemList.Add ( lineItem );

				foreach (SubLineItemDO subLineItem in lineItem.SubLineItems)
				{
					gridItemList.Add ( subLineItem );
				}
			}

			grid.DataSource = gridItemList;
			grid.DataBind ( );
		}

		/// <summary>
		/// This method will remove all the Transaction Line Items from the 
		/// line item collection.
		/// </summary>
		public void ClearTransLineItems ( )
		{
			if (this.trans.LineItems != null)
			{
				this.trans.LineItems.Clear ( );
			}
		}

		protected void AddColumn ( FieldClass field )
		{
			string id = field.ID;
			int deleteColumnLocation = -1;

			//Product fields (Product, ProductCode, ProductType) are grouped into 1 column.
			if (id.StartsWith ( "Product" ) && ( id != "ProductPrice" ))
			{
				if (bProductColumnExists)
				{
					return;
				}

				id = "Product";
				bProductColumnExists = true;
			}

			// See if the field is a user data field
			FieldGenerator colGenerator;

			var userDataFieldClass = field as UserDataFieldClass;

			if (userDataFieldClass != null)
			{
				var userDataField = userDataFieldClass;

				if (userDataField.UserDataType == USER_DATA_TYPE.LIST)
				{
					colGenerator = new LineItemUserDataListFG ( id ) { TransFieldConfiguration = this.fieldConfiguration };
				}
				else
				{
					colGenerator = new LineItemUserDataTextFG ( id ) { TransFieldConfiguration = this.fieldConfiguration };
				}
			}
			else
			{
				colGenerator = fieldGenerator.GetFieldGenerator ( "LineItem " + id );
			}

			if (colGenerator == null)
			{
				logger.Warn ( "LineItemGridGenerator.AddColumn() : No LineItemColumnGenerator found for field \"" + id + "\"." );
				return;
			}

			colGenerator.DisplayName = HttpUtility.HtmlEncode(field.DisplayName);
			colGenerator.SetTransaction ( trans );
			colGenerator.SetTransactionContext ( transContext );

			columnList.Add ( colGenerator );

			// find the location of the delete column
			deleteColumnLocation = -1;
			int loopNumber = 0;
			foreach (DataGridColumn column in grid.Columns)
			{
				if (column.HeaderText.ToUpper().Equals("DELETE"))
				{
					deleteColumnLocation = loopNumber;
					break;
				}
				++loopNumber;
			}
			var col = new TemplateColumn ( );
			if (deleteColumnLocation == -1)
			{
				grid.Columns.Add(col);
			}
			else
			{
				grid.Columns.AddAt(deleteColumnLocation, col);
			}
			col.HeaderStyle.CssClass = grid.HeaderStyle.CssClass;
			col.ItemStyle.Wrap = false;

			// vthompson
			// This will be one last check to see if the field is required.
			// There are lots of places that set the required property (such as Net and Gross fields)
			// so only change the required property if the required field is false
			if (!colGenerator.Required)
			{
				colGenerator.bFieldRequired = field.FieldRequired;
			}

			col.HeaderTemplate = new CreateHeaderItemTemplate(field.DisplayName, colGenerator.Required);
		}

		private void GridItemDataBound ( object sender, DataGridItemEventArgs e )
		{
			if (e.Item.DataItem != null)
			{
				char[] separatorList = { '.' };
				string[] stringList = e.Item.ID.Split ( separatorList );
				int lineItemIndex = int.Parse ( stringList[0] );
				int sublineItemIndex = int.Parse ( stringList[1] );

				const int CellStartIndex = 2;// was 4

				for (int cellIndex = CellStartIndex; cellIndex < (e.Item.Cells.Count - 2); ++cellIndex)// added -2
				{
					TableCell cell = e.Item.Cells[cellIndex];

					int columnIndex = cellIndex - CellStartIndex;
					var colGenerator = columnList[columnIndex] as FieldGenerator;

					if (( sublineItemIndex == -1 ) && ( ( colGenerator is ILineItemField ) == false ))
					{
						continue;
					}

					if (( sublineItemIndex > -1 ) && ( ( colGenerator is ISublineItemField ) == false ))
					{
						continue;
					}

					bool editable = grid.EditItemIndex == e.Item.ItemIndex;

					if (colGenerator != null)
					{
						cell.ID = colGenerator.FieldID + " " + e.Item.ItemIndex;
						colGenerator.GenerateField ( cell, this.trans, this.transContext, editable, lineItemIndex, sublineItemIndex );
					}

					var customFieldStates = new FMCustomFieldStatesClass ( );

					foreach (Control control in cell.Controls)
					{
						var webControl = control as WebControl;

						if (webControl != null)
						{
							if (webControl is FMComboBox)
							{
								webControl.CssClass = "tabletext txFieldComboBox";
							}
							else
							{
								webControl.CssClass = "tabletext";
							}

							//Eric Simmons (10-10-2008) Added to support CSI #6153
							customFieldStates.SetTransactionFieldState ( security, webControl );
						}

						var htmlControl = control as HtmlControl;

						if (htmlControl != null)
						{
							htmlControl.Attributes.Add ( "class", "tabletext" );
						}
					}
				}
			}
		}

		private void GridItemCreated ( object sender, DataGridItemEventArgs e )
		{
			if (e.Item.DataItem == null)
			{
				return;
			}

			if (e.Item.DataItem is SubLineItemDO)
			{
				e.Item.Cells[0].Controls[0].Visible = false;
				this.generatedSubLineItemIndex++;
			}
			else
			{
				foreach (TableCell cell in e.Item.Cells)
				{
					foreach (Control control in cell.Controls)
					{
						var literalControl = control as LiteralControl;

						if (literalControl != null)
						{
						}
					}
				}

				this.generatedLineItemIndex++;
				this.generatedSubLineItemIndex = -1;
			}

			e.Item.ID = this.generatedLineItemIndex + "." + this.generatedSubLineItemIndex;
		}
	}

	internal class CreateHeaderItemTemplate : ITemplate
	{

		//Field to store the ListItemType value
		private string headerText = "";
		private bool required = false;

		public CreateHeaderItemTemplate()
		{
			//
			// TODO: Add default constructor logic here
			//
		}

		//Parameterrised constructor
		public CreateHeaderItemTemplate(string hText, bool req)
		{
			headerText = hText;
			required = req;

		}

		//Overwrite the InstantiateIn() function of the ITemplate interface.
		public void InstantiateIn(System.Web.UI.Control container)
		{
			//Code to create the ItemTemplate and its field.
			Label label = new Label();
			label.Text = HttpUtility.HtmlEncode( headerText );

			container.Controls.Add(label);
			if (required)
			{
				Literal literal = new Literal();
				literal.Text = "<span style='color:red'>*</span>";
				container.Controls.Add(literal);

			}
		}
	}
}
