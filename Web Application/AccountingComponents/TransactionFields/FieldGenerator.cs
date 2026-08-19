/// <summary>
///   File name:	FieldGenerator.cs
///   Purpose:	   The purpose of this module is to handle the field generation for the accounting
///				   detail page. 
///	Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				   2000.  This file shall not be copied or reproduced in any form 
///				   without the express written consent of Endress+Hauser.
///	Author(s):	Thomas Beckum
///	Version:	1.0.0  Current version
///	
///	Modification History:
///   Date:			   By:						Reason:
///   ----------		--------------------	--------------------------------------------
///   2006-06-13		Richard Panachida		Commented out the Alert java script code. The
///												   error needs to be caught at the GUI layer in 
///												   order to perform data dictionary.
///   2007-02-15		Richard Panachida		Fixed the "Required" functionality (CSI 3903)
///   2007-04-10		G.Kendall				CSI 4374 - Adding special instructions feature
///   2009-01-07     Richard Panachida    Defect 649: Updated to fix the Free-to-aid functionality.
///	2009-02-11     A. Coker             Defect 1380. Do not display field if user does not 
///		                                    have view finance data security right.  
///   2009-0225      Richard Panachida    Defect 1720: Updated the GetRequiredLabelCell method to fix the
///                                       problem of not finding the required label cell in certain odd
///                                       cases.
///                                       
///   2009-03-13  Richard Panachida       Defect 1938. Added code to check a field being virtual.
///   
///   2009-04-08  Richard Panachida       Defect 2908: Added code to check for required user data dropdown fields
///                                       being selectd.
///   2009-04-15  Richard Panachida       Defect 3190: Fixed the error message to have the display name.
///   
///   2009-06-22  A.Coker                 Set cell prior to retrieving data. Price transaction fields need the
///                                       cell child controls to check if field was modified by user.
/// </summary>
namespace TransactionFields
{
    using System;
    using System.Reflection;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using FMBusinessObjects.LogClient;
    using FMBusinessObjects.DataObjects;
    using System.Web;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.UtilityObjects;
    using Varec.CommonComponents.EngineeringUnitsLibrary;

    abstract public class FieldGenerator
	{
		#region Public data members
		public bool bFieldRequired = false;
		public bool bFieldEditible = true;

		// Lighter gray than the build in color "LightGray" to be used in disabled text boxes so users can read the disabled gray text. 
		public System.Drawing.Color VarecBkgrndReadOnlyGray = System.Drawing.Color.FromArgb(240, 240, 240);
		public delegate void FieldChangedEventHandler(FieldGenerator fieldGenerator);
		public FieldChangedEventHandler FieldChanged;
		public bool GenerateGlossaryEntry { get; set; }
		#endregion

		#region Protected data members
		protected bool virtualField = false;
		protected bool isDropdownField = false;

		protected const string CUSTOM_CLIENT_SCRIPT_NAME = "CUSTOM_CLIENT_SCRIPT_NAME";
		protected const string selectedText = "--Select--";
		protected const string notSelectedText = "None";
		protected string displayName = "";

		protected TransactionDO trans;
		protected LineItemDO lineItem;
		protected SubLineItemDO sublineItem;
		protected WeightReadingDO weightReading;
		protected TransportLineItemDO transportLineItemDO;
		protected TransactionContext transContext;
		protected TableCell cell;
		protected TransactionFieldGenerator fieldGenerator;
		protected Logger logger;
		#endregion

		#region Private data members
		private FieldConfiguration fieldConfiguration;
		private Page page;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default Field Generator constructor.
		/// </summary>
		protected FieldGenerator()
		{
			GenerateGlossaryEntry = true;
		}
		#endregion

        #region Properties
        public TransactionDO Trans
        {
            set
            {
                this.trans = value;
            }
        }

        public LineItemDO LineItem
        {
            get { return this.lineItem; }
            set { this.lineItem = value; }
        }

        /// <summary>
        /// This is an abstract property that will be implemented by the derived class.
        /// It will return the field ID.
        /// </summary>
        abstract public string FieldID
		{
			get;
		}

