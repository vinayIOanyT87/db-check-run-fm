// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserDataTextFG.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   The purpose of this class is to handle the user data field control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TransactionFields
{
	using System.Web.UI;
	using System.Web.UI.WebControls;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The user data text field generator.
	/// </summary>
	public class UserDataTextFG : TextFieldGenerator, IHeaderField, ILineItemField
	{
		#region Public data members
		/// <summary>
		/// The client side script user data.
		/// </summary>
		public const string CLIENT_SIDE_SCRIPT_USER_DATA = "CLIENT_SIDE_SCRIPT_USER_DATA";

		/// <summary>
		/// The client side key user data.
		/// </summary>
		public const string CLIENT_SIDE_KEY_USER_DATA = "CLIENT_SIDE_KEY_USER_DATA";
		#endregion

		#region Protected data members
		/// <summary>
		/// The key.
		/// </summary>
		protected string key;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UserDataTextFG"/> class.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="displayName">
		/// The display name.
		/// </param>
		public UserDataTextFG ( string key, string displayName )
		{
			this.key = key;
			base.displayName = displayName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// This property will return the Field ID for a given user data field.
		/// </summary>
		public override string FieldID
		{
			get { return this.key; }
		}

		/// <summary>
		/// This property will returned either a figured data length or the 
		/// default length of 60.
		/// </summary>
		protected override short MaxColumns
		{
			get
			{
				return this.GetFieldLength ( this.FieldID, 60 );
			}
		}
		#endregion

		#region Override methods
		/// <summary>
		/// This method handles special ASP control functions such as client side scripting.
		/// </summary>
		/// <param name="control">
		/// The control.
		/// </param>
		protected override void SpecializeControl ( WebControl control )
		{
			base.SpecializeControl ( control );
			var updatePanel = control.Controls[0] as UpdatePanel;

			if (updatePanel != null)
			{
				var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

			if (textBox == null)
			{
				return;
			}

			// Register client scripts for this control if the custom client script registered is registered.
			string customClientScript = control.Page.Session[CUSTOM_CLIENT_SCRIPT_NAME] as string;

			if (!string.IsNullOrEmpty(customClientScript))
			{

					// Delay client side scripting until page pre-render event in case user clicks edit button of a
					// line item while editing another line item. Such situation causes this method to be called 
					// twice, once for for each line item. Since client side script is  allowed only once to be registered,
					// later line item's client script is ignored, which is the one we actually want.
					textBox.Page.Session[CLIENT_SIDE_SCRIPT_USER_DATA] =
						"<script type=\"text/javascript\"><!--\n" +
						"var oUserDataTextBox = document.getElementById('" + textBox.ClientID + "');\n " +
						"\n//--></script>";

					textBox.Attributes.Add ( "onChange", "javascript:try{MasterOnChange('" + this.FieldID + "');}catch(err){;}" );
				}
			}
		}
		#endregion

		#region IHeaderField Members
		/// <summary>
		/// The get data value.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public object GetDataValue ( TransactionDO transaction )
		{
			if (transaction.UserData.ContainsKey ( this.key ))
			{
				return transaction.UserData[this.key];
			}
			
			return null;
		}

		/// <summary>
		/// The get data text.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetDataText ( TransactionDO transaction )
		{
			if (this.GetDataValue(transaction) != null)
			{
				return this.GetDataValue(transaction).ToString();
			}
			
			return null;
		}

		/// <summary>
		/// The set data value.
		/// </summary>
		/// <param name="transaction">
		/// The transaction.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		public void SetDataValue ( TransactionDO transaction, object newValue )
		{
			transaction.UserData[key] = newValue as string;

			if (cell != null)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
                    updatePanel.Update();
                    var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

					if (textBox != null)
					{
						textBox.Text = newValue as string;
					}
				}
			}

			this.OnFieldChanged ( );
		}
		#endregion

		#region ILineItemField Members
		/// <summary>
		/// The set data value.
		/// </summary>
		/// <param name="currentLineItem">
		/// The current line item.
		/// </param>
		/// <param name="newValue">
		/// The new value.
		/// </param>
		public void SetDataValue(LineItemDO currentLineItem, object newValue)
		{
			if (currentLineItem.UserData.ContainsKey(this.key) == false)
			{
				return;
			}

			currentLineItem.UserData[this.key] = newValue as string;

			if (this.cell != null)
			{
				var updatePanel = cell.Controls[0] as UpdatePanel;

				if (updatePanel != null)
				{
					var textBox = updatePanel.ContentTemplateContainer.Controls[0] as TextBox;

					if (textBox != null)
					{
						textBox.Text = newValue as string;
					}
				}
			}

			this.OnFieldChanged ( );
		}

		/// <summary>
		/// The get data value.
		/// </summary>
		/// <param name="currentLineItem">
		/// The current line item.
		/// </param>
		/// <returns>
		/// The <see cref="object"/>.
		/// </returns>
		public object GetDataValue ( LineItemDO currentLineItem )
		{
			if (lineItem.UserData.ContainsKey(this.key))
			{
				return currentLineItem.UserData[this.key];
			}

			return null;
		}

		/// <summary>
		/// The get data text.
		/// </summary>
		/// <param name="currentLineItem">
		/// The current line item.
		/// </param>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string GetDataText(LineItemDO currentLineItem)
		{
			if ( this.GetDataValue(currentLineItem) != null )
			{
				return this.GetDataValue(currentLineItem).ToString( );
			}

			return null;
		}
		#endregion
	}
}
