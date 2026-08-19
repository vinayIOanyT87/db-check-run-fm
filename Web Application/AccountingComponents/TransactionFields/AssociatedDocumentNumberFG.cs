/// <summary>
/// File name:	AssociatedDocumentNumberFG.cs
/// Purpose:	The purpose of this class is to create a virtual field that
///            contains a list of document numbers that are associated to a 
///            parent transaction.
///            
/// Comments:	Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 
///				2000.  This file shall not be copied or reproduced in any form 
///				without the express written consent of Endress+Hauser.
///				
/// Author(s):	Richard Panachida
/// Version:	1.0.0  Current version
///	
/// Modification History:
/// Date:		   By:					   Reason:
/// ----------	   -----------------	   ---------------------------------------------------
/// yyyy-mm-dd	   Developer's name		Reason for the change
///
/// </summary>


namespace TransactionFields
{
	using System;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Web.UI;

	using FMControls;
	using System.Collections;
	using System.Web.UI.WebControls;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.ServiceRequests;

	public class AssociatedDocumentNumberFG : DropDownGenerator, IHeaderField
	{
		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Document Number
		/// field generator.
		/// </summary>
		public AssociatedDocumentNumberFG()
		{
			this.autoPostBack = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the field identify.
		/// </summary>
		public override string FieldID
		{
			get { return "AssociatedDocNumber"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength(this.FieldID, 30); }
		}

		/// <summary>
		/// This property return true if the field is editable.
		/// </summary>
		public override bool Editable
		{
			get { return true; }
		}
		#endregion

		#region Override methods
		/// <summary>
		/// Returns the statuses configured for use with the transaction alias
		/// </summary>
		/// <returns>A HybridDictionary containing the configured statuses</returns>
		public override HybridDictionary GetEntries()
		{
			// Create a new dictionary
			var newDictionary = new HybridDictionary();
			this.transContext.AssociatedDocNumFlags = new Hashtable();

			// Retrieve document number from all parent associated transactions.
			// Note: there should only be one parent for implementation.
			AssociatedParentTxListDO associatedParentTxListDO = this.GetAssociatedParentTxList();

			if (associatedParentTxListDO != null)
			{
				foreach (AssociatedParentTxDO associatedParentTxDO in associatedParentTxListDO.List)
				{
					if (newDictionary.Contains(associatedParentTxDO.DocumentNumber))
					{
						throw new Exception("Error: Duplicate contract number '" + associatedParentTxDO.DocumentNumber + "'.");
					}

					newDictionary.Add(associatedParentTxDO.DocumentNumber, associatedParentTxDO.TransID);
					var flags = new ArrayList();

					for (int nextSetting = 0; nextSetting < 6; nextSetting++)
					{
						flags.Add(false);
					}

					if (associatedParentTxDO.Flag01)
					{
						flags[0] = true;
					}

					if (associatedParentTxDO.Flag02)
					{
						flags[1] = true;
					}

					if (associatedParentTxDO.Flag03)
					{
						flags[2] = true;
					}

					if (associatedParentTxDO.Flag04)
					{
						flags[3] = true;
					}

					if (associatedParentTxDO.Flag05)
					{
						flags[4] = true;
					}

					if (associatedParentTxDO.Flag06)
					{
						flags[5] = true;
					}

					if (this.transContext.AssociatedDocNumFlags.Contains(associatedParentTxDO.DocumentNumber) == false)
					{
						this.transContext.AssociatedDocNumFlags.Add(associatedParentTxDO.DocumentNumber, flags);
					}
				}
			}

			return newDictionary;
		}

		#endregion

		#region Override event handlers
		/// <summary>
		/// This method handles the Text change for combo boxes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void TextChanged(object sender, EventArgs e)
		{
			this.UpdateListOfClins();
			this.UpdateListOfTransportOrderNumbers();
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will update the list of associated CLINS based on the selected Document Number.
		/// </summary>
		private void UpdateListOfClins()
		{
			var updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;
				if ( comboBox != null )
				{
					TextBox textBox = comboBox.TextBoxCntrl;

					if (textBox != null)
					{
						SetDataValue(textBox.Text);
					}
				}

				if (this.transContext.aliasClass.TransactionFieldCollection.Find("AssociatedCLIN") != null)
				{
					var associatedClinFG = this.fieldGenerator.GetFieldGenerator("AssociatedCLIN") as AssociatedCLINFG;

					if (associatedClinFG != null)
					{
						if (comboBox != null)
						{
							this.transContext.IntermediateTransID = comboBox.Text;
							string docNumber = comboBox.TextBoxCntrl.Text;

							associatedClinFG.SetDataValue(this.trans, "");
							associatedClinFG.bFieldEditible = true;

							if (!string.IsNullOrEmpty(docNumber))
							{
								var flags = this.transContext.AssociatedDocNumFlags[docNumber] as ArrayList;

								if (flags != null && (bool)flags[3])
								{
									associatedClinFG.bFieldEditible = false;
								}
							}
						}

						HybridDictionary entries = associatedClinFG.GetEntries();
						updatePanel = associatedClinFG.Cell.Controls[0] as UpdatePanel;

						if (updatePanel != null)
						{
							var clinComboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

							object fieldValue = associatedClinFG.GetDataValue(this.trans);
							if (clinComboBox != null)
							{
								clinComboBox.Clear();
								clinComboBox.Items.Clear();
								clinComboBox.Items.Add(new ListItem(string.Empty, string.Empty));

								bool itemInList = false;

								// Default the field length of CLIN combobox to the default.
								int fieldLength = clinComboBox.TextBoxCntrl.Columns;

								foreach (DictionaryEntry entry in entries)
								{
									var key = entry.Key as string;
									var value = entry.Value as string;

									clinComboBox.Items.Add(new ListItem(key, value));

									if ((fieldValue != null) && (fieldValue.ToString() == key))
									{
										clinComboBox.SelectedIndex = clinComboBox.Items.Count - 1;
										itemInList = true;
									}

									// Find the key that has the longest length to be used to set
									// the width of the combobox.
									if (string.IsNullOrEmpty(key) == false)
									{
										if (key.Length <= clinComboBox.TextBoxCntrl.Columns)
										{
											fieldLength = key.Length;
										}
									}

									// Set the combobox width.
									clinComboBox.TextBoxCntrl.Columns = fieldLength;
								}

								if ((itemInList == false) && (fieldValue != null) && (fieldValue.ToString() != ""))
								{
									int insertIndex = 0;

									foreach (ListItem item in clinComboBox.Items)
									{
										if (item.Text != string.Empty && item.Text.CompareTo(fieldValue.ToString()) > 0)
										{
											clinComboBox.Items.Insert(insertIndex, new ListItem(fieldValue.ToString(), fieldValue.ToString()));
											break;
										}

										insertIndex++;
									}

									if (comboBox != null && insertIndex == comboBox.Items.Count)
									{
										clinComboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));
									}

									clinComboBox.SelectedIndex = insertIndex;

									if (comboBox != null)
									{
										clinComboBox.HiddenFieldCntrl.Value = comboBox.SelectedIndex.ToString(CultureInfo.InvariantCulture);
									}

									clinComboBox.Text = fieldValue.ToString();
								}
								else if (fieldValue != null && fieldValue.ToString() == string.Empty && entries.Count > 0)
								{
									clinComboBox.SelectedIndex = 1;
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// This method will update the list of associated transport order numbers based on the selected Document Number.
		/// </summary>
		private void UpdateListOfTransportOrderNumbers()
		{
			var comboBox = this.cell.Controls[0] as FMComboBox;

			if (comboBox != null)
			{
				TextBox textBox = comboBox.TextBoxCntrl;

				if (textBox != null)
				{
					SetDataValue(textBox.Text);
				}
			}

			if (this.transContext.aliasClass.TransactionFieldCollection.Find("AssociatedTransportOrderNumber") != null)
			{
				var associatedTransportOrderNumberFG =
							   this.fieldGenerator.GetFieldGenerator("AssociatedTransportOrderNumber") as AssociatedTransportOrderNumberFG;

				if (associatedTransportOrderNumberFG != null)
				{
					if (comboBox != null)
					{
						this.transContext.IntermediateTransID = comboBox.Text;
						associatedTransportOrderNumberFG.SetDataValue(this.trans, string.Empty);
						associatedTransportOrderNumberFG.bFieldEditible = true;
						string docNumber = comboBox.TextBoxCntrl.Text;

						if (!string.IsNullOrEmpty(docNumber))
						{
							var flags = this.transContext.AssociatedDocNumFlags[docNumber] as ArrayList;

							if (flags != null && (bool)flags[3])
							{
								associatedTransportOrderNumberFG.bFieldEditible = false;
							}
						}
					}

					HybridDictionary entries = associatedTransportOrderNumberFG.GetEntries();
					var transportOrderNumberComboBox = (FMComboBox)associatedTransportOrderNumberFG.Cell.Controls[0];

					object fieldValue = associatedTransportOrderNumberFG.GetDataValue(this.trans);
					transportOrderNumberComboBox.Clear();
					transportOrderNumberComboBox.Items.Clear();
					transportOrderNumberComboBox.Items.Add(new ListItem("", ""));

					bool itemInList = false;

					// Default the field length of Transport Order Number combobox to the default.
					int fieldLength = transportOrderNumberComboBox.TextBoxCntrl.Columns;

					foreach (DictionaryEntry entry in entries)
					{
						var key = entry.Key as string;
						var value = entry.Value as string;

						transportOrderNumberComboBox.Items.Add(new ListItem(key, value));

						if ((fieldValue != null) && (fieldValue.ToString() == key))
						{
							transportOrderNumberComboBox.SelectedIndex = transportOrderNumberComboBox.Items.Count - 1;
							itemInList = true;
						}
						// Find the key that has the longest length to be used to set
						// the width of the combobox.
						if (string.IsNullOrEmpty(key) == false)
						{
							if (key.Length <= transportOrderNumberComboBox.TextBoxCntrl.Columns)
							{
								fieldLength = key.Length;
							}
						}

						// Set the combobox width.
						transportOrderNumberComboBox.TextBoxCntrl.Columns = fieldLength;
					}

					if ((itemInList == false) && (fieldValue != null) && (fieldValue.ToString() != ""))
					{
						transportOrderNumberComboBox.Items.Add(new ListItem(fieldValue.ToString(), fieldValue.ToString()));
						transportOrderNumberComboBox.SelectedIndex = transportOrderNumberComboBox.Items.Count - 1;
						transportOrderNumberComboBox.Text = fieldValue.ToString();
					}
					else if (fieldValue != null && fieldValue.ToString() == "" && entries.Count > 0)
					{
						transportOrderNumberComboBox.SelectedIndex = 1;
					}
				}
			}
		}
		#endregion

		#region IHeaderField Members
		/// <summary>
		/// This method will return the associated parent transaction's TransID.
		/// </summary>
		/// <param name="transaction">Transaction object.</param>
		/// <returns>associated parent transaction's TransID</returns>
		public virtual object GetDataValue(TransactionDO transaction)
		{
			AssociatedParentTxListDO associatedParentTxList = this.GetAssociatedParentTxList();

			foreach (AssociatedParentTxDO associatedParentTx in associatedParentTxList.List)
			{
				if (associatedParentTx.DocumentNumber.Equals(transaction.AssociatedDocumentNumber))
				{
					return associatedParentTx.TransID;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// This method will return the associated parent transaction's TransID.
		/// </summary>
		/// <param name="transaction">Transaction object.</param>
		/// <returns>associated parent transaction's TransID</returns>
		public virtual string GetDataText(TransactionDO transaction)
		{
			if (this.GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public virtual void SetDataValue(TransactionDO transaction, object newValue)
		{
			transaction.AssociatedDocumentNumber = (string)newValue;
			OnFieldChanged();
		}

		/// <summary>
		/// This method will return the associated parent transaction list.
		/// </summary>
		/// <returns>associated parent transaction list</returns>
		private AssociatedParentTxListDO GetAssociatedParentTxList()
		{
			var associatedParentTxSR = new GetAssociatedParentTxSR
			{
				AliasName = this.trans.Alias,
				CurrentSiteGuid = this.trans.SiteGuid,
				TransTypeID = this.trans.TransTypeID,
				TransactionAliasGuid = this.trans.TransactionAliasGuid,
				SubTypeRequest = GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX,
				Security = this.transContext.security
			};

			AssociatedParentTxListDO associatedParentTxListDO =
				FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process(associatedParentTxSR));

			return associatedParentTxListDO;
		}
		#endregion
	}
}