		protected virtual string ID
		{
			get
			{
				return this.GetType().ToString();
			}
		}

		/// <summary>
		/// This property will return true if the field is required.
		/// Otherwise, it will return false.
		/// </summary>
		virtual public bool Required
		{
			get
			{
				return bFieldRequired;
			}
			set
			{
				bFieldRequired = value;
			}
		}

		/// <summary>
		/// This property will return true if the field is editable.
		/// Otherwise, it will return false.
		/// </summary>
		virtual public bool Editable
		{
			get
			{
				return bFieldEditible;
			}
			set
			{
				bFieldEditible = value;
			}
		}

		/// <summary>
		/// This property will get the Virtual Field data member. True
		/// means that field is virtual.
		/// </summary>
		virtual public bool VirtualField
		{
			get { return this.virtualField; }
		}

		/// <summary>
		/// This property will get the dropdown field flag data member. True
		/// means that field is a dropdown field type.
		/// </summary>
		virtual public bool IsDropdownField
		{
			get { return this.isDropdownField; }
		}

		public String ClientID
		{
			get
			{
				return (this.cell == null || this.cell.Controls.Count == 0) ? null : this.cell.Controls[0].ClientID;
			}
		}

		/// <summary>
		/// This property returns the transaction field configuration object. It is used for 
		/// such things as custom field lengths.
		/// </summary>
		public FieldConfiguration TransFieldConfiguration
		{
			get { return this.fieldConfiguration; }
			set { this.fieldConfiguration = value; }
		}

		/// <summary>
		/// This property sets and gets the the current page.
		/// </summary>
		public Page Page
		{
			get { return this.page; }
			set { this.page = value; }
		}

		/// <summary>
		/// This property sets and gets the the display name property of a given field.
		/// </summary>
		public string DisplayName
		{
			get { return this.displayName; }
			set { this.displayName = value; }
		}

		/// <summary>
		/// This property will get the Active Update Panel control for the current
		/// page that caused the post back.
		/// </summary>
		private UpdatePanel ActiveUpdatePanel
		{
			get
			{
				try
				{
					ScriptManager currentScriptManager = ScriptManager.GetCurrent(this.page);

					if (currentScriptManager != null)
					{
						Control activeControl = currentScriptManager.FindControl(currentScriptManager.AsyncPostBackSourceElementID);

						while (activeControl != null && activeControl.Parent != null)
						{
							if (activeControl.Parent.GetType() == typeof(UpdatePanel))
							{
								return (UpdatePanel)activeControl.Parent;
							}

							activeControl = activeControl.Parent;
						}
					}

					return null;
				}
				catch
				{
					return null;
				}
			}
		}
		#endregion

		abstract public object GetNewValue(WebControl control);
		virtual protected void SpecializeControl(WebControl control) { }

		public void GenerateField(TableCell inCell, TransactionDO transaction, TransactionContext transactionContext, bool editable)
		{
			this.GenerateField(inCell, transaction, transactionContext, editable, -1, -1);
		}

		public void GenerateField(TableCell inCell, TransactionDO transaction, TransactionContext transactionContext, bool editable,
								  int lineItemIndex)
		{
			this.GenerateField(inCell, transaction, transactionContext, editable, lineItemIndex, -1);
		}

		protected bool AddRequiredAriaAttribute(ControlCollection controls)
		{

			foreach (Control control in controls)
			{
				if (control is TextBox)
				{
					TextBox tb = control as TextBox;
					tb.Attributes["aria-required"] = "true";
					return true;
				}
				else if (control is FMControls.FMComboBox)
				{
					FMControls.FMComboBox cb = control as FMControls.FMComboBox;
					TextBox tb = cb.TextBoxCntrl;
					if (tb != null)
					{
						tb.Attributes["aria-required"] = "true";
					}
					return true;
				}

				if (AddRequiredAriaAttribute(control.Controls))
					return true;
			}
			return false;
		}

