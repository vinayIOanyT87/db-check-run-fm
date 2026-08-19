/******************************************************************************
	FILE NAME:		FMTextBoxButtonControl.cs
	PURPOSE:		The purpose of this class is to be the base class for all
					controls that have the text box/button combination.

	COMMENTS:
		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2002
		This file shall not be copied or reproduced in any form without
		the express written consent of Endress+Hauser.

	AUTHOR(S):	Richard Panachida
	VERSION:	1.0.0  Current version

	MODIFICATION HISTORY:
		Date:		By:					Reason:
		----------	-----------------	-------------------------------------------
		2006-11-28	Richard Panachida	Error in manager role. It assigned it to a shipper
										instead of a manager.
*******************************************************************************/
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using FMBusinessObjects.UtilityObjects;

namespace FMControls
{
	abstract public class FMTextBoxButtonControl : FMTextBox
	{

		protected string buttonTitle = "Select button";

		#region Constructors
		/// <summary>
		/// This is the default constructor the FMTextBoxButtonControl base class
		/// </summary>
		public FMTextBoxButtonControl()
		{
		}
		#endregion

		protected Control FMFindControl(Control x, string c_id)
		{
			foreach (Control cc in x.Controls)
			{
				if (!string.IsNullOrWhiteSpace(cc.ID))
				{
					if (cc.ID == c_id)
						return cc;
				}
				Control c = FMFindControl(cc, c_id);
				if (c != null)
					return c;
			}
			return null;
		}

		/// <summary>
		/// This is an abstract method to enforce the derived classes to implement the
		/// page load.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		virtual protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string labelBy = this.Attributes["aria-labelledby"];
				if (!string.IsNullOrWhiteSpace(labelBy))
				{
					Control c = FMFindControl(this.Page, labelBy);
					if (c != null)
					{
						if (c is Label)
						{
							buttonTitle = ((Label)c).Text.Replace(":", string.Empty) + " select button";
						}
					}
				}
			}
			catch
			{
				;
			}
		}

		/// <summary>
		/// This method performs the on initialize process.
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e)
		{
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// This method implements the initialization of the component.
		/// </summary>
		protected void InitializeComponent()
		{
			this.Load += new System.EventHandler(this.Page_Load);
		}

		protected string GetDataDictionaryValueByKey(Guid siteGuid, string p)
		{
			return DataDictionarySingleton.Get(siteGuid, p);
		}

		/// <summary>
		/// This property overrides the base class' get and set Text property functionality.
		/// It implements the data dictionary on the text.
		/// </summary>
		public override string Text
		{
			get
			{
				try
				{
					if (Page.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
					{
						if (Page.Session["SiteGuid"] == null)
						{
							return base.Text;
						}

						Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

						string retValue = base.Text;

						if (base.Text == this.GetDataDictionaryValueByKey(SiteGuid, "{All}"))
						{
							retValue = "{All}";
						}
						else if (base.Text == this.GetDataDictionaryValueByKey(SiteGuid, "{Unassigned}"))
						{
							retValue = "{Unassigned}";
						}
						else
						{
							retValue = base.Text;
						}

						return retValue;
					}
					else
					{
						return base.Text;
					}
				}
				catch
				{
					return base.Text;
				}
			}

			set
			{
				try
				{
					if (Page?.Session["UseDataDictionary"] == null || (bool)Page.Session["UseDataDictionary"])
					{
						if ((value == "{All}") || (value == "{Unassigned}"))
						{
							if (Page?.Session["SiteGuid"] == null)
							{
								base.Text = value;
								return;
							}

							Guid SiteGuid = (Guid)Page.Session["SiteGuid"];

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
				}
				catch
				{
					base.Text = value;
				}
			}
		}
	}
}
