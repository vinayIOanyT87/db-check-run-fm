/******************************************************************************
	FILE NAME:		FMCompanyTextBox.cs
	PURPOSE:		Implementation of: FMCompanyTextBox

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	W. Gray
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:			By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-28	Richard Panachida	Error in manager role. It assigned it to a shipper
												instead of a manager.

		2006-12-11	Richard Panachida	Changed to inherit from a base class.
		
		2008-04-15	V. Thompson			(CSI 5560)
												Added functionality to display either company id, name
												or both.  Users can configure this using the Transaction
												Alias configuration.  The configuration item is called
												Show Company Name

		2008-06-19	W.Gray				7.4.5.0 - Revised to use Companies.Get with GetExtendedInfo
												(CSI 5976)
*******************************************************************************/

using FMBusinessObjects.BusinessInterfaces;
using FMBusinessObjects.ChannelFactories;
using FMBusinessObjects.DataObjects;
using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace FMControls
{
	
	/// <summary>
	/// Summary description for FMCompanyTextBox.
	/// </summary>
	public class FMCompanyTextBox : FMTextBoxButtonControl
	{
		// vthompson CSI 5560
		protected TRANSACTION_SHOW_COMPANY_NAME _showCompanyName =
			TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY;

		// Tells the control whether to display ID, name or both
		public TRANSACTION_SHOW_COMPANY_NAME ShowCompanyName
		{
			get { return _showCompanyName; }
			set { _showCompanyName = value; }
		}

		protected string _companyName = string.Empty;

		public string CompanyName
		{
			get { return _companyName; }
			set { _companyName = value; }
		}

		// end vthompson CSI 5560

		public COMPANY_ROLE _Role = COMPANY_ROLE.MAX_COMPANY_ROLE;

		public string Role
		{
			set
			{
				if (!COMPANY_ROLE.TryParse(value, out _Role)) throw new Exception("FMCompanyTextBox Role");
			}
		}

		// JS20100820 WI-14934 Introduced sub-role for further filtering
		public COMPANY_SUB_ROLE _SubRole = COMPANY_SUB_ROLE.NO_SUBROLE;
		public string SubRole
		{
			set
			{
				if (value.Equals ( "ADF" ))
					_SubRole = COMPANY_SUB_ROLE.ADF;
				else if (value.Equals ( "OTHER" ))
					_SubRole = COMPANY_SUB_ROLE.OTHER;
			}
		}

		public FMCompanyTextBox ( )
		{
		}

		override protected void Page_Load ( object sender, EventArgs e )
		{
			this.buttonTitle = "Company select button";
			base.Page_Load(sender, e);
		}

		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				try
				{
					Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

					if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
					{
						if ((value == "{All}") || (value == "{Unassigned}"))
						{
							if (Page.Session["SiteGuid"] == null)
							{
								base.Text = value;
								return;
							}

							base.Text = GetDataDictionaryValueByKey(SiteGuid, value);
						}
						else
						{
							base.Text = value;
						}
					}
					else
					{
						base.Text = value;
					}

					if (!string.IsNullOrEmpty(value) && value != "{All}" && value != "{Unassigned}")
					{
						if (Page.Session["Security"] == null)
						{
							//SitesClass Sites = new SitesClass();
							//Page.Session["Security"] = Sites.GetSecurity(Page.Session["Token"] as string);
							throw new ArgumentNullException ( "Security" );
						}

						var security = Page.Session["Security"] as SecurityClass;
						
						if (security == null)
						{
							return;
						}

						if (this.DesignMode == false)
						{
							FMChannelHelper.MakeCall<ICompanies>(
									(companyChannel) =>
									{
										Guid companyGuid = companyChannel.GetIdentityGuid ( security, Text );

										if (companyGuid != Guid.Empty)
										{
											CompanyClass company = companyChannel.Get(security, companyGuid, false);

											if (company != null)
											{
												ToolTip = company.CompanyToolTip;
												this._companyName = company.Name;
											}
										}
									}
								);	
						}
					}
					else //Assign "Company" as the alt text if the company box does not have a specific company already
					{
						var toolTip = "Company";
						if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
						{
							toolTip = GetDataDictionaryValueByKey(SiteGuid, toolTip);
						}
						this.ToolTip = toolTip;
						this._companyName = String.Empty;
					}
				}
				catch
				{
				}
			}
		}

		protected override void Render ( HtmlTextWriter writer )
		{
			// vthompson CSI 5560
			// Lots of changes were made to this method for the implementation of CSI 5560.
			// The idea is the user can configure this control to display the company ID,
			// the company name, or both.  The functionality of the TransactionDetail page
			// is dependent on the company ID.  In addition there is client-side script on
			// both the CompanySelectForm.aspx and TransactionDetail.aspx pages that was
			// updated
			IEnumerator keys = null;
			string style = string.Empty;

			// vthompson CSI 5560
			writer.WriteBeginTag ( "div" );
			writer.WriteAttribute("class", "FMCompanyTextBox");
			writer.Write ( HtmlTextWriter.TagRightChar );

			// Display the company ID and name based on the SetCompanyName property
			if (_showCompanyName == TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY ||
				_showCompanyName == TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID)
			{
				writer.WriteBeginTag ( "input" );
				writer.WriteAttribute ( "name", UniqueID );
				writer.WriteAttribute ( "type", "text" );
				writer.WriteAttribute ( "value", HttpUtility.HtmlEncode(base.Text) );
				writer.WriteAttribute ( "readonly", "readonly" );

				if (AutoPostBack)
				{
					writer.WriteAttribute ( "onchange", "__doPostBack('" + UniqueID + "','')" );
				}
				if (!Enabled)
				{
					writer.WriteAttribute ( "disabled", "disabled" );
				}

				writer.WriteAttribute ( "id", UniqueID );
				writer.WriteAttribute ( "tabindex", "-1" );
				writer.WriteAttribute ( "title", HttpUtility.HtmlEncode(ToolTip) );
				writer.WriteAttribute ( "class", CssClass );
				keys = Style.Keys.GetEnumerator ( );

				style = "background:#DDDDDD;width:" + (Width.Value - 5) + "px";
				while (keys.MoveNext ( ))
				{
					string key = (string) keys.Current;
					style += ";" + key + ": " + Style[key];
				}
				writer.WriteAttribute ( "style", style );

				writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
				writer.Write ( writer.NewLine );

				// Add the Select button
				writer.WriteBeginTag ( "input" );
				writer.WriteAttribute ( "class", "formfieldtitle" );
				writer.WriteAttribute("title", this.buttonTitle);

				// JS20100809 WI-14889 allow the read-only of this control to trigger
				// the disable behaviour of the button, leaving text readable
				if (!Enabled || ReadOnly)
				{
					writer.WriteAttribute ( "disabled", "disabled" );
				}

				keys = Style.Keys.GetEnumerator ( );

				style = "padding:0;width: 20px; height:20px";
				while (keys.MoveNext ( ))
				{
					string key = ((string) keys.Current).ToLower();
					if (key == "height")
						continue;

					if (key == "left")
						style += ";" + key + ": " + (Unit.Parse(Style[key]).Value + Width.Value + 5) + "px";
					else
						style += ";" + key + ": " + Style[key];
				}
				writer.WriteAttribute ( "style", style );

				// Must have this IF statement so that the code does not brake in design mode.
				if (DesignMode == false)
				{
					bool isADFKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(
							(hardwareKeyChannel) =>
							{
								return hardwareKeyChannel.IsADFKey();
							}
						);	

					if (isADFKey && this.Page.ToString ( ).ToUpper ( ).Contains ( "TRANSACTIONDETAIL" ))
					{
						writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + _SubRole + "','" + UniqueID + "')" );
					}
					else
					{
						writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + UniqueID + "')" );
					}
				}
				else
				{
					writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + UniqueID + "')" );
				}

				writer.WriteAttribute ( "type", "button" );
				writer.WriteAttribute ( "value", "..." );
				writer.WriteAttribute("id", UniqueID + " Select Button");

				writer.Write ( HtmlTextWriter.SelfClosingTagEnd );

				// vthompson CSI 5560
				// close the opening div
				writer.WriteEndTag ( "div" );

				if (_showCompanyName == TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID)
				{
					writer.WriteBeginTag ( "div" );
					writer.Write ( HtmlTextWriter.TagRightChar );

					writer.WriteBeginTag ( "input" );
					writer.WriteAttribute ( "name", "CompanyName" + UniqueID );
					writer.WriteAttribute ( "type", "text" );
					writer.WriteAttribute ( "value",  HttpUtility.HtmlEncode(this._companyName) );
					writer.WriteAttribute ( "readonly", "readonly" );

					if (!Enabled)
					{
						writer.WriteAttribute ( "disabled", "disabled" );
					}

					writer.WriteAttribute ( "id", "CompanyName" + UniqueID );
					writer.WriteAttribute ( "tabindex", "-1" );
					writer.WriteAttribute ( "title",  HttpUtility.HtmlEncode(ToolTip) );
					writer.WriteAttribute ( "class", CssClass );
					keys = Style.Keys.GetEnumerator ( );

					style = "background:#DDDDDD;width:" + (Width.Value - 5) + "px";
					while (keys.MoveNext ( ))
					{
						string key = (string) keys.Current;
						style += ";" + key + ": " + Style[key];
					}
					writer.WriteAttribute ( "style", style );
					writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
					writer.Write ( writer.NewLine );

					// Close the div tag
					writer.WriteEndTag ( "div" );
				}
				else
				{
					// Show only the ID field but the name element must be on the page
					// otherwise client side scripting errors may occur
					writer.WriteBeginTag ( "input" );
					writer.WriteAttribute ( "name", "CompanyName" + UniqueID );
					writer.WriteAttribute ( "type", "hidden" );
					writer.WriteAttribute ( "value",  HttpUtility.HtmlEncode(this._companyName) );
					writer.WriteAttribute ( "id", "CompanyName" + UniqueID );
					writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
				}

				writer.Write ( writer.NewLine );

			}
			else
			{
				// Show name only.  
				writer.WriteBeginTag ( "div" );
				writer.Write ( HtmlTextWriter.TagRightChar );

				writer.WriteBeginTag ( "input" );
				writer.WriteAttribute ( "name", "CompanyName" + UniqueID );
				writer.WriteAttribute ( "type", "text" );
				writer.WriteAttribute ( "value",  HttpUtility.HtmlEncode(this._companyName) );
				writer.WriteAttribute ( "readonly", "readonly" );

				if (!Enabled)
				{
					writer.WriteAttribute ( "disabled", "disabled" );
				}

				writer.WriteAttribute ( "id", "CompanyName" + UniqueID );
				writer.WriteAttribute ( "tabindex", "-1" );
				writer.WriteAttribute ( "title",  HttpUtility.HtmlEncode(ToolTip) );
				writer.WriteAttribute ( "class", CssClass );
				keys = Style.Keys.GetEnumerator ( );

				style = "background:#DDDDDD;width:" + (Width.Value - 5) + "px";
				while (keys.MoveNext ( ))
				{
					string key = (string) keys.Current;
					style += ";" + key + ": " + Style[key];
				}
				writer.WriteAttribute ( "style", style );
				writer.Write ( HtmlTextWriter.SelfClosingTagEnd );
				writer.Write ( writer.NewLine );

				// In this case the select button will be next to the visible name field
				// Add the Select button
				writer.WriteBeginTag ( "input" );
				writer.WriteAttribute ( "class", "formfieldtitle" );
				writer.WriteAttribute("title", this.buttonTitle);

				if (!this.Enabled)
				{
					writer.WriteAttribute ( "disabled", "disabled" );
				}

				keys = Style.Keys.GetEnumerator ( );

				style = "padding:0;width: 20px; height:20px";
				while (keys.MoveNext ( ))
				{
					string key = ((string)keys.Current).ToLower();
					if (key == "height")
					{
						continue;
					}

					if (key == "left")
					{
						style += ";" + key + ": " + (Unit.Parse(Style[key]).Value + Width.Value + 5) + "px";
					}
					else
					{
						style += ";" + key + ": " + Style[key];
					}
				}
				writer.WriteAttribute ( "style", style );

				// Must have this IF statement so that the code does not brake in design mode.
				if (DesignMode == false)
				{
					bool isADFKey = FMChannelHelper.MakeCall<IHardwareKey, bool>(
							(hardwareKeyChannel) =>
							{
								return hardwareKeyChannel.IsADFKey();
							}
						);

					if (isADFKey && this.Page.ToString().ToUpper().Contains("TRANSACTIONDETAIL"))
					{
						writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + _SubRole + "','" + UniqueID + "')" );
					}
					else
					{
						writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + UniqueID + "')" );
					}
				}
				else
				{
					writer.WriteAttribute ( "onclick", "CompanySelect('" + _Role.ToString ( ) + "','" + UniqueID + "')" );
				}

				writer.WriteAttribute ( "type", "button" );
				writer.WriteAttribute ( "value", "..." );

				writer.Write ( HtmlTextWriter.SelfClosingTagEnd );

				// vthompson CSI 5560
				// close the opening div
				writer.WriteEndTag ( "div" );

				// The company ID information still must be on the page
				writer.WriteBeginTag ( "input" );
				writer.WriteAttribute ( "name", UniqueID );
				writer.WriteAttribute ( "type", "hidden" );
				writer.WriteAttribute ( "value",  HttpUtility.HtmlEncode(base.Text) );
				writer.WriteAttribute ( "id", UniqueID );
				writer.Write ( HtmlTextWriter.SelfClosingTagEnd );

				writer.Write ( writer.NewLine );
			}
		}
	}
}