        /// <summary>
        /// This method will generate a field on the header, line item, sub-line item, weight reading,
        /// or transport line item.
        /// </summary>
        /// <param name="transactionContext"></param>
        /// <param name="editable"></param>
        /// <param name="lineItemIndex"></param>
        /// <param name="sublineItemIndex"></param>
        /// <param name="inCell"></param>
        /// <param name="transaction"></param>
        public void GenerateField(TableCell inCell, TransactionDO transaction, TransactionContext transactionContext, bool editable,
								  int lineItemIndex, int sublineItemIndex)
		{
			this.transContext = transactionContext;
			this.trans = transaction;
			lineItem = null;
			sublineItem = null;

			if (lineItemIndex > -1)
			{
				if ((this is ILineItemField) || (this is ISublineItemField))
				{
					this.lineItem = trans.LineItems[lineItemIndex];
					if ((FieldID == "LineItem GrossQuantity" && this.lineItem.Quantity.GrossManualValueFlag == false)
					|| (FieldID == "LineItem DeliveredGrossQuantity" && this.lineItem.Quantity.DeliveredGrossManualValueFlag == false)
					|| (FieldID == "LineItem NetQuantity" && this.lineItem.Quantity.NetManualValueFlag == false)
					|| (FieldID == "LineItem DeliveredNetQuantity" && this.lineItem.Quantity.DeliveredNetManualValueFlag == false)
					|| (FieldID == "LineItem MassQuantity" && this.lineItem.Quantity.MassManualValueFlag == false)
					|| (FieldID == "LineItem PackageQuantity" && this.lineItem.Quantity.PackageManualValueFlag == false)
					|| (FieldID == "LineItem Vcf" && this.lineItem.Quantity.VcfManualValueFlag == false))
					{
						inCell.Style.Add("color", "DarkGray");
					}               
               

					if (sublineItemIndex > -1)
					{
						this.sublineItem = this.lineItem.SubLineItems[sublineItemIndex];
						if ((FieldID == "LineItem GrossQuantity" && this.sublineItem.Quantity.GrossManualValueFlag == false)
						|| (FieldID == "LineItem DeliveredGrossQuantity" && this.sublineItem.Quantity.DeliveredGrossManualValueFlag == false)
						|| (FieldID == "LineItem NetQuantity" && this.sublineItem.Quantity.NetManualValueFlag == false)
						|| (FieldID == "LineItem DeliveredNetQuantity" && this.sublineItem.Quantity.DeliveredNetManualValueFlag == false)
						|| (FieldID == "LineItem MassQuantity" && this.sublineItem.Quantity.MassManualValueFlag == false)
						|| (FieldID == "LineItem PackageQuantity" && this.sublineItem.Quantity.PackageManualValueFlag == false)
						|| (FieldID == "LineItem Vcf" && this.sublineItem.Quantity.VcfManualValueFlag == false))
						{
							inCell.Style.Add("color", "DarkGray");
						}
					}

					if (editable == false && this.GeneratedField == false && this.transContext.aliasClass.MultipleLineItems)
					{
						inCell.Text = GetFormattedValue();
						return;
					}
				}
				else if (this is IWeightReadingField)
				{
					this.weightReading = trans.WeightReadings[lineItemIndex];

					if ((editable == false)
						&& this.GeneratedField == false
						&& this.transContext.aliasClass.MultipleWeightReadings)
					{
						inCell.Text = GetFormattedValue();
						return;
					}
				}
				else if (this is ITransportLineItemField)
				{
					this.transportLineItemDO = trans.TransportInfoList[lineItemIndex];

					if ((editable == false)
						&& (this.GeneratedField == false)
						&& (this.transContext.aliasClass.MultipleTransportLineItems))
					{
						inCell.Text = GetFormattedValue();
						return;
					}
				}
			}
			else
			{
				if (this is IWeightReadingField)
				{
					this.weightReading = trans.WeightReadings[0];
				}
				else if (this is ITransportLineItemField)
				{
					this.transportLineItemDO = trans.TransportInfoList[0];
				}
			}

			TableCell requiredCell = GetRequiredLabelCell(inCell);
			if (requiredCell != null)
			{
				if (this.Required)
				{
					requiredCell.Text = "*";
					requiredCell.Style.Add("color", "red");
				}
				else if (this.TransFieldConfiguration.FileFound)
				{
					bool? conditional = this.TransFieldConfiguration.IsFieldRequiredByExternalInterface(FieldID, trans.Alias);

					if (conditional != null)
					{
						if ((bool)conditional)
						{
							requiredCell.Text = "#";
							requiredCell.Style.Add("color", "red");
						}
						else
						{
							requiredCell.Text = "*";
							requiredCell.Style.Add("color", "red");
						}
					}
				}
			}

			// Check for a glossary exemption
			if (this.TransFieldConfiguration.FileFound)
			{
				GenerateGlossaryEntry = (this.TransFieldConfiguration.IsFieldExemptedFromGlossary(FieldID) == false);
			}

			// The cell is persisted in the generator for editable fields
			// i.e. only one field instance is editable at a time. 
			this.cell = inCell;

			Generate(editable);
			SpecializeControl(this.cell);
			if ((requiredCell != null && requiredCell.Text == "*") || this.Required)
			{
				AddRequiredAriaAttribute(this.cell.Controls);
			}
		}

