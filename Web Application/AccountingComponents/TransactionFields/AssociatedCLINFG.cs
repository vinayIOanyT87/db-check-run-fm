/// <summary>
/// File name:	AssociatedCLINFG.cs
/// Purpose:	The purpose of this class is to create a field that
///            contains a list of CLINs that are associated to a 
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
	using System.Collections;
	using System.Web.UI;
	using System.Web.UI.WebControls;
	using System.Collections.Specialized;
	using FMBusinessObjects.DataObjects;
	using FMBusinessObjects.ServiceRequests;
	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMControls;

	public class AssociatedCLINFG : DropDownGenerator, IHeaderField
	{
		#region Private data members
		private string transID;
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Associated Document Number
		/// field generator.
		/// </summary>
		public AssociatedCLINFG ( )
		{
			this.transID = "";
			this.bFieldEditible = true;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property returns the field identify.
		/// </summary>
		public override string FieldID
		{
			get { return "AssociatedCLIN"; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 30.
		/// </summary>
		protected override short MaxColumns
		{
			get { return this.GetFieldLength ( this.FieldID, 10 ); }
		}

		/// <summary>
		/// This property return true if the field is editable.
		/// </summary>
		public override bool Editable
		{
			get { return this.bFieldEditible; }
		}

		/// <summary>
		/// This property gets and sets the Transaction ID data member.
		/// </summary>
		public string TransID
		{
			get { return this.transID; }
			set
			{
				this.transID = value;

				if (string.IsNullOrEmpty(this.transID))
				{
					this.transID = string.Empty;
				}
			}
		}

		/// <summary>
		/// This method will return the control.
		/// </summary>
		public TableCell Cell
		{
			get { return this.cell; }
		}
		#endregion

		#region Override methods
		/// <summary>
		/// Returns the statuses configured for use with the transaction alias
		/// </summary>
		/// <returns>A HybridDictionary containing the configured statuses</returns>
		public override HybridDictionary GetEntries ( )
		{
			// Create a new dictionary
			var newDictionary = new HybridDictionary ( );

			if (string.IsNullOrEmpty(this.transContext.IntermediateTransID))
			{
				newDictionary = this.GetEntriesBasedOnDocumentNumber ( );
			}
			else
			{
				// Retrieve document number from all parent associated transactions.
				// Note: there should only be one parent for implementation.
				var associatedParentTxSR = new GetAssociatedParentTxSR
				                           {
					                           TransID = this.transContext.IntermediateTransID,
					                           SubTypeRequest = GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_LINE,
					                           Security = this.transContext.security
				                           };

				AssociatedParentTxListDO associatedParentTxListDO = FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process ( associatedParentTxSR )
																);

				if (associatedParentTxListDO != null)
				{
					foreach (AssociatedParentTxDO associatedParentTxDO in associatedParentTxListDO.List)
					{
						newDictionary.Add ( associatedParentTxDO.CLIN, associatedParentTxDO.CLIN );
					}
				}
			}

			return newDictionary;
		}

		/// <summary>
		/// This method will retrieve a list of CLINs based on the associated document number.
		/// </summary>
		private HybridDictionary GetEntriesBasedOnDocumentNumber ( )
		{
			// Create a new dictionary
			var newDictionary = new HybridDictionary ( );

			if (!string.IsNullOrEmpty(this.trans.AssociatedDocumentNumber))
			{
				string docNumber = this.trans.AssociatedDocumentNumber;
				var associatedParentTxSR = new GetAssociatedParentTxSR
				                           {
					                           AliasName			= this.trans.Alias,
					                           CurrentSiteGuid		= this.trans.SiteGuid,
					                           TransTypeID			= this.trans.TransTypeID,
					                           TransactionAliasGuid = this.trans.TransactionAliasGuid,
					                           SubTypeRequest		= GetAssociatedParentTxSR.AssociatedParentTxRequest.GET_ASSOCIATED_PARENT_TX_LINE_PER_DOC,
					                           AssociatedDocNumber	= docNumber,
					                           Security				= this.transContext.security
				                           };

				AssociatedParentTxListDO associatedParentTxListDO = FMChannelHelper.MakeCall<IGetAssociatedParentTxProcessor, AssociatedParentTxListDO>(
																	 x =>
																	 x.Process ( associatedParentTxSR )
																);

				if (associatedParentTxListDO != null)
				{
					foreach (AssociatedParentTxDO associatedParentTxDO in associatedParentTxListDO.List)
					{
						newDictionary.Add ( associatedParentTxDO.CLIN, associatedParentTxDO.CLIN );
					}
				}

				if (this.transContext.AssociatedDocNumFlags != null)
				{
					var flags = this.transContext.AssociatedDocNumFlags[docNumber] as ArrayList;
					if (( flags != null ) && (bool) flags[3])
					{
						this.bFieldEditible = false;
					}
				}
			}

			return newDictionary;
		}
		#endregion

		#region IHeaderField Members
		public object GetDataValue ( TransactionDO transaction )
		{
			var updatePanel = cell.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var comboBox = updatePanel.ContentTemplateContainer.Controls[0] as FMComboBox;

				if (comboBox != null && trans.AssociatedCLIN != null)
				{
					ListItem item = comboBox.Items.FindByText(trans.AssociatedCLIN);

					if (item != null)
					{
						return item.Value;
					}
				}
			}

			return string.Empty;
		}

		public string GetDataText ( TransactionDO transaction )
		{
			if (GetDataValue(transaction) != null)
			{
				return GetDataValue(transaction).ToString();
			}

			return null;
		}

		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			transaction.AssociatedCLIN = (string) newValue;
			OnFieldChanged ( );
		}
		#endregion
	}
}