		public abstract void Generate(bool editable);

		public void Retrieve(WebControl control, TransactionDO transaction, TransactionContext transactionContext)
		{
			Retrieve(control, transaction, transactionContext, -1, -1);
		}

		public void Retrieve(WebControl control, TransactionDO transaction, TransactionContext transactionContext,
			int lineItemIndex)
		{
			Retrieve(control, transaction, transactionContext, lineItemIndex, -1);
		}

		/// <summary>
		/// This method will retrieve data from the control and compare it to the data object.
		/// If different, then the new value from the control is set in the field generator.
		/// </summary>
		/// <param name="control"></param>
		/// <param name="transaction"></param>
		/// <param name="transactionContext"></param>
		/// <param name="lineItemIndex"></param>
		/// <param name="sublineItemIndex"></param>
		public void Retrieve(WebControl control, TransactionDO transaction, TransactionContext transactionContext,
							 int lineItemIndex, int sublineItemIndex)
		{

			if (control is TableCell)
			{
				cell = control as TableCell;
			}

			this.transContext = transactionContext;
			this.trans = transaction;
			lineItem = null;
			sublineItem = null;

			if (lineItemIndex > -1)
			{
				if ((this is ILineItemField) || (this is ISublineItemField))
				{
					this.lineItem = this.trans.LineItems[lineItemIndex];

					if (sublineItemIndex > -1)
					{
						this.sublineItem = this.lineItem.SubLineItems[sublineItemIndex];
					}
				}
				else if (this is IWeightReadingField)
				{
					this.weightReading = this.trans.WeightReadings[lineItemIndex];
				}
				else if (this is ITransportLineItemField)
				{
					this.transportLineItemDO = this.trans.TransportInfoList[lineItemIndex];
				}
			}

			// Get the new value from the control.
			object newValue = GetNewValue(control);

			// Validate to see if the field is required. If so, then an 
			// exception will be thrown if the value is blank.
			if (this.Required)
			{
				control.BackColor = System.Drawing.Color.Red;

				if (newValue == null)
				{
					throw new FMFieldRequiredException();
				}

				if (this.isDropdownField && newValue.Equals(selectedText))
				{
					throw new FMFieldRequiredException();
				}

				if (newValue.Equals(string.Empty))
				{
					throw new FMFieldRequiredException();
				}

				control.BackColor = System.Drawing.Color.Transparent;
			}

			object oldValue = GetDataValue();

			if ((oldValue == null) && ((newValue == null) || (newValue.Equals(string.Empty))))
			{
				return;
			}

			if ((newValue == null) && ((oldValue.Equals(string.Empty))))
			{
				return;
			}

			if ((newValue != null) && (oldValue != null))
			{
				if (newValue.Equals(oldValue))
				{
					return;
				}
			}

			SetDataValue(newValue);
		}

		public void SetTransaction(TransactionDO transaction)
		{
			this.trans = transaction;
		}

		public void SetTransactionContext(TransactionContext transactionContext)
		{
			this.transContext = transactionContext;
		}

		public object GetDataValue()
		{
			if (this.logger == null)
			{
				this.logger = new Logger("Accounting");
			}

			if (sublineItem != null)
			{
				var sublineItemField = this as ISublineItemField;

				if (sublineItemField != null)
				{
					return sublineItemField.GetDataValue(sublineItem);
				}

				logger.Error("FieldGenerator.GetDataValue() : Field " + FieldID +
					" does not implement ISublineItemField.");
				return null;
			}

			if (lineItem != null)
			{
				var lineItemField = this as ILineItemField;

				if (lineItemField != null)
				{
					return lineItemField.GetDataValue(lineItem);
				}

				logger.Error("FieldGenerator.GetDataValue() : Field " + FieldID +
					" does not implement ILineItemField.");
				return null;
			}

			if (this.weightReading != null)
			{
				var weightReadingField = this as IWeightReadingField;

				if (weightReadingField != null)
				{
					return weightReadingField.GetDataValue(this.weightReading);
				}

				logger.Error("FieldGenerator.GetDataValue() : Field " + FieldID +
							 " does not implement IWeightReadingField.");

				return null;
			}

			if (this.transportLineItemDO != null)
			{
				var transportLineItemField = this as ITransportLineItemField;

				if (transportLineItemField != null)
				{
					return transportLineItemField.GetDataValue(this.transportLineItemDO);
				}

				logger.Error("FieldGenerator.GetDataValue() : Field " + FieldID +
							 " does not implement ITransportLineItemField.");

				return null;
			}

			var header = this as IHeaderField;

			if (header != null)
			{
				return header.GetDataValue(trans);
			}

			logger.Error("FieldGenerator.GetDataValue() : Field " + FieldID +
				" does not implement IHeaderField.");
			return null;
		}

        protected double ConvertUnits(double source, EngineeringUnit sourceUnits, EngineeringUnit resultUnits)
        {
            // Use the accounting site conversion functions to convert
            double result = 0;

            EngineeringUnits.Convert(source, sourceUnits, ref result, resultUnits, 0);

            return result;
        }
        /// <summary>
        /// This method will return the text of the field generator.
        /// </summary>
        /// <returns></returns>
        protected string GetDataText()
		{
			try
			{
				if (this.logger == null)
				{
					this.logger = new Logger("Accounting");
				}

				if (sublineItem != null)
				{
					var sublineItemField = this as ISublineItemField;

					if (sublineItemField != null)
					{
						return sublineItemField.GetDataText(sublineItem);
					}

					logger.Error("FieldGenerator.GetDataText() : Field " + FieldID +
						" does not implement ISublineItemField.");
					return null;
				}

				if (lineItem != null)
				{
					var lineItemField = this as ILineItemField;

					if (lineItemField != null)
					{
						return lineItemField.GetDataText(lineItem);
					}

					logger.Error("FieldGenerator.GetDataText() : Field " + FieldID +
						" does not implement ILineItemField.");
					return null;
				}

				if (this.weightReading != null)
				{
					var weightReadingField = this as IWeightReadingField;

					if (weightReadingField != null)
					{
						return weightReadingField.GetDataText(this.weightReading);
					}

					logger.Error("FieldGenerator.GetDataText() : Field " + FieldID +
								 " does not implement IWeightReadingField.");

					return null;
				}

				if (this.transportLineItemDO != null)
				{
					var transportLineItemField = this as ITransportLineItemField;

					if (transportLineItemField != null)
					{
						return transportLineItemField.GetDataText(this.transportLineItemDO);
					}

					logger.Error("FieldGenerator.GetDataText() : Field " + FieldID +
								 " does not implement ITransportLineItemField.");

					return null;
				}

				var header = this as IHeaderField;

				if (header != null)
				{
					return header.GetDataText(trans);
				}

				logger.Error("FieldGenerator.GetDataText() : Field " + FieldID +
							 " does not implement IHeaderField.");

				return null;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// This method will save the new value from the control into the data object.
		/// </summary>
		/// <param name="newValue"></param>
		public void SetDataValue(object newValue)
		{
			if (this.logger == null)
			{
				this.logger = new Logger("Accounting");
			}

			if (sublineItem != null)
			{
				var sublineItemField = this as ISublineItemField;

				if (sublineItemField != null)
				{
					sublineItemField.SetDataValue(sublineItem, newValue);
					return;
				}

				logger.Error("FieldGenerator.SetDataValue() : Field " + FieldID +
					" does not implement ISublineItemField.");
				return;
			}

			if (lineItem != null)
			{
				var lineItemField = this as ILineItemField;

				if (lineItemField != null)
				{
					lineItemField.SetDataValue(lineItem, newValue);
					return;
				}

				logger.Error("FieldGenerator.SetDataValue() : Field " + FieldID +
					" does not implement ILineItemField.");
				return;
			}

			if (weightReading != null)
			{
				var weightReadingField = this as IWeightReadingField;

				if (weightReadingField != null)
				{
					weightReadingField.SetDataValue(weightReading, newValue);
					return;
				}

				logger.Error("FIeldGenerator.SetDataValue() : Field " + FieldID +
					" does not implement IWeightReadingField.");
			}

			if (this.transportLineItemDO != null)
			{
				var transportLineItemField = this as ITransportLineItemField;

				if (transportLineItemField != null)
				{
					transportLineItemField.SetDataValue(this.transportLineItemDO, newValue);
					return;
				}

				logger.Error("FIeldGenerator.SetDataValue() : Field " + FieldID +
							 " does not implement ITransportLineItemField.");
			}

			var header = this as IHeaderField;

			if (header != null)
			{
				header.SetDataValue(trans, newValue);
				return;
			}

			logger.Error("FieldGenerator.SetDataValue() : Field " + FieldID +
						 " does not implement IHeaderField.");
		}

		/// <summary>
		/// This method will return the label text that is associated to the control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		protected string GetLabel(System.Web.UI.WebControls.WebControl control)
		{
			if (control.ID.StartsWith("LineItem")
				|| control.ID.StartsWith("AGR")
				|| control.ID.StartsWith("TransportLineItem"))
			{
				var item = control.Parent as DataGridItem;

				if (item != null)
				{
					var grid = item.Parent.Parent as DataGrid;

					int columnIndex = item.Cells.GetCellIndex(control as TableCell);

					if (grid != null)
					{
						string header = this.cell.Page.Server.HtmlDecode(grid.Columns[columnIndex].HeaderText);
						string label = System.Text.RegularExpressions.Regex.Replace(header, "<[^>]*>", "");
						char[] trimList = { '*' };

						label = label.TrimEnd(trimList);

						return label;
					}
				}
			}

			TableCell labelCell = GetLabelCell(control);

			if (labelCell == null || labelCell.ClientID == null || labelCell.ClientID.Length <= 0)
			{
				return null;
			}

			string labelValue = labelCell.ClientID;
			labelValue = labelValue.Replace("FieldLabel", string.Empty);
			labelValue = labelValue.Replace("LineItem", string.Empty);
			labelValue = labelValue.Trim();

			return labelValue;
		}

		/// <summary>
		/// This method will return the label cell control.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		protected TableCell GetLabelCell(Control control)
		{
			while (control.ID.StartsWith("FieldValue") == false)
			{
				control = control.Parent;

				if (string.IsNullOrEmpty(control.ID))
				{
					return null;
				}
			}

			string labelID = control.ID.Replace("FieldValue", "FieldLabel");
			var label = control.FindControl(labelID) as TableCell;
			return label;

		}

		/// <summary>
		/// This method will retrieve the matching Required Label Field Control based on the 
		/// Field Value control ID. It will return null if not found.
		/// </summary>
		/// <param name="control"></param>
		/// <returns></returns>
		protected TableCell GetRequiredLabelCell(WebControl control)
		{
			while (control != null && control.ID.StartsWith("FieldValue") == false)
			{
				control = control.Parent as WebControl;

				if (control != null && string.IsNullOrEmpty(control.ID))
				{
					return null;
				}
			}

			if (control != null)
			{
				string labelID = control.ID.Replace("FieldValue", "FieldRequiredLabel");
				var label = control.FindControl(labelID) as TableCell;

				// In some cases the control is not found by the FindControl method. Therefore,
				// search for the control in its parent control list to see if it can be found.
				if (label == null)
				{
					ControlCollection parentCntrls = control.Parent.Controls;

					foreach (Control cntrl in parentCntrls)
					{
						if (cntrl.ID.Equals(labelID))
						{
							label = (TableCell)cntrl;
							break;
						}
					}
				}

				return label;
			}

			return null;
		}

		internal void SetFieldGenerator(TransactionFieldGenerator inFieldGenerator)
		{
			this.fieldGenerator = inFieldGenerator;
			this.logger = this.fieldGenerator.logger ?? new Logger("Accounting");
		}

		virtual public string GetFormattedValue()
		{
			string formattedValue = GetDataText();
         return HttpUtility.HtmlEncode(formattedValue);
		}

		virtual protected bool GeneratedField
		{
			get { return false; }
		}

		protected void InvokeMethodOnCellPage(string methodName, object sender)
		{
			MethodInfo methodInfo = cell.Page.GetType().GetMethod(methodName);

			if (methodInfo != null)
			{
				methodInfo.Invoke(cell.Page, new object[] { sender });
			}
		}

		/// <summary>
		/// This method is a pass through method that accepts a default
		/// length of short. It calls the GetFieldLength method with an 
		/// integer.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="defaultLength"></param>
		/// <returns></returns>
		protected short GetFieldLength(string fieldName, short defaultLength)
		{
			return this.GetFieldLength(fieldName, (int)defaultLength);
		}

		/// <summary>
		/// This method will return a custom configured field length or return
		/// a default length based on the field name.
		/// </summary>
		/// <param name="fieldName"></param>
		/// <param name="defaultLength"></param>
		/// <returns></returns>
		protected short GetFieldLength(string fieldName, int defaultLength)
		{
		    try
		    {
		        if (this.TransFieldConfiguration.FileFound == true)
		        {
		            var length = (short)this.TransFieldConfiguration.GetFieldLength(fieldName, this.trans.Alias);

		            if (length == -1)
		            {
		                length = (short)this.TransFieldConfiguration.GetFieldLength(fieldName, null);

		                if (length != -1)
		                {
		                    return length;
		                }
		            }
		            else
		            {
		                return length;
		            }
		        }
		    }
		    catch
		    {
                // we have a default field length... don't let an error prevent the default
		    }

		    return (short)defaultLength;
		}

        protected string GetDataDictionaryValueByKey(Guid siteGuid, string text)
        {
            return DataDictionarySingleton.Get(siteGuid, text);
        }


		/// <summary>
		/// This method will register an error script to display to the user. The
		/// reason is that the Update Panel requires a script to display an error.
		/// </summary>
		/// <param name="errorMessage"></param>
		protected void RenderErrorMessage(string errorMessage)
		{
			if (this.transContext.security.UseDataDictionary)
			{
				errorMessage = GetDataDictionaryValueByKey( this.transContext.security.SiteGuid, errorMessage);
			}

			string alertString = "<script language=\"jscript\" type=\"text/jscript\">\r\n" + "$(window).load(function() {\r\n"
								+ " alert(" + HttpUtility.JavaScriptStringEncode(errorMessage, true) + ");"	
								+ "});\r\n </script>\r\n";



			UpdatePanel activeUpdate = this.ActiveUpdatePanel;

			if (activeUpdate != null)
			{
				ScriptManager.RegisterStartupScript(activeUpdate, activeUpdate.GetType(), "ErrorMessageScript" + Guid.NewGuid(), alertString, false);
			}

			else 
			{
				if (this.Page == null)
				{
					ScriptManager.RegisterStartupScript(this.fieldGenerator.Page, this.GetType(), "ErrorMessageScript" + Guid.NewGuid(), alertString, false);
				}
				else
				{
					ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ErrorMessageScript" + Guid.NewGuid(), alertString, false);				
				}
			}
		}

		protected virtual void OnFieldChanged()
		{
			// Make a temporary copy of the event to avoid possibility of
			// a race condition if the last subscriber unsubscribes
			// immediately after the null check and before the event is raised.
			FieldChangedEventHandler handler = FieldChanged;
			if (handler != null)
			{
				handler(this);
			}
		}
	}
}
